using System;
using System.Linq.Expressions;

namespace FastRT
{
    public static class TypeAccessor
    {
        public static IMemberAccessor GetMemberAccessor(Type objectType, string memberName, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            return RTHelper.GetMemberAccessor(objectType, memberName, readOnly, memberCache);
        }
    }
    public static class TypeAccessor<T> where T : class
    {
        public static IMemberAccessor<T, V> GetMemberAccessor<V>(Expression<Func<T, V>> memberFunc, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            return RTHelper.GetMemberAccessor(memberFunc, readOnly, memberCache);
        }
        public static IMemberAccessor<T, V> GetMemberAccessor<V>(string memberName, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            return RTHelper.GetMemberAccessor<T, V>(memberName, readOnly, memberCache);
        }
    }
}