using System;

namespace FastRT
{
    public interface ITypeResolver
    {
        string Name { get; }
        Type ResolveType();
    }
}