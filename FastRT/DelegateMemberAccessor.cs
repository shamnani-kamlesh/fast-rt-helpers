using System;
using FastRT.Impl;

namespace FastRT
{
    /// <summary>
    /// DelegateMemberAccessor class can be used at runtime to quickly access a members value of an object
    /// </summary>
    /// <typeparam name="TObjectType">The System Type of the Object which the member belongs to.</typeparam>
    /// <typeparam name="TMemberType">The System Type of the Member being accessed.</typeparam>
    public sealed class DelegateMemberAccessor<TObjectType, TMemberType> : IMemberAccessor<TObjectType, TMemberType>, IMemberInfo
    {
        private readonly Func<TObjectType, TMemberType> _delMemberGet;
        private readonly Action<TObjectType, TMemberType> _delMemberSet;

        public string MemberName { get; private set; }
        public Type MemberType { get; private set; }
        public Type ObjectType { get; private set; }

        public DelegateMemberAccessor(string memberName, bool readOnly = false, IObjectCache<string> memberCache = null)
        {
            MemberName = memberName;
            MemberType = typeof(TMemberType);
            ObjectType = typeof(TObjectType);
            _delMemberGet = RuntimeDelegateFactory.RetrieveMemberGetValueDelegate<TObjectType, TMemberType>(memberName, memberCache);
            if(!readOnly)
                _delMemberSet = RuntimeDelegateFactory.RetrieveMemberSetValueDelegate<TObjectType, TMemberType>(memberName, memberCache);
        }

        public bool HasGetter
        {
            get { return _delMemberGet != null; }
        }

        public bool HasSetter
        {
            get { return _delMemberSet != null; }
        }

        object IMemberAccessor.GetValue(object instance)
        {
            return GetValue((TObjectType) instance);
        }

        void IMemberAccessor.SetValue(object instance, object value)
        {
            SetValue((TObjectType) instance, (TMemberType) value);
        }

        public TMemberType GetValue(TObjectType instance)
        {
            if (_delMemberGet != null)
                return _delMemberGet(instance);

            throw new InvalidOperationException(String.Format(
                "Member: '{0}' of Type: '{1}' does not have a public Get accessor",
                MemberName, ObjectType.Name));
        }

        public void SetValue(TObjectType instance, TMemberType value)
        {
            if (_delMemberSet != null)
                _delMemberSet(instance, value);
            else
                throw new InvalidOperationException(String.Format(
                    "Member: '{0}' of Type: '{1}' does not have a public Set accessor",
                    MemberName, ObjectType.Name));
        }

        public override string ToString()
        {
            return ObjectType.FullName + "." + MemberName;
        }
    }
}