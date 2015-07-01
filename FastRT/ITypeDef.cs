using System.Collections.Generic;

namespace FastRT
{
    public interface ITypeDef : ITypeResolver
    {
        IEnumerable<KeyValuePair<string, ITypeResolver>> PropertyDefList { get; }
    }

    public interface IListDef : ITypeResolver
    {
        ITypeResolver ElementType { get; }    
    }
}