using System;
using System.Collections.Generic;
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
    public class OnlineUserController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Clear()
        {
            foreach (string str in base.Request.Forms("ids").Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str, out guid))
                {
                    OnlineUser.Remove(guid, -1);
                }
            }
            return "清除成功!";
        }

        [Validate]
        public IActionResult Index()
        {
            List<RoadFlow.Model.OnlineUser> all = OnlineUser.GetAll();
            JArray array = new JArray();
            foreach (RoadFlow.Model.OnlineUser user in all)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)user.UserId.ToString());
                obj1.Add("Name", (JToken)user.UserName);
                obj1.Add("Organize", (JToken)user.UserOrganize);
                obj1.Add("LoginTime", (JToken)DateTimeExtensions.ToDateTimeString(user.LoginTime));
                obj1.Add("LastTime", (JToken)DateTimeExtensions.ToDateTimeString(user.LastTime));
                obj1.Add("LastUrl", (JToken)user.LastUrl);
                obj1.Add("IP", (JToken)user.IP);
                obj1.Add("City", (JToken)user.City);
                obj1.Add("Agent", (JToken)user.BrowseAgent);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            base.ViewData["json"]= array.ToString(0, Array.Empty<JsonConverter>());
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }
    }



   



}
