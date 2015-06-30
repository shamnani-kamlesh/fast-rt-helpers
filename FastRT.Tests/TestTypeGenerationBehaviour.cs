using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;

namespace FastRT.Tests
{
    class TestTypeGenerationBehaviour
    {
        [Test]
        public void ShouldCheckIfTypeAlreadyExists()
        {
            Dictionary<string, Type> typeDef = new Dictionary<string, Type>
            {
                {"StringProp", typeof(String)},
                {"IntProp", typeof(int)},
                {"DateProp", typeof(DateTime)},
                {"DoubleNullableProp", typeof(double?)}
            };

            Type t = TypeGenerator.MakeType("TestClassA", typeDef);
            Assert.That(t, Is.Not.Null);

            Type t2 = TypeGenerator.GetType("TestClassA");
            Assert.That(t2, Is.Not.Null);
            Assert.Throws<ArgumentException>(() => TypeGenerator.MakeType("TestClassA", typeDef));

            Assert.That(t2, Is.EqualTo(t));
        }

        [Test]
        public void ShouldGenerateTypeFromData()
        {
            Dictionary<string, Type> typeDef = new Dictionary<string, Type>
            {
                {"StringProp", typeof(String)},
                {"IntProp", typeof(int)},
                {"DateProp", typeof(DateTime)},
                {"DoubleNullableProp", typeof(double?)}
            };

            Type t = TypeGenerator.MakeType("NewTypeA", typeDef);
            Assert.That("NewTypeA", Is.EqualTo(t.Name));

            var pi = t.GetField("StringProp");
            Assert.That(typeof(string), Is.EqualTo(pi.FieldType));

            pi = t.GetField("IntProp");
            Assert.That(typeof(int), Is.EqualTo(pi.FieldType));

            pi = t.GetField("DateProp");
            Assert.That(typeof(DateTime), Is.EqualTo(pi.FieldType));

            pi = t.GetField("DoubleNullableProp");
            Assert.That(typeof(double?), Is.EqualTo(pi.FieldType));

            IObjectAccessor a = (IObjectAccessor)Activator.CreateInstance(t);
            a["StringProp"] = "test";
            string s = (string)a["StringProp"];
            Assert.That(s, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldGenerateUniqueTypeIfNameEmpty()
        {
            Dictionary<string, Type> typeDef = new Dictionary<string, Type>
            {
                {"StringProp", typeof(String)},
                {"IntProp", typeof(int)},
                {"DateProp", typeof(DateTime)},
                {"DoubleNullableProp", typeof(double?)}
            };

            var t1 = TypeGenerator.MakeType(null, typeDef);
            var t2 = TypeGenerator.MakeType(null, typeDef);

            Assert.That(t1.Name, Is.Not.EqualTo(t2.Name));
        }

        [Test]
        public void ShouldGenerateObjectFromData()
        {
            var now = DateTime.Now;
            Dictionary<string, object> objDef = new Dictionary<string, object>
            {
                {"StringProp", "test data"},
                {"IntProp", 48},
                {"DateProp", now},
                {"DoubleProp", 45.98}
            };

            var obj = TypeGenerator.MakeObject("NewTypeB", objDef);
            Type t = obj.GetType();
            Assert.That("NewTypeB", Is.EqualTo(t.Name));

            var pi = t.GetField("StringProp");
            Assert.That(typeof(string), Is.EqualTo(pi.FieldType));
            Assert.That(pi.GetValue(obj), Is.EqualTo("test data"));

            pi = t.GetField("IntProp");
            Assert.That(typeof(int), Is.EqualTo(pi.FieldType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(48));

            pi = t.GetField("DateProp");
            Assert.That(typeof(DateTime), Is.EqualTo(pi.FieldType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(now));

            pi = t.GetField("DoubleProp");
            Assert.That(typeof(double), Is.EqualTo(pi.FieldType));
            Assert.That(pi.GetValue(obj), Is.EqualTo(45.98));
        }

        [Test]
        public void ShouldBeJsonSerializable()
        {
            var now = DateTime.Now;
            Dictionary<string, object> objDef = new Dictionary<string, object>
            {
                {"StringProp", "test data"},
                {"IntProp", 48},
                {"DateProp", now},
                {"DoubleProp", 45.98},
                {"ArrayProp", new int[]{1,3,5,7}},
                {"ListProp", new List<string>(new [] {"a", "b", "c"})}
            };
   
            var obj = TypeGenerator.MakeObject(null, objDef);

            string s = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Console.WriteLine(s);

            var obj2 = (IObjectAccessor)JsonConvert.DeserializeObject(s, obj.GetType());

            Assert.That(obj2["StringProp"], Is.EqualTo("test data"));
            Assert.That(obj2["IntProp"], Is.EqualTo(48));
            Assert.That(obj2["DateProp"], Is.EqualTo(now));
            Assert.That(obj2["DoubleProp"], Is.EqualTo(45.98));
            Assert.That(obj2.GetValue<int[]>("ArrayProp").Length, Is.EqualTo(4));
            Assert.That(obj2.GetValue<List<string>>("ListProp").Count, Is.EqualTo(3));
        }

        [Test]
        public void ShouldGenerateReferencedTypes()
        {
            var elTypeDef = TypeGenerator.MakeTypeDef("TElement", new Dictionary<string, Type>
            {
                {"StringProp", typeof(String)},
                {"IntProp", typeof(int)},
                {"DateProp", typeof(DateTime)},
                {"DoubleNullableProp", typeof(double?)}
            });

            var pointTypeDef = TypeGenerator.MakeTypeDef("TPoint", new Dictionary<string, Type>
            {
                {"X", typeof(double)},
                {"Y", typeof(double)}
            });

            var objTypeDef = TypeGenerator.MakeTypeDef("TRoot", new Dictionary<string, Type>
            {
                {"ListProp", TypeGenerator.MakeListDef(elTypeDef)},
                {"Center", pointTypeDef.AsType()},
                {"Parent", TypeGenerator.MakeTypeRef("TRoot")},
                {"Nodes", TypeGenerator.MakeListDef(TypeGenerator.MakeTypeRef("TRoot"))}
            });

            IObjectFactory ta = objTypeDef.MakeObjectFactory();
            IObjectFactory elTA = elTypeDef.MakeObjectFactory();
            IObjectFactory sTA = pointTypeDef.MakeObjectFactory();

            IObjectAccessor obj = ta.NewObject();

            var list = elTA.NewList();
            list.Add(elTA.NewObject());
            list.Add(elTA.NewObject());
            list.Add(elTA.NewObject());
            
            obj.SetValue("ListProp", list);
            foreach (var el in obj.GetValue<IEnumerable<IObjectAccessor>>("ListProp"))
            {
                Assert.That(el.GetType().Name, Is.EqualTo("TElement"));
                el[0] = "test";
            }

            IObjectAccessor st = sTA.NewObject();
            st.SetValue("X", 10.5);
            st.SetValue("Y", 48.96);

            obj.SetValue("Center", st);

            var parent = ta.NewObject();
            obj.SetValue("Parent", parent);

            var nodes = ta.NewList();
            nodes.Add(obj);

            parent.SetValue("Nodes", nodes);
        }
    }

}
