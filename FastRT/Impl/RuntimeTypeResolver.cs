using System;

namespace FastRT.Impl
{
    internal class RuntimeTypeResolver : ITypeResolver
    {
        private readonly Type _type;

        public RuntimeTypeResolver(Type type)
        {
            _type = type;
        }

        public string Name 
        {
            get { return _type.Name; }
        }

        public Type ResolveType()
        {
            return _type;
        }
    }
}