using System;

namespace FastRT
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int _order;
        public OrderAttribute(int order = 0)
        {
            _order = order;
        }

        public int Order { get { return _order; } }
    }
}
