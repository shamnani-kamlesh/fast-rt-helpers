using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FastRT.Impl
{
    public sealed class ObjectFactory : IObjectFactory
    {
        private Func<object> _ctorFunc;
        private Func<object> _listCtorFunc;

        public ObjectFactory(Type systemType)
        {
            if(systemType == null)
                throw new ArgumentNullException("systemType");
            if(!typeof(IObjectAccessor).IsAssignableFrom(systemType))
                throw new ArgumentException("Type must implement IObjectAccessor interface: " + systemType.FullName, "systemType");

            SystemType = systemType;            
        }

        public IObjectAccessor NewObject()
        {
            if (_ctorFunc == null)
                _ctorFunc = MakeCtorFunc(SystemType);
            return (IObjectAccessor) _ctorFunc();
        }

        public IList NewList()
        {
            if (_listCtorFunc == null)
                _listCtorFunc = MakeListCtorFunc();
            return (IList) _listCtorFunc();
        }

        public Type SystemType { get; private set; }

        private static Func<object> MakeCtorFunc(Type t)
        {
            var newExpr = Expression.New(t);
            return Expression.Lambda<Func<object>>(newExpr).Compile();
        }

        private Func<object> MakeListCtorFunc()
        {
            Type listType = typeof(List<>).MakeGenericType(SystemType);
            return MakeCtorFunc(listType);
        }
    }
}