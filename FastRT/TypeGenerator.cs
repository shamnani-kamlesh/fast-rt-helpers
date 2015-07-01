using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Threading;
using FastRT.Impl;

namespace FastRT
{
    public static class TypeGenerator
    {
        private static readonly ModuleBuilder s_mb;
        private static readonly ReaderWriterLockSlim s_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        static TypeGenerator()
        {
            AssemblyName aName = new AssemblyName("FastRT.DynamicTypeGenerator");
            AssemblyBuilder sAb = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            s_mb = sAb.DefineDynamicModule(aName.Name);
        }

        /// <summary>
        /// Returns previously defined type with a specified name
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>Type for the given name or null if not found</returns>
        public static Type GetType(string typeName)
        {
            s_lock.EnterReadLock();
            try
            {
                return string.IsNullOrEmpty(typeName) ? null : s_mb.GetType(typeName, false, false);
            }
            finally
            {
                s_lock.ExitReadLock();
            }
        }

        public static Type MakeType(string name, IEnumerable<KeyValuePair<string, Type>> propDefs)
        {
            return MakeType((ITypeDef) MakeTypeDef(name, propDefs));
        }

        /// <summary>
        /// Builds a new type from the provided list of properties. The new type implements IObjectAccessor interface
        /// </summary>
        /// <param name="typeDef">Type definition with list of property names and corresponding types.</param>
        /// <returns>Newly created type</returns>
        public static Type MakeType(ITypeDef typeDef)
        {
            if (typeDef == null)
                throw new ArgumentNullException("typeDef");

            string typeName = typeDef.Name;
            if (string.IsNullOrEmpty(typeName))
                typeName = MakeUniqueName();

            s_lock.EnterWriteLock();
            try
            {
                TypeBuilder tb = s_mb.DefineType(typeName, TypeAttributes.Public);
                tb.SetParent(typeof(ObjectAccessorBase<>).MakeGenericType(tb));

                int order = 0;
                foreach (var fb in typeDef.PropertyDefList.Select(entry => tb.DefineField(entry.Key, entry.Value.ResolveType(), FieldAttributes.Public)))
                    AddOrderAttribute(fb, order++);

                return tb.CreateType();
            }
            finally
            {
                s_lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Instantiates a new object from the provided data. 
        /// Type is generated automatically or picked up an existing one with the matching name.
        /// All null values will be represented as properties with "System.Object" type
        /// </summary>
        /// <param name="typeName">Type name. When null, a new unique name is generated</param>
        /// <param name="objDef">List of property names and corresponding values. Property type is inferred from the value type</param>
        /// <returns>Instantiated object</returns>
        public static IObjectAccessor MakeObject(string typeName, IEnumerable<KeyValuePair<string, object>> objDef)
        {
            s_lock.EnterUpgradeableReadLock();
            Type newT;
            var defEntries = objDef as IList<KeyValuePair<string, object>> ?? objDef.ToList();
            try
            {
                newT = GetType(typeName) ?? MakeType(MakeTypeDef(typeName, defEntries));
            }
            finally
            {
                s_lock.ExitUpgradeableReadLock();
            }

            IObjectAccessor obj = (IObjectAccessor)Activator.CreateInstance(newT);
            foreach (var entry in defEntries)
                obj[entry.Key] = entry.Value;

            return obj;
        }

        public static Type MakeTypeDef(string name, IEnumerable<KeyValuePair<string, Type>> typeDef)
        {
            var propDefs = typeDef.Select(entry => new KeyValuePair<string, ITypeResolver>(entry.Key, entry.Value.AsTypeResolver()));
            return new FutureTypeInfo(name, propDefs);
        }

        public static ITypeResolver AsTypeResolver(this Type type)
        {
            return (type as ITypeResolver) ?? new RuntimeTypeResolver(type);
        }

        public static Type MakeListDef(Type elementType)
        {
            return new FutureListInfo(elementType.AsTypeResolver());
        }

        public static Type MakeListDef(ITypeResolver elType)
        {
            return new FutureListInfo(elType);
        }

        public static Type MakeTypeRef(string typeName)
        {
            return new FutureTypeRef(typeName);
        }

        public static IObjectFactory MakeObjectFactory(this Type type)
        {
            return new ObjectFactory(type.AsTypeResolver().ResolveType());
        }

        public static IObjectAccessor NewObjectAccessor(this IObjectFactory factory)
        {
            return factory.NewObject<IObjectAccessor>();
        }

        public static Type ResolveTypeDef(ITypeDef td)
        {
            return GetType(td.Name) ?? MakeType(td);
        }

        public static Type ResolveListDef(IListDef listDef)
        {
            return typeof(List<>).MakeGenericType(listDef.ElementType.ResolveType());
        }

        private static void AddOrderAttribute(FieldBuilder fb, int order)
        {
            ConstructorInfo ci = typeof(OrderAttribute).GetConstructor(new[] { typeof(int) });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(ci, new object[] { order });
            fb.SetCustomAttribute(attributeBuilder);
        }

        private static ITypeDef MakeTypeDef(string name, IEnumerable<KeyValuePair<string, object>> objDef)
        {
            var propDefs = objDef.Select(d => new KeyValuePair<string, ITypeResolver>(d.Key, GetMemberTypeFromValue(d.Value)));
            return new FutureTypeInfo(name, propDefs);
        }

        private static ITypeResolver GetMemberTypeFromValue(object value)
        {
            return AsTypeResolver(value == null ? typeof(Object) : value.GetType());
        }

        private static string MakeUniqueName()
        {
            return "AutoGen_" + Guid.NewGuid().ToString("N");
        }
    }

}