using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Simon.Asynchronous.Demo
{
    public class TaskDemo
    {
        public static void Run()
        {
            Case1();
            //Case2();
            //Case3();
            //Case4();
            //Case5();
            //Case6();
        }

        static void Case1()
        {
            Task task = new Task(() =>
            {
                Console.WriteLine("i am a task. thread id:{0}", Thread.CurrentThread.ManagedThreadId);
            });

            //task.Start();//异步执行
            task.RunSynchronously();//同步执行

            Console.WriteLine("main thread id:{0}", Thread.CurrentThread.ManagedThreadId);
        }

        static void Case2()
        {
            Task task = new Task((str) =>
            {
                Console.WriteLine(str);
            }, "i am param");
            task.Start();
        }

        static void Case3()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            long i = 0;
            Task task = new Task((token) =>
            {
                while (true)
                {
                    if (((CancellationToken)token).IsCancellationRequested)
                    {
                        break;
                    }
                    i++;
                    Console.WriteLine(i.ToString());
                    Thread.Sleep(100);
                }
            }, tokenSource.Token);

            task.Start();
            Console.WriteLine("task is started.");

            Thread.Sleep(500);//阻塞了主线程

            tokenSource.Cancel();
            Console.WriteLine("main thread is completed.");
        }

        static void Case4()
        {
            Task t1 = new Task(() =>
            {
                Thread.Sleep(new Random().Next(100, 500));
                Console.WriteLine("t1 id:{0}", Thread.CurrentThread.ManagedThreadId);
            });

            Task t2 = new Task(() =>
            {
                Thread.Sleep(new Random().Next(100, 500));
                Console.WriteLine("t2 id:{0}", Thread.CurrentThread.ManagedThreadId);
            });

            Task t3 = new Task(() =>
            {
                Thread.Sleep(new Random().Next(100, 500));
                Console.WriteLine("t3 id:{0}", Thread.CurrentThread.ManagedThreadId);
            });
            t1.Start();
            t2.Start();
            t3.Start();

            //Task.WaitAll(t1,t2,t3);//等待参数中的task结束后继续主线程
            Task.WaitAny(t1, t2, t3);

            Console.WriteLine("main thread completed.");
        }

        static void Case5()
        {
            Task task1 = new Task(() =>
            {
                throw new ArgumentException("args");
            });
            task1.Start();

            Task task2 = new Task(() =>
            {
                throw new ArgumentNullException("args is null");//exceiption will be not throwed.
            });
            task2.Start();

            try
            {
                Task.WaitAll(task1, task2);//it will throw Aggregate exception
            }
            catch (AggregateException e)
            {
                foreach (var inner in e.InnerExceptions)
                {
                    Console.WriteLine(inner.ToString());
                }
                e.Handle((inner) =>
                {
                    if (inner is ArgumentNullException)
                    {
                        Console.WriteLine("args are wrong.");
                        return true;
                    }
                    else if (inner is ArgumentException)
                    {
                        Console.WriteLine("args is null");
                        return true;
                    }
                    return false;
                });
            }
        }

        static void Case6()
        {
            Task<int> task = new Task<int>(() =>//Task<T>表示返回值
            {
                int rs = 0;
                for (int i = 0; i < 100; i++)
                {
                    rs++;
                }
                return rs;
            });
            task.Start();
            Console.WriteLine(task.Result.ToString());
        }
    }
}
