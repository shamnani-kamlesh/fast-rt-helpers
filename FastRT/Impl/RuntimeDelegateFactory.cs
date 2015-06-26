using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FastRT.Impl
{
    internal static class RuntimeDelegateFactory
    {
        //delegate types for get & set operations
        public delegate TMemberType MemberGetDelegate<in TObjectType, out TMemberType>(TObjectType obj);
        public delegate void MemberSetDelegate<in TObjectType, in TMemberType>(TObjectType obj, TMemberType val);

        //delegates cache
        private static readonly ObjectCache<Type> s_delegateCache = new ObjectCache<Type>();

        /// <summary>
        /// Attempts to get or create a Get delegate of the required delegate type 
        /// </summary>
        /// <returns>If there is not a public get accessor for the requested member, then this method will return null</returns>
        public static MemberGetDelegate<TObjectType, TMemberType> RetrieveMemberGetValueDelegate<TObjectType, TMemberType>(string memberName, IObjectCache<string> cacheMemberDelegates = null)
        {
            if(cacheMemberDelegates == null)
                cacheMemberDelegates = RetrieveTypeMemberDelegateCache(typeof(TObjectType));
            string decoratedMemberName = GetMemberKey<TObjectType, TMemberType>(memberName, "get");
            return cacheMemberDelegates.GetValue(decoratedMemberName, () => CreateMemberGetDelegate<TObjectType, TMemberType>(memberName));
        }

        /// <summary>
        /// Attempts to get or create a Set delegate of the required delegate type 
        /// </summary>
        /// <returns>If there is not a public set accessor for the requested member, then this method will return null</returns>
        public static MemberSetDelegate<TObjectType, TMemberType> RetrieveMemberSetValueDelegate<TObjectType, TMemberType>(string memberName, IObjectCache<string> cacheMemberDelegates = null)
        {
            if(cacheMemberDelegates == null)
                cacheMemberDelegates = RetrieveTypeMemberDelegateCache(typeof(TObjectType));
            string decoratedMemberName = GetMemberKey<TObjectType, TMemberType>(memberName, "set");
            return cacheMemberDelegates.GetValue(decoratedMemberName, () => CreateMemberSetDelegate<TObjectType, TMemberType>(memberName));
        }

        private static string GetMemberKey<TObjectType, TMemberType>(string memberName, string opType)
        {
            return "(" + typeof(TMemberType).FullName + ")" + typeof(TObjectType).FullName + "." + memberName + "_" + opType;
        }

        /// <summary>
        /// Creates a wrapper delegate for handling proper type casting or boxing
        /// </summary>
        private static IMemberAccessDelegateProvider<TObjectType, TMemberType> CreateDelegateWrapper<TObjectType, TMemberType>(Func<Type, Delegate> delegateFactory, Type realMemberType, Type delegateGenericType)
        {
            Type delType = delegateGenericType.MakeGenericType(typeof(TObjectType), realMemberType);
            Delegate del = delegateFactory(delType);
            Type wrapperType = typeof(CastingDelegateWrapper<,>).MakeGenericType(typeof(TObjectType), realMemberType);
            var constructorInfo = wrapperType.GetConstructor(new[] { delType });
            if (constructorInfo == null)
                throw new InvalidOperationException("Cannot create a RT delegate wrapper for the method");

            IMemberAccessDelegateProvider<TObjectType, TMemberType> wrapper =
                (IMemberAccessDelegateProvider<TObjectType, TMemberType>)constructorInfo.Invoke(new object[] { del });

            return wrapper;
        }

        /// <summary>
        /// Attempts to create a Get delegate of the required delegate type 
        /// </summary>
        /// <returns>If there is not a public get accessor for the requested member, then this method will return null</returns>
        private static MemberGetDelegate<TObjectType, TMemberType> CreateMemberGetDelegate<TObjectType, TMemberType>(string memberName)
        {
            var result = TryCreateMemberGetDelegateForProperty<TObjectType, TMemberType>(memberName);
            return result ?? TryCreateMemberGetDelegateForField<TObjectType, TMemberType>(memberName);
        }

        /// <summary>
        /// Attempts to create a Set delegate of the required delegate type
        /// </summary>
        /// <returns>If there is not a Public Set accessor for the requested member, then this method will return null;</returns>
        private static MemberSetDelegate<TObjectType, TMemberType> CreateMemberSetDelegate<TObjectType, TMemberType>(string memberName)
        {
            var result = TryCreateMemberSetDelegateForProperty<TObjectType, TMemberType>(memberName);
            return result ?? TryCreateMemberSetDelegateForField<TObjectType, TMemberType>(memberName);
        }

        private static MemberGetDelegate<TObjectType, TMemberType> TryCreateMemberGetDelegateForField<TObjectType, TMemberType>(string memberName)
        {
            Type objectType = typeof (TObjectType);
            FieldInfo fi = objectType.GetField(memberName);
            MemberGetDelegate<TObjectType, TMemberType> result = null;
            if (fi != null)
            {
                // Member is a Field...
                DynamicMethod dm = new DynamicMethod("Get" + memberName,
                    fi.FieldType, new[] {objectType}, objectType);
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fi);
                il.Emit(OpCodes.Ret);

                //if we need a delegate with boxing or casting behavior, we have to create a special wrapper
                if (typeof (Object) == typeof (TMemberType) && fi.FieldType != typeof (Object))
                {
                    var wrapper = CreateDelegateWrapper<TObjectType, TMemberType>(dm.CreateDelegate, fi.FieldType, typeof (MemberGetDelegate<,>));
                    result = wrapper.GetMemberGetDelegate();
                }
                else
                {
                    result = (MemberGetDelegate<TObjectType, TMemberType>) dm.CreateDelegate(typeof (MemberGetDelegate<TObjectType, TMemberType>));
                }
            }

            return result;
        }

        private static MemberGetDelegate<TObjectType, TMemberType> TryCreateMemberGetDelegateForProperty<TObjectType, TMemberType>(string memberName)
        {
            PropertyInfo pi = typeof(TObjectType).GetProperty(memberName);
            MemberGetDelegate<TObjectType, TMemberType> result = null;

            if (pi != null)
            {
                // Member is a Property...
                MethodInfo mi = pi.GetGetMethod(true);
                if (mi != null)
                {
                    //if we need a delegate with boxing or casting behavior, we have to create a special wrapper
                    if (typeof (Object) == typeof (TMemberType) && mi.ReturnType != typeof (Object))
                    {
                        var wrapper = CreateDelegateWrapper<TObjectType, TMemberType>(dt => Delegate.CreateDelegate(dt, mi), mi.ReturnType, typeof (MemberGetDelegate<,>));
                        result = wrapper.GetMemberGetDelegate();
                    }
                    else
                    {
                        result = (MemberGetDelegate<TObjectType, TMemberType>) Delegate.CreateDelegate(typeof (MemberGetDelegate<TObjectType, TMemberType>), mi);
                    }
                }
            }
            return result;
        }

        private static MemberSetDelegate<TObjectType, TMemberType> TryCreateMemberSetDelegateForField<TObjectType, TMemberType>(string memberName)
        {
            Type objectType = typeof (TObjectType);
            FieldInfo fi = objectType.GetField(memberName);
            MemberSetDelegate<TObjectType, TMemberType> result = null;
            if (fi != null)
            {
                // Member is a Field...
                DynamicMethod dm = new DynamicMethod("Set" + memberName, null, new[] {objectType, fi.FieldType}, objectType);
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fi);
                il.Emit(OpCodes.Ret);

                //if we need a delegate with boxing or casting behavior, we have to create a special wrapper
                if (typeof (Object) == typeof (TMemberType) && fi.FieldType != typeof (Object))
                {
                    var wrapper = CreateDelegateWrapper<TObjectType, TMemberType>(dm.CreateDelegate, fi.FieldType, typeof (MemberSetDelegate<,>));
                    result = wrapper.GetMemberSetDelegate();
                }
                else
                {
                    result = (MemberSetDelegate<TObjectType, TMemberType>) dm.CreateDelegate(typeof (MemberSetDelegate<TObjectType, TMemberType>));
                }
            }
            return result;
        }

        private static MemberSetDelegate<TObjectType, TMemberType> TryCreateMemberSetDelegateForProperty<TObjectType, TMemberType>(string memberName)
        {
            MemberSetDelegate<TObjectType, TMemberType> result = null;
            PropertyInfo pi = typeof (TObjectType).GetProperty(memberName);
            if (pi != null)
            {
                // Member is a Property...
                MethodInfo mi = pi.GetSetMethod(true);
                if (mi != null)
                {
                    //if we need a delegate with boxing or casting behavior, we have to create a special wrapper
                    if (typeof (Object) == typeof (TMemberType) && pi.PropertyType != typeof (Object))
                    {
                        var wrapper = CreateDelegateWrapper<TObjectType, TMemberType>(dt => Delegate.CreateDelegate(dt, mi), pi.PropertyType, typeof (MemberSetDelegate<,>));
                        result = wrapper.GetMemberSetDelegate();
                    }
                    else
                    {
                        result = (MemberSetDelegate<TObjectType, TMemberType>) Delegate.CreateDelegate(typeof (MemberSetDelegate<TObjectType, TMemberType>), mi);
                    }
                }
            }
            return result;
        }

        private static IObjectCache<string> RetrieveTypeMemberDelegateCache(Type ObjectType)
        {
            return s_delegateCache.GetValue(ObjectType, () => new ObjectCache<string>());
        }
    }
}
