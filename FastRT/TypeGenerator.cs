﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Threading;

namespace FastRT.Tests
{
    public static class TypeGenerator
    {
        private static AssemblyBuilder s_ab;
        private static ModuleBuilder s_mb;
        private static ReaderWriterLockSlim s_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        static TypeGenerator()
        {
            AssemblyName aName = new AssemblyName("FastRT.DynamicTypeGenerator");
            s_ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            s_mb = s_ab.DefineDynamicModule(aName.Name);
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

        /// <summary>
        /// Builds a new type from the provided list of properties. The new type implements IObjectAccessor interface
        /// </summary>
        /// <param name="typeDef">List of property names and corresponding types.</param>
        /// <param name="typeName">Type name. If null, a new unique name is generated</param>
        /// <returns>Newly created type</returns>
        public static Type MakeType(IEnumerable<KeyValuePair<string, Type>> typeDef, string typeName = null)
        {
            if (typeDef == null)
                throw new ArgumentNullException("typeDef");

            if (string.IsNullOrEmpty(typeName))
                typeName = MakeUniqueName();

            s_lock.EnterWriteLock();
            try
            {
                TypeBuilder tb = s_mb.DefineType(typeName, TypeAttributes.Public);
                tb.SetParent(typeof(ObjectAccessorBase<>).MakeGenericType(tb));

                int order = 0;
                foreach (var entry in typeDef)
                {
                    var fb = tb.DefineField(entry.Key, entry.Value, FieldAttributes.Public);
                    AddOrderAttribute(fb, order++);
                }

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
        /// <param name="objDef">List of property names and corresponding values. Property type is inferred from the value type</param>
        /// <param name="typeName">Type name. When null, a new unique name is generated</param>
        /// <returns>Instantiated object</returns>
        public static IObjectAccessor MakeObject(IEnumerable<KeyValuePair<string, object>> objDef, string typeName = null)
        {
            s_lock.EnterUpgradeableReadLock();
            Type newT;
            try
            {
                newT = GetType(typeName) ?? MakeType(MakeTypeDef(objDef), typeName);
            }
            finally
            {
                s_lock.ExitUpgradeableReadLock();
            }

            IObjectAccessor obj = (IObjectAccessor)Activator.CreateInstance(newT);
            foreach (var entry in objDef)
                obj[entry.Key] = entry.Value;

            return obj;
        }

        private static void AddOrderAttribute(FieldBuilder fb, int order)
        {
            ConstructorInfo ci = typeof(OrderAttribute).GetConstructor(new[] { typeof(int) });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(ci, new object[] { order });
            fb.SetCustomAttribute(attributeBuilder);
        }

        private static IEnumerable<KeyValuePair<string, Type>> MakeTypeDef(IEnumerable<KeyValuePair<string, object>> objDef)
        {
            return objDef.Select(d => new KeyValuePair<string, Type>(d.Key, GetMemberTypeFromValue(d.Value)));
        }

        private static Type GetMemberTypeFromValue(object value)
        {
            return value == null ? typeof(Object) : value.GetType();
        }

        private static string MakeUniqueName()
        {
            return "AutoGen_" + Guid.NewGuid().ToString("N");
        }
    }

}