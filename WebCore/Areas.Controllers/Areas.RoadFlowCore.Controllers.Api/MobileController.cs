using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers.Api
{
    [Route("RoadFlowCore/Api/[controller]"), ApiController]
    public class MobileController : ControllerBase
    {
        /// <summary>
        /// 查询完成任务
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("QueryCompletedTask")]
        public string QueryCompletedTask()
        {
            int num2;
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            int num = base.Request.Forms("pagenumber").ToInt(1);
            string str = base.Request.Forms("title");
            DataTable table = new FlowTask().GetCompletedTask(8, num, enterpriseWeiXinUserId, "", str, "", "", "ReceiveTime DESC", out num2);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("FlowId", (JToken)row["FlowId"].ToString());
                obj1.Add("StepId", (JToken)row["StepId"].ToString());
                obj1.Add("InstanceId", (JToken)row["InstanceId"].ToString());
                obj1.Add("GroupId", (JToken)row["GroupId"].ToString());
                obj1.Add("Title", (JToken)row["Title"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("CompletedTime1", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["CompletedTime1"].ToString())));
                JObject obj3 = obj1;
                array.Add(obj3);
            }
            JObject obj4 = new JObject();
            obj4.Add("total", (JToken)num2);
            obj4.Add("hasnext", ((8 * num) >= num2) ? 0 : 1);
            obj4.Add("data", array);
            JObject obj2 = obj4;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 查询文档
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("QueryDocument")]
        public string QueryDocument()
        {
            int num2;
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            string str = base.Request.Forms("dirid");
            int num = base.Request.Forms("pagenumber").ToInt(1);
            string str2 = base.Request.Forms("title");
            string str3 = "DocRank,WriteTime DESC";
            string str4 = str.IsGuid() ? ("'" + str + "'") : "";
            DataTable table = new Doc().GetPagerList(out num2, 8, num, enterpriseWeiXinUserId, str2, str4, "", "", str3, -1);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("title", (JToken)row["Title"].ToString());
                obj1.Add("userName", (JToken)row["WriteUserName"].ToString());
                obj1.Add("writeTime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["WriteTime"].ToString())));
                JObject obj3 = obj1;
                array.Add(obj3);
            }
            JObject obj4 = new JObject();
            obj4.Add("name", (JToken)new DocDir().GetAllParentNames(StringExtensions.ToGuid(str), true, false, @"\"));
            obj4.Add("total", (JToken)num2);
            obj4.Add("hasnext", ((8 * num) >= num2) ? 0 : 1);
            obj4.Add("data", array);
            JObject obj2 = obj4;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 查询开始流程
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("QueryStatrFlow")]
        public string QueryStatrFlow()
        {
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            List<RoadFlow.Model.FlowRun> startFlows = new Flow().GetStartFlows(enterpriseWeiXinUserId);
            JArray array = new JArray();
            Dictionary dictionary = new Dictionary();
            string str = dictionary.GetIdByCode("system_applibrarytype").ToString();
            foreach (IGrouping<Guid, RoadFlow.Model.FlowRun> grouping in Enumerable.OrderBy<IGrouping<Guid, RoadFlow.Model.FlowRun>, Guid>(Enumerable.GroupBy<RoadFlow.Model.FlowRun, Guid>((IEnumerable<RoadFlow.Model.FlowRun>)startFlows,
                key=> key.Type),
               key=>key.Key))
            {
                string str2 = dictionary.GetAllParentTitle(grouping.Key, true, false, str, false);
                JObject obj1 = new JObject();
                obj1.Add("type", (JToken)str2);
                JObject obj2 = obj1;
                JArray array2 = new JArray();
                foreach (RoadFlow.Model.FlowRun run in grouping)
                {
                    string str3 = run.Color.IsNullOrWhiteSpace() ? "#117eef" : run.Color;
                    string str4 = run.Ico.IsNullOrEmpty() ? "fa-pencil-square-o" : run.Ico;
                    JObject obj4 = new JObject();
                    obj4.Add("id", (JToken)run.Id);
                    obj4.Add("name", (JToken)run.Name);
                    obj4.Add("ico", (JToken)str4);
                    obj4.Add("bgcolor", (JToken)str3);
                    JObject obj3 = obj4;
                    array2.Add(obj3);
                }
                obj2.Add("flows", array2);
                array.Add(obj2);
            }
            return array.ToString(0, Array.Empty<JsonConverter>());
        }



        /// <summary>
        /// 查询等待任务
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("QueryWaitTask")]
        public string QueryWaitTask()
        {
            int num2;
            Guid enterpriseWeiXinUserId = Current.EnterpriseWeiXinUserId;
            int num = base.Request.Forms("pagenumber").ToInt(1);
            string str = base.Request.Forms("title");
            DataTable table = new FlowTask().GetWaitTask(8, num, enterpriseWeiXinUserId, "", str, "", "", "ReceiveTime DESC", out num2);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("FlowId", (JToken)row["FlowId"].ToString());
                obj1.Add("StepId", (JToken)row["StepId"].ToString());
                obj1.Add("InstanceId", (JToken)row["InstanceId"].ToString());
                obj1.Add("GroupId", (JToken)row["GroupId"].ToString());
                obj1.Add("Title", (JToken)row["Title"].ToString());
                obj1.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj1.Add("ReceiveTime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                JObject obj3 = obj1;
                array.Add(obj3);
            }
            JObject obj4 = new JObject();
            obj4.Add("total", (JToken)num2);
            obj4.Add("hasnext", ((8 * num) >= num2) ? 0 : 1);
            obj4.Add("data", array);
            JObject obj2 = obj4;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }

       
}



 

}
