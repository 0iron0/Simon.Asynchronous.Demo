using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Simon.Asynchronous.Demo
{
    public class BackgroupWorkerDemo
    {
        public static void Run()
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,//允许report进度，默认false
                WorkerSupportsCancellation = true//允许cancel命令，默认false
            };

            worker.DoWork += (sender, e) =>
            {
                for (int i = 0; i <= 100; i += 10)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    worker.ReportProgress(i);
                    Thread.Sleep(500);
                }
            };

            worker.ProgressChanged += (sender, e) =>
            {
                Console.WriteLine("progress:{0}", e.ProgressPercentage);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                {
                    Console.WriteLine("cancellation");
                }
                else if (e.Error != null)
                {
                    Console.WriteLine("error:{0}", e.Error.ToString());
                }
                else
                {
                    Console.WriteLine(e.Result ?? "Empty");//可以保存操作结果的对象
                    Console.WriteLine("finished.");
                }
            };

            worker.RunWorkerAsync();

            //if (worker.IsBusy)
            //    worker.CancelAsync();

            Console.WriteLine("main thread completed.");
        }
    }
}
