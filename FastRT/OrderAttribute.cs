using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastRT
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int _order;
        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            _order = order;
        }

        public int Order { get { return _order; } }
    }
}
