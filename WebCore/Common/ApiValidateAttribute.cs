using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebCore
{
    public class ApiValidateAttribute : Attribute, IActionFilter, IFilterMetadata
    {
        // Fields
        [CompilerGenerated]
        private bool Checkk__BackingField = true;

    // Methods
    public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public  void OnActionExecuting(ActionExecutingContext context)
        {

         context.HttpContext.Request.EnableBuffering(0x7800);
            //  BufferingHelper.EnableRewind(context.HttpContext.Request, 0x7800, null);

            var   str =  new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
           
            context.HttpContext.Request.Body.Position = 0L;
            JObject obj2 = null;
            try
            {
                obj2 = JObject.Parse(str.Result.ToString());
            }
            catch
            {
            }
            if (obj2 == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content = ApiTools.GetErrorJson("json格式错误", 0x3e9);
                context.Result = result1;
            }
            else
            {
                string str2 = obj2.Value<string>("token");

              //  string str2 = obj2.Value<string>("systemcode");
                if (str2.IsNullOrEmpty())
                {
                    ContentResult result2 = new ContentResult();
                    result2.Content = ApiTools.GetErrorJson("token为空", 0x3e9);
                    context.Result = result2;
                }
                else
                {

                    ValueTuple<string, DateTime> tuple1 = Token.ParseToken(str2);
                    string str3 = tuple1.Item1;
                    DateTime time = tuple1.Item2;
                    if (str3.IsNullOrWhiteSpace() || (time < DateTimeExtensions.Now))
                    {
                        ContentResult result3 = new ContentResult
                        {
                            Content = ApiTools.GetErrorJson("token错误", 0x3e9)
                        };
                        context.Result=result3;
                    }
                    else if (new RoadFlow.Business.FlowApiSystem().Get(str3.Trim()) == null)
                    {
                        ContentResult result4 = new ContentResult
                        {
                            Content = ApiTools.GetErrorJson("token错误", 0x3e9)
                        };
                        context.Result=result4;
                    }



               
                }
            }
        }


     
        public bool Check { get; set; }
    }


 




}
