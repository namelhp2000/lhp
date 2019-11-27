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
    public class MenuController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Body()
        {
            Guid guid;
            string str = base.Request.Querys("menuid");
            string str2 = base.Request.Querys("parentid");
            base.ViewData["appId"]= base.Request.Querys("appid");
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["queryString"]= base.Request.UrlQuery();
            Menu menu = new Menu();
            AppLibrary library = new AppLibrary();
            RoadFlow.Model.Menu menu2 = null;
            string str3 = string.Empty;
            if (StringExtensions.IsGuid(str, out guid))
            {
                menu2 = menu.Get(guid);
                if ((menu2 != null) && menu2.AppLibraryId.HasValue)
                {
                    RoadFlow.Model.AppLibrary library2 = library.Get(menu2.AppLibraryId.Value);
                    if (library2 != null)
                    {
                        str3 = library2.Type.ToString();
                    }
                }
            }
            if (menu2 == null)
            {
                RoadFlow.Model.Menu menu1 = new RoadFlow.Model.Menu();
                menu1.Id=Guid.NewGuid();
                menu1.ParentId=StringExtensions.ToGuid(str2);
                menu1.Sort = menu.GetMaxSort(StringExtensions.ToGuid(str2));
                menu2 = menu1;
            }
            base.ViewData["typeOptions"]= library.GetTypeOptions(str3);
            return this.View(menu2);
        }

        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("menuid"), out guid))
            {
                return "ID错误";
            }
            Menu menu = new Menu();
            RoadFlow.Model.Menu menu2 = menu.Get(guid);
            if (menu2 == null)
            {
                return "未找到要删除的菜单";
            }
            RoadFlow.Model.Menu[] menus = new RoadFlow.Model.Menu[] { menu2 };
            menu.Delete(menus);
            Log.Add("删除了菜单-" + menu2.Title, menu2.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");

            return "删除成功!";
        }

        public IActionResult Empty()
        {
            return this.View();
        }

        [Validate]
        public IActionResult Index()
        {
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string Save(RoadFlow.Model.Menu menuModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            Menu menu = new Menu();
            if (StringExtensions.IsGuid(base.Request.Querys("menuid"), out guid))
            {
                RoadFlow.Model.Menu menu2 = menu.Get(guid);
                string oldContents = (menu2 == null) ? "" : menu2.ToString();
                menu.Update(menuModel);
                Log.Add("修改了菜单-" + menuModel.Title, "", LogType.系统管理, oldContents, menuModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                menu.Add(menuModel);
                Log.Add("添加了菜单-" + menuModel.Title, menuModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveSort()
        {
            Menu menu = new Menu();
            string str = base.Request.Forms("sort");
            string str2 = base.Request.Querys("parentid");
            List<RoadFlow.Model.Menu> childs = menu.GetChilds(StringExtensions.ToGuid(str2));
            int num = 0;
            foreach (string str3 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid mid;
                if (StringExtensions.IsGuid(str3, out mid))
                {
                    RoadFlow.Model.Menu menu2 = childs.Find(delegate (RoadFlow.Model.Menu p) {
                        return p.Id == mid;
                    });
                    if (menu2 != null)
                    {
                        menu2.Sort = num += 5;
                    }
                }
            }
            menu.Update(childs.ToArray());
            return "排序成功!";
        }

        [Validate]
        public IActionResult Sort()
        {
            Menu menu = new Menu();
            string str = base.Request.Querys("parentid");
            List<RoadFlow.Model.Menu> childs = menu.GetChilds(StringExtensions.ToGuid(str));
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["parentId"]= str;
            return this.View(childs);
        }

        [Validate]
        public IActionResult Tree()
        {
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        [Validate]
        public string Tree1()
        {
            DataTable menuAppDataTable = new Menu().GetMenuAppDataTable();
            if (menuAppDataTable.Rows.Count == 0)
            {
                return "[]";
            }
            DataRow[] rowArray = menuAppDataTable.Select("ParentId='" + Guid.Empty.ToString() + "'");
            if (rowArray.Length == 0)
            {
                return "[]";
            }
            DataRow row = rowArray[0];
            DataRow[] rowArray2 = menuAppDataTable.Select("ParentId='" + row["Id"].ToString() + "'");
            JArray array = new JArray();
            JObject obj1 = new JObject();
            obj1.Add("id", (JToken)row["Id"].ToString());
            obj1.Add("parentID", (JToken)Guid.Empty);
            obj1.Add("title", (JToken)row["Title"].ToString());
            obj1.Add("ico", (JToken)row["Ico"].ToString());
            obj1.Add("link", (JToken)row["Address"].ToString());
            obj1.Add("type", 0);
            obj1.Add("model", (JToken)row["OpenMode"].ToString());
            obj1.Add("width", (JToken)row["Width"].ToString());
            obj1.Add("height", (JToken)row["Height"].ToString());
            obj1.Add("hasChilds", rowArray2.Length);
            JObject obj2 = obj1;
            array.Add(obj2);
            JArray array2 = new JArray();
            foreach (DataRow row2 in rowArray2)
            {
                JObject obj4 = new JObject();
                obj4.Add("id", (JToken)row2["Id"].ToString());
                obj4.Add("parentID", (JToken)row["Id"].ToString());
                obj4.Add("title", (JToken)row2["Title"].ToString());
                obj4.Add("ico", (JToken)row2["Ico"].ToString());
                obj4.Add("link", (JToken)row2["Address"].ToString());
                obj4.Add("type", 0);
                obj4.Add("model", (JToken)row2["OpenMode"].ToString());
                obj4.Add("width", (JToken)row2["Width"].ToString());
                obj4.Add("height", (JToken)row2["Height"].ToString());
                obj4.Add("hasChilds", menuAppDataTable.Select("ParentId='" + row2["Id"].ToString() + "'").Length);
                JObject obj3 = obj4;
                array2.Add(obj3);
            }
            obj2.Add("childs", array2);
            return array.ToString();
        }

        [Validate]
        public string TreeRefresh()
        {
            string str = base.Request.Querys("refreshid");
            DataTable menuAppDataTable = new Menu().GetMenuAppDataTable();
            if (menuAppDataTable.Rows.Count == 0)
            {
                return "[]";
            }
            DataRow[] rowArray = menuAppDataTable.Select("ParentId='" + str + "'");
            if (rowArray.Length == 0)
            {
                return "[]";
            }
            JArray array = new JArray();
            foreach (DataRow row in rowArray)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("parentID", (JToken)str);
                obj1.Add("title", (JToken)row["Title"].ToString());
                obj1.Add("ico", (JToken)row["Ico"].ToString());
                obj1.Add("link", (JToken)row["Address"].ToString());
                obj1.Add("type", 0);
                obj1.Add("model", (JToken)row["OpenMode"].ToString());
                obj1.Add("width", (JToken)row["Width"].ToString());
                obj1.Add("height", (JToken)row["Height"].ToString());
                obj1.Add("hasChilds", menuAppDataTable.Select("ParentId='" + row["Id"].ToString() + "'").Length);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            return array.ToString();
        }
    }


   



  

}
