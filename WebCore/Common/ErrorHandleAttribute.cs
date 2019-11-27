//using LogUtility;
//using Microsoft.AspNetCore.Mvc.Filters;
//using RoadFlow.Utility.Logs;
//using System;
//using System.Linq;
//using LogUtility.Extensions;

//namespace WebCore
//{
//    /// <summary>
//    /// 异常处理过滤器
//    /// </summary>
//    public class ErrorHandleAttribute : ExceptionFilterAttribute
//    {
//        /// <summary>
//        /// 异常处理过滤器
//        /// </summary>
//        /// <param name="ExecutedContext"></param>

//        public override void OnException(ExceptionContext ExecutedContext)
//        {

//            //异常信息
//            string exceptionMsg = ExecutedContext.Exception.Message;
//            //异常定位
//            string exceptionPosition = ExecutedContext.Exception.StackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.None).Where(s => !string.IsNullOrWhiteSpace(s)).First().Trim();

//            string exceptionStackTrace = ExecutedContext.Exception.StackTrace;
//            //string stack
//            //记录的message
//            string message = Environment.NewLine + $"[异常信息]：{exceptionMsg} " + Environment.NewLine + $"[异常定位]：{exceptionPosition}" + Environment.NewLine + $"[异常堆栈]：{exceptionStackTrace}";

//            NLog.LogManager.Configuration.Variables["exceptionPosition"] = message;


//        }





//    }


//}
