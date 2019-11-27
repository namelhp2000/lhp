using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RoadFlow.Utility.Cache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if NETCOREAPP3_0
using Microsoft.Extensions.Hosting;
#endif



namespace RoadFlow.Utility
{



   


    /// <summary>
    /// 工具包
    /// </summary>
    public class Tools
    {
        // Fields
        /// <summary>
        /// 访问上下文
        /// </summary>
        private static IHttpContextAccessor _accessor;

        /// <summary>
        /// 宿主环境
        /// </summary>
#if NETCOREAPP3_0
        private static IWebHostEnvironment _hostingEnvironment;
#else
        private static IHostingEnvironment _hostingEnvironment;
#endif




        /// <summary>
        /// 配置主机环境   WebCore配置服务依赖 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
#if NETCOREAPP3_0
        public static void ConfigureHostingEnvironment(IWebHostEnvironment hostingEnvironment)
#else
        public static void ConfigureHostingEnvironment(IHostingEnvironment hostingEnvironment)
#endif
        {
            _hostingEnvironment = hostingEnvironment;


        }



        /// <summary>
        /// 配置访问上下文    WebCore配置服务依赖
        /// </summary>
        /// <param name="accessor"></param>
        public static void ConfigureHttpContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

        }


        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static object ExecuteExpression(string Expression)
        {
            return new DataTable().Compute(Expression, "");
        }


        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ValueTuple<object, Exception> ExecuteMethod(string method, params object[] args)
        {
            string str;
            //映射
            Assembly assembly = GetAssembly(method, out str);
            if (null == assembly)
            {
                return new ValueTuple<object, Exception>(null, new Exception("未能载入资源"));
            }
            try
            {
                string str2 = method.Substring(0, method.LastIndexOf('.'));
                string str3 = method.Substring(method.LastIndexOf('.') + 1);
                Type type = assembly.GetType(str2, true);
                object obj2 = Activator.CreateInstance(type, false);
                return new ValueTuple<object, Exception>(type.GetMethod(str3).Invoke(obj2, args), null);
            }
            catch (Exception exception)
            {
                return new ValueTuple<object, Exception>(null, exception);
            }
        }


        /// <summary>
        /// 获取绝对URL 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetAbsoluteURL(HttpRequest request = null)
        {
            if (request == null)
            {
                HttpContext httpContext = HttpContext;
                request = (httpContext != null) ? httpContext.Request : null;
            }
            if (request == null)
            {
                return string.Empty;
            }
            return new StringBuilder().Append(request.Scheme).Append("://").Append(request.Host).Append((string)request.PathBase).Append((string)request.Path).Append(request.QueryString).ToString();
        }



        /// <summary>
        /// 获取dll集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string name, out string dllName)
        {
            dllName = string.Empty;
            string str = "assembly_" + name;
            object obj2 = IO.Get(str);
            if (obj2 != null)
            {
                return (Assembly)obj2;
            }
            if (name.IsNullOrWhiteSpace())
            {
                return null;
            }
            char[] separator = new char[] { '.' };
            StringBuilder builder = new StringBuilder();
            Assembly assembly = null;
            foreach (string str2 in name.Split(separator))
            {
                try
                {
                    builder.Append(str2);
                    builder.Append(".");
                    char[] trimChars = new char[] { '.' };
                    dllName = builder.ToString().TrimEnd(trimChars);
                    assembly = Assembly.Load(dllName);
                    if (null != assembly)
                    {
                        break;
                    }
                }
                catch
                {
                }
            }
            IO.Insert(str, assembly);
            return assembly;
        }

