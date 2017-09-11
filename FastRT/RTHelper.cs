using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FastRT
{
    public static class RTHelper
    {
        public static IMemberAccessor<T,V> GetMemberAccessor<T, V>(Expression<Func<T, V>> func, bool readOnly = false, IObjectCache<string> memberCache = null) where T : class
        {
            var mi = GetMemberFromExpression(func);
            return new DelegateMemberAccessor<T, V>(mi.Name, readOnly, memberCache);
        }

        public static IMemberAccessor<T, V> GetMemberAccessor<T, V>(string memberName, bool readOnly = false, IObjectCache<string> memberCache = null) where T : class
        {
            return new DelegateMemberAccessor<T, V>(memberName, readOnly, memberCache);
        }

        public static IMemberAccessor GetMemberAccessor(Type objectType, string memberName, bool readOnly = false, IObjectCache<string> memberCache = null) 
        {
            return CreateMemberAccessor(GetMemberInfo(objectType, memberName), readOnly, memberCache);
        }

        public static IMemberAccessor GetMemberAccessor(MemberInfo mi, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            return CreateMemberAccessor(GetMemberInfo(mi), readOnly, memberCache);
        }

        private static IMemberAccessor CreateMemberAccessor(MemberTypeInfo mi, bool readOnly, IObjectCache<string> memberCache)
        {
            var genType = typeof(DelegateMemberAccessor<,>).MakeGenericType(mi.DeclaringType, mi.MemberType);
            var result = Activator.CreateInstance(genType, mi.Name, readOnly, memberCache);
            return (IMemberAccessor)result;
        }

        public static IObjectMemberAccessor<V> GetObjectMemberAccessor<T, V>(this T obj, Expression<Func<T, V>> func, bool readOnly = false, IObjectCache<string> memberCache = null) where T : class
        {
            var mi = GetMemberFromExpression(func);
            return new ObjectMemberAccessor<T, V>(obj, new DelegateMemberAccessor<T, V>(mi.Name, readOnly, memberCache));
        }

        public static IObjectMemberAccessor<V> GetObjectMemberAccessor<T, V>(this T obj, string memberName, bool readOnly = false, IObjectCache<string> memberCache = null) where T : class
        {
            return new ObjectMemberAccessor<T, V>(obj, new DelegateMemberAccessor<T, V>(memberName, readOnly, memberCache));
        }

        public static IObjectMemberAccessor GetObjectMemberAccessor(this object obj, string memberName, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            if(obj == null)
                throw new ArgumentNullException(nameof(obj));
            IMemberAccessor ma = GetMemberAccessor(obj.GetType(), memberName, readOnly, memberCache);
            var oaType = typeof (ObjectMemberAccessor<,>).MakeGenericType(ma.ObjectType, ma.MemberType);
            var result = Activator.CreateInstance(oaType, obj, ma);
            return (IObjectMemberAccessor) result;
        }

        public static MemberInfo GetMemberFromExpression<T, V>(Expression<Func<T, V>> expr)
        {
            if (expr == null)
                throw new ArgumentNullException(nameof(expr));

            MemberExpression me = expr.Body as MemberExpression;
            if (me == null)
            {
                if (expr.Body.NodeType == ExpressionType.Convert && typeof(V) == typeof(object))
                    me = ((UnaryExpression)expr.Body).Operand as MemberExpression;
                if (me == null)
                    throw new ArgumentException("Unsupported expression (must be a property or field reference): " + expr);
            }

            PropertyInfo pi = me.Member as PropertyInfo;
            if (pi != null)
                return pi;

            FieldInfo fi = me.Member as FieldInfo;
            if (fi != null)
                return fi;

            throw new ArgumentException("Unsupported expression (must be a property or field reference): " + expr);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider prv, bool inherit = true) where T : Attribute
        {
            return prv.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
        }

        public static V GetDefaultValue<V>()
        {
            return (V)GetDefaultValue(typeof(V));
        }

        public static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        public static string GetFullName(this MemberInfo mi)
        {
            if(mi == null || mi.DeclaringType == null)
                throw new ArgumentException("Member Info doesn't have a Declaring Type", nameof(mi));

            return mi.DeclaringType.Name + "." + mi.Name;
        }

        public static Type GetMemberType(Type objectType, string memberName)
        {
            return GetMemberInfo(objectType, memberName).MemberType;
        }

        private static MemberTypeInfo GetMemberInfo(Type objectType, string memberName)
        {
            var member = objectType.GetMember(memberName).FirstOrDefault();
            if (member == null)
            {
                throw new NotImplementedException("Property or field not found: " + objectType.FullName + "." + memberName);
            }

            return GetMemberInfo(member);
        }

        private static MemberTypeInfo GetMemberInfo(MemberInfo member)
        {
            Type memberType;
            if (member is PropertyInfo info)
            {
                memberType = info.PropertyType;
            }
            else if (member is FieldInfo field)
            {
                memberType = field.FieldType;
            }
            else
            {
                throw new NotSupportedException("Member type not supported: " + member.MemberType);
            }

            return new MemberTypeInfo
            {
                Name = member.Name,
                MemberType = memberType,
                DeclaringType = member.DeclaringType
            };
        }

        internal class MemberTypeInfo
        {
            public string Name { get; set; }
            public Type MemberType { get; set; }
            public Type DeclaringType { get; set; }
        }

    }
}