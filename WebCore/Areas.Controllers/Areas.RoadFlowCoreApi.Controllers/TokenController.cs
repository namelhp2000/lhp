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
    public class TokenController : ControllerBase
    {

        /// <summary>
        /// 得到访问token
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetToken")]
        public string GetToken()
        {
            DateTime time;
            JObject bodyJObject = ApiTools.GetBodyJObject(base.Request.Body);
            if (bodyJObject == null)
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }
            string token = ApiTools.GetToken(bodyJObject.Value<string>("systemcode"), out time);
            if (token.IsNullOrWhiteSpace())
            {
                return ApiTools.GetErrorJson("参数错误", 0x3e9);
            }
            JObject jObject = ApiTools.GetJObject(0, "ok");
            jObject.Add("token", (JToken)token);
            jObject.Add("expires", (JToken)time.ToDateTimeString());
            return jObject.ToString(0, Array.Empty<JsonConverter>());
        }
    }






}
