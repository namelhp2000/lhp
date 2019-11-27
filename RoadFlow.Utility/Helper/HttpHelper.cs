using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RoadFlow.Utility
{
    public static class HttpHelper
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpGet(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        string name = pair.Key;
                        client.DefaultRequestHeaders.Add(name, pair.Value);
                    }
                }
                if (timeout > 0)
                {

                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                byte[] bytes = client.GetByteArrayAsync(url).Result;
                return Encoding.UTF8.GetString(bytes);
            }
        }

        /// <summary>
        /// 异步get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [AsyncStateMachine((typeof(HttpGetAsyncd__1)))]
        public static Task<string> HttpGetAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            HttpGetAsyncd__1 d__ = new HttpGetAsyncd__1();
            d__.url = url;
            d__.headers = headers;
            d__.timeout = timeout;
            d__.s_t__builder = AsyncTaskMethodBuilder<string>.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start<HttpGetAsyncd__1>(ref d__);
            return d__.s_t__builder.Task;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            string str;
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> pair in headers)
                    {
                        string name = pair.Key;
                        client.DefaultRequestHeaders.Add(name, pair.Value);
                    }
                }
                if (timeout > 0)
                {
                    client.Timeout = new TimeSpan(0, 0, timeout);
                }
                using (HttpContent content = (HttpContent)new StringContent(postData ?? "", encoding ?? Encoding.UTF8))
                {
                    if (contentType != null)
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }
                    using (HttpResponseMessage message = client.PostAsync(url, content).Result)
                    {
                        byte[] bytes = message.Content.ReadAsByteArrayAsync().Result;
                        str = Encoding.UTF8.GetString(bytes);
                    }
                }
            }
            return str;
        }

        /// <summary>
        /// 异步Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [AsyncStateMachine(typeof(HttpPostAsyncd__3))]
        public static Task<string> HttpPostAsync(string url, string postData, Dictionary<string, string> headers = null, string contentType = null, int timeout = 0, Encoding encoding = null)
        {
            HttpPostAsyncd__3 d__ = new HttpPostAsyncd__3();
            d__.url = url;
            d__.postData = postData;
            d__.headers = headers;
            d__.contentType = contentType;
            d__.timeout = timeout;
            d__.encoding = encoding;
            d__.s_t__builder = AsyncTaskMethodBuilder<string>.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start<HttpPostAsyncd__3>(ref d__);
            return d__.s_t__builder.Task;
        }



        public static Task<string> HttpPostAsync(string url, string postData = null, string contentType = null, int timeOut = 30, Dictionary<string, string> headers = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
            if (headers != null)
            {
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            }

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(postData ?? "");
                using (Stream sendStream = request.GetRequestStream())
                {
                    sendStream.Write(bytes, 0, bytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    return streamReader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }

        }
        public static Task<string> HttpGetAsync(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (headers != null)
                {
                    foreach (var header in headers)
                        request.Headers[header.Key] = header.Value;
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    return streamReader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(ex.Message);
            }
        }




        /// <summary>
        /// 检查验证结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }





        // Nested Types
        [CompilerGenerated]
        private struct HttpGetAsyncd__1 : IAsyncStateMachine
        {
            // Fields
            public int s_1__state;
            public AsyncTaskMethodBuilder<string> s_t__builder;
            private TaskAwaiter<byte[]> s_u__1;
            private HttpClient client5__2;
            public Dictionary<string, string> headers;
            public int timeout;
            public string url;

            // Methods
            void IAsyncStateMachine.MoveNext()
            {
                string str;
                int num = this.s_1__state;
                try
                {
                    if (num != 0)
                    {
                        this.client5__2 = new HttpClient();
                    }
                    try
                    {
                        TaskAwaiter<byte[]> awaiter;
                        if (num != 0)
                        {
                            if (this.headers != null)
                            {
                                Dictionary<string, string>.Enumerator enumerator = this.headers.GetEnumerator();
                                try
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        KeyValuePair<string, string> pair = enumerator.Current;
                                        string name = pair.Key;
                                        this.client5__2.DefaultRequestHeaders.Add(name, pair.Value);
                                    }
                                }
                                finally
                                {
                                    if (num < 0)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                            }
                            if (this.timeout > 0)
                            {
                                this.client5__2.Timeout = new TimeSpan(0, 0, this.timeout);
                            }
                            awaiter = this.client5__2.GetByteArrayAsync(this.url).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.s_1__state = num = 0;
                                this.s_u__1 = awaiter;
                                this.s_t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<byte[]>, HttpHelper.HttpGetAsyncd__1>(ref awaiter, ref this);
                                return;
                            }
                        }
                        else
                        {
                            awaiter = this.s_u__1;
                            this.s_u__1 = new TaskAwaiter<byte[]>();
                            this.s_1__state = num = -1;
                        }
                        byte[] result = awaiter.GetResult();
                        str = Encoding.Default.GetString(result);
                    }
                    finally
                    {
                        if ((num < 0) && (this.client5__2 != null))
                        {
                            this.client5__2.Dispose();
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.s_1__state = -2;
                    this.s_t__builder.SetException(exception);
                    return;
                }
                this.s_1__state = -2;
                this.s_t__builder.SetResult(str);
            }

            [DebuggerHidden]
            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.s_t__builder.SetStateMachine(stateMachine);
            }




        }

        [CompilerGenerated]
        private struct HttpPostAsyncd__3 : IAsyncStateMachine
        {
            // Fields
            public int s_1__state;
            public AsyncTaskMethodBuilder<string> s_t__builder;
            private TaskAwaiter<HttpResponseMessage> s_u__1;
            private TaskAwaiter<byte[]> s_u__2;
            private HttpClient client5__2;
            private HttpContent content5__3;
            private HttpResponseMessage responseMessage5__4;
            public string contentType;
            public Encoding encoding;
            public Dictionary<string, string> headers;
            public string postData;
            public int timeout;
            public string url;

            // Methods
            void IAsyncStateMachine.MoveNext()
            {
                string str;
                int num = this.s_1__state;
                try
                {
                    if (num > 1)
                    {
                        this.client5__2 = new HttpClient();
                    }
                    try
                    {
                        if (num > 1)
                        {
                            if (this.headers != null)
                            {
                                Dictionary<string, string>.Enumerator enumerator = this.headers.GetEnumerator();
                                try
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        KeyValuePair<string, string> pair = enumerator.Current;
                                        string name = pair.Key;
                                        this.client5__2.DefaultRequestHeaders.Add(name, pair.Value);
                                    }
                                }
                                finally
                                {
                                    if (num < 0)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                            }
                            if (this.timeout > 0)
                            {
                                this.client5__2.Timeout = new TimeSpan(0, 0, this.timeout);
                            }
                            this.content5__3 = (HttpContent)new StringContent(this.postData ?? "", this.encoding ?? Encoding.UTF8);
                        }
                        try
                        {
                            HttpResponseMessage message;
                            TaskAwaiter<HttpResponseMessage> awaiter;
                            switch (num)
                            {
                                case 0:
                                    break;

                                case 1:
                                    goto Label_0168;

                                default:
                                    if (this.contentType != null)
                                    {
                                        this.content5__3.Headers.ContentType = new MediaTypeHeaderValue(this.contentType);
                                    }
                                    awaiter = this.client5__2.PostAsync(this.url, this.content5__3).GetAwaiter();
                                    if (awaiter.IsCompleted)
                                    {
                                        goto Label_0157;
                                    }
                                    this.s_1__state = num = 0;
                                    this.s_u__1 = awaiter;
                                    this.s_t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<HttpResponseMessage>, HttpHelper.HttpPostAsyncd__3>(ref awaiter, ref this);
                                    return;
                            }
                            awaiter = this.s_u__1;
                            this.s_u__1 = new TaskAwaiter<HttpResponseMessage>();
                            this.s_1__state = num = -1;
                        Label_0157:
                            message = awaiter.GetResult();
                            this.responseMessage5__4 = message;
                        Label_0168:;
                            try
                            {
                                TaskAwaiter<byte[]> awaiter2;
                                if (num != 1)
                                {
                                    awaiter2 = this.responseMessage5__4.Content.ReadAsByteArrayAsync().GetAwaiter();
                                    if (!awaiter2.IsCompleted)
                                    {
                                        this.s_1__state = num = 1;
                                        this.s_u__2 = awaiter2;
                                        this.s_t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<byte[]>, HttpHelper.HttpPostAsyncd__3>(ref awaiter2, ref this);
                                        return;
                                    }
                                }
                                else
                                {
                                    awaiter2 = this.s_u__2;
                                    this.s_u__2 = new TaskAwaiter<byte[]>();
                                    this.s_1__state = num = -1;
                                }
                                byte[] result = awaiter2.GetResult();
                                str = Encoding.UTF8.GetString(result);
                            }
                            finally
                            {
                                if ((num < 0) && (this.responseMessage5__4 != null))
                                {
                                    this.responseMessage5__4.Dispose();
                                }
                            }
                        }
                        finally
                        {
                            if ((num < 0) && (this.content5__3 != null))
                            {
                                this.content5__3.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        if ((num < 0) && (this.client5__2 != null))
                        {
                            this.client5__2.Dispose();
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.s_1__state = -2;
                    this.s_t__builder.SetException(exception);
                    return;
                }
                this.s_1__state = -2;
                this.s_t__builder.SetResult(str);
            }

            [DebuggerHidden]
            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.s_t__builder.SetStateMachine(stateMachine);
            }


        }
    }


}
