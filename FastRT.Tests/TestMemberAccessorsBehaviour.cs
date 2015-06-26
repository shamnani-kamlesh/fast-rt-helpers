using NUnit.Framework;

namespace FastRT.Tests
{
    public class TestMemberAccessorsBehaviour
    {
        [Test]
        public void ShouldWorkForKnownTypeAndKnownMember()
        {
            var a1 = CreateTestObject();
            IMemberAccessor<TestA, string> ma = TypeAccessor<TestA>.GetMemberAccessor(x => x.PropString);
            ShouldBehaveSimilar(ma, a1);


            IObjectMemberAccessor<double> oma = a1.GetObjectMemberAccessor(x => x.Field1);
            ShouldBehaveSimilar(oma, a1);
        }

        [Test]
        public void ShouldWorkForKnownTypeAndUnknownMember()
        {
            var a1 = CreateTestObject();
            IMemberAccessor<TestA, string> ma = TypeAccessor<TestA>.GetMemberAccessor<string>("PropString");
            ShouldBehaveSimilar(ma, a1);

            IObjectMemberAccessor<double> oma = a1.GetObjectMemberAccessor<TestA, double>("Field1");
            ShouldBehaveSimilar(oma, a1);
        }

        [Test]
        public void ShouldWorkForUnknownTypeAndUnknownMember()
        {
            var a1 = CreateTestObject();
            IMemberAccessor ma = TypeAccessor.GetMemberAccessor(typeof(TestA), "PropInt");
            Assert.That(ma.GetValue(a1), Is.EqualTo(a1.PropInt));            
            ma.SetValue(a1, a1.PropInt + 10);
            Assert.That(ma.GetValue(a1), Is.EqualTo(a1.PropInt));

            IObjectMemberAccessor oma = a1.GetObjectMemberAccessor("Field1");
            Assert.That(oma.GetValue(), Is.EqualTo(a1.Field1));
            oma.SetValue(12.56);
            Assert.That(oma.GetValue(), Is.EqualTo(a1.Field1));
            a1.Field1 -= 345;
            Assert.That(oma.GetValue(), Is.EqualTo(a1.Field1));
        }

        [Test]
        public void ShouldWorkForForcedObjectReturnType()
        {
            var a1 = CreateTestObject();
            IObjectMemberAccessor<object> oma = a1.GetObjectMemberAccessor<TestA, object>(x => x.Field1);
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));
            oma.Value = 12.56;
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));
            a1.Field1 -= 345;
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));            
        }

        private static void ShouldBehaveSimilar(IMemberAccessor<TestA, string> ma, TestA a1)
        {
            Assert.That(ma.GetValue(a1), Is.EqualTo(a1.PropString));
            ma.SetValue(a1, "updated " + a1.PropString);
            Assert.That(ma.GetValue(a1), Is.EqualTo(a1.PropString));
            a1.PropString = "new " + a1.PropString;
            Assert.That(ma.GetValue(a1), Is.EqualTo(a1.PropString));
        }

        private static void ShouldBehaveSimilar(IObjectMemberAccessor<double> oma, TestA a1)
        {
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));
            oma.Value += 12.56;
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));
            a1.Field1 -= 345;
            Assert.That(oma.Value, Is.EqualTo(a1.Field1));
        }


        private static TestA CreateTestObject()
        {
            TestA a1 = new TestA {PropString = "test", Field1 = 23.34, PropInt = 48};
            return a1;
        }

    }
}
