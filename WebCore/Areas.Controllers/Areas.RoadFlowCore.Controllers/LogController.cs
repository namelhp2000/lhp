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
    public class LogController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Detail()
        {
            string str = base.Request.Querys("logid");
            RoadFlow.Model.Log log = new Log().Get(StringExtensions.ToGuid(str));
            return this.View(log);
        }

        [Validate]
        public IActionResult Index()
        {
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            base.ViewData["appId"]= str;
            base.ViewData["tabId"]= str2;
            base.ViewData["typeOptions"]= new Log().GetTypeOptions("");
            return this.View();
        }

        [Validate]
        public string Query()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");

            string str3 = base.Request.Forms("Title");
            string str4 = base.Request.Forms("Type");
            string str5 = base.Request.Forms("UserID");
            string str6 = base.Request.Forms("Date1");
            string str7 = base.Request.Forms("Date2");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            string str8 = (str.IsNullOrEmpty() ? "WriteTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            DataTable table = new Log().GetPagerList(out num3, pageSize, pageNumber, str3, str4, str5.RemoveUserPrefix(), str6, str7, str8);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                string[] textArray1 = new string[] { "<a href='javascript:void(0);' class='blue' onclick=\"detail('", row["Id"].ToString(), "');\">", row["Title"].ToString(), "</a>" };
                obj3.Add("Title", (JToken)string.Concat((string[])textArray1));
                obj3.Add("Type", (JToken)row["Type"].ToString());
                obj3.Add("WriteTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["WriteTime"].ToString())));
                obj3.Add("UserName", (JToken)row["UserName"].ToString());
                obj3.Add("IPAddress", (JToken)row["IPAddress"].ToString());
                obj3.Add("CityAddress", (JToken)row["CityAddress"].ToString());
                obj3.Add("Opation", (JToken)("<a href='javascript:void(0);' class='list' onclick=\"detail('" + row["Id"].ToString() + "');\"><i class='fa fa-search'></i>查看</a>"));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }
    }


   



}
