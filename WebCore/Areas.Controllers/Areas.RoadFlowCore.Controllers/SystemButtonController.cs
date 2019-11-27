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
    public class SystemButtonController : Controller
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.SystemButton> list = new SystemButton().Delete(str);
            Log.Add("删除了系统按钮库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("buttonid");
            SystemButton button = new SystemButton();
            RoadFlow.Model.SystemButton button2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                button2 = button.Get(guid);
            }
            if (button2 == null)
            {
                RoadFlow.Model.SystemButton button1 = new RoadFlow.Model.SystemButton();
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

            //RoadFlow.FreeSql.DataContext context = new RoadFlow.FreeSql.DataContext("sqlserver", "server=.;database=NewRoadFlowCore;uid=sa;pwd=sa");


            //var items = new List<RoadFlow.FreeSql.Model.Topic>();
            //for (var a = 0; a < 10; a++) items.Add(new RoadFlow.FreeSql.Model.Topic { Id = a + 1, Title = $"newtitle{a}", Clicks = a * 100 });


            //var t1 = context.Add<RoadFlow.FreeSql.Model.Topic>(items.First());

            //  var t16 = context.QueryAll<RoadFlow.FreeSql.Model.Topic>();


            List<RoadFlow.Model.SystemButton> all = new SystemButton().GetAll();
            JArray array = new JArray();
            foreach (RoadFlow.Model.SystemButton button in all)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)button.Id);
                obj1.Add("Name", (JToken)button.Name);
                obj1.Add("Ico", button.Ico.IsNullOrWhiteSpace() ? ((JToken)"") : (button.Ico.IsFontIco() ? ((JToken)("<i class='fa " + button.Ico + "'></i>")) : ((JToken)("<img src='" + button.Ico + "'/>"))));
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
        public string Save(RoadFlow.Model.SystemButton systemButtonModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            SystemButton button = new SystemButton();

            if (StringExtensions.IsGuid(base.Request.Querys("buttonid"), out guid))
            {
                RoadFlow.Model.SystemButton button2 = button.Get(guid);
                string oldContents = (button2 == null) ? "" : button2.ToString();
                button.Update(systemButtonModel);
                Log.Add("修改了系统按钮库-" + systemButtonModel.Name, "", LogType.系统管理, oldContents, systemButtonModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                button.Add(systemButtonModel);
                Log.Add("添加了系统按钮库-" + systemButtonModel.Name, systemButtonModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }
    }


   


}
