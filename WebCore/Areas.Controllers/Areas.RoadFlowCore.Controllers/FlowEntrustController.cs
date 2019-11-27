using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class FlowEntrustController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.FlowEntrust> list = new List<RoadFlow.Model.FlowEntrust>();
            FlowEntrust entrust = new FlowEntrust();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.FlowEntrust entrust2 = entrust.Get(guid);
                    if (entrust2 != null)
                    {
                        list.Add(entrust2);
                    }
                }
            }
            int num = entrust.Delete(list.ToArray());
            Log.Add("删除了流程委托", JsonConvert.SerializeObject(list), LogType.流程管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("entrustid");
            RoadFlow.Model.FlowEntrust entrust = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                entrust = new FlowEntrust().Get(guid);
            }
            if (entrust == null)
            {
                RoadFlow.Model.FlowEntrust entrust1 = new RoadFlow.Model.FlowEntrust();
                entrust1.Id=Guid.NewGuid();
                entrust1.StartTime= DateTimeExtensions.Now;
                entrust1.WriteTime= DateTimeExtensions.Now;
                entrust = entrust1;
                if ("1".Equals(base.Request.Querys("isoneself")))
                {
                    entrust.UserId=Current.UserId;
                }
            }
            base.ViewData["isAdd"]= (bool)base.Request.Querys("entrustid").ToString().IsNullOrWhiteSpace()?"1":"0";
            base.ViewData["isOneSelf"]= base.Request.Querys("isoneself");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["pageSize"]= base.Request.Querys("pagesize");
            base.ViewData["pageNumber"]= base.Request.Querys("pagenumber");
            base.ViewData["flowOptions"]= new Flow().GetOptions(entrust.FlowId.ToString());
            return this.View(entrust);
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["isOneSelf"]= base.Request.Querys("isoneself");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"), "&isoneself=", base.Request.Querys("isoneself") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View();
        }

        [Validate]
        public string Query()
        {
            int num3;
            string str = base.Request.Forms("UserID");
            string str2 = base.Request.Forms("Date1");
            string str3 = base.Request.Forms("Date2");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            string str6 = base.Request.Querys("isoneself");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            bool flag = "asc".EqualsIgnoreCase(str5);
            string str7 = (str4.IsNullOrEmpty() ? "WriteTime" : str4) + " " + (str5.IsNullOrEmpty() ? "DESC" : str5);
            Guid guid = "1".Equals(str6) ? Current.UserId : new User().GetUserId(str);
            DataTable table = new FlowEntrust().GetPagerList(out num3, pageSize, pageNumber, GuidExtensions.IsEmptyGuid(guid) ? "" : guid.ToString(), str2, str3, str7);

            JArray array = new JArray();
            User user = new User();
            Flow flow = new Flow();
            foreach (DataRow row in table.Rows)
            {
                Guid guid2;
                string str8 = string.Empty;
                DateTime time = StringExtensions.ToDateTime(row["StartTime"].ToString());
                DateTime time2 = StringExtensions.ToDateTime(row["EndTime"].ToString());
                DateTime time3 = DateTimeExtensions.Now;
                if (time >= time3)
                {
                    str8 = "<span>未开始</span>";
                }
                else if (time2 <= time3)
                {
                    str8 = "<span style=\"color:#666;\">已结束</span>";
                }
                else
                {
                    str8 = "<span style=\"color:green;font-weight:bold;\">委托中</span>";
                }
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("UserID", (JToken)user.GetName(StringExtensions.ToGuid(row["UserID"].ToString())));
                obj1.Add("ToUserID", (JToken)user.GetNames(row["ToUserID"].ToString()));
                obj1.Add("FlowID", !StringExtensions.IsGuid(row["FlowID"].ToString(), out guid2) ? ((JToken)"全部流程") : ((JToken)flow.GetName(guid2)));
                obj1.Add("WriteTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["WriteTime"].ToString())));
                obj1.Add("StartTime", (JToken)DateTimeExtensions.ToDateTimeString(time));
                obj1.Add("EndTime", (JToken)DateTimeExtensions.ToDateTimeString(time2));
                obj1.Add("Note", (JToken)row["Note"].ToString());
                obj1.Add("Status", (JToken)str8);
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.FlowEntrust flowEntrustModel)
        {
            Guid guid;
            flowEntrustModel.UserId=new User().GetUserId(base.Request.Forms("UserId").ToString());
            FlowEntrust entrust = new FlowEntrust();
            if (StringExtensions.IsGuid(base.Request.Querys("entrustid"), out guid))
            {
                RoadFlow.Model.FlowEntrust entrust2 = entrust.Get(guid);
                string oldContents = (entrust2 == null) ? "" : entrust2.ToString();
                entrust.Update(flowEntrustModel);
                Log.Add("修改了流程委托-" + flowEntrustModel.Id, "", LogType.流程管理, oldContents, flowEntrustModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                entrust.Add(flowEntrustModel);
                Log.Add("添加了流程委托-" + flowEntrustModel.Id, flowEntrustModel.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }
    }


 



}
