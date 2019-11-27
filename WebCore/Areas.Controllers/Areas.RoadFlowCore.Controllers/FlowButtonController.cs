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
    public class FlowButtonController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.FlowButton> list = new List<RoadFlow.Model.FlowButton>();
            FlowButton button = new FlowButton();
            List<RoadFlow.Model.FlowButton> all = button.GetAll();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid bid;
                if (StringExtensions.IsGuid(str2, out bid))
                {
                    RoadFlow.Model.FlowButton button2 = all.Find(delegate (RoadFlow.Model.FlowButton p) {
                        return p.Id == bid;
                    });
                    if (button2 != null)
                    {
                        list.Add(button2);
                    }
                }
            }
            button.Delete(list.ToArray());
            Log.Add("删除了流程按钮", JsonConvert.SerializeObject(list), LogType.流程管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("buttonid");
            FlowButton button = new FlowButton();
            RoadFlow.Model.FlowButton button2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                button2 = button.Get(guid);
            }
            if (button2 == null)
            {
                RoadFlow.Model.FlowButton button1 = new RoadFlow.Model.FlowButton();
                button1.Id=Guid.NewGuid();
                button1.Sort = button.GetMaxSort();
                button2 = button1;
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(button2);
        }

        [Validate]
        public IActionResult Index()
        {
            List<RoadFlow.Model.FlowButton> all = new FlowButton().GetAll();
            JArray array = new JArray();
            foreach (RoadFlow.Model.FlowButton button in all)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)button.Id);
                obj1.Add("Title", (JToken)button.Title);
                obj1.Add("Ico", button.Ico.IsNullOrWhiteSpace() ? ((JToken)"") : (button.Ico.IsFontIco() ? ((JToken)("<i class=\"fa " + button.Ico + "\" style=\"font-size:14px;\"></i>")) : ((JToken)("<img src=\"" + base.Url.Content("~" + button.Ico) + "\" alt=\"\" />"))));
                obj1.Add("Note", (JToken)button.Note);
                obj1.Add("Sort", (JToken)button.Sort);
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + button.Id + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["json"]= array.ToString();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.FlowButton flowButtonModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            FlowButton button = new FlowButton();
            if (StringExtensions.IsGuid(base.Request.Querys("buttonid"), out guid))
            {
                RoadFlow.Model.FlowButton button2 = button.Get(guid);
                string oldContents = (button2 == null) ? "" : button2.ToString();
                button.Update(flowButtonModel);
                Log.Add("修改了流程按钮-" + flowButtonModel.Title, "", LogType.流程管理, oldContents, flowButtonModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                button.Add(flowButtonModel);
                Log.Add("添加了流程按钮-" + flowButtonModel.Title, flowButtonModel.ToString(), LogType.流程管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }
    }


  



}
