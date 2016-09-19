using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace FastRT.Tests
{
    [TestFixture]
    public class TestMemberValueAccessor
    {
        private static TestA CreateTestObject()
        {
            TestA a1 = new TestA { PropString = "test", Field1 = 23.34, PropInt = 48 };
            return a1;
        }

        [Test]
        public void ShouldReadKnownProperties()
        {
            var mva = new MemberValueAccessor(typeof(TestA).GetProperties());
            var a1 = CreateTestObject();

            mva.GetValue("PropString", a1).Should().Be("test");
            mva.GetValue("PropInt", a1).Should().Be(48);
        }

        [Test]
        public void ShouldReadKnownFields()
        {
            var mva = new MemberValueAccessor(typeof(TestA).GetFields());
            var a1 = CreateTestObject();

            mva.GetValue("Field1", a1).Should().Be(23.34);
        }

        [Test]
        public void ShouldWriteKnownProperties()
        {
            var mva = new MemberValueAccessor(typeof(TestA).GetProperties());
            var a1 = CreateTestObject();
            mva.SetValue("PropString", a1, "new value");
            mva.GetValue("PropString", a1).Should().Be("new value");

            mva.SetValue("PropInt", a1, 10);
            mva.GetValue("PropInt", a1).Should().Be(10);
        }

        [Test]
        public void ShouldWriteKnownFields()
        {
            var mva = new MemberValueAccessor(typeof(TestA).GetFields());
            var a1 = CreateTestObject();

            mva.SetValue("Field1", a1, 0.25);
            mva.GetValue("Field1", a1).Should().Be(0.25);
        }

    }
}
