using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility.Cache;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business.EnterpriseWeiXin
{
    public class Common
    {
        // Fields
        public static readonly string AppId = Config.Enterprise_WeiXin_AppId;
        public static readonly string SessionKey = "roadflow_enterprise_userid";

        // Methods
        public static bool CheckLogin(bool isRedirect = true)
        {
            if (GetUserId().IsNotEmptyGuid())
            {
                return true;
            }
            if (isRedirect)
            {
                List<RoadFlow.Model.Dictionary> childs = new Dictionary().GetChilds("EnterpriseWeiXin");
                if (!Enumerable.Any<Dictionary>((IEnumerable<Dictionary>)childs))
                {
                    return false;
                }
                HttpContext httpContext = Tools.HttpContext;
                if (httpContext.Request.Path.HasValue)
                {
                    SessionExtensions.SetString(httpContext.Session, "EnterpriseWeiXin_LastURL", httpContext.Request.Url());
                }
                string str = Enumerable.First<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs).Value.Trim1();
                string str2 = (Config.Enterprise_WeiXin_WebUrl + "/RoadFlowCore/Mobile/GetUserAccount").UrlEncode();
                string[] textArray1 = new string[] { "https://open.weixin.qq.com/connect/oauth2/authorize?appid=", AppId, "&redirect_uri=", str2, "&response_type=code&scope=snsapi_base&agentid=", str, "&state=STATE#wechat_redirect" };
                string str3 = string.Concat((string[])textArray1);
                Tools.HttpContext.Response.Redirect(str3);
            }
            return false;
        }





        public static string GetAccessToken(string corpsecret)
        {
            string str = "EnterpriseWeiXin_AccessToken_" + corpsecret;
            object obj2 = IO.Get(str);
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            string contents = HttpHelper.HttpGet("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + AppId + "&corpsecret=" + corpsecret, null, 0);
            JObject obj3 = JObject.Parse(contents);
            if (obj3.Value<int>("errcode") == 0)
            {
                string str3 = obj3.Value<string>("access_token");
                int num = obj3.Value<int>("expires_in");
                IO.Insert(str, str3, DateTimeExtensions.Now.AddSeconds((double)(num - 300)));
                return str3;
            }
            Log.Add("获取企业微信access_token错误", contents, LogType.其它, "", "", "", "", "", "", "", "");
            return string.Empty;
        }


        public static RoadFlow.Model.User GetUser()
        {
            Guid userId = GetUserId();
            if (!userId.IsEmptyGuid())
            {
                return new RoadFlow.Business.User().Get(userId);
            }
            return null;
        }



        public static string GetUserAccountByCode(string code)
        {
            List<RoadFlow.Model.Dictionary> childs = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (!Enumerable.Any<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs))
            {
                return string.Empty;
            }
            string contents = HttpHelper.HttpGet("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=" + GetAccessToken(Enumerable.First<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs).Note.Trim1()) + "&code=" + code, null, 0);
            JObject obj2 = JObject.Parse(contents);
            if (obj2.Value<int>("errcode") != 0)
            {
                Log.Add("根据微信返回code得到帐号发生了错误", contents, LogType.其它, "", "", "Code:" + code, "", "", "", "", "");
                return string.Empty;
            }
            return obj2.Value<string>("UserId");
        }

        public static Guid GetUserId()
        {
            HttpContext httpContext = Tools.HttpContext;
            if (httpContext != null)
            {
                Guid guid;
                Guid guid2;
                if (SessionExtensions.GetString(httpContext.Session, SessionKey).IsGuid(out guid))
                {
                    return guid;
                }
                if (Config.IsDebug && Config.DebugUserId.IsGuid(out guid2))
                {
                    return guid2;
                }
            }
            return Guid.Empty;
        }


        public static string SendMessage(RoadFlow.Model.User receiveUser, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            if (receiveUser.Mobile.IsNullOrWhiteSpace() && receiveUser.Email.IsNullOrWhiteSpace())
            {
                return (receiveUser.Name + "未绑定微信!");
            }
            List<RoadFlow.Model.User> receiveUsers = new List<RoadFlow.Model.User> {
            receiveUser
            };
            return SendMessage(receiveUsers, contentJson, msgType, agentId);


        }

        public static string SendMessage(List<RoadFlow.Model.User> receiveUsers, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            List<List<RoadFlow.Model.User>> list = new List<List<RoadFlow.Model.User>>();
            int num = 0x3e8;
            int num2 = 1;
            while (num == 0x3e8)
            {
                List<RoadFlow.Model.User> list2 = Enumerable.ToList<RoadFlow.Model.User>(Enumerable.Take<RoadFlow.Model.User>(Enumerable.Skip<RoadFlow.Model.User>((IEnumerable<RoadFlow.Model.User>)receiveUsers, (num2++ * 0x3e8) - 0x3e8), 0x3e8));
                list.Add(list2);
                num = list2.Count;
            }
            bool flag = true;
            foreach (List<RoadFlow.Model.User> list3 in list)
            {
                StringBuilder builder = new StringBuilder();
                foreach (RoadFlow.Model.User user in list3)
                {
                    builder.Append(user.Account);
                    builder.Append("|");
                }
                if (!SendMessage(builder.ToString().TrimEnd('|'), contentJson, msgType, agentId).IsNullOrWhiteSpace() & flag)
                {
                    flag = false;
                }
            }
            return (flag ? string.Empty : "发送错误");

        }

        public static string SendMessage(string userAccounts, JObject contentJson, string msgType = "text", int agentId = 0)
        {
            List<RoadFlow.Model.Dictionary> childs = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (!Enumerable.Any<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs))
            {
                return "未在数据字典中设置微信应用";
            }
            string corpsecret = string.Empty;
            if (agentId == 0)
            {
                RoadFlow.Model.Dictionary dictionary = childs.Find(
                    key=> key.Title.Contains("消息"));
                if (dictionary == null)
                {
                    return "未找到接收消息的应用";
                }
                agentId = dictionary.Value.ToInt(-2147483648);
                corpsecret = dictionary.Note.Trim1();
            }
            else
            {
                RoadFlow.Model.Dictionary dictionary2 = childs.Find(delegate (RoadFlow.Model.Dictionary p) {
                    return p.Value.Equals(((int)agentId).ToString());
                });
                if (dictionary2 == null)
                {
                    return ("未找到" + ((int)agentId) + "对应的应用");
                }
                corpsecret = dictionary2.Note.Trim1();
            }
            string accessToken = GetAccessToken(corpsecret);
            string url = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token=" + accessToken;
            JObject obj1 = new JObject();
            obj1.Add("msgtype", (JToken)msgType);
            obj1.Add("agentid", (JToken)agentId);
            obj1.Add(msgType, contentJson);
            obj1.Add("safe", 0);
            obj1.Add("touser", (JToken)userAccounts);
            string postData = obj1.ToString(0, Array.Empty<JsonConverter>());
            string str5 = HttpHelper.HttpPost(url, postData, null, null, 0, null);
            JObject obj2 = JObject.Parse(str5);
            if (obj2.Value<int>("errcode") != 0)
            {
                Log.Add("发送企业微信消息发生了错误", postData, LogType.其它, "", "", "返回:" + str5 + "  URL:" + url, "", "", "", "", "");
                return obj2.Value<string>("errmsg");
            }
            return string.Empty;
        }

    
}



 

}
