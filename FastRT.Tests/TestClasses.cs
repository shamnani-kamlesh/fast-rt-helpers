using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FastRT.Tests
{
    public class TestA
    {
        public string PropString { get; set; }
        public int PropInt { get; set; }
        public double Field1;
    }

    public class TestGenA : ObjectAccessorBase<TestGenA>
    {
        [Order(0)]
        public string PropString { get; set; }
        [Order(1)]
        public int PropInt { get; set; }
        [Order(2)]
        public double? Field1;
        [Order(3)]
        public DateTime DateProp { get; set; }
        [Order(4)]
        public TimeSpan? Interval { get; set; }
    }

    public class TestGenB : ObjectAccessorBase<TestGenB>
    {
        [Order]
        public string StringProp1 { get; set; }
    }

}