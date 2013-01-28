using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Simon.Asynchronous.Demo
{
    //**********************************//
    //TaskFactory就是对Task的封装，使操作更简单
    //**********************************//
    public class TaskFactoryDemo
    {
        public static void Run()
        {
            //Case1();
            //Case2();

            Case3();
        }

        private static void Case1()
        {
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("i am started via a task factory.");
            });

            var task = Task.Factory.StartNew<string>(() => "i am a return value.");
            Console.WriteLine(task.Result);
        }

        private static void Case2()
        {
            //第一种方式:
            var parent = Task.Factory.StartNew(() =>
            {
                var nonChildTask = Task.Factory.StartNew(
                    () => Console.WriteLine("I'm not a child task. thread id:{0}", Thread.CurrentThread.ManagedThreadId)
                );
                var childTask = Task.Factory.StartNew(
                    () => Console.WriteLine("I'm a child task. thread id:{0}", Thread.CurrentThread.ManagedThreadId),
                TaskCreationOptions.AttachedToParent);

                Console.WriteLine("I'm parent task. thread id:{0}", Thread.CurrentThread.ManagedThreadId);
            });
        }

        private static void Case3()
        {
            var tasks = new Task[3];
            for (int i = 0; i < tasks.Length; i++)
            {
                int taskIndex = i;
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    int seed = Guid.NewGuid().GetHashCode();
                    int waitTime = new Random(seed).Next(10, 100);
                    Thread.Sleep(waitTime);
                    Console.WriteLine("Task{0} Finished", taskIndex);
                });
            }
            Task.WaitAny(tasks);
            Console.WriteLine("completed.");
        }
    }
}
