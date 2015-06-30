using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection.Emit;
using NUnit.Framework;

namespace FastRT.Tests
{
    [TestFixture]
    class TestActivationPerfomance
    {
        [Test]
        public void TestCreateObjects()
        {
            const int total = 100000;
            object[] resultArray = new object[total];
            long time;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < total; i++)
            {
                resultArray[i] = Activator.CreateInstance(typeof (TestClassX));
            }

            time = sw.ElapsedTicks;
            Console.WriteLine("Activator.CreateInstance: " + time);
            GC.Collect();

            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                resultArray[i] = new TestClassX();
            }

            time = sw.ElapsedTicks;
            Console.WriteLine("Static C#: " + time);
            GC.Collect();

            var ci = typeof(TestClassX).GetConstructor(new Type[0]);
            DynamicMethod dm = new DynamicMethod("ctor", typeof(TestClassX), new Type[0]);
            var ilgen = dm.GetILGenerator();
            ilgen.Emit(OpCodes.Newobj, ci);
            ilgen.Emit(OpCodes.Ret);
            Func<TestClassX> ctorFunc = (Func<TestClassX>)dm.CreateDelegate(typeof(Func<TestClassX>));

            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                resultArray[i] = ctorFunc();
            }

            time = sw.ElapsedTicks;
            Console.WriteLine("DynamicMethod: " + time);
            GC.Collect();

            var newExpr = Expression.New(typeof (TestClassX));

            var del2 = Expression.Lambda<Func<object>>(newExpr).Compile();
            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                resultArray[i] = del2();
            }

            time = sw.ElapsedTicks;
            Console.WriteLine("Expression.New (untyped): " + time);
            GC.Collect();


            var del = Expression.Lambda<Func<TestClassX>>(newExpr).Compile();
            sw.Restart();
            for (int i = 0; i < total; i++)
            {
                resultArray[i] = del();
            }

            time = sw.ElapsedTicks;
            Console.WriteLine("Expression.New (typed): " + time);
            GC.Collect();



        }
    }

    public class TestClassX
    {
        public int X;
        public string S;
    }
}
