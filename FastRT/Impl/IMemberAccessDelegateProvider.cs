using System;
namespace FastRT.Impl
{
    internal interface IMemberAccessDelegateProvider<in TObjectType, TMemberType>
    {
        Func<TObjectType, TMemberType> GetMemberGetDelegate();
        Action<TObjectType, TMemberType> GetMemberSetDelegate();
    }
}