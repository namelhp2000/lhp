using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business.WeiXin
{
    public class Common
    {
        // Fields
        private static string Access_Token = string.Empty;
        private static DateTime LastTime = DateTimeExtensions.Now;

        // Methods
        public static string GetAccessToken()
        {
            if ((LastTime <= DateTimeExtensions.Now) || Access_Token.IsNullOrWhiteSpace())
            {
                string contents = HttpHelper.HttpGet("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + Config.WeiXin_AppId + "&secret=" + Config.WeiXin_AppSecret, null, 0);
                JObject obj2 = JObject.Parse(contents);
                if (obj2.ContainsKey("errcode"))
                {
                    Log.Add("get access_token err", contents, LogType.其它, "", "", "", "", "", "", "", "");
                    return string.Empty;
                }
                string str = obj2.Value<string>("access_token");
                string str4 = obj2.Value<string>("expires_in");
                if (str.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }
                Access_Token = str;
                LastTime = DateTimeExtensions.Now.AddSeconds((double)str4.ToInt(0));
            }
            return Access_Token;
        }
    }


}
