using System;
using System.Collections;

namespace FastRT
{
    public interface IObjectFactory
    {
        T NewObject<T>();
        object NewObject();
        IList NewList();
        Type SystemType { get; }
    }
}