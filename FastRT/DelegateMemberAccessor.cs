using FastRT.Impl;

namespace FastRT
{
    /// <summary>
    /// DelegateMemberAccessor class can be used at runtime to quickly access a members value of an object
    /// </summary>
    /// <typeparam name="TObjectType">The System Type of the Object which the member belongs to.</typeparam>
    /// <typeparam name="TMemberType">The System Type of the Member being accessed.</typeparam>
    public sealed class DelegateMemberAccessor<TObjectType, TMemberType> : FuncMemberAccessor<TObjectType, TMemberType>
    {
        private readonly string _memberName;
        //TODO: make this class internal (update all references in mobileezy)!!!

        public DelegateMemberAccessor(string memberName, bool readOnly = false, IObjectCache<string> memberCache = null) 
            :base(
            RuntimeDelegateFactory.RetrieveMemberGetValueDelegate<TObjectType, TMemberType>(memberName, memberCache),
            readOnly ? null : RuntimeDelegateFactory.RetrieveMemberSetValueDelegate<TObjectType, TMemberType>(memberName, memberCache),
            true)
        {
            _memberName = memberName;
        }

        public override string MemberName
        {
            get { return _memberName; }
        }

        public override string ToString()
        {
            return ObjectType.FullName + "." + MemberName;
        }
    }
}