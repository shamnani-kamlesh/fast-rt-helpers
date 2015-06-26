namespace FastRT.Impl
{
    internal interface IMemberAccessDelegateProvider<in TObjectType, TMemberType>
    {
        RuntimeDelegateFactory.MemberGetDelegate<TObjectType, TMemberType> GetMemberGetDelegate();
        RuntimeDelegateFactory.MemberSetDelegate<TObjectType, TMemberType> GetMemberSetDelegate();
    }
}