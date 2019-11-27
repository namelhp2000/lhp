using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Mapper;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Controllers
{
    /// <summary>
    /// SQl编辑保存的模板
    /// </summary>
    public class SQLTemplateController : Controller
    {

        #region ---编辑模块--  更新修改需要添加 context.SaveChanges();语句
        // Methods
        public IActionResult Edit()
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
            //获取Option下拉框
            base.ViewData["flowOptions"] = new Flow().GetOptions(new RoadFlow.Model.FlowEntrust().FlowId.ToString());
            //传递到列表的
            base.ViewData["queryString"] = base.Request.UrlQuery();
            base.ViewData["pageSize"] = base.Request.Querys("pagesize");
            base.ViewData["pageNumber"] = base.Request.Querys("pagenumber");


            return this.View(dataTable);
        }


        /// <summary>
        /// 保存方法
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            //Request.Form：获取以POST方式提交的数据。
           // Request.QueryString：获取地址栏参数（以GET方式提交的数据）。
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
            JObject obj1 = new JObject();
            obj1.Add("success", 1);
            obj1.Add("message", "保存成功");
            obj1.Add("instanceid", (JToken)guid.ToString());
            obj1.Add("title", (JToken)str2);
            JObject obj2 = obj1;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }


        /// <summary>
        /// 编辑保存
        /// </summary>
        /// <returns></returns>
        public string Save1()
        {
            string str = base.Request.Querys("entrustid");
            string str2 = base.Request.Forms("s_ic_no");
            string str3 = base.Request.Forms("s_name");
            string str4 = base.Request.Forms("s_glide");
            DataTable table1 = new DataTable();
            DataTable table2 = new DataTable();
            using (DataContext context = new DataContext("sqlserver", "server=localhost;database=his_qzzg;uid=sa;pwd=his_qzzg_ptzy.cn", true))
            {
                object[] objArray1 = new object[] { str4 };
                table1 = context.GetDataTable("select * from  Public_menu_user where  s_glide={0}", objArray1);
                object[] objArray2 = new object[] { str4 };
                table2 = context.GetDataTable("select * from  Public_menu_user_office where  s_user_glide={0}", objArray2);
            }

            // "INSERT INTO Public_menu_user(s_glide, n_soft, s_menu_name, s_item_text, s_item_name, n_enabled, n_visibled, n_handle, n_prior_handle, s_regedit, s_computor) SELECT '9823' , n_soft , s_menu_name , s_item_text , s_item_name , 1 as n_enabled , n_visibled , n_handle , n_prior_handle , '管理员' as s_regedit , '192.168.1.166' as s_computor FROM Public_menu_user WHERE(n_soft = 39) AND(n_visibled = 1) and s_glide ={0}   go  INSERT INTO Public_menu_user_office(s_user_glide, n_soft_serial, s_office, n_kind, s_regedit, s_computor)select s_glide,39,s_office ,0, '管理员' , '192.168.1.166'  from Public_sysuser where s_ic_no = {1}

            string message = "";
            using (DataContext context = new DataContext("sqlserver", "server=localhost;database=his_qzzg;uid=sa;pwd=his_qzzg_ptzy.cn", true))
            {
                if (table1.Rows.Count > 0 && table2.Rows.Count > 0)
                {
                    string sql = "update Public_menu_user_office set Public_menu_user_office.s_office=Public_sysuser.s_office from Public_sysuser where  Public_sysuser.s_glide= Public_menu_user_office.s_user_glide and n_soft_serial = '39' and s_ic_no ={0}";
                    message = "更新";
                    object[] objects = new object[] { str2 };
                    context.Execute(sql, objects);
                }
                else if (table1.Rows.Count == 0 || table2.Rows.Count == 0)
                {
                    if (table1.Rows.Count == 0)
                    {
                        string sql = "INSERT INTO Public_menu_user(s_glide, n_soft, s_menu_name, s_item_text, s_item_name, n_enabled, n_visibled, n_handle, n_prior_handle, s_regedit, s_computor) SELECT {0} , n_soft , s_menu_name , s_item_text , s_item_name , 1 as n_enabled , n_visibled , n_handle , n_prior_handle , '管理员' as s_regedit , '192.168.1.166' as s_computor FROM Public_menu_user WHERE(n_soft = 39) AND(n_visibled = 1) and s_glide = '20150924082734'";

                        message = "新增Public_menu_user";
                        object[] objArray2 = new object[] { str4 };
                        context.Execute(sql, objArray2);
                    }
                    if (table2.Rows.Count == 0)
                    {
                        string sql1 = "INSERT INTO Public_menu_user_office(s_user_glide, n_soft_serial, s_office, n_kind, s_regedit, s_computor)select s_glide,39,s_office ,0, '管理员' , '192.168.1.166'  from Public_sysuser where s_ic_no = {0}";
                        message += "新增Public_menu_user_office";
                        object[] objArray3 = new object[] { str2 };
                        context.Execute(sql1, objArray3);
                    }


                }

                context.SaveChanges();
            }
            JObject obj1 = new JObject();
            obj1.Add("success", 1);
            obj1.Add("message", (JToken)message);
            obj1.Add("s_ic_no", (JToken)str2.ToString());
            obj1.Add("s_name", (JToken)str3);
            JObject obj2 = obj1;
            return obj2.ToString(0, Array.Empty<JsonConverter>());
        }
        #endregion

        #region  Index--模块

        /// <summary>
        /// 传递Url的值可以根据需求进行更改替换
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
            return this.View();
        }
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string Query()
        {
            //**************查询返回总条数***************
            int num3;

            //************排序设置方法*******************

            //---需修改的地方---
            string orderstr = "s_ic_no";   //当排序值为空，按照该值进行排序


            //排序字段抓取
            string str1 = base.Request.Forms("sidx");
            //排序方式抓取
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();

            bool flag = "asc".EqualsIgnoreCase(str2);
            //排序字符串结果
            string order = (str1.IsNullOrEmpty() ? orderstr : str1) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);


            //*************字段抓取从str3开始*****************************
            Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();

            //---需修改的地方---
            //前端获取字段的值，最好方式是字段名称与值一样的名称
            string str4 = base.Request.Querys("instanceid");
            string s_ic_no = base.Request.Forms("s_ic_no");
            string s_name = base.Request.Forms("s_name");

            //---需修改的地方---
            //对应字段放置于字典中

            dics.Add(new ValueTuple<string, string>("s_ic_no", s_ic_no), datatype.stringType);
            dics.Add(new ValueTuple<string, string>("s_name", s_name), datatype.stringType);


            //******************表所有的sql语句*************************
            //数据表名称
            //---需修改的地方---
            string tablename = "select * from   Public_sysuser ";


            string allsql = " SELECT* FROM  " + tablename;

            //**********************数据连接类型与连接字符串***************************
            //---需修改的地方---
            string ConnType = "sqlserver";
            string ConnString = "server = localhost; database = his_qzzg; uid = sa; pwd = his_qzzg_ptzy.cn";


            //**************抓取数据转tableTemplate该方法的转变*******************
            DataTable table = GetPagerTemplate.GetWherePagerList(out num3, pageSize, pageNumber, dics, tablename, order, ConnType, ConnString);


            //*******************************前端处理的对象*******************************
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                //---需修改的地方---
                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();
                obj1.Add("s_ic_no", (JToken)row["s_ic_no"].ToString());
                obj1.Add("s_name", (JToken)row["s_name"].ToString());
                obj1.Add("s_sex", (JToken)row["s_sex"].ToString());
                obj1.Add("d_input_time", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_input_time"].ToString())));
                obj1.Add("s_office", (JToken)row["s_office"].ToString());
                obj1.Add("s_glide", (JToken)row["s_glide"].ToString());
                //Opation的值最好是主键，方便通过该值调用相关数据
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["s_glide"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }

            //***************************返回结果传递给前端******************************
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }


        #region JObject设置的模板

        /*
           字符串添加对象
           obj1.Add("s_ic_no", (JToken) row["s_ic_no"].ToString());
            
           日期添加对象
            obj1.Add("d_input_time", (JToken)DateExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_input_time"].ToString())));

           
           超链接添加对象
             obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["s_glide"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
           
              string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"editButton('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-square-o\"></i>按钮</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));

             string[] textArray1 = new string[] { "<a href=\"javascript:void(0);\" class=\"blue\" onclick=\"showDoc('", row["Id"].ToString(), "');\">", row["Title"].ToString(), "</a>", user.IsRead(StringExtensions.ToGuid(row["Id"].ToString()), userId) ? "" : "<img style=\"border:0;margin-left:4px;vertical-align:middle;\" src=\"/RoadFlowResources/images/loading/new.png\"/>" };
                string str9 = string.Concat((string[])textArray1);


        //序列化的
         JObject obj1 = new JObject();
            obj1.Add("success", 1);
            obj1.Add("message", "保存成功");
            obj1.Add("instanceid", (JToken)guid.ToString());
            obj1.Add("title", (JToken)str2);
            JObject obj2 = obj1;
            return obj2.ToString(0, Array.Empty<JsonConverter>());


        //字符串包含功能
        value.ToLower().Contains(row["Id"].ToString().ToLower()



         
        */
        #endregion






        #endregion


        #region 下载


        //public  void DownLoad(string FileName)
        //{
        //    string filePath = MapPathFile(FileName);
        //    long chunkSize = 204800;             //指定块大小 
        //    byte[] buffer = new byte[chunkSize]; //建立一个200K的缓冲区 
        //    long dataToRead = 0;                 //已读的字节数   
        //    FileStream stream = null;
        //    try
        //    {
        //        //打开文件   
        //        stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        dataToRead = stream.Length;

        //        //添加Http头   
        //        base.Response.ContentType = "application/octet-stream";
        //        base.Response.AddHeader("Content-Disposition", "attachement;filename=" + HttpUtility.UrlEncode(Path.GetFileName(filePath)));
        //        base.Response.AddHeader("Content-Length", dataToRead.ToString());

        //        while (dataToRead > 0)
        //        {
        //            if (HttpContext.Current.Response.IsClientConnected)
        //            {
        //                int length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));
        //                base.Response.OutputStream.Write(buffer, 0, length);
        //                base.Response.Flush();
        //                base.Response.Clear();

        //                dataToRead -= length;
        //            }
        //            else
        //            {
        //                dataToRead = -1; //防止client失去连接 
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        base.Response.Write("Error:" + ex.Message);
        //    }
        //    finally
        //    {
        //        if (stream != null) stream.Close();
        //        base.Response.Close();
        //    }
        //}

        //public  string MapPathFile(string FileName)
        //{
        //    return base.Server.MapPath(FileName);
        //}

        #endregion


    }

}
