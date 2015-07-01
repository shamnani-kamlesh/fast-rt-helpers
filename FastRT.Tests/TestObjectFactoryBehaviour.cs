using System.Collections.Generic;
using NUnit.Framework;

namespace FastRT.Tests
{
    [TestFixture]
    class TestObjectFactoryBehaviour
    {
        [Test]
        public void ShouldCreateObjects()
        {
            IObjectFactory of = typeof(TestGenA).MakeObjectFactory();

            Assert.That(of.SystemType, Is.EqualTo(typeof(TestGenA)));

            var obj = of.NewObject<IObjectAccessor>();
            Assert.That(obj.GetType(), Is.EqualTo(typeof(TestGenA)));
            obj["PropInt"] = 48;
            int i = (int) obj["PropInt"];
            Assert.That(i, Is.EqualTo(48));

            of = typeof (TestA).MakeObjectFactory();
            var t = of.NewObject<TestA>();
            t.Field1 = 12.34;
        }

        [Test]
        public void ShouldCreateLists()
        {
            IObjectFactory of = typeof(TestGenA).MakeObjectFactory();
            var list = of.NewList();
            list.Add(of.NewObject());
            list.Add(of.NewObject());

            Assert.That(2, Is.EqualTo(list.Count));

            foreach (var obj in (IEnumerable<IObjectAccessor>)list)
            {
                obj["PropString"] = "test";
            }
        }
    }
}
