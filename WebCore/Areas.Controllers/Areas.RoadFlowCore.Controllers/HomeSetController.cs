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
    public class HomeSetController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.HomeSet> list = new List<RoadFlow.Model.HomeSet>();
            HomeSet set = new HomeSet();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.HomeSet set2 = set.Get(guid);
                    if (set2 != null)
                    {
                        list.Add(set2);
                    }
                }
            }
            Log.Add("删除了首页设置", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            set.Delete(list.ToArray());
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("id");
            HomeSet set = new HomeSet();
            RoadFlow.Model.HomeSet set2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                set2 = set.Get(guid);
            }
            if (set2 == null)
            {
                RoadFlow.Model.HomeSet set1 = new RoadFlow.Model.HomeSet();
                set1.Id=Guid.NewGuid();
                set1.Type = -1;
                set1.DataSourceType = -1;
                set1.Sort = set.GetMaxSort();
                set2 = set1;
            }
            base.ViewData["dbconnOptions"]= new DbConnection().GetOptions(set2.DbConnId.ToString());
            string[] textArray1 = new string[] { "id=", base.Request.Querys("id"), "&appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"]= string.Concat((string[])textArray1);
            return this.View(set2);
        }

        [Validate, ValidateAntiForgeryToken]
        public string EditSave(RoadFlow.Model.HomeSet homeSetModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            HomeSet set = new HomeSet();
            if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                RoadFlow.Model.HomeSet set2 = set.Get(guid);
                string oldContents = (set2 == null) ? "" : set2.ToString();
                set.Update(homeSetModel);
                Log.Add("修改了首页设置 - " + homeSetModel.Name, "", LogType.系统管理, oldContents, homeSetModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                set.Add(homeSetModel);
                Log.Add("添加了首页设置 - " + homeSetModel.Name, homeSetModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["query"]="appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string Query()
        {
            int num3;
            string str = base.Request.Forms("sidx");
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();
            string str3 = (str.IsNullOrEmpty() ? "Type,Name" : str) + " " + (str2.IsNullOrEmpty() ? "ASC" : str2);
            string str4 = base.Request.Forms("Name1");
            string str5 = base.Request.Forms("Title1");
            string str6 = base.Request.Forms("Type");
            DataTable table = new HomeSet().GetPagerData(out num3, pageSize, pageNumber, str4, str5, str6, str3);
            JArray array = new JArray();
            Organize organize = new Organize();
            foreach (DataRow row in table.Rows)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Name", (JToken)row["Name"].ToString());
                obj1.Add("Title", (JToken)row["Title"].ToString());
                obj1.Add("Type", (row["Type"].ToString().ToInt(-2147483648) == 0) ? "顶部信息统计" : ((row["Type"].ToString().ToInt(-2147483648) == 1) ? "左边模块" : "右边模块"));
                obj1.Add("DataSourceType", (row["DataSourceType"].ToString().ToInt(-2147483648) == 0) ? "SQL语句" : ((row["DataSourceType"].ToString().ToInt(-2147483648) == 1) ? "C#方法" : "URL地址"));
                obj1.Add("UseOrganizes", (JToken)organize.GetNames(row["UseOrganizes"].ToString()));
                obj1.Add("Note", (JToken)row["Note"].ToString());
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }
    }


   



}
