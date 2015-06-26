using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace FastRT.Tests
{
    class TestTypeGenerationBehaviour
    {
        [Test]
        public void ShouldGenerateTypeFromDictionary()
        {
            Dictionary<string, Type> typeDef = new Dictionary<string, Type>
            {
                {"StringProp", typeof(String)},
                {"IntProp", typeof(int)},
                {"DateProp", typeof(DateTime)},
                {"DoubleNullableProp", typeof(double?)}
            };

            Type t = TypeGenerator.MakeType(typeDef, "NewTypeA");
            Assert.That("NewTypeA", Is.EqualTo(t.Name));
            Assert.That(4, Is.EqualTo(t.GetMembers().Length));

            var pi = t.GetProperty("StringProp");
            Assert.That(typeof(string), Is.EqualTo(pi.PropertyType));

            pi = t.GetProperty("IntProp");
            Assert.That(typeof(int), Is.EqualTo(pi.PropertyType));

            pi = t.GetProperty("DateProp");
            Assert.That(typeof(DateTime), Is.EqualTo(pi.PropertyType));

            pi = t.GetProperty("DoubleNullableProp");
            Assert.That(typeof(double?), Is.EqualTo(pi.PropertyType));

        }

        [Test]
        public void ShouldGenerateTypeFromData()
        {
            var now = DateTime.Now;
            Dictionary<string, object> objDef = new Dictionary<string, object>
            {
                {"StringProp", "test data"},
                {"IntProp", 48},
                {"DateProp", now},
                {"DoubleProp", 45.98}
            };

            object obj = TypeGenerator.MakeObject(objDef, "NewTypeB");
            Type t = obj.GetType();
            Assert.That("NewTypeB", Is.EqualTo(t.Name));
            Assert.That(4, Is.EqualTo(t.GetMembers().Length));

            var pi = t.GetProperty("StringProp");
            Assert.That(typeof(string), Is.EqualTo(pi.PropertyType));
            Assert.That(pi.GetValue(obj), Is.EqualTo("test data"));

            pi = t.GetProperty("IntProp");
            Assert.That(typeof(int), Is.EqualTo(pi.PropertyType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(48));

            pi = t.GetProperty("DateProp");
            Assert.That(typeof(DateTime), Is.EqualTo(pi.PropertyType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(now));

            pi = t.GetProperty("DoubleProp");
            Assert.That(typeof(double), Is.EqualTo(pi.PropertyType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(45.98));
        }

        [Test]
        public void ShouldInstantiateObjectsFromData()
        {
            
        }

        [Test]
        public void ShouldPopulateObjectsFromData()
        {
            
        }

        [Test]
        public void ShouldSupportReferenceMemberTypes()
        {
            
        }

        [Test]
        public void ShouldSupportRecursiveMemberTypes()
        {
            
        }

        [Test]
        public void ShouldSupportStructs()
        {
            
        }
    }
}
