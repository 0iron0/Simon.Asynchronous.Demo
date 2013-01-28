using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Simon.Asynchronous.Demo
{
    public class RequestState
    {
        const int BUFFER_SIZE = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public HttpWebRequest Request;
        public Stream ResponseStream;
        // 创建适当编码类型的解码器
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();

        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            RequestData = new StringBuilder("");
            Request = null;
            ResponseStream = null;
        }
    }

    //**********************************//
    //WebRequest派生出HttpWebRequest/FileWebRequest/FtpWebRequest
    //WebResponse派生出HttpWebResponse/FileWebResponse/FtpWebResponse
    //**********************************//
    //WebClient封装了WebRequest和WebResponse
    //**********************************//
    public class WebClientDemo
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;

        public static void Run()
        {
            string url = @"http://www.163.com";

            //if (string.IsNullOrEmpty(url))
            //{
            //    showusage();
            //    return;
            //}

            // 从命令行获取 URI
            Uri HttpSite = new Uri(url);

            // 创建请求对象
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(HttpSite);

            // 创建状态对象
            RequestState rs = new RequestState();

            // 将请求添加到状态，以便它可以被来回传递
            rs.Request = wreq;

            // 发出异步请求
            IAsyncResult r = (IAsyncResult)wreq.BeginGetResponse(new AsyncCallback(RespCallback), rs);

            // 将 ManualResetEvent 设置为 Wait，
            // 以便在调用回调前，应用程序不退出
            allDone.WaitOne();
        }

        public static void showusage()
        {
            Console.WriteLine("尝试获取 (GET) 一个 URL");
            Console.WriteLine("\r\n用法：:");
            Console.WriteLine("ClientGetAsync URL");
            Console.WriteLine("示例：:");
            Console.WriteLine("ClientGetAsync http://www.microsoft.com/net/");
        }

        private static void RespCallback(IAsyncResult ar)
        {
            // 从异步结果获取 RequestState 对象
            RequestState rs = (RequestState)ar.AsyncState;

            // 从 RequestState 获取 HttpWebRequest
            HttpWebRequest req = rs.Request;

            // 调用 EndGetResponse 生成 HttpWebResponse 对象
            // 该对象来自上面发出的请求
            HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);

            // 既然我们拥有了响应，就该从
            // 响应流开始读取数据了
            Stream ResponseStream = resp.GetResponseStream();

            // 该读取操作也使用异步完成，所以我们
            // 将要以 RequestState 存储流
            rs.ResponseStream = ResponseStream;

            // 请注意，rs.BufferRead 被传入到 BeginRead。
            // 这是数据将被读入的位置。
            IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
        }


        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            // 从 asyncresult 获取 RequestState 对象
            RequestState rs = (RequestState)asyncResult.AsyncState;

            // 取出在 RespCallback 中设置的 ResponseStream
            Stream responseStream = rs.ResponseStream;

            // 此时 rs.BufferRead 中应该有一些数据。
            // 读取操作将告诉我们那里是否有数据
            int read = responseStream.EndRead(asyncResult);

            if (read > 0)
            {
                // 准备 Char 数组缓冲区，用于向 Unicode 转换
                Char[] charBuffer = new Char[BUFFER_SIZE];

                // 将字节流转换为 Char 数组，然后转换为字符串
                // len 显示多少字符被转换为 Unicode
                int len = rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);
                String str = new String(charBuffer, 0, len);

                // 将最近读取的数据追加到 RequestData stringbuilder 对象中，
                // 该对象包含在 RequestState 中
                rs.RequestData.Append(str);


                // 现在发出另一个异步调用，读取更多的数据
                // 请注意，将不断调用此过程，直到
                // responseStream.EndRead 返回 -1
                IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), rs);
            }
            else
            {
                if (rs.RequestData.Length > 1)
                {
                    // 所有数据都已被读取，因此将其显示到控制台
                    string strContent;
                    strContent = rs.RequestData.ToString();
                    Console.WriteLine(strContent);
                }

                // 关闭响应流
                responseStream.Close();

                // 设置 ManualResetEvent，以便主线程可以退出
                allDone.Set();
            }
            return;
        }
    }
}