        /// <summary>
        /// 获取Browser(获取浏览器信息)
        /// </summary>
        /// <returns></returns>
        public static string GetBrowseAgent()
        {
            HttpContext httpContext = HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }
            return httpContext.Request.Headers["User-Agent"];
        }


        /// <summary>
        /// 获取内容根路径
        /// </summary>
        /// <returns></returns>
        public static string GetContentRootPath()
        {
            if (_hostingEnvironment != null)
            {
                return _hostingEnvironment.ContentRootPath;

            }
            return string.Empty;
        }



        /// <summary>
        /// 获取当前主题颜色
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTheme()
        {
            string str;
            HttpContext httpContext = HttpContext;
            if (httpContext == null)
            {
                return "blue";
            }
            if (!httpContext.Request.Cookies.TryGetValue("rf_core_theme", out str))
            {
                return "blue";
            }
            return str;
        }


        /// <summary>
        /// 获取HttpHost请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetHttpHost(HttpRequest request = null)
        {
            if (request == null)
            {
                HttpContext httpContext = HttpContext;
                request = (httpContext != null) ? httpContext.Request : null;
            }
            if (request == null)
            {
                return string.Empty;
            }
            return new StringBuilder().Append(request.Scheme).Append("://").Append(request.Host).Append((string)request.PathBase).ToString();
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            HttpContext httpContext = HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }
            return httpContext.Connection.RemoteIpAddress.ToString();
        }





        /// <summary>
        /// 获取页码数值
        /// </summary>
        /// <returns></returns>

        public static int GetPageNumber()
        {
            int num;
            string str = HttpContext.Request.Forms("pagenumber");
            if (!str.IsInt())
            {
                str = HttpContext.Request.Querys("pagenumber");
            }
            if (!str.IsInt(out num))
            {
                return 1;
            }
            return num;
        }


        /// <summary>
        /// 获取页面大小
        /// </summary>
        /// <param name="setCookie"></param>
        /// <returns></returns>

        public static int GetPageSize(bool setCookie = true)
        {
            int num2;
            string str2;
            string str = HttpContext.Request.Forms("pagesize");
            if (!str.IsInt())
            {
                str = HttpContext.Request.Querys("pagesize");
            }
            if (!str.IsInt() && HttpContext.Request.Cookies.TryGetValue("roadflowcorepagesize", out str2))
            {
                str = str2;
            }
            int pageSize = str.IsInt(out num2) ? num2 : Config.PageSize;
            if (pageSize <= 0)
            {
                pageSize = Config.PageSize;
            }
            if (setCookie)
            {
                CookieOptions options1 = new CookieOptions();
                options1.Expires = new DateTimeOffset?((DateTimeOffset)DateTimeExtensions.Now.AddYears(5));
                HttpContext.Response.Cookies.Append("roadflowcorepagesize", ((int)pageSize).ToString(), options1);
            }
            return pageSize;
        }




        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random(Guid.NewGuid().ToInt());
            for (int i = 0; i < length; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + (num % 10));
                }
                else
                {
                    ch = (char)(0x41 + (num % 0x1a));
                }
                if (((char)ch).Equals('o') || ((char)ch).Equals('0'))
                {
                    length++;
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }


        /// <summary>
        /// 获取来源URL
        /// </summary>
        /// <returns></returns>
        public static string GetReferer()
        {
            HttpContext httpContext = HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }
            return httpContext.Request.Headers["Referer"];
        }


        /// <summary>
        /// 获取Url
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetURL(HttpRequest request = null)
        {
            if (request == null)
            {
                HttpContext httpContext = HttpContext;
                request = (httpContext != null) ? httpContext.Request : null;
            }
            if (request == null)
            {
                return string.Empty;
            }
            return new StringBuilder().Append((string)request.PathBase).Append((string)request.Path).Append(request.QueryString).ToString();
        }


        /// <summary>
        /// 获取验证错误消息
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string GetValidateErrorMessag(ModelStateDictionary modelState)
        {
            StringBuilder builder = new StringBuilder();
            int num = 1;
            builder.Append("验证错误：\n");
            foreach (string str in modelState.Keys)
            {
                ModelStateEntry entry = modelState[str];
                if (Enumerable.Any<ModelError>((IEnumerable<ModelError>)entry.Errors))
                {
                    builder.Append(num++);
                    builder.Append("、");
                    builder.Append(Enumerable.First<ModelError>((IEnumerable<ModelError>)entry.Errors).ErrorMessage);
                    builder.Append("\n");
                }
            }
            return builder.ToString();
        }


        /// <summary>
        /// 获取验证Img图片
        /// </summary>
        /// <param name="code"></param>
        /// <param name="bgImg"></param>
        /// <returns></returns>
        public static MemoryStream GetValidateImg(out string code, string bgImg)
        {
            code = GetRandomString(4);
            Random random = new Random();
            Bitmap bitmap = new Bitmap((int)Math.Ceiling((double)(code.Length * 17.2)), 0x1c);
            Image image = Image.FromFile(bgImg);
            Font font = new Font("Arial", 16f, FontStyle.Italic);
            Font font2 = new Font("Arial", 16f, FontStyle.Italic);
            new LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            Graphics graphics1 = Graphics.FromImage(bitmap);
            graphics1.DrawImage(image, 0, 0, new Rectangle(random.Next(image.Width - bitmap.Width), random.Next(image.Height - bitmap.Height), bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            graphics1.DrawString(code, font2, Brushes.White, 0f, 1f);
            graphics1.DrawString(code, font, Brushes.ForestGreen, 0f, 1f);
            int num = bitmap.Width;
            int num2 = random.Next(5, bitmap.Height);
            int num3 = random.Next(5, bitmap.Height);
            graphics1.DrawLine(new Pen(Color.Green, 2f), 1, num2, num - 2, num3);
            graphics1.DrawRectangle(new Pen(Color.Transparent), 0, 10, bitmap.Width - 1, bitmap.Height - 1);
            MemoryStream stream = new MemoryStream();
            bitmap.Save((Stream)stream, ImageFormat.Png);
            return stream;
        }


        /// <summary>
        /// 获取wwwroot路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebRootPath()
        {
            if (_hostingEnvironment != null)
            {
                return _hostingEnvironment.WebRootPath;

            }
            return string.Empty;
        }



        /// <summary>
        /// 是否异步Ajax
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjax(HttpRequest request = null)
        {
            if (request == null)
            {
                HttpContext httpContext = HttpContext;
                request = (httpContext != null) ? httpContext.Request : null;
            }
            if (request == null)
            {
                return false;
            }
            bool flag = false;
            if (request.Headers.ContainsKey("x-requested-with"))
            {
                flag = request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            return flag;
        }

        /// <summary>
        /// 是否访问接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsPhoneAccess(HttpRequest request)
        {
            if (request == null)
            {
                HttpContext httpContext = HttpContext;
                if (httpContext != null)
                {
                    request = httpContext.Request;
                }
            }
            if (request == null)
            {
                return false;
            }
            string input = request.Headers["User-Agent"];
            Regex regex = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", ((RegexOptions)RegexOptions.Multiline) | ((RegexOptions)RegexOptions.IgnoreCase));
            Regex regex2 = new Regex(@".*wechat.*(\r\n)?", ((RegexOptions)RegexOptions.Multiline) | ((RegexOptions)RegexOptions.IgnoreCase));
            if (!new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", ((RegexOptions)RegexOptions.Multiline) | ((RegexOptions)RegexOptions.IgnoreCase)).IsMatch(input) && !regex.IsMatch(input.Substring(0, 4)))
            {
                return regex2.IsMatch(input);
            }
            return true;
        }

        /// <summary>
        /// HttpContext访问端口
        /// </summary>
        // Properties
        public static HttpContext HttpContext
        {
            get
            {
                //Util扩展
                return WebHelper.HttpContext;
                // return _accessor.HttpContext;
            }
        }



        public static string GetCurrentLanguage()
        {
            string cookieLanguage = Config.Language_Current;
            if (!cookieLanguage.IsNullOrWhiteSpace())
            {
                return cookieLanguage;
            }
            cookieLanguage = GetCookieLanguage();
            if (!cookieLanguage.IsNullOrWhiteSpace())
            {
                return cookieLanguage;
            }
            return "zh-CN";
        }


        public static string GetCookieLanguage()
        {
            string str2;
            string str = string.Empty;
            if (HttpContext.Request.Cookies.TryGetValue(".AspNetCore.Culture", out str2))
            {
                char[] separator = new char[] { '|' };
                string[] strArray = str2.Split(separator);
                if (strArray.Length != 0)
                {
                    char[] trimChars = new char[] { 'c', '=' };
                    str = strArray[0].TrimStart(trimChars).Trim();
                }
            }
            return str;
        }


    }


}
