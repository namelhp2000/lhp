using System;
using System.Collections.Generic;
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
    public class FlowController : ControllerBase
    {

        /// <summary>
        /// 在流程运行时，得到一个流程可以退回的步骤
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetBackSteps"), ApiValidate]
        public string GetBackSteps()
        {
            Guid guid;
            Guid receiveId;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }
            ValueTuple<string, DateTime> tuple1 = Token.ParseToken(bodyJObject.Value<string>("token"));
            string str = tuple1.Item1;
            DateTime time = tuple1.Item2;
            string str2 = bodyJObject.Value<string>("userid");
            if (str.IsNullOrEmpty())
            {
                return ApiTools.GetErrorJson("参数systemcode为空", 0x3e9);
            }
            if (!StringExtensions.IsGuid(bodyJObject.Value<string>("taskid"), out guid))
            {
                return ApiTools.GetErrorJson("参数taskid不是有效的Guid类型字符串", 0x3e9);
            }
            FlowTask task = new FlowTask();
           RoadFlow.Model. FlowTask task2 = task.Get(guid);
            if (task2 == null)
            {
                return ApiTools.GetErrorJson("未找到taskid对应的任务实体", 0x3e9);
            }
            if (new Flow().GetFlowRunModel(task2.FlowId, true,null) == null)
            {
                return ApiTools.GetErrorJson("未找到流程运行时", 0x3e9);
            }
            if (!StringExtensions.IsGuid(str2, out receiveId))
            {
                receiveId = task2.ReceiveId;
            }
            ValueTuple<string, string, List<Step>> tuple2 = task.GetBackSteps(task2.FlowId, task2.StepId, task2.GroupId, task2.Id, task2.InstanceId, receiveId,null);
            string str4 = tuple2.Item1;
            string str5 = tuple2.Item2;
            List<Step> list = tuple2.Item3;
            JObject jObject = ApiTools.GetJObject(0, "ok");
            JArray array = new JArray();
            foreach (Step step in list)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)step.Id);
                obj1.Add("name", (JToken)step.Name);
                obj1.Add("users", (JToken)step.RunDefaultMembers);
                obj1.Add("note", (JToken)step.Note);
                JObject obj4 = obj1;
                array.Add(obj4);
            }
            jObject.Add("data", array);
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }

        /// <summary>
        /// 得到一个流程的设计json
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetFlowJson"), ApiValidate]
        public string GetFlowJson()
        {
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }
            ValueTuple<string, DateTime> tuple1 = Token.ParseToken(bodyJObject.Value<string>("token"));
            string str = tuple1.Item1;
            DateTime time = tuple1.Item2;

         
            if (str.IsNullOrEmpty())
            {
                return ApiTools.GetErrorJson("参数systemcode为空", 0x3e9);
            }
            Guid systemId = new FlowApiSystem().GetIdBySystemCode(str);
            if (GuidExtensions.IsEmptyGuid(systemId))
            {
                return ApiTools.GetErrorJson("未找到systemcode对应的系统", 0x3e9);
            }
            string flowId = bodyJObject.Value<string>("flowid");
            RoadFlow.Model.Flow flow = new Flow().GetAll().Find(delegate (RoadFlow.Model.Flow p) {
                Guid? nullable = p.SystemId;
                Guid guid1 = systemId;
                return (nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == guid1) : true) : false) && (p.Id == StringExtensions.ToGuid(flowId));
            });
            if (flow == null)
            {
                return ApiTools.GetErrorJson("未找到流程", 0x3e9);
            }
            JObject jObject = ApiTools.GetJObject(0, "ok");
            jObject.Add("data", flow.RunJSON.IsNullOrWhiteSpace() ? ((JToken)flow.DesignerJSON) : ((JToken)flow.RunJSON));
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 得到对应系统标识的所有设计的流程
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetFlows"), ApiValidate]
        public string GetFlows()
        {
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            ValueTuple<string, DateTime> tuple1 = Token.ParseToken(bodyJObject.Value<string>("token"));
            string str = tuple1.Item1;
            DateTime time = tuple1.Item2;

           
            if (str.IsNullOrEmpty())
            {
                return ApiTools.GetErrorJson("参数systemcode为空", 0x3e9);
            }
            Guid systemId = new FlowApiSystem().GetIdBySystemCode(str);
            if (GuidExtensions.IsEmptyGuid(systemId))
            {
                return ApiTools.GetErrorJson("未找到systemcode对应的系统", 0x3e9);
            }
            List<RoadFlow.Model.Flow> list = new Flow().GetAll().FindAll(delegate (RoadFlow.Model.Flow p) {
                Guid? nullable = p.SystemId;
                Guid guid1 = systemId;
                if (!nullable.HasValue)
                {
                    return false;
                }

                return !nullable.HasValue || (nullable.GetValueOrDefault() == guid1);
            });
            JObject jObject = ApiTools.GetJObject(0, "ok");
            jObject.Add("data", JArray.FromObject(list));
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }



        /// <summary>
        /// 在流程运行时，得到一个流程可以发送到后面的步骤
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetSendSteps"), ApiValidate]
        public string GetSendSteps()
        {
            Guid firstStepId;

            Guid guid3;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            ValueTuple<string, DateTime> tuple1 = Token.ParseToken(bodyJObject.Value<string>("token"));
            string str = tuple1.Item1;
            DateTime time = tuple1.Item2;

            if (str.IsNullOrEmpty())
            {
                return ApiTools.GetErrorJson("参数systemcode为空", 0x3e9);
            }
            string str2 = bodyJObject.Value<string>("flowid");
            string str3 = bodyJObject.Value<string>("stepid");
            string str4 = bodyJObject.Value<string>("taskid");
            string str5 = bodyJObject.Value<string>("userid");
            bool isFreeSend = "true".EqualsIgnoreCase(bodyJObject.Value<string>("freesend"));
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(StringExtensions.ToGuid(str2), true);
            if (flowRunModel == null)
            {
                return ApiTools.GetErrorJson("未找到流程运行时", 0x3e9);
            }
            if (!StringExtensions.IsGuid(str3, out firstStepId))
            {
                firstStepId = flowRunModel.FirstStepId;
            }
            FlowTask task = new FlowTask();
            Guid empty = Guid.Empty;
            string instanceId = string.Empty;
            bool isMobile = false;
            if (StringExtensions.IsGuid(str4, out guid3))
            {
                RoadFlow.Model.FlowTask task2 = task.Get(guid3);
                if (task2 != null)
                {
                    empty = task2.GroupId;
                    instanceId = task2.InstanceId;
                }
            }
            ValueTuple<string, string, List<Step>> tuple2 = task.GetNextSteps(flowRunModel, firstStepId, empty, guid3, instanceId, StringExtensions.ToGuid(str5), isFreeSend, isMobile, null);
            string str7 = tuple2.Item1;
            string str8 = tuple2.Item2;
            List<Step> list = tuple2.Item3;
            JObject jObject = ApiTools.GetJObject(0, "ok");
            JArray array = new JArray();
            foreach (Step step in list)
            {
                JObject obj5 = new JObject();
                obj5.Add("id", (JToken)step.Id);
                obj5.Add("name", (JToken)step.Name);
                obj5.Add("users", (JToken)step.RunDefaultMembers);
                obj5.Add("note", (JToken)step.Note);
                JObject obj4 = obj5;
                array.Add(obj4);
            }
            jObject.Add("data", array);
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 得到一个流程一个步骤的表单地址
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetStepFormAddress"), ApiValidate]
        public string GetStepFormAddress()
        {
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }

            ValueTuple<string, DateTime> tuple1 = Token.ParseToken(bodyJObject.Value<string>("token"));
            string str = tuple1.Item1;
            DateTime time = tuple1.Item2;
            if (str.IsNullOrEmpty())
            {
                return ApiTools.GetErrorJson("参数systemcode为空", 0x3e9);
            }
            string str2 = bodyJObject.Value<string>("flowid");
            string stepId = bodyJObject.Value<string>("stepid");
            RoadFlow.Model.FlowRun flowRunModel = new Flow().GetFlowRunModel(StringExtensions.ToGuid(str2), true,null);
            if (flowRunModel == null)
            {
                return ApiTools.GetErrorJson("未找到流程运行时", 0x3e9);
            }
            Step step = flowRunModel.Steps.Find(delegate (Step p) {
                return p.Id == StringExtensions.ToGuid(stepId);
            });
            if (step == null)
            {
                return ApiTools.GetErrorJson("未找到步骤", 0x3e9);
            }
            JObject jObject = ApiTools.GetJObject(0, "ok");
            AppLibrary library = new AppLibrary();
            if (GuidExtensions.IsNotEmptyGuid(step.StepForm.Id))
            {
                RoadFlow.Model.AppLibrary library2 = library.Get(step.StepForm.Id);
                if (library2 != null)
                {
                    jObject.Add("address", (JToken)library2.Address);
                }
            }
            if (GuidExtensions.IsNotEmptyGuid(step.StepForm.MobileId))
            {
                RoadFlow.Model.AppLibrary library3 = library.Get(step.StepForm.MobileId);
                if (library3 != null)
                {
                    jObject.Add("addressmobile", (JToken)library3.Address);
                }
            }
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }


        [HttpPost("GetStepAddress"), ApiValidate]
        public string GetStepAddress()
        {
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }
            List<RoadFlow.Model.User> list = new RoadFlow.Business.User().GetAll();
           
            return JsonConvert.SerializeObject(list);
        }

    }


  



}
