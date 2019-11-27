using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowArchiveController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Index()
        {
            base.ViewData["flowOptions"]= new Flow().GetOptions("");
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public string query()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            string str3 = (str.IsNullOrEmpty() ? "WriteTime" : str) + " " + (str2.IsNullOrEmpty() ? "ASC" : str2);
            string str4 = base.Request.Forms("FlowID");
            string str5 = base.Request.Forms("stepName");
            string str6 = base.Request.Forms("flowTitle");
            string str7 = base.Request.Forms("Date1");
            string str8 = base.Request.Forms("Date2");
            DataTable table = new FlowArchive().GetPagerData(out num3, pageSize, pageNumber, str4, str5, str6, str7, str8, str3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                string[] textArray1 = new string[] { "<a class=\"blue\" href=\"javascript:void(0);\" onclick=\"show('", row["Id"].ToString(), "', '", row["FlowId"].ToString(), "','", row["StepId"].ToString(), "','", row["TaskId"].ToString(), "','", row["GroupId"].ToString(), "','", row["InstanceId"].ToString(), "');return false;\">", row["Title"].ToString(), "</a>" };
                string str9 = string.Concat((string[])textArray1);
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Title", (JToken)str9);
                obj3.Add("FlowName", (JToken)row["FlowName"].ToString());
                obj3.Add("StepName", (JToken)row["StepName"].ToString());
                obj3.Add("WriteTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["WriteTime"].ToString())));
                obj3.Add("UserName", (JToken)row["UserName"].ToString());
                string[] textArray2 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"show('", row["Id"].ToString(), "', '", row["FlowId"].ToString(), "','", row["StepId"].ToString(), "','", row["TaskId"].ToString(), "','", row["GroupId"].ToString(), "','", row["InstanceId"].ToString(), "');return false;\"><i class=\"fa fa-search\"></i>查看</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray2));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }
    }


   



}
