using System;
using System.Collections;

namespace FastRT
{
    public interface IObjectFactory
    {
        IObjectAccessor NewObject();
        IList NewList();
        Type SystemType { get; }
    }
}