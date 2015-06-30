using System;
using System.Collections.Generic;

namespace FastRT
{
    public interface ITypeDef
    {
        string Name { get; }
        IEnumerable<KeyValuePair<string, Type>> PropertyDefList { get; }
        Type AsType();
        IObjectFactory MakeObjectFactory();
    }
}