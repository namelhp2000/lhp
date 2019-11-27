using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Mapper;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.Controllers.Areas.TableTemplate.Controllers
{
    [Area("TableTemplate")]
    public class LayuiTableController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string Query()
        {
            int num3;
            //页面值
            int pageNumber = StringExtensions.ToInt(base.Request.Querys("pageNumber"), 1);//默认page

            //页面大小
            int pageSize = StringExtensions.ToInt(base.Request.Querys("pageSize"), 15);//默认limit
            string name = base.Request.Querys("name");
        

            Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();
            dics.Add(("Name", name), datatype.stringType);
            string sqlstr = "select * from RF_User";
            DataTable table = GetPagerTemplate.GetWherePagerList(out num3, pageSize, pageNumber, dics, sqlstr, "Name asc");

            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)row["Id"].ToString());
                obj1.Add("Name", (JToken)row["Name"].ToString());
                obj1.Add("Account", (JToken)row["Account"].ToString());
                obj1.Add("Sex", (JToken)row["Sex"].ToString());
                JObject obj3 = obj1;
                array.Add(obj3);
            }
            JObject obj4 = new JObject();
            //设置备注的字段，就无需使用parseData解析数据属性
            obj4.Add("status", 0);//默认code 
            obj4.Add("message", "");//默认msg
            obj4.Add("total", num3);//默认count
            obj4.Add("data", array);//默认data
            JObject obj2 = obj4;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }
    }
}
