using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastRT.Tests
{
    [TestFixture]
    class TestObjectAccessorBehaviour
    {
        [Test]
        public void ShouldWorkWithMemberNames()
        {
            TestGenA a = new TestGenA();
            var now = DateTime.Now;
            a.DateProp = now;
            Assert.That(a["DateProp"], Is.EqualTo(now));

            a.Interval = null;
            Assert.That(a["Interval"], Is.Null);

            a.Interval = TimeSpan.FromSeconds(34);
            Assert.That(a["Interval"], Is.EqualTo(TimeSpan.FromSeconds(34)));
        }

        [Test]
        public void ShouldWorkWithMemberIndexes()
        {
            TestGenA a = new TestGenA();
            a[0] = "test";
            a[1] = 48;
            a[2] = 34.56;
            a[3] = new DateTime(2015, 1, 10);
            a[4] = TimeSpan.FromMinutes(3);

            Assert.That(a.PropString, Is.EqualTo("test"));
            Assert.That(a.PropInt, Is.EqualTo(48));
            Assert.That(a.Field1, Is.EqualTo(34.56));
            Assert.That(a.DateProp, Is.EqualTo(new DateTime(2015, 1, 10)));
            Assert.That(a.Interval, Is.EqualTo(TimeSpan.FromMinutes(3)));
        }

        [Test]
        public void ShouldntMixMembersDifferentClasses()
        {
            TestGenA a = new TestGenA();
            TestGenB b = new TestGenB();
            a["PropInt"] = 48;
            a[0] = "data";
            b[0] = "test";

            Assert.Throws<IndexOutOfRangeException>(() => { b[1] = 34; });
        }
    }
}
