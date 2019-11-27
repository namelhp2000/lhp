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
    public class MessageController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Index()
        {
            return this.View();
        }

        [Validate(CheckApp = false, CheckUrl = false)]
        public IActionResult NoRead()
        {
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public string QuerySendList()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize =  Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            bool flag = "asc".EqualsIgnoreCase(str2);
            string str3 = (str.IsNullOrEmpty() ? "SendTime" : str) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);
            string str4 = base.Request.Forms("Contents");
            string str5 = base.Request.Forms("Date1");
            string str6 = base.Request.Forms("Date2");
            string str7 = base.Request.Forms("status");
            string str8 = Current.UserId.ToString();
            Message message = new Message();
            DataTable table = message.GetSendList(out num3, pageSize, pageNumber, str8, str4, str5, str6, str7, str3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                string htmlstring = row["Contents"].ToString();
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                string[] textArray1 = new string[] { "<a class=\"blue\" href=\"javascript:void(0);\" onclick=\"show('", row["Id"].ToString(), "');return false;\">", htmlstring.RemoveHTML().Trim1().CutOut(60, "…"), "</a>" };
                obj3.Add("Contents", (JToken)string.Concat((string[])textArray1));
                obj3.Add("SenderName", (JToken)row["SenderName"].ToString());
                obj3.Add("SendTime", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["SendTime"].ToString())));
                obj3.Add("SendType", (JToken)message.GetSendTypeString(row["SendType"].ToString()));
                obj3.Add("Files", (JToken)row["Files"].ToString().ToFilesShowString(false));
                obj3.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"show('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-eye\"></i>查看</a>"));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate]
        public string QuerySendRead()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            bool flag = "asc".EqualsIgnoreCase(str2);
            string str3 = (str.IsNullOrEmpty() ? "UserId" : str) + " " + (str2.IsNullOrEmpty() ? "ASC" : str2);
            string str4 = base.Request.Querys("messageid");
            MessageUser user = new MessageUser();
            Organize organize = new Organize();
            User user2 = new User();
            DataTable table = user.GetReadUserList(out num3, pageSize, pageNumber, str4, str3);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                DateTime time;
                JObject obj1 = new JObject();
                obj1.Add("UserId", (JToken)user2.GetName(StringExtensions.ToGuid(row["UserId"].ToString())));
                obj1.Add("Organize", (JToken)user2.GetOrganizeMainShowHtml(StringExtensions.ToGuid(row["UserId"].ToString()), true));
                obj1.Add("IsRead", row["IsRead"].ToString().Equals("0") ? "未读" : "已读");
                obj1.Add("ReadTime", StringExtensions.IsDateTime(row["ReadTime"].ToString(), out time) ? ((JToken)DateTimeExtensions.ToDateTimeString(time)) : ((JToken)""));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        [Validate(CheckApp = false, CheckUrl = false)]
        public IActionResult ReadList()
        {
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSend()
        {
            string str = base.Request.Forms("ReceiverId");
            string str2 = base.Request.Forms("ReceiveType");
            string str3 = base.Request.Forms("Contents");
            string str4 = base.Request.Forms("Files");
            if ((str.IsNullOrWhiteSpace() || str2.IsNullOrWhiteSpace()) || str3.IsNullOrWhiteSpace())
            {
                return "必填数据不能为空!";
            }
            RoadFlow.Model.Message message1 = new RoadFlow.Model.Message
            {
                Contents = str3,
                Files = str4
            };
            message1.Id=Guid.NewGuid();
            message1.ReceiverIdString = str;
            message1.SenderId=new Guid?(Current.UserId);
            message1.SenderName = Current.UserName;
            message1.SendType = str2;
            message1.SendTime= DateTimeExtensions.Now;
            message1.Type = 0;
            RoadFlow.Model.Message message = message1;
            string str5 = new Message().Send(message, (List<RoadFlow.Model.User>)null);
            if (!"1".Equals(str5))
            {
                return str5;
            }
            return "发送成功!";
        }

        [Validate]
        public IActionResult Send()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate]
        public IActionResult SendList()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            return this.View();
        }

        [Validate]
        public IActionResult SendShow()
        {
            string str = base.Request.Querys("messageid");
            RoadFlow.Model.Message message = new Message().Get(StringExtensions.ToGuid(str));
            if (message == null)
            {
                ContentResult result1 = new ContentResult();
                result1.Content="没有找到该消息!";
                return result1;
            }
            string str2 = base.Request.Querys("showreadlist");
            if (!"1".Equals(str2))
            {
                new MessageUser().UpdateIsRead(message.Id, Current.UserId);
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["showreadlist"]= str2;
            return this.View(message);
        }

        [Validate(CheckApp = false)]
        public string UpdateRead()
        {
            string str = base.Request.Forms("ids");
            Guid userId = Current.UserId;
            MessageUser user = new MessageUser();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid2;
                if (StringExtensions.IsGuid(str2, out guid2))
                {
                    user.UpdateIsRead(guid2, userId);
                }
            }
            return "标记成功!";
        }





        [Validate(CheckApp = false)]
        public string DeleteSerd()
        {
            string str = base.Request.Forms("ids");
            Guid userId = Current.UserId;
            MessageUser user = new MessageUser();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid2;
                if (StringExtensions.IsGuid(str2, out guid2))
                {
                    user.DeleteSerd(guid2, userId);
                }
            }
            return "删除成功!";
        }


    }


  



}
