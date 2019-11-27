using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business.EnterpriseWeiXin
{
    public class Organize
    {
        // Fields
        private readonly string DeptUrl = "https://qyapi.weixin.qq.com/cgi-bin/department/";
        public int RootDeptId;
        public string Secret;
        private readonly string Url = "https://qyapi.weixin.qq.com/cgi-bin/user/";

        // Methods
        public Organize()
        {
            List<RoadFlow.Model.Dictionary> childs = new Dictionary().GetChilds("EnterpriseWeiXin");
            if (Enumerable.Any<RoadFlow.Model.Dictionary>((IEnumerable<RoadFlow.Model.Dictionary>)childs))
            {
                RoadFlow.Model.Dictionary dictionary = childs.Find(
                    key=> key.Title.Equals("通讯录同步"));
                if (dictionary != null)
                {
                    this.Secret = dictionary.Note.Trim1();
                    this.RootDeptId = dictionary.Value.ToInt(0);
                }
            }
            if (this.Secret.IsNullOrWhiteSpace())
            {
                throw new Exception("通讯录同步Secret为空");
            }
        }

        public string AddDept(RoadFlow.Model.Organize organize)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                if (organize == null)
                {
                    return "要添加的部门为空";
                }
                string url = this.DeptUrl + "create?access_token=" + Common.GetAccessToken(this.Secret);
                JObject obj1 = new JObject();
                obj1.Add("name", (JToken)organize.Name);
                obj1.Add("parentid", (JToken)organize.ParentId.ToInt());
                obj1.Add("order", (JToken)organize.Sort);
                obj1.Add("id", (JToken)organize.IntId);
                string postData = obj1.ToString(0, Array.Empty<JsonConverter>());
                string str3 = HttpHelper.HttpPost(url, postData, null, null, 0, null);
                JObject obj3 = JObject.Parse(str3);
                if (obj3.Value<int>("errcode") > 0)
                {
                    Log.Add("企业微信添加部门发生了错误", postData, LogType.其它, "", "", "返回：" + str3 + " url:" + url, "", "", "", "", "");
                    return obj3.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        public string AddUser(RoadFlow.Model.User user)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                if (user == null)
                {
                    return "要添加的用户为空";
                }
                if (user.Mobile.IsNullOrWhiteSpace() && user.Email.IsNullOrWhiteSpace())
                {
                    return "手机号和邮箱不能同时为空";
                }
                string url = this.Url + "create?access_token=" + Common.GetAccessToken(this.Secret);
                JObject obj1 = new JObject();
                obj1.Add("userid", (JToken)user.Account);
                obj1.Add("name", (JToken)user.Name);
                obj1.Add("position", (JToken)new User().GetOrganizeMainShowHtml(user.Id, false).TrimAll());
                obj1.Add("mobile", (JToken)user.Mobile);
                JArray array1 = new JArray();
                array1.Add((JToken)this.RootDeptId);
                obj1.Add("department", array1);
                obj1.Add("order", 0);
                obj1.Add("enable", (user.Status == 0) ? 1 : 0);
                obj1.Add("email", (JToken)user.Email);
                JObject obj2 = obj1;
                if (user.Tel.IsTelNumber())
                {
                    obj2.Add("telephone", (JToken)user.Tel);
                }
                if (user.Sex.HasValue)
                {
                    obj2.Add("gender", user.Sex.Value + 1);
                }
                string postData = obj2.ToString(0, Array.Empty<JsonConverter>());
                string str3 = HttpHelper.HttpPost(url, postData, null, null, 0, null);
                JObject obj3 = JObject.Parse(str3);
                if (obj3.Value<int>("errcode") > 0)
                {
                    Log.Add("企业微信添加人员发生了错误", postData, LogType.其它, "", "", "返回：" + str3 + " url:" + url, "", "", "", "", "");
                    return obj3.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        public string DeleteDept(RoadFlow.Model.Organize organize)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                if (organize == null)
                {
                    return "要删除的部门为空";
                }
                object[] objArray1 = new object[] { this.DeptUrl, "delete?access_token=", Common.GetAccessToken(this.Secret), "&id=", (int)organize.IntId };
                string url = string.Concat((object[])objArray1);
                string str2 = HttpHelper.HttpGet(url, null, 0);
                JObject obj2 = JObject.Parse(str2);
                if (obj2.Value<int>("errcode") > 0)
                {
                    Log.Add("企业微信删除部门发生了错误", url, LogType.其它, "", "", "返回：" + str2, "", "", "", "", "");
                    return obj2.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        public string DeleteUser(string account)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                string[] textArray1 = new string[] { this.Url, "delete?access_token=", Common.GetAccessToken(this.Secret), "&userid=", account };
                string url = string.Concat((string[])textArray1);
                string str2 = HttpHelper.HttpGet(url, null, 0);
                JObject obj2 = JObject.Parse(str2);
                if (obj2.Value<int>("errcode") > 0)
                {
                    Log.Add("企业微信删除人员发生了错误", url, LogType.其它, "", "", "返回：" + str2, "", "", "", "", "");
                    return obj2.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        public JObject GetUser(string account)
        {
            string[] textArray1 = new string[] { this.Url, "get?access_token=", Common.GetAccessToken(this.Secret), "&userid=", account };
            string url = string.Concat((string[])textArray1);
            string str2 = HttpHelper.HttpGet(url, null, 0);
            JObject obj2 = JObject.Parse(str2);
            if (obj2.Value<int>("errcode") > 0)
            {
                Log.Add("企业微信获取人员发生了错误", url, LogType.其它, "", "", "返回：" + str2, "", "", "", "", "");
                return null;
            }
            return obj2;
        }

        public string UpdateDept(RoadFlow.Model.Organize organize)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                if (organize == null)
                {
                    return "要更新的部门为空";
                }
                string url = this.DeptUrl + "update?access_token=" + Common.GetAccessToken(this.Secret);
                JObject obj1 = new JObject();
                obj1.Add("name", (JToken)organize.Name);
                obj1.Add("parentid", (JToken)organize.ParentId.ToInt());
                obj1.Add("order", (JToken)organize.Sort);
                obj1.Add("id", (JToken)organize.IntId);
                string postData = obj1.ToString(0, Array.Empty<JsonConverter>());
                string str3 = HttpHelper.HttpPost(url, postData, null, null, 0, null);
                JObject obj3 = JObject.Parse(str3);
                if (obj3.Value<int>("errcode") > 0)
                {
                    Log.Add("企业微信更新部门发生了错误", postData, LogType.其它, "", "", "返回：" + str3 + " url:" + url, "", "", "", "", "");
                    return obj3.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        public string UpdateUser(RoadFlow.Model.User user)
        {
            if (Config.Enterprise_WeiXin_IsSyncOrg)
            {
                if (user == null)
                {
                    return "要添加或修改的用户为空";
                }
                if (user.Mobile.IsNullOrWhiteSpace() && user.Email.IsNullOrWhiteSpace())
                {
                    return "手机号和邮箱不能同时为空";
                }
                string url = this.Url + "update?access_token=" + Common.GetAccessToken(this.Secret);
                JObject obj1 = new JObject();
                obj1.Add("userid", (JToken)user.Account);
                obj1.Add("name", (JToken)user.Name);
                obj1.Add("position", (JToken)new User().GetOrganizeMainShowHtml(user.Id, false).TrimAll());
                obj1.Add("mobile", (JToken)user.Mobile);
                JArray array1 = new JArray();
                array1.Add((JToken)this.RootDeptId);
                obj1.Add("department", array1);
                obj1.Add("order", 0);
                obj1.Add("enable", (user.Status == 0) ? 1 : 0);
                obj1.Add("email", (JToken)user.Email);
                JObject obj2 = obj1;
                if (user.Tel.IsTelNumber())
                {
                    obj2.Add("telephone", (JToken)user.Tel);
                }
                if (user.Sex.HasValue)
                {
                    obj2.Add("gender", user.Sex.Value + 1);
                }
                string postData = obj2.ToString(0, Array.Empty<JsonConverter>());
                string str3 = HttpHelper.HttpPost(url, postData, null, null, 0, null);
                JObject obj3 = JObject.Parse(str3);
                int num = obj3.Value<int>("errcode");
                if (num > 0)
                {
                    if (0xeacf == num)
                    {
                        return this.AddUser(user);
                    }
                    Log.Add("企业微信修改人员发生了错误", postData, LogType.其它, "", "", "返回：" + str3 + " url:" + url, "", "", "", "", "");
                    return obj3.Value<string>("errmsg");
                }
            }
            return string.Empty;
        }

        // Nested Types
     
}



 




 

}
