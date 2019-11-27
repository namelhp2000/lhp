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

namespace WebCore.Areas.Controllers.Areas.TableTemplate.Controllers
{
    [Area("TableTemplate")]
    public class JqGridTableController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            string str = base.Request.Querys("appid");
            string str2 = base.Request.Querys("tabid");
            string str3 = base.Request.Querys("typeid");
            base.ViewData["appId"] = str;
            base.ViewData["tabId"] = str;
            base.ViewData["typeId"] = str3;
            string[] textArray1 = new string[] { "typeid=", str3, "&appid=", str, "&tabid=", str2 };
            base.ViewData["query"] = string.Concat((string[])textArray1);
           
            return View();
        }


        [Validate]
        public string Query()
        {
            Guid guid;
            int num3;
            string str = base.Request.Forms("Title");
            string str2 = base.Request.Forms("Address");
            string str3 = base.Request.Forms("typeid");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize(true);
            int pageNumber = Tools.GetPageNumber();
            string str6 = (str4.IsNullOrEmpty() ? "Type,Title" : str4) + " " + (str5.IsNullOrEmpty() ? "ASC" : str5);
            Dictionary dictionary = new Dictionary();
            if (StringExtensions.IsGuid(str3, out guid))
            {
                str3 = StringExtensions.JoinSqlIn<Guid>(dictionary.GetAllChildsId(guid, true), true);
            }
            DataTable table = new AppLibrary().GetPagerList(out num3, pageSize, pageNumber, str, str2, str3, str6);
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)row["Id"].ToString());
                obj3.Add("Title", (JToken)row["Title"].ToString());
                obj3.Add("Address", (JToken)row["Address"].ToString());
                obj3.Add("TypeTitle", (JToken)dictionary.GetTitle(StringExtensions.ToGuid(row["Type"].ToString())));
                string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"editButton('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-square-o\"></i>按钮</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

    }
}
