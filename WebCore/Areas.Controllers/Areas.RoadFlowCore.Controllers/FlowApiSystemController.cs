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
    public class FlowApiSystemController : Controller
    {
        // Methods
        public string CheckCode()
        {
            string str = base.Request.Querys("id");
            string str2 = base.Request.Forms("value");
            return (new FlowApiSystem().ValidateSystemCode(StringExtensions.ToGuid(str), str2) ? "1" : "系统标识重复!");
        }

        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.FlowApiSystem> list = new List<RoadFlow.Model.FlowApiSystem>();
            FlowApiSystem system = new FlowApiSystem();
            List<RoadFlow.Model.FlowApiSystem> all = system.GetAll();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid bid;
                if (StringExtensions.IsGuid(str2, out bid))
                {
                    RoadFlow.Model.FlowApiSystem system2 = all.Find(delegate (RoadFlow.Model.FlowApiSystem p) {
                        return p.Id == bid;
                    });
                    if (system2 != null)
                    {
                        list.Add(system2);
                    }
                }
            }
            system.Delete(list.ToArray());
            Log.Add("删除了接口系统", JsonConvert.SerializeObject(list), LogType.流程管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("systemid");
            FlowApiSystem system = new FlowApiSystem();
            RoadFlow.Model.FlowApiSystem system2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                system2 = system.Get(guid);
            }
            if (system2 == null)
            {
                RoadFlow.Model.FlowApiSystem system1 = new RoadFlow.Model.FlowApiSystem();
                system1.Id=Guid.NewGuid();
                system1.Sort = system.GetMaxSort();
                system2 = system1;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(system2);
        }

        [Validate]
        public IActionResult Index()
        {
            List<RoadFlow.Model.FlowApiSystem> all = new FlowApiSystem().GetAll();
            JArray array = new JArray();
            foreach (RoadFlow.Model.FlowApiSystem system in all)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)system.Id);
                obj1.Add("Name", (JToken)system.Name);
                obj1.Add("SystemCode", (JToken)system.SystemCode);
                obj1.Add("SystemIP", (JToken)system.SystemIP);
                obj1.Add("Note", (JToken)system.Note);
                obj1.Add("Sort", (JToken)system.Sort);
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + system.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["json"]= array.ToString();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.FlowApiSystem flowApiSystemModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            FlowApiSystem system = new FlowApiSystem();
            if (StringExtensions.IsGuid(base.Request.Querys("systemid"), out guid))
            {
                RoadFlow.Model.FlowApiSystem system2 = system.Get(guid);
                string oldContents = (system2 == null) ? "" : system2.ToString();
                system.Update(flowApiSystemModel);
                Log.Add("修改了接口系统-" + flowApiSystemModel.Name, "", LogType.流程管理, oldContents, flowApiSystemModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                system.Add(flowApiSystemModel);
                Log.Add("添加了接口系统-" + flowApiSystemModel.Name, flowApiSystemModel.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }
    }





}
