using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadFlow.Mapper;
using RoadFlow.Utility;
using RoadFlow.Utility.WindowInfo;
using System;
using System.IO;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Controllers
{




    #region  安装流程





    public class InstallController : Controller
    {


        #region 视图步骤

        /// <summary>
        /// 产品名称
        /// </summary>
        private const string ProductName = "软件开发平台";
        /// <summary>
        /// 产品版本号
        /// </summary>
        private const string ProductVersion = "V1.0.0.0";


        /// <summary>
        /// 视图→阅读许可协议
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {

            //判断是否在线安装过
            if (System.IO.File.Exists(Current.WebRootPath + @"/RoadFlowResources/install/install.lock"))
            {
                base.Response.WriteAsync("安装程序已经运行过了，如果要重新安装，请先删除/RoadFlowResources/install/install.lock");
            }


            //许可协议显示
            string license = System.IO.File.ReadAllText(Current.WebRootPath + @"/RoadFlowResources/install/license.txt", Encoding.ASCII).Replace("\n", "<br/>");
            license = license.Replace("{$Version}", ProductName + " " + ProductVersion);
            license = license.Replace("{$year}", DateTime.Now.Year.ToString());
            license = license.Replace("{$month}", DateTime.Now.Month.ToString());
            license = license.Replace("{$day}", DateTime.Now.Day.ToString());

            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;
            base.ViewData["License"] = license;


            return View();
        }



        /// <summary>
        ///视图→检查安装环境
        /// </summary>
        /// <returns></returns>
        public IActionResult Index1()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;

            //判断ASP.NET的版本号
            string ASPstr = Environment.Version.Major < 4 ? "" : "success";
            base.ViewData["ASPVersion"] = $@"<i class=""Identification {ASPstr} ""></i>ASP.NET {Environment.Version.Major.ToString()}.{Environment.Version.Minor}.{Environment.Version.Build.ToString()}.{Environment.Version.Revision}";

            //判断IIS版本号
            string IISstr = StringExtensions.ToInt(IISWorker.GetIIsVersion()) < 7 ? "" : "success";
            base.ViewData["IIS"] = $@"<i class=""Identification {IISstr}""></i>  IIS {IISWorker.GetIIsVersion()}";

            //判断/wwwroot/RoadFlowResources/install/
            base.ViewData["installFolder"] = FileHelper.CheckFolderCanSave("/wwwroot/RoadFlowResources/install/") ? "success" : "";


            //判断/
            base.ViewData["PathFolder"] = FileHelper.CheckFolderCanSave("/") ? "success" : "";


            ////判断文件/w1.txt
            //base.ViewData["w1File"]=FileHelper.CheckFileCanSave("/w1.txt") ? "success" : "";
            /////apps.json
            //base.ViewData["appjson"] = FileHelper.CheckFileCanSave("/apps.json") ? "success" : "";
            return View();
        }





        /// <summary>
        ///视图→创建数据库
        /// </summary>
        /// <returns></returns>
        public IActionResult Index2()
        {

            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;



            return View();
        }





        /// <summary>
        ///视图→网站基本配置
        /// </summary>
        /// <returns></returns>   
        public IActionResult Index3()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;
            base.ViewData["Host"] = base.Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + base.Request.HttpContext.Connection.LocalPort;


            return View();
        }



        /// <summary>
        ///视图→选择网站风格  未实现该功能
        /// </summary>
        /// <returns></returns>      
        public IActionResult Index4()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;


            return View();
        }


        /// <summary>
        ///视图→安装完成
        /// </summary>
        /// <returns></returns>

        public IActionResult Index5()
        {
            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            base.ViewData["ProductName"] = ProductName;
            base.ViewData["ProductVersion"] = ProductVersion;

            //添加用户管理账户
            string UserName = base.Request.Querys("UserName");
            string UserPass = base.Request.Querys("UserPass").Trim();
            var useId = Guid.NewGuid();
            var organizeId = Guid.NewGuid();
            //加密密码
            var Password = new RoadFlow.Business.User().GetMD5Password(useId, UserPass);
            var pass1 = new RoadFlow.Business.User().GetInitPassword(useId);


            //删除用户
            string userdelete = $@"delete from RF_User where Name='{UserName}'";
            //添加用户
            string userSql = $@"INSERT INTO RF_User  (Id,Name,Account,Password,Sex,Status) VALUES ('{useId}','{UserName}','{UserName}','{Password}',0,0) ";
            //添加组织角色
            string organizeUsersql = $@"INSERT INTO RF_OrganizeUser (Id,OrganizeId,UserId,IsMain,Sort) VALUES ('{organizeId}','9255ddbd-8963-4625-ac25-514afab45372','{useId}',1,5000)";
            //执行语句
            using (DataContext context = new DataContext("SqlServer", Config.SQLServerconfig))
            {
                context.Execute(userdelete + "   " + userSql + "    " + organizeUsersql);
                context.SaveChanges();
            }
            //  FileHelper.WriteTxt("true", Current.WebRootPath + @"/RoadFlowResources/install/install.lock");
            FileHelper.SaveFile(Current.WebRootPath + @"/RoadFlowResources/install/install.lock", "true", null, FileMode.Create);
            return View();
        }




        #endregion







        #region  步骤

        string errMsg = string.Empty;

        string SQLServer = string.Empty;
        /// <summary>
        /// 开始安装数据库
        /// </summary>
        [ValidateAntiForgeryToken]
        public void beginInstallDB()
        {


            string DBService = base.Request.Forms("TxtDBService"); //服务器
            string DBName = base.Request.Forms("TxtDBName");//数据库名
            string DBUser = base.Request.Forms("TxtDBUser");//数据库用户名
            string DBPass = base.Request.Forms("TxtDBPass");//数据库密码

            if (string.IsNullOrEmpty(DBName))
            {
                errMsg = "请输入数据库名称！";
            }
            else if (string.IsNullOrEmpty(DBService))
            {
                errMsg = "请输入服务器名称！";
            }
            else
            {
                string path = Current.WebRootPath + @"/RoadFlowResources/install/DBStructure.sql";
                if (System.IO.File.Exists(path))
                {
                    //数据库master
                    SQLServer = $@"server ={DBService}; database =master; uid = {DBUser}; pwd = {DBPass}";
                    try
                    {
                        //创建DBName数据库
                        using (DataContext context = new DataContext("SqlServer", SQLServer, false))
                        {
                            context.Execute($@"IF Not EXISTS (select name from master.dbo.sysdatabases where name = N'{DBName}')  CREATE DATABASE {DBName} COLLATE Chinese_PRC_CI_AS");
                            context.SaveChanges();
                        }
                    }
                    catch (Exception exception)
                    {
                        errMsg = exception.Message;
                        return;
                    }
                    int length = 0;
                    int num2 = 0;
                    using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                    {
                        char[] separator = new char[] { '\n' };
                        length = reader.ReadToEnd().Split(separator).Length;
                    }
                    string fulldate = string.Empty;
                    fulldate = Current.WebRootPath + @"/RoadFlowResources/install/fulldata.sql";

                    //  string tempdate = "temp.sql";
                    //拆分数据
                    //  this.breakUpData(fulldate, tempdate);
                    //切换成模板的temp数据
                    //   fulldate = Current.WebRootPath + @"/RoadFlowResources/install/" + tempdate;

                    using (StreamReader reader2 = new StreamReader(fulldate, Encoding.UTF8))
                    {
                        char[] chArray2 = new char[] { '\n' };
                        length += reader2.ReadToEnd().Split(chArray2).Length;
                    }
                    //数据库新建表与数据
                    SQLServer = $@"server ={DBService}; database = {DBName}; uid = {DBUser}; pwd = {DBPass}";
                    Config.SQLServerconfig = SQLServer;
                    this.ExceteSql(SQLServer, DBName, path, length, ref num2);
                    this.ExceteSql(SQLServer, DBName, fulldate, length, ref num2);
                    //  await  Task.Run(()=> this.ExceteSql(SQLServer, DBName, fulldate, length, ref num2));
                    //if (System.IO.File.Exists(fulldate))
                    //{
                    //    System.IO.File.Delete(fulldate);
                    //}

                    errMsg = "success";

                }
                else
                {
                    errMsg = "数据库脚本文件不存在！";
                }
            }


        }


        /// <summary>
        /// 大文件拆分数据小文件
        /// </summary>
        /// <param name="sourceSql">数据源路径</param>
        /// <param name="TempSql">模本源路径</param>
        private void breakUpData(string sourceSql, string TempSql)
        {
            using (StreamReader reader = new StreamReader(sourceSql, Encoding.UTF8))
            {
                string str;
                StringBuilder builder = new StringBuilder();
                goto Label_0062;
                Label_0014:
                str = reader.ReadLine();
                if (string.IsNullOrEmpty(str) || (!str.ToUpper().Trim().Equals("GO") && (str.ToLower().Trim().IndexOf("total records") == -1)))
                {
                    builder.AppendLine(str);
                }
                Label_005A:
                if (!reader.EndOfStream)
                {
                    goto Label_0014;
                }
                Label_0062:
                if (!reader.EndOfStream)
                {
                    goto Label_005A;
                }
                string TemSqlPath = Current.WebRootPath + @"/RoadFlowResources/install/" + TempSql;
                //  FileHelper.WriteTxt(builder.ToString(), TemSqlPath);
                FileHelper.SaveFile(TemSqlPath, builder.ToString(), null, null);
            }
        }







        /// <summary>
        /// 执行SQL语句把表与数据添加到服务器
        /// </summary>
        /// <param name="SQLServer">数据库连接</param>
        /// <param name="DBName">数据库名称</param>
        /// <param name="path">文件路径</param>
        /// <param name="datalength">数据大小</param>
        /// <param name="int_1">返回的数据大小 用于 进度条</param>
        private void ExceteSql(string SQLServer, string DBName, string path, int datalength, ref int int_1)
        {
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                StringBuilder builder = new StringBuilder();
                try
                {
                    while (!reader.EndOfStream)
                    {
                        builder.Remove(0, builder.Length);
                        while (!reader.EndOfStream)
                        {
                            string str = reader.ReadLine();
                            if (!string.IsNullOrEmpty(str) && (str.ToUpper().Trim().Equals("GO") || (str.ToLower().Trim().IndexOf("total records") != -1)))
                            {
                                break;
                            }
                            int_1++;
                            builder.AppendLine(str);
                        }
                        if ((int_1 % 50) == 0)
                        {
                            this.progressBar(".progressBar", datalength, int_1);
                        }
                        if (!string.IsNullOrEmpty(builder.ToString()))
                        {
                            //执行语句
                            using (DataContext context = new DataContext("SqlServer", SQLServer))
                            {

                                context.Execute(builder.ToString());
                                context.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    errMsg = exception.Message;
                }
            }
            this.progressBar(".progressBar", datalength, datalength);
        }



        /// <summary>
        /// 进度条
        /// </summary>
        /// <param name="htmlstr">前端html页码</param>
        /// <param name="progressSum">进度总数</param>
        /// <param name="progressValue">进度变化值</param>
        private void progressBar(string htmlstr, int progressSum, int progressValue)
        {
            float num = (((float)progressValue) / ((float)progressSum)) * 100f;
            base.Response.WriteAsync("<script>");
            string[] textArray1 = new string[] { "$(\"", htmlstr, "\").width(\"", num.ToString("f2"), "%\");" };
            base.Response.WriteAsync(string.Concat(textArray1));
            string[] textArray2 = new string[] { "$(\"", htmlstr, "_percent\").css(\"left\",\"", num.ToString("f2"), "%\");" };
            base.Response.WriteAsync(string.Concat(textArray2));
            string[] textArray3 = new string[] { "$(\"", htmlstr, "_percent\").text(\"", num.ToString("f2"), "%\");" };
            base.Response.WriteAsync(string.Concat(textArray3));
            string[] textArray4 = new string[] { "$(\"", htmlstr, "_totalnum\").text(\"", FormatComputySize((double)progressSum), "\");" };
            base.Response.WriteAsync(string.Concat(textArray4));
            string[] textArray5 = new string[] { "$(\"", htmlstr, "_currnum\").text(\"", FormatComputySize((double)progressValue), "\");" };
            base.Response.WriteAsync(string.Concat(textArray5));
            base.Response.WriteAsync("</script>");
            base.Response.Body.Flush();
        }


        /// <summary>
        /// 格式大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string FormatComputySize(double size)
        {
            string[] strArray = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double num = 1024.0;
            int index = 0;
            while (size >= num)
            {
                size /= num;
                index++;
            }
            return (Math.Round(size) + strArray[index]);
        }






        /// <summary>
        /// 回声错误提示
        /// </summary>
        public void EchoErrorTips()
        {
            string s = string.Format(" \r\n                               <div id='errtips' style='display:none'>\r\n                             <div style=\"color:brown;font-weight:bold;font-size:18px;\">出错啦！！！{0}</div>\r\n                                 <div class=\"nextStep pd_25 p-relative t-c w80 mao\"><br/><br/><br/>\r\n                                    <a href=\"javascript:history.back();\" class=\"nextStepBtn c-white ro_3 fz-14\">上一步</a>\r\n                                 </div>\r\n                            </div>", this.errMsg);
            base.Response.WriteAsync(s);
            base.Response.WriteAsync("<script>$('.progressShow').hide();$('.progressShow_error').html($('#errtips').html()).show();</script>");
        }







        #endregion


    }











    #endregion


}
