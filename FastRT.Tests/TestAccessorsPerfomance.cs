using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;

namespace FastRT.Tests
{
    class TestAccessorsPerfomance
    {
        [Test]
        public void TestMemberAccessorsPerfomance()
        {
            const int total = 1000000;
            TestA a = new TestA{PropString = "test"};

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < total; i++)
            {
                string v = a.PropString;
                a.PropString = "new data";
            }
            sw.Stop();

            Console.WriteLine("Native C# time: " + sw.ElapsedMilliseconds);

            IObjectMemberAccessor<string> oma = a.GetObjectMemberAccessor(x => x.PropString);
            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                string v = oma.Value;
                oma.Value = "new data";
            }
            sw.Stop();

            Console.WriteLine("FastRT time: " + sw.ElapsedMilliseconds);

            PropertyInfo pi = a.GetType().GetProperty("PropString");

            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                string v = (string) pi.GetValue(a, null);
                pi.SetValue(a, "new data", null);
            }
            sw.Stop();

            Console.WriteLine("Reflection time: " + sw.ElapsedMilliseconds);

        }

        [Test]
        public void TestMemberValueAccessorsPerfomance()
        {
            const int total = 1000000;
            TestA a = new TestA { PropString = "test", PropInt = 48 };

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < total; i++)
            {
                string v = a.PropString;
                a.PropString = "new data";
                int i1 = a.PropInt;
                a.PropInt = i;
            }
            sw.Stop();

            Console.WriteLine("Native C# time: " + sw.ElapsedMilliseconds);

            var mva = new MemberValueAccessor(typeof(TestA).GetProperties());
            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                object v = mva.GetValue("PropString", a);
                mva.SetValue("PropString", a, "new data");

                object i1 = mva.GetValue("PropInt", a);
                mva.SetValue("PropInt", a, i);
            }
            sw.Stop();

            Console.WriteLine("FastRT time: " + sw.ElapsedMilliseconds);

            PropertyInfo pStr = a.GetType().GetProperty("PropString");
            PropertyInfo pInt = a.GetType().GetProperty("PropInt");

            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                object v = pStr.GetValue(a, null);
                pStr.SetValue(a, "new data", null);

                object i1 = pInt.GetValue(a, null);
                pInt.SetValue(a, i);
            }
            sw.Stop();

            Console.WriteLine("Reflection time: " + sw.ElapsedMilliseconds);

        }


    }
}
