using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Simon.Asynchronous.Demo
{
    public class ParallelDemo
    {
        public static void Run()
        {
            //Case1();

            //Case2();

            Case3();
        }

        static void ActionTest(string value)
        {
            Console.WriteLine("current thread id:{0}, value:{1}",
                Thread.CurrentThread.ManagedThreadId,
                value);
        }

        static void Case1()
        {
            Action[] actions = new Action[]
            {
                ()=>ActionTest("test1"),
                ()=>ActionTest("test2"),
                ()=>ActionTest("test3"),
                ()=>ActionTest("test4"),
                ()=>ActionTest("test5")
            };
            Parallel.Invoke(actions);
            Console.WriteLine("Invocation completed.");
        }

        static void Case2()
        {
            string[] parameters = { "a1", "a2", "a3", "a4", "a5" };
            Parallel.ForEach(parameters, (parameter) => ActionTest(parameter));
            Console.WriteLine("parallel.foreach is completed.");

            Parallel.For(0, parameters.Length, (index) => ActionTest(parameters[index]));
            Console.WriteLine("parallel.for is completed.");

            parameters.ParallelForEach((parameter) => ActionTest(parameter));
            Console.WriteLine("extension method is completed.");
        }

        static void Case3()
        {
            List<string> data = new List<string>();
            for (int i = 0; i < 10000000; i++)
            {
                data.Add(i.ToString());
            }

            List<string> result = new List<string>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ConcurrentStack<string> concurrentResult = new ConcurrentStack<string>();//线程安全集合:ConcurrentStack/ConcurrentQueue/ConcurrentDictionary
            Parallel.For(0, data.Count - 1, (i) =>
            {
                string item = data[i];
                if (item.Equals(1000000) || item.Equals(10000000))
                    concurrentResult.Push(item);
            });
            sw.Stop();
            Console.WriteLine("duration:{0}", sw.Elapsed);

            sw.Reset();
            sw.Start();
            //sw.Restart();
            foreach (string item in data)
            {
                if (item.Equals(1000000) || item.Equals(10000000))
                    result.Add(item);
            }
            sw.Stop();
            Console.WriteLine("duration:{0}", sw.Elapsed);
        }
    }

    public static class ParallelExtention
    {
        public static void ParallelForEach<T>(this IEnumerable<T> source, Action<T> hanlder)
        {
            Parallel.ForEach(source, hanlder);
        }
    }
}
