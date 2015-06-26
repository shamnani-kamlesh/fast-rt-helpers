using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastRT.Tests
{
    public static class TypeGenerator
    {
        public static Type MakeType(IDictionary<string, Type> typeDef, string typeName = null)
        {
            throw new NotImplementedException();
        }

        public static object MakeObject(IDictionary<string, object> objDef, string typeName = null)
        {
            //all null values will be represented as properties with Object type
            throw new NotImplementedException();
        }

        public static IObjectGenerator MakeObjectGenerator(Type t)
        {
            throw new NotImplementedException();
        }
    }

    public interface IObjectGenerator
    {
        object NewObject(Dictionary<string, object> values);
        object NewObject();
        Type ObjectType { get; }
    }
}