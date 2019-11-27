using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{
    [Area("RoadFlowCore")]
    public class DbConnectionController : Controller
    {
        // Methods
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {
            string str = base.Request.Forms("ids");
            DbConnection connection = new DbConnection();
            List<RoadFlow.Model.DbConnection> list = new List<RoadFlow.Model.DbConnection>();
            foreach (string str2 in str.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                Guid guid;
                if (StringExtensions.IsGuid(str2, out guid))
                {
                    RoadFlow.Model.DbConnection connection2 = connection.Get(guid);
                    if (connection2 != null)
                    {
                        list.Add(connection2);
                    }
                }
            }
            connection.Delete(list.ToArray());
            Log.Add("删除了数据连接", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            return "删除成功!";
        }

        [Validate]
        public IActionResult Edit()
        {
            Guid guid;
            string str = base.Request.Querys("connid");
            DbConnection connection = new DbConnection();
            RoadFlow.Model.DbConnection connection2 = null;
            if (StringExtensions.IsGuid(str, out guid))
            {
                connection2 = connection.Get(guid);
            }
            if (connection2 == null)
            {
                RoadFlow.Model.DbConnection connection1 = new RoadFlow.Model.DbConnection();
                connection1.Id=Guid.NewGuid();
                connection1.Sort = connection.GetMaxSort();
                connection2 = connection1;
            }
            base.ViewData["connTypeOptions"]= connection.GetConnTypeOptions(connection2.ConnType);
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(connection2);
        }

        [Validate]
        public string GetFieldsOptions()
        {
            Guid guid;
            string str = base.Request.Querys("connid");
            string str2 = base.Request.Querys("table");
            string str3 = base.Request.Querys("field");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "";
            }
            return new DbConnection().GetTableFieldOptions(guid, str2, str3);
        }

        public string GetTableJSON()
        {
            string str = base.Request.Forms("dbs");
            if (str.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            JArray array = null;
            try
            {
                array = JArray.Parse(str);
            }
            catch
            {
                return "[]";
            }
            if (array == null)
            {
                return "[]";
            }
            DbConnection connection = new DbConnection();
            JArray array2 = new JArray();
            foreach (JObject obj2 in array)
            {
                Guid guid;
                string str3 = obj2.Value<string>("table");
                string str4 = obj2.Value<string>("link");
                int num = 0;
                if (StringExtensions.IsGuid(str4, out guid))
                {
                    RoadFlow.Model.DbConnection connection2 = connection.Get(guid);
                    if (connection2 != null)
                    {
                        JObject obj5 = new JObject();
                        obj5.Add("table", (JToken)str3);
                        obj5.Add("connId", (JToken)str4);
                        obj5.Add("type", (JToken)num);
                        obj5.Add("connName", (JToken)(connection2.Name + "(" + connection2.ConnType + ")"));
                        JObject obj3 = obj5;
                        List<RoadFlow.Model.TableField> tableFields = connection.GetTableFields(guid, str3);
                        JArray array3 = new JArray();
                        foreach (RoadFlow.Model.TableField field in tableFields)
                        {
                            JObject obj6 = new JObject();
                            obj6.Add("name", (JToken)field.FieldName);
                            obj6.Add("comment", (JToken)field.Comment);
                            JObject obj4 = obj6;
                            array3.Add(obj4);
                        }
                        obj3.Add("fields", array3);
                        array2.Add(obj3);
                    }
                }
            }
            return array2.ToString();
        }

        [Validate]
        public string GetTableOptions()
        {
            Guid guid;
            string str = base.Request.Querys("connid");
            string str2 = base.Request.Querys("table");
            if (!StringExtensions.IsGuid(str, out guid))
            {
                return "";
            }
            return new DbConnection().GetTableOptions(guid, str2);
        }

        [Validate]
        public IActionResult Index()
        {
            List<RoadFlow.Model.DbConnection> all = new DbConnection().GetAll();
            JArray array = new JArray();
            foreach (RoadFlow.Model.DbConnection connection in all)
            {
                JObject obj3 = new JObject();
                obj3.Add("id", (JToken)connection.Id);
                obj3.Add("Name", (JToken)connection.Name);
                obj3.Add("ConnType", (JToken)connection.ConnType);
                obj3.Add("ConnString", (JToken)connection.ConnString);
                obj3.Add("Note", (JToken)connection.Note);
                object[] objArray1 = new object[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('", connection.Id, "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"test('", connection.Id, "');return false;\"><i class=\"fa fa-magic\"></i>测试</a><a onclick=\"table('", connection.Id, "','", connection.Name, "');\" style=\"background:url(", base.Url.Content("~/RoadFlowResources/images/ico/find.png"), ") no-repeat left center; padding-left:18px; margin-left:5px;\" href=\"javascript:void(0);\">管理表</a>"  };
                obj3.Add("Opation", (JToken)string.Concat((object[])objArray1));
                JObject obj2 = obj3;
                array.Add(obj2);
            }
            base.ViewData["json"]= array.ToString();
            base.ViewData["tabId"]= base.Request.Querys("tabid");
            base.ViewData["query"]= "appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            return this.View();
        }

        [Validate, ValidateAntiForgeryToken]
        public string SaveEdit(RoadFlow.Model.DbConnection dbConnectionModel)
        {
            Guid guid;
            if (!base.ModelState.IsValid)
            {
                return Tools.GetValidateErrorMessag(base.ModelState);
            }
            DbConnection connection = new DbConnection();
            if (StringExtensions.IsGuid(base.Request.Querys("connid"), out guid))
            {
                RoadFlow.Model.DbConnection connection2 = connection.Get(guid);
                string oldContents = (connection2 == null) ? "" : connection2.ToString();
                connection.Update(dbConnectionModel);
                Log.Add("修改了数据连接-" + dbConnectionModel.Name, "", LogType.系统管理, oldContents, dbConnectionModel.ToString(), "", "", "", "", "", "");
            }
            else
            {
                connection.Add(dbConnectionModel);
                Log.Add("添加了数据连接-" + dbConnectionModel.Name, dbConnectionModel.ToString(), LogType.系统管理, "", "", "", "", "", "", "", "");
            }
            return "保存成功!";
        }

        [Validate, ValidateAntiForgeryToken]
        public string TestConn()
        {
            Guid guid;
            if (!StringExtensions.IsGuid(base.Request.Querys("connid"), out guid))
            {
                return "连接ID错误!";
            }
            string str2 = new DbConnection().TestConnection(guid);
            if (!"1".Equals(str2))
            {
                return str2;
            }
            return "连接成功!";
        }

        [Validate(CheckApp = false)]
        public string TestSql()
        {
            Guid guid;
            string str = base.Request.Forms("connid");
            string str2 = base.Request.Forms("sql");
            string str3 = StringExtensions.IsGuid(str, out guid) ? new DbConnection().TestSQL(guid, str2, null) : "连接ID错误!";
            if (!str3.IsInt())
            {
                return str3;
            }
            return "1";
        }


       
        public IActionResult Table()
        {
            string str = base.Request.Querys("connid");
            List<Tuple<string, int>> list = new List<Tuple<string, int>>();
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            string str2 = string.Empty;
            string type = string.Empty;
            List<string> systemDataTables = RoadFlow.Utility.Config.systemDataTables;
            if (!str.IsGuid())
            {
                base.ViewData["alertMsg"] = "数据连接ID错误";
                return this.View();
            }
            RoadFlow.Model.DbConnection connection2 = connection.Get(str.ToGuid());
            if (connection2 == null)
            {
                base.ViewData["alertMsg"] = "未找到数据连接";
                return this.View();
            }
            type = connection2.ConnType;
            foreach (string str4 in connection.GetTables(connection2.Id, 1))
            {
                list.Add(new Tuple<string, int>(str4, 0));
            }
            foreach (string str5 in connection.GetTables(connection2.Id, 2))
            {
                list.Add(new Tuple<string, int>(str5, 1));
            }
            JArray array = new JArray();
          
            using (List<Tuple<string, int>>.Enumerator enumerator3 = list.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    Predicate<string> match = null;
                    Tuple<string, int> table = enumerator3.Current;
                    if (match == null)
                    {
                        match = p => p.Equals(table.Item1, StringComparison.CurrentCultureIgnoreCase);
                    }
                    bool flag = systemDataTables.Find(match) != null;
                    StringBuilder builder = new StringBuilder("<a class=\"viewlink\" href=\"javascript:void(0);\" onclick=\"queryTable('" + str + "','" + table.Item1 + "');\">查询</a>;");
                    builder.Append("<a class=\"viewlink\" href=\"javascript:void(0);\" onclick=\"edit('" + str + "','" + table.Item1 + "');\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>;");
                    builder.Append("<a class=\"viewlink\" href=\"javascript:void(0);\" onclick=\"del('" + str + "','" + table.Item1 + "');\"><i class=\"fa fa-remove\"></i>删除</a>");
                    JObject obj1 = new JObject();
                    obj1.Add("Name", (JToken)table.Item1);
                    obj1.Add("Type", (JToken)((table.Item2 == 0) ? (flag ? "系统表" : "表") : "视图"));
                    obj1.Add("Opation", (JToken)builder.ToString());
                    JObject obj2 = obj1;
                    array.Add(obj2);
                   
                }
            }
            str2 = "&connid=" + str + "&appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid");
            base.ViewData["Query"] = str2;
            base.ViewData["dbconnID"] = str;
            base.ViewData["DBType"] = type;
            base.ViewData["list"] = array.ToString();
            return this.View();
        }

       
       
        public IActionResult TableQuery()
        {
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Model.DbConnection conn = null;
            string defaultQuerySql = string.Empty;
            str = base.Request.Querys("tablename");
            str2 = base.Request.Querys("dbconnid");
            conn = connection.Get(str2.ToGuid());
            if (conn == null)
            {
                base.ViewData["LiteralResult"] = "未找到数据连接";
                base.ViewData["LiteralResultCount"] = "";
                base.ViewData["sqltext"] = defaultQuerySql;
                base.ViewData["Query"] = base.Request.UrlQuery();
                return base.View();
            }

            //if ( collection != null)
            //{
            //    defaultQuerySql = base.Request.Forms("sqltext");
            //}
            if (!str.IsNullOrEmpty())
            {
                defaultQuerySql = connection.GetDefaultQuerySql(conn, str);
            }
            else
            {
                base.ViewData["LiteralResult"] = "";
                base.ViewData["LiteralResultCount"] = "";
                base.ViewData["sqltext"] = defaultQuerySql;
                base.ViewData["Query"] = base.Request.UrlQuery();
                return base.View();
            }
            if (defaultQuerySql.IsNullOrEmpty())
            {
                base.ViewData["LiteralResult"] = "SQL为空！";
                base.ViewData["LiteralResultCount"] = "";
                base.ViewData["sqltext"] = defaultQuerySql;
                base.ViewData["Query"] = base.Request.UrlQuery();
                return base.View();
            }
            if (!connection.CheckSql(defaultQuerySql))
            {
                base.ViewData["LiteralResult"] = "SQL含有破坏系统表的语句，禁止执行！";
                base.ViewData["LiteralResultCount"] = "";
                base.ViewData["sqltext"] = defaultQuerySql;
                base.ViewData["Query"] = base.Request.UrlQuery();
                Log.Add("尝试执行有破坏系统表的SQL语句", defaultQuerySql, LogType.数据连接, "", "", null);
                return base.View();
            }
            DataTable table = connection.GetDataTable(conn, defaultQuerySql, null);
          Log.Add("执行了SQL", defaultQuerySql, LogType.数据连接, table.ToJsonString(), "", null);
            base.ViewData["LiteralResult"] = RoadFlow.Utility.DataTableExtention.DataTableToHtml(table);
            base.ViewData["LiteralResultCount"] = "(共" + table.Rows.Count + "行)";
            base.ViewData["sqltext"] = defaultQuerySql;
            base.ViewData["Query"] = base.Request.UrlQuery();
            return base.View();
        }


        /// <summary>
        /// 代码生成视图
        /// </summary>
        /// <returns></returns>
        public IActionResult FormGeneration()
        {
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Model.DbConnection conn = null;
            string defaultQuerySql = string.Empty;
            str = base.Request.Querys("tablename");
            str2 = base.Request.Querys("dbconnid");
            conn = connection.Get(str2.ToGuid());
            base.ViewData["tablename"] = str;
            base.ViewData["dbconnid"] = str2;
            return base.View();
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        public string BuildCode()
        {
           
            //设置关联数据Guid
            var  linkId= base.Request.Forms("id");
            //获取选中表名
            var tables = base.Request.Forms("tablename");
            //设置区域名称
            var areaName= base.Request.Forms("areaName");
            //设置勾选生成类型
            string buildType =  base.Request.Forms("CodeGroupID");
            //是否使用外部连接数据
            var dblink = base.Request.Forms("dblink");

            //其他项目生成代码
            var nameSpace = base.Request.Forms("nameSpace");
       //     nameSpace = nameSpace.IsNullOrEmpty() ? "RoadFlow" : nameSpace;

           var message= new RoadFlow.Business.RapidDevelopment().BuildCode(linkId, areaName, tables, buildType, dblink, nameSpace);
            if(!message.IsNullOrEmpty())
            {
                return "未设置主键的表：" + message + ",请设置主键后，再重新生成，其他表生成成功！";
            }
            return "生成成功！";
        }




        public  string   TableQuerySql()
        {
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Model.DbConnection conn = null;
            string defaultQuerySql = string.Empty;
            str = base.Request.Querys("tablename");
            str2 = base.Request.Querys("dbconnid");
           
            conn = connection.Get(str2.ToGuid());
            string LiteralResult = "";
            string LiteralResultCount = "";
            string sqltext = "";
            string Query = "";
            JObject obj1 = new JObject();
           
            if (conn == null)
            {
                LiteralResult = "未找到数据连接";
                LiteralResultCount = "";
                obj1.Add("LiteralResult", LiteralResult);
                obj1.Add("LiteralResultCount", LiteralResultCount);
                JObject obj2 = obj1;
                return obj2.ToString(0, Array.Empty<JsonConverter>());
            }

            if (base.Request.Forms("sqltext") != null)
            {
                defaultQuerySql = base.Request.Forms("sqltext");
            }
            else if (!str.IsNullOrEmpty())
            {
                defaultQuerySql = connection.GetDefaultQuerySql(conn, str);
            }
            else
            {
                LiteralResult = "";
                LiteralResultCount = "";
                obj1.Add("LiteralResult", LiteralResult);
                obj1.Add("LiteralResultCount", LiteralResultCount);
                JObject obj2 = obj1;
                return obj2.ToString(0, Array.Empty<JsonConverter>());
            }
            if (defaultQuerySql.IsNullOrEmpty())
            {
                LiteralResult = "SQL为空！";
                LiteralResultCount = "";
                obj1.Add("LiteralResult", LiteralResult);
                obj1.Add("LiteralResultCount", LiteralResultCount);
                JObject obj2 = obj1;
                return obj2.ToString(0, Array.Empty<JsonConverter>());
            }
            if (!connection.CheckSql(defaultQuerySql))
            {
                LiteralResult = "SQL含有破坏系统表的语句，禁止执行！";
                LiteralResultCount = "";
                Log.Add("尝试执行有破坏系统表的SQL语句", defaultQuerySql, LogType.数据连接, "", "", null);
                obj1.Add("LiteralResult", LiteralResult);
                obj1.Add("LiteralResultCount", LiteralResultCount);
                JObject obj2 = obj1;
                return obj2.ToString(0, Array.Empty<JsonConverter>());
            }
            DataTable table = connection.GetDataTable(conn, defaultQuerySql, null);
            Log.Add("执行了SQL", defaultQuerySql, LogType.数据连接, table.ToJsonString(), "", null);
            LiteralResult = RoadFlow.Utility.DataTableExtention.DataTableToHtml(table);
            LiteralResultCount = "(共" + table.Rows.Count + "行)";
            sqltext = defaultQuerySql;
            Query = base.Request.UrlQuery();
            obj1.Add("LiteralResult", LiteralResult);
            obj1.Add("LiteralResultCount", LiteralResultCount);
            obj1.Add("sqltext", sqltext);
            obj1.Add("Query", Query);
            JObject obj3 = obj1;
            return obj3.ToString(0, Array.Empty<JsonConverter>());
        }




        public IActionResult TableDelete()
        {

            return this.View();
        }


        public ActionResult TableEdit_MySql()
        {
            return (ActionResult)this.TableEdit_MySql(null);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult TableEdit_MySql(IFormCollection collection)
        {

            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            DataTable model = new DataTable();
            IDbConnection conn = null;
            List<string> primaryKey = new List<string>();
            RoadFlow.Model.DbConnection dbconn = null;
            bool flag = false;
            List<string> systemDataTables = Config.systemDataTables;
            str = base.Request.Querys("dbconnid");
            str2 = base.Request.Querys("tablename");
            if (str2.IsNullOrEmpty())
            {
                str2 = "NEWTABLE_" + Tools.GetRandomString(5);
                flag = true;
            }
            if (str.IsGuid() && !str2.IsNullOrEmpty())
            {
                dbconn = connection.Get(str.ToGuid());
                if (dbconn != null)
                {
                    conn = connection.GetConnection(dbconn);
                    if (conn != null)
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        if (!flag)
                        {
                            model = connection.GetTableSchema(conn, str2, dbconn.ConnType);
                            primaryKey = connection.GetPrimaryKey(dbconn, str2);
                        }
                        else
                        {
                            model = connection.GetTableSchema(conn, "Log", dbconn.ConnType);
                            model.Rows.Clear();
                        }
                    }
                }
            }
            if (flag)
            {
                str2 = "";
            }
            if (model.Rows.Count == 0)
            {
                DataRow row = model.NewRow();
                row["f_name"] = "ID";
                row["t_name"] = "int";
                row["is_null"] = 0;
                row["isidentity"] = 1;
                model.Rows.Add(row);
                primaryKey.Add("ID");
            }
            base.ViewData["PrimaryKeyList"] = primaryKey;
            base.ViewData["IsAddTable"] = flag;
            base.ViewData["tableName"] = str2;
            if (collection != null)
            {
                if (dbconn == null)
                {
                    base.ViewData["ClientScript"] = "alert('未找到数据连接!');";
                    return base.View(model);
                }
                string[] strArray = base.Request.Forms("f_name").Split(new char[] { ',' });
                string str4 = base.Request.Form["tablename"];
                string oldtablename = base.Request.Form["oldtablename"];
                string str5 = base.Request.Forms("delfield");
                StringBuilder builder = new StringBuilder();
                string str6 = "temp_" + Guid.NewGuid().ToString("N");
                if (systemDataTables.Find(p => p.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase)) != null)
                {
                    ((dynamic)base.ViewBag).ClientScript = "alert('您不能修改系统表!');";
                    return base.View(model);
                }
                if (flag)
                {
                    builder.Append("CREATE TABLE `" + str4 + "` (`" + str6 + "` varchar(255) PRIMARY KEY NOT NULL);");
                    oldtablename = str4;
                }
                builder.Append("ALTER TABLE `" + oldtablename + "` ");
                if (primaryKey.Count > 0)
                {
                    builder.Append("DROP PRIMARY KEY,");
                }
                foreach (string str7 in str5.Split(new char[] { ',' }))
                {
                    if (!str7.IsNullOrEmpty())
                    {
                        builder.Append("DROP COLUMN `" + str7 + "`,");
                    }
                }
                foreach (string str8 in strArray)
                {
                    string str9 = base.Request.Form[str8 + "_name1"];
                    string str10 = base.Request.Form[str8 + "_type"];
                    string str11 = base.Request.Form[str8 + "_length"];
                    string str12 = base.Request.Form[str8 + "_isnull"];
                    string str13 = base.Request.Form[str8 + "_isidentity"];
                    string str14 = base.Request.Form[str8 + "_primarykey"];
                    string str15 = base.Request.Form[str8 + "_defaultvalue"];
                    string str16 = base.Request.Form[str8 + "_isadd"];
                    if (!str9.IsNullOrEmpty() && !str10.IsNullOrEmpty())
                    {
                        string str17 = string.Empty;
                        switch (str10)
                        {
                            case "varchar":
                                str17 = str10 + "(" + (str11.IsInt() ? ((str11.ToInt() <= -1) ? "255" : str11) : "255") + ")";
                                break;

                            case "char":
                                str17 = str10 + "(" + (str11.IsInt() ? str11 : "255") + ")";
                                break;

                            case "datetime":
                            case "text":
                            case "longtext":
                            case "int":
                            case "float":
                                str17 = str10;
                                break;

                            case "decimal":
                                str17 = str10 + "(" + (str11.IsNullOrEmpty() ? "18,2" : str11) + ")";
                                break;
                        }
                        string str18 = ("1" == str12) ? " NULL" : " NOT NULL";
                        string str19 = ("1" == str13) ? " AUTO_INCREMENT" : "";
                        string str20 = str15.IsNullOrEmpty() ? "" : (" DEFAULT " + str15);
                        bool flag2 = "1" == str16;
                        if (flag2)
                        {
                            builder.Append("ADD COLUMN `" + str9 + "` " + str17 + str19 + str18 + ",");
                        }
                        else if (!str13.IsNullOrEmpty())
                        {
                            builder.Append("MODIFY COLUMN `" + str9 + "` " + str17 + str19 + str18 + str20 + ",");
                        }
                        else if (!flag2 && !str8.Equals(str9, StringComparison.CurrentCultureIgnoreCase))
                        {
                            builder.Append("CHANGE COLUMN `" + str8 + "` `" + str9 + "` " + str17 + str19 + str18 + str20 + ",");
                        }
                        else
                        {
                            builder.Append("MODIFY COLUMN `" + str8 + "` " + str17 + str19 + str18 + str20 + ",");
                        }
                        if ("1" == str14)
                        {
                            builder.Append("ADD PRIMARY KEY (`" + str8 + "`),");
                        }
                    }
                }
                if (!str4.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase))
                {
                    builder.Append("RENAME TABLE `" + oldtablename + "` TO `" + str4 + "`,");
                }
                if (flag)
                {
                    builder.Append("DROP COLUMN `" + str6 + "`,");
                }
                string sql = builder.ToString().TrimEnd(new char[] { ',' }) + ";";
                bool flag3 = connection.TestSql(dbconn, sql, false);
                string str22 = "TableEdit_MySql?dbconnid=" + str + "&tablename=" + str4 + "&connid=" + str + "&appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid") + "&s_Name=" + base.Request.Querys("s_Name");
                if (flag3)
                {
                    RoadFlow.Business.Log.Add("修改表结构成功-" + dbconn.Name + "-" + oldtablename, sql, LogType.数据连接, "", "", null);
                    base.ViewData["ClientScript"] = "alert('保存成功!');window.location='" + str22 + "';";
                    return base.View(model);
                }
                RoadFlow.Business.Log.Add("修改表结构失败-" + dbconn.Name + "-" + oldtablename, sql, LogType.数据连接, "", "", null);
                base.ViewData["ClientScript"] = "alert('保存失败!');window.location='" + str22 + "';";
            }
            base.ViewData["Query"] = base.Request.UrlQuery();
            return base.View(model);
        }

        public ActionResult TableEdit_Oracle()
        {
            return (ActionResult)this.TableEdit_Oracle(null);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult TableEdit_Oracle(IFormCollection collection)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            DataTable model = new DataTable();
            IDbConnection conn = null;
            List<string> primaryKey = new List<string>();
            RoadFlow.Model.DbConnection dbconn = null;
            bool flag = false;
            List<string> systemDataTables = Config.systemDataTables;
            str = base.Request.Querys("dbconnid");
            str2 = base.Request.Querys("tablename");
            if (str2.IsNullOrEmpty())
            {
                str2 = "NEWTABLE_" + Tools.GetRandomString(5);
                flag = true;
            }
            if (str.IsGuid() && !str2.IsNullOrEmpty())
            {
                dbconn = connection.Get(str.ToGuid());
                if (dbconn != null)
                {
                    conn = connection.GetConnection(dbconn);
                    if (conn != null)
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        model = connection.GetTableSchema(conn, str2, dbconn.ConnType);
                        primaryKey = connection.GetPrimaryKey(dbconn, str2);
                    }
                }
            }
            if (flag)
            {
                str2 = "";
            }
            if (model.Rows.Count == 0)
            {
                DataRow row = model.NewRow();
                row["f_name"] = "ID";
                row["t_name"] = "int";
                row["is_null"] = 0;
                row["isidentity"] = 1;
                model.Rows.Add(row);
                primaryKey.Add("ID");
            }
            base.ViewData["PrimaryKeyList"] = primaryKey;
            base.ViewData["IsAddTable"] = flag;
            base.ViewData["tableName"] = str2;
            if (collection != null)
            {
                if (dbconn == null)
                {
                    base.ViewData["ClientScript"] = "alert('未找到数据连接!');";
                    return base.View(model);
                }
                string[] strArray = base.Request.Forms("f_name").Split(new char[] { ',' });
                string str4 = base.Request.Form["tablename"];
                string oldtablename = base.Request.Form["oldtablename"];
                string str5 = base.Request.Forms("delfield");
                new StringBuilder();
                List<string> strList = new List<string>();
                string str6 = "temp_" + Guid.NewGuid().ToString("N");
                if (systemDataTables.Find(p => p.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase)) != null)
                {
                    base.ViewData["ClientScript"] = "alert('您不能修改系统表!');";
                    return base.View(model);
                }
                if (flag)
                {
                    strList.Add("CREATE TABLE \"" + str4 + "\" (\"" + str6 + "\" varchar2(50) NULL)");
                    oldtablename = str4;
                }
                if (primaryKey.Count > 0)
                {
                    strList.Add("ALTER TABLE \"" + oldtablename + "\" DROP PRIMARY KEY");
                }
                StringBuilder builder = new StringBuilder();
                foreach (string str7 in str5.Split(new char[] { ',' }))
                {
                    if (!str7.IsNullOrEmpty())
                    {
                        builder.Append("\"" + str7 + "\",");
                    }
                }
                if (builder.Length > 0)
                {
                    strList.Add("ALTER TABLE \"" + oldtablename + "\" DROP (" + builder.ToString().TrimEnd(new char[] { ',' }) + ")");
                }
                StringBuilder builder2 = new StringBuilder();
                foreach (string str8 in strArray)
                {
                    string str9 = base.Request.Form[str8 + "_name1"];
                    string str10 = base.Request.Form[str8 + "_type"];
                    string str11 = base.Request.Form[str8 + "_length"];
                    string str12 = base.Request.Form[str8 + "_isnull"];
                    string str13 = base.Request.Form[str8 + "_isidentity"];
                    string str14 = base.Request.Form[str8 + "_primarykey"];
                    string str15 = base.Request.Form[str8 + "_defaultvalue"];
                    string str16 = base.Request.Form[str8 + "_isadd"];
                    if (!str9.IsNullOrEmpty() && !str10.IsNullOrEmpty())
                    {
                        string str17 = string.Empty;
                        switch (str10.ToLower())
                        {
                            case "varchar2":
                            case "nvarchar2":
                                str17 = str10 + "(" + (str11.IsInt() ? ((str11.ToInt() == -1) ? "50" : str11) : "50") + ")";
                                break;

                            case "char":
                                str17 = str10 + "(" + (str11.IsInt() ? str11 : "50") + ")";
                                break;

                            case "date":
                            case "clog":
                            case "nclog":
                            case "int":
                            case "float":
                                str17 = str10;
                                break;

                            case "number":
                                str17 = str10 + "(" + (str11.IsNullOrEmpty() ? "18,2" : str11) + ")";
                                break;
                        }
                        int num = (model.Select("F_Name='" + str8 + "'").Length > 0) ? model.Select("F_Name='" + str8 + "'")[0]["IS_NULL"].ToString().ToInt() : -1;
                        string str18 = "";
                        if ("1" == str12)
                        {
                            if (num == 0)
                            {
                                str18 = " NULL";
                            }
                        }
                        else if (num == 1)
                        {
                            str18 = " NOT NULL";
                        }
                        string str19 = ("1" == str13) ? " " : "";
                        string str20 = !str15.IsNullOrEmpty() ? (" DEFAULT " + str15) : "";
                        bool flag2 = "1" == str16;
                        if (flag2)
                        {
                            strList.Add("ALTER TABLE \"" + oldtablename + "\" ADD (\"" + str9 + "\" " + str17 + str19 + str20 + str18 + ")");
                        }
                        else if (!str13.IsNullOrEmpty())
                        {
                            strList.Add("ALTER TABLE \"" + oldtablename + "\" MODIFY (\"" + str9 + "\" " + str17 + str19 + str20 + str18 + ")");
                        }
                        else
                        {
                            strList.Add("ALTER TABLE \"" + oldtablename + "\" MODIFY (\"" + str8 + "\" " + str17 + str19 + str20 + str18 + ")");
                        }
                        if ("1" == str14)
                        {
                            builder2.Append("\"" + str9 + "\",");
                        }
                        if (!flag2 && !str8.Equals(str9, StringComparison.CurrentCultureIgnoreCase))
                        {
                            strList.Add("ALTER TABLE \"" + oldtablename + "\" RENAME COLUMN \"" + str8 + "\" TO \"" + str9 + "\"");
                        }
                    }
                }
                if (builder2.Length > 0)
                {
                    strList.Add("ALTER TABLE \"" + oldtablename + "\" ADD CONSTRAINT \"" + str4 + "_PK\" PRIMARY KEY (" + builder2.ToString().TrimEnd(new char[] { ',' }) + ")");
                }
                if (!str4.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase))
                {
                    strList.Add("ALTER TABLE \"" + oldtablename + "\" RENAME TO \"" + str4 + "\"");
                }
                if (flag)
                {
                    strList.Add("ALTER TABLE \"" + oldtablename + "\" DROP (\"" + str6 + "\")");
                }
                string contents = strList.ToString(";");
                bool flag3 = true;
                foreach (string str22 in strList)
                {
                    if (!connection.TestSql(dbconn, str22, false) && flag3)
                    {
                        flag3 = false;
                    }
                }
                string str23 = "TableEdit_Oracle?dbconnid=" + str + "&tablename=" + str4 + "&connid=" + str + "&appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid") + "&s_Name=" + base.Request.Querys("s_Name");
                if (flag3)
                {
                   RoadFlow.Business.  Log.Add("修改表结构成功-" + dbconn.Name + "-" + oldtablename, contents, LogType.数据连接, "", "", null);
                    base.ViewData["ClientScript"] = "alert('保存成功!');window.location='" + str23 + "';";
                    return base.View(model);
                }
                RoadFlow.Business.Log.Add("修改表结构失败-" + dbconn.Name + "-" + oldtablename, contents, LogType.数据连接, "", "", null);
                base.ViewData["ClientScript"] = "alert('保存失败!');window.location='" + str23 + "';";
            }
            base.ViewData["Query"] = base.Request.UrlQuery();
            return base.View(model);
        }

        public ActionResult TableEdit_SqlServer()
        {
            return (ActionResult)this.TableEdit_SqlServer(null);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult TableEdit_SqlServer(IFormCollection collection)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
            DataTable model = new DataTable();
            IDbConnection conn = null;
            List<string> primaryKey = new List<string>();
            RoadFlow.Model.DbConnection dbconn = null;
            bool flag = false;
            List<string> systemDataTables = Config.systemDataTables;
            str = base.Request.Querys("dbconnid");
            str2 = base.Request.Querys("tablename");
            if (str2.IsNullOrEmpty())
            {
                str2 = "NEWTABLE_" + Tools.GetRandomString(5);
                flag = true;
            }
            if (str.IsGuid() && !str2.IsNullOrEmpty())
            {
                dbconn = connection.Get(str.ToGuid());
                if (dbconn != null)
                {
                    conn = connection.GetConnection(dbconn);
                    if (conn != null)
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        model = connection.GetTableSchema(conn, str2, dbconn.ConnType);
                        primaryKey = connection.GetPrimaryKey(dbconn, str2);
                    }
                }
            }
            if (flag)
            {
                str2 = "";
            }
            if (model.Rows.Count == 0)
            {
                DataRow row = model.NewRow();
                row["f_name"] = "ID";
                row["t_name"] = "int";
                row["is_null"] = 0;
                row["isidentity"] = 1;
                model.Rows.Add(row);
                primaryKey.Add("ID");
            }
            base.ViewData["PrimaryKeyList"] = primaryKey;
            base.ViewData["IsAddTable"] = flag;
            base.ViewData["tableName"] = str2;
            if (collection != null)
            {
                string str23;
                if (dbconn == null)
                {
                    ((dynamic)base.ViewBag).ClientScript = "alert('未找到数据连接!');";
                    return base.View(model);
                }
                string[] source = base.Request.Forms("f_name").Split(new char[] { ',' });
                string str4 = base.Request.Form["tablename"];
                string oldtablename = base.Request.Form["oldtablename"];
                string str5 = base.Request.Forms("delfield");
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                if (systemDataTables.Find(p => p.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase)) != null)
                {
                    base.ViewData["ClientScript"] = "alert('您不能修改系统表!');";
                    return base.View(model);
                }
                if (flag)
                {
                    builder.Append("CREATE TABLE [" + str4 + "] (");
                    oldtablename = str4;
                }
                else
                {
                    foreach (string str6 in connection.GetConstraints(dbconn, oldtablename))
                    {
                        builder.Append("ALTER TABLE [" + oldtablename + "] DROP CONSTRAINT [" + str6 + "];");
                    }
                    StringBuilder builder3 = new StringBuilder();
                    foreach (string str7 in str5.Split(new char[] { ',' }))
                    {
                        if (!str7.IsNullOrEmpty() && (model.Select("f_name='" + str7 + "'").Length > 0))
                        {
                            builder3.Append("[" + str7 + "],");
                        }
                    }
                    if (builder3.Length > 0)
                    {
                        builder.Append("ALTER TABLE [" + oldtablename + "] DROP COLUMN " + builder3.ToString().TrimEnd(new char[] { ',' }) + ";");
                    }
                }
                List<string> list4 = new List<string>();
                foreach (string str8 in source)
                {
                    string str9 = base.Request.Form[str8 + "_name1"];
                    string str10 = base.Request.Form[str8 + "_type"];
                    string str11 = base.Request.Form[str8 + "_length"];
                    string str12 = base.Request.Form[str8 + "_isnull"];
                    string str13 = base.Request.Form[str8 + "_isidentity"];
                    string str14 = base.Request.Form[str8 + "_primarykey"];
                    string str15 = base.Request.Form[str8 + "_defaultvalue"];
                    string str16 = base.Request.Form[str8 + "_isadd"];
                    if (!str9.IsNullOrEmpty() && !str10.IsNullOrEmpty())
                    {
                        string str17 = string.Empty;
                        switch (str10)
                        {
                            case "varchar":
                            case "nvarchar":
                                str17 = str10 + "(" + (str11.IsInt() ? ((str11.ToInt() == -1) ? "MAX" : str11) : "50") + ")";
                                break;

                            case "char":
                                str17 = str10 + "(" + (str11.IsInt() ? str11 : "50") + ")";
                                break;

                            case "datetime":
                            case "text":
                            case "uniqueidentifier":
                            case "int":
                            case "money":
                            case "float":
                                str17 = str10;
                                break;

                            case "decimal":
                                str17 = str10 + "(" + (str11.IsNullOrEmpty() ? "18,2" : str11) + ")";
                                break;
                        }
                        string str18 = ("1" == str12) ? " NULL" : " NOT NULL";
                        string str19 = ("1" == str13) ? " IDENTITY(1,1)" : "";
                        bool flag2 = "1" == str16;
                        if ("1" == str14)
                        {
                            list4.Add(str9);
                        }
                        if (flag)
                        {
                            builder.Append("[" + str9 + "] ");
                            builder.Append(str17);
                            builder.Append(" " + str18);
                            builder.Append(" " + str19);
                            if (!str15.IsNullOrEmpty())
                            {
                                builder.Append(" DEFAULT " + str15);
                            }
                            if (!str8.Equals(source.Last<string>(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                builder.Append(",");
                            }
                        }
                        else
                        {
                            if ((!flag2 && str19.IsNullOrEmpty()) && (model.Select("f_name='" + str8 + "' and isidentity=1").Length > 0))
                            {
                                builder.Append("ALTER TABLE [" + oldtablename + "] DROP COLUMN [" + str8 + "];");
                                builder.Append("ALTER TABLE [" + oldtablename + "] ADD [" + str9 + "] " + str17 + str19 + str18 + ";");
                            }
                            else if (!str19.IsNullOrEmpty() && !flag2)
                            {
                                builder.Append("ALTER TABLE [" + oldtablename + "] DROP COLUMN [" + str8 + "];ALTER TABLE [" + oldtablename + "] ADD [" + str9 + "] int NOT NULL IDENTITY(1,1);");
                            }
                            else
                            {
                                if (flag2)
                                {
                                    builder.Append("ALTER TABLE [" + oldtablename + "] ADD [" + str9 + "] " + str17 + str19 + str18 + ";");
                                }
                                else
                                {
                                    builder.Append("ALTER TABLE [" + oldtablename + "] ALTER COLUMN [" + str8 + "] " + str17 + str19 + str18 + ";");
                                }
                                if (!flag2 && !str8.Equals(str9, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    builder.Append("EXEC sp_rename N'[" + oldtablename + "].[" + str8 + "]', N'" + str9 + "', 'COLUMN';");
                                }
                            }
                            if (!str15.IsNullOrEmpty())
                            {
                                builder.Append("ALTER TABLE [" + oldtablename + "] ADD CONSTRAINT [DF_" + str4 + "_" + str8 + "] DEFAULT (" + str15 + ") FOR [" + str8 + "];");
                            }
                        }
                    }
                }
                if (flag)
                {
                    if (list4.Count > 0)
                    {
                        builder.Append(", PRIMARY KEY (");
                        foreach (string str20 in list4)
                        {
                            builder.Append("[" + str20 + "]");
                            if (!str20.Equals(list4.Last<string>()))
                            {
                                builder.Append(",");
                            }
                        }
                        builder.Append(")");
                    }
                    builder.Append(")");
                }
                else
                {
                    if (list4.Count > 0)
                    {
                        builder2.Append("ALTER TABLE [" + str4 + "] ADD CONSTRAINT [PK_" + str4 + "] PRIMARY KEY (");
                        foreach (string str21 in list4)
                        {
                            builder2.Append("[" + str21 + "]");
                            if (!str21.Equals(list4.Last<string>()))
                            {
                                builder2.Append(",");
                            }
                        }
                        builder2.Append(");");
                    }
                    if (!str4.Equals(oldtablename, StringComparison.CurrentCultureIgnoreCase))
                    {
                        builder.Append("EXEC sp_rename '" + oldtablename + "', '" + str4 + "';");
                    }
                }
                string sql = builder.ToString();
                bool flag3 = connection.TestSql(dbconn, sql, out str23, false);
                if (flag3 && (builder2.Length > 0))
                {
                    flag3 = connection.TestSql(dbconn, builder2.ToString(), out str23, false);
                }
                string str24 = "TableEdit_SqlServer?dbconnid=" + str + "&tablename=" + str4 + "&connid=" + str + "&appid=" + base.Request.Querys("appid") + "&tabid=" + base.Request.Querys("tabid") + "&s_Name=" + base.Request.Querys("s_Name");
                if (flag3)
                {
                   RoadFlow.Business.Log.Add("修改表结构成功-" + dbconn.Name + "-" + oldtablename, sql,LogType.数据连接, "", "", null);
                    base.ViewData["ClientScript"] = "alert('保存成功!');window.location='" + str24 + "';";
                    return base.View(model);
                }
                RoadFlow.Business.Log.Add("修改表结构失败-" + dbconn.Name + "-" + oldtablename, sql, LogType.数据连接, "", "", null);
                base.ViewData["ClientScript"] = "alert('保存失败-" + str23.Replace("'", "") + "!');window.location='" + str24 + "';";
            }
            base.ViewData["Query"] = base.Request.UrlQuery();
            return base.View(model);
        }






    }








}
