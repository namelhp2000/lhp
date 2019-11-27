using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCoreApi.Controllers
{
    [Route("RoadFlowCoreApi/[controller]"), ApiController]
    public class FlowTaskController : ControllerBase
    {

        /// <summary>
        /// 执行流程，发送或退回流程
        /// </summary>
        /// <returns></returns>
        [HttpPost("ExecuteTask"), ApiValidate]
        public string ExecuteTask()
        {
            Guid guid;
            Guid guid2;
            Guid guid3;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            string str = bodyJObject.Value<string>("flowid");
            string type = bodyJObject.Value<string>("type");
            string str3 = bodyJObject.Value<string>("comment");
            string str4 = bodyJObject.Value<string>("taskid");
            string instanceId = bodyJObject.Value<string>("instanceid");
            string str6 = bodyJObject.Value<string>("senderid");
            string title = bodyJObject.Value<string>("tasktitle");
            JArray array = null;
            try
            {
                array = bodyJObject.Value<JArray>("steps");
            }
            catch
            {
            }
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return ApiTools.GetErrorJson("参数flowid不是Guid", 0x3e9);
            }
            if (!StringExtensions.IsGuid(str6, out guid2))
            {
                return ApiTools.GetErrorJson("参数senderid不是Guid", 0x3e9);
            }
            User user = new User();
            Organize organize = new Organize();
            RoadFlow.Model. User user2 = user.Get(guid2);
            if (user2 == null)
            {
                return ApiTools.GetErrorJson("未找到发送人", 0x3e9);
            }
            FlowTask task = new FlowTask();
            RoadFlow.Model.FlowTask task2 = null;
            if (StringExtensions.IsGuid(str4, out guid3))
            {
                task2 = task.Get(guid3);
            }
            Execute executeModel = new Execute
            {
                Comment = str3,
                ExecuteType = task.GetExecuteType(type),
                   FlowId = guid,
                InstanceId = instanceId,
                Sender = user2,
                Title = title

            };
          
            if (task2 != null)
            {
                executeModel.GroupId=task2.GroupId;
                executeModel.StepId=task2.StepId;
                executeModel.TaskId=task2.Id;
                if (instanceId.IsNullOrWhiteSpace())
                {
                    instanceId = task2.InstanceId;
                }
                if (title.IsNullOrWhiteSpace())
                {
                    title = task2.Title;
                }
            }
            else
            {
                RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(guid, true,null);
                if (flowRunModel == null)
                {
                    return ApiTools.GetErrorJson("未找到流程运行时", 0x3e9);
                }
                executeModel.StepId=flowRunModel.FirstStepId;
            }
            if (array != null)
            {
                List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>> list = new List<ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>>();

                foreach (JObject obj4 in array)
                {
                    Guid guid5;
                    int num2;

                    DateTime time;
                    Guid guid4 = StringExtensions.ToGuid(obj4.Value<string>("id"));

                    string str9 = obj4.Value<string>("name");
                    string str10 = obj4.Value<string>("beforestepid");
                    string str11 = obj4.Value<string>("parallelorserial");


                    List<RoadFlow.Model.User> allUsers = organize.GetAllUsers(obj4.Value<string>("users"));
                    string str12 = obj4.Value<string>("completedtime");



                    list.Add(new ValueTuple<Guid, string, Guid?, int?, List<RoadFlow.Model.User>, DateTime?>(guid4, str9, StringExtensions.IsGuid(str10, out guid5) ? new Guid?(guid5) : null, str11.IsInt(out num2) ? new int?(num2) : null, allUsers, StringExtensions.IsDateTime(str12, out time) ? new DateTime?(time) : null));
                }
                executeModel.Steps=list;
            }
            ExecuteResult result = task.Execute(executeModel);
            int errcode = result.IsSuccess ? 0 : 0x7d1;
            string messages = result.Messages;
            JObject jObject = ApiTools.GetJObject(errcode, messages);
            jObject.Add("data", JObject.FromObject(result));
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 查询已办事项列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetCompletedTasks"), ApiValidate]
        public string GetCompletedTasks()
        {
            Guid guid;
            int num3;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            string str = bodyJObject.Value<string>("userid");
            int size = bodyJObject.Value<string>("pagesize").ToInt(10);
            int number = bodyJObject.Value<string>("pagenumber").ToInt(1);
            string str2 = bodyJObject.Value<string>("flowid");
            string title = bodyJObject.Value<string>("title");
            string startDate = bodyJObject.Value<string>("startdate");
            string endDate = bodyJObject.Value<string>("enddate");
            string str6 = bodyJObject.Value<string>("order");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return ApiTools.GetErrorJson("用户id错误", 0x3e9);
            }
            if (str2.IsNullOrWhiteSpace())
            {
                return ApiTools.GetErrorJson("流程ID不能为空", 0x3e9);
            }
            DataTable table = new FlowTask().GetCompletedTask(size, number, guid, str2, title, startDate, endDate, str6.IsNullOrWhiteSpace() ? "CompletedTime1 DESC" : str6, out num3);
            JArray array = new JArray();
            JObject jObject = ApiTools.GetJObject(0, "ok");
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                DateTime time2;
                JObject obj5 = new JObject();
                obj5.Add("id", (JToken)row["Id"].ToString());
                obj5.Add("title", (JToken)row["Title"].ToString());
                obj5.Add("flowid", (JToken)row["FlowId"].ToString());
                obj5.Add("flowname", (JToken)row["FlowName"].ToString());
                obj5.Add("stepid", (JToken)row["StepId"].ToString());
                obj5.Add("instanceid", (JToken)row["InstanceId"].ToString());
                obj5.Add("groupid", (JToken)row["GroupId"].ToString());
                obj5.Add("stepname", (JToken)row["StepName"].ToString());
                obj5.Add("senderid", (JToken)row["SenderId"].ToString());
                obj5.Add("sendername", (JToken)row["SenderName"].ToString());
                obj5.Add("receivetime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                obj5.Add("completedtime", StringExtensions.IsDateTime(row["CompletedTime"].ToString(), out time) ? ((JToken)DateTimeExtensions.ToShortDateTimeString(time)) : ((JToken)""));
                obj5.Add("completedtime1", StringExtensions.IsDateTime(row["CompletedTime1"].ToString(), out time2) ? ((JToken)DateTimeExtensions.ToShortDateTimeString(time2)) : ((JToken)""));
                obj5.Add("executetype", (JToken)row["ExecuteType"].ToString().ToInt(-2147483648));

                obj5.Add("status", (JToken)row["status"].ToString().ToInt(-2147483648));
                obj5.Add("note", (JToken)row["Note"].ToString());
                JObject obj4 = obj5;
                array.Add(obj4);
            }
            jObject.Add("count", (JToken)num3);
            jObject.Add("data", array);
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }



        /// <summary>
        /// 查询一个实例组列表(用于在第三方系统显示一个流程实例的处理过程)
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetGroupTasks"), ApiValidate]
        public string GetGroupTasks()
        {
            Guid guid;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            if (!StringExtensions.IsGuid(bodyJObject.Value<string>("groupid"), out guid))
            {
                return ApiTools.GetErrorJson("参数groupid不是有效的Guid字符串", 0x3e9);
            }
            List<RoadFlow.Model.FlowTask> listByGroupId = new FlowTask().GetListByGroupId(guid);
            JObject jObject = ApiTools.GetJObject(0, "ok");
            jObject.Add("data", JArray.FromObject(listByGroupId));
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 查询待办事项列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetWaitTasks"), ApiValidate]
        public string GetWaitTasks()
        {
            Guid guid;
            int num3;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            string str = bodyJObject.Value<string>("userid");
            int size = bodyJObject.Value<string>("pagesize").ToInt(10);
            int number = bodyJObject.Value<string>("pagenumber").ToInt(1);
            string str2 = bodyJObject.Value<string>("flowid");
            string title = bodyJObject.Value<string>("title");
            string startDate = bodyJObject.Value<string>("startdate");
            string endDate = bodyJObject.Value<string>("enddate");
            string str6 = bodyJObject.Value<string>("order");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return ApiTools.GetErrorJson("用户id错误", 0x3e9);
            }
            if (str2.IsNullOrWhiteSpace())
            {
                return ApiTools.GetErrorJson("流程ID不能为空", 0x3e9);
            }
            DataTable table = new FlowTask().GetWaitTask(size, number, guid, str2, title, startDate, endDate, str6.IsNullOrWhiteSpace() ? "ReceiveTime DESC" : str6, out num3);
            JArray array = new JArray();
            JObject jObject = ApiTools.GetJObject(0, "ok");
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                JObject obj5 = new JObject();
                obj5.Add("id", (JToken)row["Id"].ToString());
                obj5.Add("title", (JToken)row["Title"].ToString());
                obj5.Add("flowid", (JToken)row["FlowId"].ToString());
                obj5.Add("flowname", (JToken)row["FlowName"].ToString());
                obj5.Add("stepid", (JToken)row["StepId"].ToString());
                obj5.Add("instanceid", (JToken)row["InstanceId"].ToString());
                obj5.Add("groupid", (JToken)row["GroupId"].ToString());
                obj5.Add("stepname", (JToken)row["StepName"].ToString());
                obj5.Add("senderid", (JToken)row["SenderId"].ToString());
                obj5.Add("sendername", (JToken)row["SenderName"].ToString());
                obj5.Add("receivetime", (JToken)DateTimeExtensions.ToShortDateTimeString(StringExtensions.ToDateTime(row["ReceiveTime"].ToString())));
                obj5.Add("completedtime", StringExtensions.IsDateTime(row["CompletedTime"].ToString(), out time) ? ((JToken)DateTimeExtensions.ToShortDateTimeString(time)) : ((JToken)""));
                obj5.Add("status", (JToken)row["status"].ToString().ToInt(-2147483648));
                obj5.Add("note", (JToken)row["Note"].ToString());
                JObject obj4 = obj5;
                array.Add(obj4);
            }
            jObject.Add("count", (JToken)num3);
            jObject.Add("data", array);
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }
    }


   



}
