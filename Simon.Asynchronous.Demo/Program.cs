using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simon.Asynchronous.Demo
{
    //******************************************//
    //.net4.5 异步编程 支持三种模式：
    //APM, Asynchronous Programming Model, 异步编程模型
    //EAP, Event-based Asynchronous Pattern, 基于事件的异步编程模式
    //TAP, Task-based Asychronous Pattern, 基于任务的异步编程模式
    //******************************************//
    //APM:Beginxxx()/Endxxx()操作, 委托操作/流操作
    //EAP:WebClient/BackgroupWorker
    //TAP:Task/TaskFactory/Parallel
    //******************************************//
    class Program
    {
        delegate void PrintDelegate(string s);
        static void Main(string[] args)
        {
            //Case1();

            //Case2();

            //Case3();

            //Case4();

            //EAPDemo.Run();

            //WebClientDemo.Run();

            //BackgroupWorkerDemo.Run();

            //TaskDemo.Run();

            //TaskFactoryDemo.Run();

            //ParallelDemo.Run();

            Console.ReadKey();
        }

        static void Case1()
        {
            Console.WriteLine("main thread is started.");

            PrintDelegate print = new PrintDelegate(Print);
            IAsyncResult result = print.BeginInvoke("hello world.", null, null);

            Console.WriteLine("main thread is going.");

            print.EndInvoke(result);//EndInvode()负责一直阻塞当前线程，等待返回结果后继续执行

            Console.WriteLine("compeleted.");
        }

        static void Case2()
        {
            Console.WriteLine("main thread is started.");

            PrintDelegate print = new PrintDelegate(Print);
            IAsyncResult result = print.BeginInvoke("hello world.", null, null);

            Console.WriteLine("main thread is going.");

            result.AsyncWaitHandle.WaitOne(-1, false);//同样阻塞当前线程，等待结果返回

            Console.WriteLine("compeleted.");
        }

        static void Case3()
        {
            Console.WriteLine("main thread is started.");

            PrintDelegate print = new PrintDelegate(Print);
            IAsyncResult result = print.BeginInvoke("hello world.", null, null);

            Console.WriteLine("main thread is going.");

            while (!result.IsCompleted)//轮询执行是否完成
            {
                Console.WriteLine("waiting...");
                Thread.Sleep(200);
            }

            Console.WriteLine("compeleted.");
        }

        static void Case4()
        {
            Console.WriteLine("main thread is started.");

            PrintDelegate print = new PrintDelegate(Print);
            IAsyncResult result = print.BeginInvoke("hello world.", new AsyncCallback(PrintCompleted), print);//使用回调的方式，不用阻塞当前线程

            Console.WriteLine("main thread is going.");

            Console.WriteLine("compeleted.");
        }

        static void Print(string s)
        {
            Console.WriteLine("async thread is running.");
            Thread.Sleep(2000);
            Console.WriteLine("output string:{0}", s);
            Thread.Sleep(2000);
        }

        static void PrintCompleted(IAsyncResult result)
        {
            (result.AsyncState as PrintDelegate).EndInvoke(result);
            Console.WriteLine("async thread is completed.");
        }
    }
}
