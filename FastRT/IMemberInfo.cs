using System;

namespace FastRT
{
    public interface IMemberInfo 
    {
        string MemberName { get; }
        Type MemberType { get; }
        Type ObjectType { get; }
    }
}