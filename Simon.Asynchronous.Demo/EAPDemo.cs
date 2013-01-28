using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace Simon.Asynchronous.Demo
{
    public class EAPDemo
    {
        public static void Run()
        {
            Console.WriteLine("main thread is started.");
            string url = @"http://www.163.com";
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(UpdateProgress);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadCompleted);
                client.DownloadStringAsync(new Uri(url));
            }
            Console.WriteLine("main thread is completed.");
        }

        static void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("current progress:{0}", e.ProgressPercentage);
        }

        static void DownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string log = "AsyncRun:download_completed";
            log += "|cancel=" + e.Cancelled.ToString();
            if (e.Error != null)
            {
                //出现异常，就记录异常
                log += "|error=" + e.Error.Message;
            }
            else
            {
                //没有出现异常，则记录结果
                log += "|result_size=" + e.Result.Length;
            }
            Console.WriteLine(e.Result);
            Console.WriteLine(log);
        }
    }

}
