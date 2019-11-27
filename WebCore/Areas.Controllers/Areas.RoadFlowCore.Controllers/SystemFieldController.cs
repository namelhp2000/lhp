using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Mapper;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class SystemFieldController : Controller
    {


        #region 编辑--模块

      

        /// <summary>
        /// 编辑保存
        /// </summary>
     
        /// <returns></returns>
       
        public string Save()
        {
            DataTable table = new RoadFlow.Business.SystemField().SystemTable1();
            if (table.Rows.Count  == 0)
            {
                return "0";
            }
            List<RoadFlow.Model.SystemField> list2 = new List<RoadFlow.Model.SystemField>();
            foreach (DataRow row in table.Rows)
            {
                if (!row["SystemTable"].ToString().IsNullOrWhiteSpace() && !row["FieldName"].ToString().IsNullOrWhiteSpace())
                {

                    RoadFlow.Model.SystemField systemfield = new RoadFlow.Model.SystemField();
                    systemfield.Id = Guid.NewGuid();
                    systemfield.SystemTable = row["SystemTable"].ToString();
                    systemfield.TableId = int.Parse(row["TableId"].ToString());

                    systemfield.FieldName = row["FieldName"].ToString();

                    systemfield.FieldDescribe = row["FieldDescribe"].ToString();

                    systemfield.FieldType = row["FieldType"].ToString();

                    systemfield.FieldDecimalDigits = row["FieldDecimalDigits"].ToString();

                    systemfield.FieldLength = int.Parse(row["FieldLength"].ToString());

                    systemfield.FieldIdentity = row["FieldIdentity"].ToString();

                    systemfield.FieldPrimaryKey = row["FieldPrimaryKey"].ToString();

                    systemfield.FieldAllowEmpty = row["FieldAllowEmpty"].ToString();

                    systemfield.FieldDefaultValue = row["FieldDefaultValue"].ToString();
                    systemfield.CreateDate = DateTime.Now;
                    list2.Add(systemfield);
                }
            }
             using (DataContext context = new DataContext())
            {
                context.Execute(" DELETE FROM RF_SystemField ");
                context.AddRange<RoadFlow.Model.SystemField>(list2.ToArray());
                context.SaveChanges();
            }
            return "保存成功!";
        }

        #endregion


        #region  Index--模块
        [Validate]
        public IActionResult Index()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
        
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            return this.View();
        }
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string Query()
        {
            ValueTuple<string, string>[] tuples = new ValueTuple<string, string>[3];
            
           
            string SystemTable = base.Request.Forms("SystemTable");
            string FieldName = base.Request.Forms("FieldName");
            string FieldDescribe = base.Request.Forms("FieldDescribe");
            string str4 = base.Request.Forms("sidx");
            string str5 = base.Request.Forms("sord");
            bool flag = "asc".EqualsIgnoreCase(str5);
            string order = (str4.IsNullOrEmpty() ? "SystemTable" : str4) + " " + (str5.IsNullOrEmpty() ? "DESC" : str5);
            tuples[0] = new ValueTuple<string, string>("SystemTable", SystemTable);
            tuples[1] = new ValueTuple<string, string>("FieldName", FieldName);
            tuples[2] = new ValueTuple<string, string>("FieldDescribe", FieldDescribe);



            //抓取数据转table
            DataTable table = new SystemField().SystemTable(tuples, order);
            JArray array = new JArray();
        
            foreach (DataRow row in table.Rows)
            {
              
               
                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();
                obj1.Add("SystemTable", (JToken)row["SystemTable"].ToString());
                obj1.Add("TableId", (JToken)row["TableId"].ToString());
                obj1.Add("FieldName", (JToken)row["FieldName"].ToString());
                obj1.Add("FieldDescribe", (JToken)row["FieldDescribe"].ToString());
                obj1.Add("FieldType", (JToken)row["FieldType"].ToString());
                obj1.Add("FieldDecimalDigits", (JToken)row["FieldDecimalDigits"].ToString());
                obj1.Add("FieldLength", (JToken)row["FieldLength"].ToString());
                obj1.Add("FieldIdentity", (JToken)row["FieldIdentity"].ToString());
                obj1.Add("FieldPrimaryKey", (JToken)row["FieldPrimaryKey"].ToString());
                obj1.Add("FieldAllowEmpty", (JToken)row["FieldAllowEmpty"].ToString());
                obj1.Add("FieldDefaultValue", (JToken)row["FieldDefaultValue"].ToString());
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["SystemTable"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }

        #endregion


    


    }





}
