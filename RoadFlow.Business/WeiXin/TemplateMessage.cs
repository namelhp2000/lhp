using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business.WeiXin
{
    public class TemplateMessage
    {
        // Methods
        public static string SendWaitTaskMessage(RoadFlow.Model.FlowTask flowTask)
        {
            if (Config.WeiXin_IsUse)
            {
                if ((flowTask == null) || flowTask.ReceiveId.IsEmptyGuid())
                {
                    return string.Empty;
                }
                RoadFlow.Model.User user = new User().Get(flowTask.ReceiveId);
                if ((user == null) || user.WeiXinOpenId.IsNullOrWhiteSpace())
                {
                    return string.Empty;
                }
                HttpContext httpContext = Tools.HttpContext;
                string str = (httpContext == null) ? "0" : httpContext.Request.Querys("rf_appopenmodel");
                object[] objArray1 = new object[] { "Index", flowTask.FlowId, flowTask.StepId, flowTask.Id, flowTask.GroupId, flowTask.InstanceId, (httpContext == null) ? "" : httpContext.Request.Querys("appid"), (httpContext == null) ? "" : httpContext.Request.Querys("tabid"), str };
                string str2 = Config.WeiXin_WebUrl + string.Format("/RoadFlowCore/FlowRun/{0}?flowid={1}&stepid={2}&taskid={3}&groupid={4}&instanceid={5}&appid={6}&tabid={7}&rf_appopenmodel={8}&ismobile=1", (object[])objArray1);
                string accessToken = Common.GetAccessToken();
                string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + accessToken;
                JObject obj1 = new JObject();
                obj1.Add("touser", (JToken)user.WeiXinOpenId);
                obj1.Add("template_id", "1wFmd0moKTb8HIw1ZIqUGIbo7h253R5IjOQAW5_UkcQ");
                obj1.Add("url", (JToken)str2);
                JObject obj2 = obj1;
                JObject obj3 = new JObject();
                JObject obj9 = new JObject();
                obj9.Add("value", (JToken)flowTask.Title);
                obj9.Add("color", "");
                JObject obj4 = obj9;
                obj3.Add("first", obj4);
                JObject obj10 = new JObject();
                obj10.Add("value", (JToken)flowTask.SenderName);
                JObject obj5 = obj10;
                obj3.Add("keyword1", obj5);
                JObject obj11 = new JObject();
                obj11.Add("value", (JToken)flowTask.ReceiveTime.ToShortDateTimeString());
                JObject obj6 = obj11;
                obj3.Add("keyword2", obj6);
                JObject obj12 = new JObject();
                obj12.Add("value", flowTask.CompletedTime.HasValue ? ((JToken)("要求完成时间：" + flowTask.CompletedTime.Value.ToShortDateTimeString())) : ((JToken)""));
                JObject obj7 = obj12;
                obj3.Add("remark", obj7);
                obj2.Add("data", obj3);
                string contents = HttpHelper.HttpPost(url, obj2.ToString(0, Array.Empty<JsonConverter>()), null, null, 0, null);
                if (JObject.Parse(contents).Value<string>("errcode").Equals("0"))
                {
                    return "1";
                }
                Log.Add("发送公众号模板消息错误", contents, LogType.其它, "", "", "", "", "", "", "", "");
            }
            return string.Empty;
        }
    }






}
