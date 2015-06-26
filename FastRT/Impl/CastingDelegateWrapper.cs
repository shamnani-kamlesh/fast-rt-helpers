using System;

namespace FastRT.Impl
{
    /// <summary>
    /// Wrapper class that is needed for type casting during property access via generic delegates when the exact property type does not match the requested delegate type
    /// </summary>
    /// <typeparam name="TObjectType">property's OwnerType</typeparam>
    /// <typeparam name="TMemberType">property's type</typeparam>
    internal class CastingDelegateWrapper<TObjectType, TMemberType> : IMemberAccessDelegateProvider<TObjectType, Object>
    {
        private readonly RuntimeDelegateFactory.MemberGetDelegate<TObjectType, TMemberType> _delegateGet;
        private readonly RuntimeDelegateFactory.MemberSetDelegate<TObjectType, TMemberType> _delegateSet;
        private readonly object _defaultValue;

        public CastingDelegateWrapper(RuntimeDelegateFactory.MemberGetDelegate<TObjectType, TMemberType> del)
        {
            _delegateGet = del;
            _delegateSet = null;
        }

        public CastingDelegateWrapper(RuntimeDelegateFactory.MemberSetDelegate<TObjectType, TMemberType> del)
        {
            _delegateGet = null;
            _delegateSet = del;

            if (typeof(TMemberType).IsValueType)
                _defaultValue = Activator.CreateInstance<TMemberType>();
        }

        private object InvokeGet(TObjectType obj)
        {
            if (_delegateGet != null)
                return _delegateGet(obj);
            return null;
        }

        private void InvokeSet(TObjectType obj, Object value)
        {
            if (_delegateSet != null)
            {
                if (value == null)
                    value = _defaultValue;
                _delegateSet(obj, (TMemberType)value);
            }
        }

        public RuntimeDelegateFactory.MemberGetDelegate<TObjectType, object> GetMemberGetDelegate()
        {
            return InvokeGet;
        }

        public RuntimeDelegateFactory.MemberSetDelegate<TObjectType, object> GetMemberSetDelegate()
        {
            return InvokeSet;
        }
    }
}
