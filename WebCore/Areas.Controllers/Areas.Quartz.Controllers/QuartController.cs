using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using RoadFlow.Utility;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.Controllers.Areas.Quartz.Controllers
{
    /// <summary>
    /// 项目初始化中，若是作业未开始运作的话，在作业列表中，使用开启任务与立即执行都未能够生效，只有使用修改过后，作业触发器才能正式生效。
    /// </summary>
    [Area("Quartz")]
    public class QuartController : Controller
    {

        private readonly ISchedulerFactory _schedulerFactory;
        public QuartController(ISchedulerFactory schedulerFactory)
        {
            this._schedulerFactory = schedulerFactory;
        }




        

      


        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetJobs()
        {
            return Json(await _schedulerFactory.GetJobs());
        }
        

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(RoadFlow.Model.TaskOptions model)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Json(new { status = false, msg=Tools.GetValidateErrorMessag(base.ModelState) });
            }
            RoadFlow.Business.TaskOptions taskoptionsbusiness = new RoadFlow.Business.TaskOptions();
            if (StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                RoadFlow.Model.TaskOptions taskoptionsmodel = taskoptionsbusiness.Get(guid);
                string oldContents = (taskoptionsmodel == null) ? "" : taskoptionsmodel.ToString();
               // taskoptionsbusiness.Update(model);
                RoadFlow.Business.Log.Add("修改了 -TaskOptions表 ", "", LogType.系统管理, oldContents, model.ToString(), "", "", "", "", "", "");
                return Json(await _schedulerFactory.Update(model));
            }
            else
            {
              
           //     taskoptionsbusiness.Add(model);
                RoadFlow.Business.Log.Add("添加了 -TaskOptions表", model.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
                return Json(await model.AddJob(_schedulerFactory));
            } 
        }



        /// <summary>
        /// 暂停的项目
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Pause()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.TaskOptions> list = new List<RoadFlow.Model.TaskOptions>();
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.TaskOptions> all = new RoadFlow.Business.TaskOptions().GetAll();
            string[] strArray = str.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid strvalue;
                if (strArray[i].IsGuid(out strvalue))
                {
                    RoadFlow.Model.TaskOptions option1 = all.Find(delegate (RoadFlow.Model.TaskOptions p)
                    {
                        return p.Id == strvalue;
                    });
                  
                    await _schedulerFactory.Pause(option1);
                    list.Add(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
                        return p.Id == strvalue;
                    }));

                }
            }
            RoadFlow.Business.Log.Add("批量暂停TaskOptions表库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return Json(new { status = false, msg = "暂停作业成功" });


         //   return Json(await _schedulerFactory.Pause(taskOptions));
        }
        
        public async Task<IActionResult> Start()
        {

            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.TaskOptions> list = new List<RoadFlow.Model.TaskOptions>();
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.TaskOptions> all = new RoadFlow.Business.TaskOptions().GetAll();
            string[] strArray = str.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid strvalue;
                if (strArray[i].IsGuid(out strvalue))
                {
                    RoadFlow.Model.TaskOptions option1 = all.Find(delegate (RoadFlow.Model.TaskOptions p)
                    {
                        return p.Id == strvalue;
                    });
                
                    await _schedulerFactory.Start(option1);
                   
                    list.Add(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
                        return p.Id == strvalue;
                    }));

                }
            }
            RoadFlow.Business.Log.Add("批量启动TaskOptions表库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return Json(new { status = false, msg = "作业开启成功" });




           // return Json(await _schedulerFactory.Start(taskOptions));
        }
        
        public async Task<IActionResult> Run()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.TaskOptions> list = new List<RoadFlow.Model.TaskOptions>();
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.TaskOptions> all = new RoadFlow.Business.TaskOptions().GetAll();
            string[] strArray = str.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid strvalue;
                if (strArray[i].IsGuid(out strvalue))
                {
                    RoadFlow.Model.TaskOptions option1 = all.Find(delegate (RoadFlow.Model.TaskOptions p)
                    {
                        return p.Id == strvalue;
                    });
                  
                    //await _schedulerFactory.Start(option1);
                    await _schedulerFactory.Run(option1);
                    list.Add(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
                        return p.Id == strvalue;
                    }));

                }
            }
            RoadFlow.Business.Log.Add("批量启动TaskOptions表库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return Json(new { status = false, msg = "作业立即执行成功" });





          //  return Json(await _schedulerFactory.Run(taskOptions));
        }


        #region 代码生成控制器

        /// <summary>
        /// 批量删除TaskOptions
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBatch()
        {
            string str = base.Request.Forms("ids");
            List<RoadFlow.Model.TaskOptions> list = new List<RoadFlow.Model.TaskOptions>();
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.TaskOptions> all =new RoadFlow.Business.TaskOptions().GetAll();
            string[] strArray = str.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                Guid strvalue;
                if (strArray[i].IsGuid(out strvalue))
                {
                    await _schedulerFactory.Remove(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
                        return p.Id == strvalue;
                    }));
                    list.Add(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
                        return p.Id == strvalue;
                    }));

                }
            }
            RoadFlow.Business.Log.Add("批量删除了TaskOptions表库", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return Json(new { status = false, msg = "批量删除作业成功" });
        }


        /// <summary>
        /// 删除TaskOptions
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("id"), out guid))
            {
                return    Json(new { status = false, msg = "Id错误" });  
            }
            RoadFlow.Model.TaskOptions taskoptionsmodel = new RoadFlow.Business.TaskOptions().Get(guid);
            RoadFlow.Business.Log.Add("删除了TaskOptions表库", JsonConvert.SerializeObject(taskoptionsmodel), LogType.系统管理, "", "", "", "", "", "", "", "");  
            return Json(await _schedulerFactory.Remove(taskoptionsmodel));
        }



       











        #region Index控制器视图

        /// <summary>
        /// Index视图
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Index()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            return View();
        }



        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string Query()
        {
            //总数大小
            int count;
            //查询条件
            string condition = base.Request.Forms("condition");
            //查询值
            string conditionValue = base.Request.Forms("conditionvalue");
            //排序字段
            string orderField = base.Request.Forms("sidx");
            //排序方式
            string orderWay = base.Request.Forms("sord");
            //页码大小
            int pageSize = Tools.GetPageSize();
            //页码值
            int pageNumber = Tools.GetPageNumber();
            //排序是否是asc方式
            bool flag = "asc".EqualsIgnoreCase(orderWay);
            //排序字符串
            string order = (orderField.IsNullOrEmpty() ? "Id" : orderField) + "  " + (orderWay.IsNullOrEmpty() ? "DESC" : orderWay);

            //抓取数据转table
            DataTable table = new RoadFlow.Business.TaskOptions().GetWherePagerList(out count, pageSize, pageNumber, order, condition, conditionValue);
            JArray array = new JArray();


            RoadFlow.Model.TaskOptions taskoptionsmodel = new RoadFlow.Model.TaskOptions();
            foreach (DataRow row in table.Rows)
            {
                string statustr = row["Status"].ToString() == "0" ? "<span style='color:green'>正常</span>" : "<span style='color:red'>暂停</span>";
                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();
                //根据需求调整显示列表
                obj1.Add("id", (JToken)row["Id"].ToString());

                obj1.Add("Id", (JToken)row["Id"].ToString());
                obj1.Add("TaskName", (JToken)row["TaskName"].ToString());
                obj1.Add("GroupName", (JToken)row["GroupName"].ToString());
                obj1.Add("Interval", (JToken)row["Interval"].ToString());
                obj1.Add("ApiUrl", (JToken)row["ApiUrl"].ToString());
                obj1.Add("AuthKey", (JToken)row["AuthKey"].ToString());
                obj1.Add("AuthValue", (JToken)row["AuthValue"].ToString());
                obj1.Add("Describe", (JToken)row["Describe"].ToString());
                obj1.Add("RequestType", (JToken)row["RequestType"].ToString());
                obj1.Add("LastRunTime", (JToken)(row["LastRunTime"].ToString().IsNullOrWhiteSpace()?"": DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["LastRunTime"].ToString()))));
                obj1.Add("Status", (JToken)statustr);
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>  <a class=\"list\" href=\"javascript:void(0);\" onclick=\"del('" + row["Id"].ToString() + "');return false;\"><i class=\"fa fa-remove (alias)\"></i>删除</a> <a class=\"list\" href=\"javascript:void(0);\" onclick=\"find('" + row["Id"].ToString() + "','"+ row["TaskName"].ToString() + "');return false;\"><i class=\"fa fa fa-search\"></i>查看</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)count, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }


        #endregion


        #region 编辑控制器视图    

        /// <summary>
        /// 编辑视图      **********新增模块设置默认值，需要根据需求进行手工调整***************  
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Edit()
        {
            string isAdd = String.Empty;
            Guid guid;
            string str = base.Request.Querys("id");
            //实体数据
            RoadFlow.Model.TaskOptions TaskOptionsmodel = null;
            //修改
            if (StringExtensions.IsGuid(str, out guid))
            {
                TaskOptionsmodel = new RoadFlow.Business.TaskOptions().Get(guid);
                isAdd = "0";
            }
            //新增
            if (TaskOptionsmodel == null)
            {
                RoadFlow.Model.TaskOptions TaskOptionsmodel1 = new RoadFlow.Model.TaskOptions();
                //新增添加需要根据实际调整 增加主键Guid设置

                TaskOptionsmodel1.Id = Guid.NewGuid();
                TaskOptionsmodel1.Status =0;
                TaskOptionsmodel = TaskOptionsmodel1;
                isAdd = "1";
            }
            base.ViewData["queryString"] = base.Request.UrlQuery();
            base.ViewData["pageSize"] = base.Request.Querys("pagesize");
            base.ViewData["pageNumber"] = base.Request.Querys("pagenumber");
            base.ViewData["isAdd"] = isAdd;
            return this.View(TaskOptionsmodel);
        }














        #endregion



        #region  定时日志

        /// <summary>
        /// Index视图
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult IndexLog()
        {
            //Url查询获取
            string str = base.Request.Querys("id");
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid"),"&id=",str };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            return View();
        }


        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string QueryLog()
        {
            //总数大小
            int count;
            string str = base.Request.Querys("id");
            //排序字段
            string orderField = base.Request.Forms("sidx");
            //排序方式
            string orderWay = base.Request.Forms("sord");
            //页码大小
            int pageSize = Tools.GetPageSize();
            //页码值
            int pageNumber = Tools.GetPageNumber();
            //排序是否是asc方式
            bool flag = "asc".EqualsIgnoreCase(orderWay);
            //排序字符串
            string order = (orderField.IsNullOrEmpty() ? "BeginDate" : orderField) + "  " + (orderWay.IsNullOrEmpty() ? "DESC" : orderWay);
            string sql = $"select * from TaskLog where TaskId='{str}' ";
            //抓取数据转table
            DataTable table = new RoadFlow.Business.TaskLog().GetWherePagerList(out count, pageSize, pageNumber, order, sql,"", "");
            JArray array = new JArray();


            RoadFlow.Model.TaskLog tasklogmodel = new RoadFlow.Model.TaskLog();
            foreach (DataRow row in table.Rows)
            {

                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();
                //根据需求调整显示列表
                obj1.Add("id", (JToken)row["Id"].ToString());

                obj1.Add("Id", (JToken)row["Id"].ToString());
                obj1.Add("TaskId", (JToken)row["TaskId"].ToString());
                obj1.Add("TaskName", (JToken)row["TaskName"].ToString());
                obj1.Add("BeginDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["BeginDate"].ToString())));
                obj1.Add("EndDate", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["EndDate"].ToString())));
                obj1.Add("Msg", (JToken)row["Msg"].ToString());
                 JObject obj2 = obj1;
                array.Add(obj2);
            }
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)count, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }


        #endregion

        #endregion 代码生成器

    }
}
