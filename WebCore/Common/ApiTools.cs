using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Business.EnterpriseWeiXin;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore
{
    public class ApiTools
    {
        // Methods
        public static   JObject GetBodyJObject(Stream body)
        {
            var str =  new StreamReader(body).ReadToEndAsync();
            try
            {
                return JObject.Parse(str.Result.ToString());
            }
            catch
            {
                return null;
            }
        }



        // Methods
        public static string GetErrorJson(string errmsg, int errcode = 0x3e9)
        {
            return GetJObject(errcode, errmsg).ToString(0, Array.Empty<JsonConverter>());
        }

        public static JObject GetJObject(int errcode = 0, string errmsg = "ok")
        {
            JObject obj1 = new JObject();
            obj1.Add("errcode", (JToken)errcode);
            obj1.Add("errmsg", (JToken)errmsg);
            return obj1;
        }

        public static string GetToken(string systemCode)
        {
            if (!systemCode.IsNullOrWhiteSpace())
            {
                new FlowApiSystem().Get(systemCode);
            }
            return "";

        }

        public static string GetToken(string systemCode, out DateTime expireTime)
        {
            expireTime = DateTime.MinValue;
            if (systemCode.IsNullOrWhiteSpace())
            {
                return "";
            }
           RoadFlow.Model. FlowApiSystem system2 = new FlowApiSystem().Get(systemCode);
            if (system2 == null)
            {
                return "";
            }
            return Token.CreateToken(system2.Id.ToUpperString(), systemCode, systemCode, out expireTime);
        }



    }



}
