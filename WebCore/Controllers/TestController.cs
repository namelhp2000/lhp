using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Controllers
{
    #region 旧的方法
    //public class TestController : Controller
    //{
    //    // Methods
    //    public void FlowSubmitAfter(EventParam eventParam)
    //    {
    //        Log.Add("进入了流程提交后事件-" + eventParam.TaskTitle, eventParam.ToString(), Log.Type.其它, "", "", "", "", "", "", "", "");
    //    }

    //    public string FlowSubmitBefor(EventParam eventParam)
    //    {
    //        Log.Add("进入了流程提交前事件-" + eventParam.TaskTitle, eventParam.ToString(), Log.Type.其它, "", "", "", "", "", "", "", "");
    //        return "1";
    //    }

    //    public IActionResult Index()
    //    {
    //        Stopwatch stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        List<RoadFlow.Model.FlowTask> list = new List<RoadFlow.Model.FlowTask>();
    //        object obj2 = IO.Get("test");
    //        if (obj2 == null)
    //        {
    //            using (DataContext context = new DataContext())
    //            {
    //                list = context.QueryAll<RoadFlow.Model.FlowTask>();
    //                IO.Insert("test", list);
    //                goto Label_0061;
    //            }
    //        }
    //        list = obj2 as List<RoadFlow.Model.FlowTask>;
    //        base.ViewData["msg"]= "缓存";
    //        Label_0061:
    //        stopwatch.Stop();
    //        return this.View();
    //    }

    //    public string ProgramShow(DataRow dr)
    //    {
    //        return ("custom-" + dr["f15"].ToString());
    //    }

    //    public bool StepSkip(EventParam eventParam)
    //    {
    //        FlowTask other = (FlowTask)eventParam.Other;
    //        return false;
    //    }

    //    public string SubFlowActive(EventParam eventParam)
    //    {
    //        string str = Guid.NewGuid().ToString();
    //        string str2 = "子流程测试-" + eventParam.TaskTitle;
    //        string sql = "insert into RF_Test(Id,f1) values({0},{1})";
    //        using (DataContext context = new DataContext())
    //        {
    //            object[] objects = new object[] { str, str2 };
    //            context.Execute(sql, objects);
    //            context.SaveChanges();
    //        }
    //        JObject obj1 = new JObject();
    //        obj1.Add("instanceId", (JToken)str);
    //        obj1.Add("title", (JToken)str2);
    //        JObject obj2 = obj1;
    //        return obj2.ToString();
    //    }

    //    public IActionResult Test()
    //    {
    //        base.ViewData["test"]= SessionExtensions.GetString(base.HttpContext.Session, "test");
    //        return this.View();
    //    }

    //    public string TestOptions()
    //    {
    //        return "<option value='1'>test1</option><option value='2'>test2</option><option value='3'>test3</option>";
    //    }

    //    public string TestOptions1()
    //    {
    //        return "<option value='1'>test1</option><option value='2'>test2</option><option value='3'>test3</option>";
    //    }
    //}
    #endregion



    #region 新的方法 2.8.3

    public class TestController : Controller
    {
        // Methods
        public IActionResult CustomForm()
        {
            string str = base.Request.Querys("instanceid");
            DataTable dataTable = new DataTable();
            if (!str.IsNullOrWhiteSpace())
            {
                using (DataContext context = new DataContext())
                {
                    object[] objArray1 = new object[] { str };
                    dataTable = context.GetDataTable("select * from rf_test where id={0}", objArray1);
                }
            }
            return this.View(dataTable);
        }

        public IActionResult CustomForm1()
        {
            string str = base.Request.Querys("instanceid");
            DataTable dataTable = new DataTable();
            if (!str.IsNullOrWhiteSpace())
            {
                using (DataContext context = new DataContext())
                {
                    object[] objArray1 = new object[] { str };
                    dataTable = context.GetDataTable("select * from rf_test where id={0}", objArray1);
                }
            }
            base.ViewData["queryString"]= base.Request.UrlQuery();
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View(dataTable);
        }




        public void FlowSubmitAfter(EventParam eventParam)
        {
            Log.Add("进入了流程提交后事件-" + eventParam.TaskTitle, eventParam.ToString(), LogType.其它, "", "", "", "", "", "", "", "");
        }

        public string FlowSubmitBefor(EventParam eventParam)
        {
            Log.Add("进入了流程提交前事件-" + eventParam.TaskTitle, eventParam.ToString(), LogType.其它, "", "", "", "", "", "", "", "");
            return "1";
        }

        public string GetTableHtml(object param)
        {
            JArray array = (JArray)param;
            StringBuilder builder = new StringBuilder();
            builder.Append("<table width='99%' align='center'><thead><tr>");
            builder.Append("<th>列1</th>");
            builder.Append("<th>列2</th>");
            builder.Append("<th>列3</th>");
            builder.Append("<th>列4</th>");
            builder.Append("<th>列5</th>");
            builder.Append("<th>列6</th>");
            builder.Append("</tr></thead>");
            builder.Append("<tbody><tr>");
            builder.Append("<td></td>");
            builder.Append("<td>行2</td>");
            builder.Append("<td>行3</td>");
            builder.Append("<td>行4</td>");
            builder.Append("<td>行5</td>");
            builder.Append("<td>行6</td>");
            builder.Append("</tr></tbody>");
            builder.Append("</table>");
            return builder.ToString();
        }

        public IActionResult Index()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch.Stop();
        
            return this.View();
        }

        public string ProgramShow(DataRow dr)
        {
            return ("custom-" + dr["f15"].ToString());
        }

        public string SaveCustomForm()
        {
            string str = base.Request.Querys("instanceid");
            string str2 = base.Request.Forms("Title");
            string str3 = base.Request.Forms("Contents");
            string sql = str.IsNullOrWhiteSpace() ? "insert into RF_Test(Id,F1,F2) values({0},{1},{2})" : "update rf_test set f1={0},f2={1} where id={2}";
            Guid guid = Guid.NewGuid();
            using (DataContext context = new DataContext())
            {
                if (str.IsNullOrWhiteSpace())
                {
                    object[] objects = new object[] { guid, str2, str3 };
                    context.Execute(sql, objects);
                }
                else
                {
                    object[] objArray2 = new object[] { str2, str3, str };
                    context.Execute(sql, objArray2);
                }
                context.SaveChanges();
            }
            //格式为：{"success":1,"message":"保存成功","instanceid":"1","title":"任务标题"}
            // success:1表示成功 0表示失败，message:失败时的提示信息 instanceid:保存后的主键值 title:返回待办任务的标题
            JObject obj1 = new JObject();
            obj1.Add("success", 1);
            obj1.Add("message", "保存成功");
            obj1.Add("instanceid", (JToken)guid.ToString());
            obj1.Add("title", (JToken)str2);
            JObject obj2 = obj1;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }

        public bool StepSkip(EventParam eventParam)
        {
            FlowTask other = (FlowTask)eventParam.Other;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventParam"></param>
        /// <returns></returns>
        public string SubFlowActive(EventParam eventParam)
        {
            string str = Guid.NewGuid().ToString();
            string str2 = "子流程测试-" + eventParam.TaskTitle;
            string sql = "insert into RF_Test(Id,f1) values({0},{1})";
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { str, str2 };
                context.Execute(sql, objects);
                context.SaveChanges();
            }
            JObject obj1 = new JObject();
            obj1.Add("instanceId", (JToken)str);
            obj1.Add("title", (JToken)str2);
            JObject obj2 = obj1;
            return obj2.ToString();
        }

        public string SubFlowCompleted(EventParam eventParam)
        {
            Log.Add("执行了子流程完成后事件", eventParam.ToString(), LogType.其它, "", "", "", "", "", "", "", "");
            return "1";
        }

        public IActionResult Test()
        {
            base.ViewData["test"]= SessionExtensions.GetString(base.HttpContext.Session, "test");
            return this.View();
        }

        public string TestOptions()
        {
            return "<option value='1'>test1</option><option value='2'>test2</option><option value='3'>test3</option>";
        }

        public string TestOptions1()
        {
            return "<option value='1'>test1</option><option value='2'>test2</option><option value='3'>test3</option>";
        }





    }




    #endregion

}
