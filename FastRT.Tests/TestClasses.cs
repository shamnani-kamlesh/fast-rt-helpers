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
        [Order]
        public string PropString { get; set; }
        [Order]
        public int PropInt { get; set; }
        [Order]
        public double? Field1;
        [Order]
        public DateTime DateProp { get; set; }
        [Order]
        public TimeSpan? Interval { get; set; }
    }

    public class TestGenB : ObjectAccessorBase<TestGenB>
    {
        [Order]
        public string StringProp1 { get; set; }
    }

}