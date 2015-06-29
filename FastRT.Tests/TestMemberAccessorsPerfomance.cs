using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;

namespace FastRT.Tests
{
    class TestMemberAccessorsPerfomance
    {
        [Test]
        public void TestPerfomance()
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

    }
}
