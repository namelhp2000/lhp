using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RoadFlow.Mapper
{
    /// <summary>
    /// SQLServer 设置查询Index模板
    /// </summary>
    public static class GetPagerTemplate
    {

        #region 模板查询列表,不能异步查询


        /// <summary>
        /// 针对多表与条件***提供查询条件的获取页码列表模板    查询语句内环不要有where  
        /// </summary>
        /// <param name="count">总数</param>
        /// <param name="size">页码大小</param>
        /// <param name="number">页码值</param>
        /// <param name="queryDics">双字典对应值</param>
        /// <param name="allsqlwhere">查询表全数据</param>
        /// <param name="order">排序</param>
        /// <param name="ConnType">连接类型</param>
        /// <param name="ConnString">连接字符串</param>
        /// <param name="sqltype">where是最外环为0，where在嵌套内部，外环不含where条件为1，where在嵌套内部，外环含where条件为2   默认为0</param>
        /// <returns></returns>
        public static DataTable GetWherePagerList(out int count, int size, int number, Dictionary<ValueTuple<string, string>, datatype> queryDics, string allsqlwhere, string order, string ConnType="", string ConnString="",int sqltype=0)
        {
            if(sqltype==0)  //嵌套内不含where ，外环判断是否含where
            {
                if (allsqlwhere.IndexOf("where", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    allsqlwhere = allsqlwhere + " and 1 = 1";
                }
                else
                {
                    allsqlwhere = allsqlwhere + " where 1 = 1";
                }
            }
            else if(sqltype == 1) //嵌套where，外环不含where
            {
                allsqlwhere = allsqlwhere + "  where 1 = 1";
            }
            else //where在嵌套内部，外环含where条件为
            {
                allsqlwhere = allsqlwhere + " and 1 = 1";
            }
            using (DataContext context = (ConnString.IsNullOrEmpty()) ? new DataContext() : new DataContext(ConnType, ConnString, true))
            {
                ValueTuple<string, DbParameter[]> tuple1 = QueryTypeReturn(queryDics, allsqlwhere, order, ConnType);
                string sql = tuple1.Item1;
                DbParameter[] param = tuple1.Item2;
                string sqlstr = DataInstance(ConnType.IsNullOrEmpty()? "sqlserver": ConnType).GetPaerSql(sql, size, number, out count, ConnType, ConnString, param, order);
                return context.GetDataTable(sqlstr, param);
            }
        }

        #region  查询列表内部操作




        /// <summary>
        /// 查询条件返回对应字符串与参数模板  查询条件模板设置
        /// </summary>
        /// <param name="queryDics">字段类型转换字典</param>
        /// <param name="sql">sql语句</param>
        /// <param name="order">排序字段</param>
        /// <returns></returns>
        public static ValueTuple<string, DbParameter[]> QueryTypeReturn(Dictionary<ValueTuple<string, string>, datatype> queryDics, string sql, string order, string ConnType)
        {
            StringBuilder builder1 = new StringBuilder();
            List<DbParameter> list = new List<DbParameter>();
            foreach (var queryField in queryDics)
            {
                ValueTuple<string, DbParameter> tupleQuery = DataInstance(ConnType.IsNullOrEmpty() ? "sqlserver" : ConnType).GetConditions(queryField.Key.Item1.ToString(), queryField.Key.Item2.ToString(), queryField.Value);
                if (!tupleQuery.Item1.IsNullOrEmpty())
                {
                    builder1.Append(tupleQuery.Item1);
                }
                if (tupleQuery.Item2 != null)
                {
                    list.Add(tupleQuery.Item2);
                }
            }
            return new ValueTuple<string, DbParameter[]>(sql + builder1.ToString(), list.ToArray());
        }







      
        /// <summary>
        /// 连接数据库类型，反对对应数据接口
        /// </summary>
        /// <param name="ConnType"></param>
        /// <returns></returns>
        private static IData DataInstance(string ConnType)
        { 
                switch (ConnType.ToLower())
                {
                    case "sqlserver":
                        return new DataSqlServer();

                    case "mysql":
                        return new DataMySql();

                    case "oracle":
                        return new DataOracle();
                    case "postgresql":
                        return new DataPostgreSql();
                    case "sqlite":
                        return new DataSqlite();

                }
                throw new Exception("不支持的数据库类型");   
        }





        #endregion

        #endregion


        #region 查询实例化模板

        #region 查询设置模板

        //public string Query()
        //{
        //    //**************查询返回总条数***************
        //    int num3;

        //    //************排序设置方法*******************

        //    //---需修改的地方---
        //    string orderstr = "s_ic_no";   //当排序值为空，按照该值进行排序


        //    //排序字段抓取
        //    string str1 = base.Request.Forms("sidx");
        //    //排序方式抓取
        //    string str2 = base.Request.Forms("sord");
        //    int pageSize = Tools.GetPageSize();
        //    int pageNumber = Tools.GetPageNumber();

        //    bool flag = "asc".EqualsIgnoreCase(str2);
        //    //排序字符串结果
        //    string order = (str1.IsNullOrEmpty() ? orderstr : str1) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);


        //    //*************字段抓取从str3开始*****************************
        //    Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();


        //    //---需修改的地方---
        //    //前端获取字段的值，最好方式是字段名称与值一样的名称
        //    string str4 = base.Request.Querys("instanceid");
        //    string s_ic_no = base.Request.Forms("s_ic_no");
        //    string s_name = base.Request.Forms("s_name");

        //    //---需修改的地方---
        //    //对应字段放置于字典中
        //    dics.Add(new ValueTuple<string, string>("s_ic_no", s_ic_no), datatype.stringType);


        //    dics.Add(new ValueTuple<string, string>("s_name", s_name), datatype.stringType);

        //    //******************表所有的sql语句*************************
        //    //数据表名称
        //    //---需修改的地方---
        //    string tablename = "Public_sysuser";




        //    //**********************数据连接类型与连接字符串***************************
        //    //---需修改的地方---
        //    string ConnType = "sqlserver";
        //    string ConnString = "server = localhost; database = his_qzzg; uid = sa; pwd = his_qzzg_ptzy.cn";


        //    //**************抓取数据转tableTemplate该方法的转变*******************
        //    DataTable table = SQLTemplate.GetPagerList(out num3, pageSize, pageNumber, dics, tablename, order, ConnType, ConnString);


        //    //*******************************前端处理的对象*******************************
        //    JArray array = new JArray();
        //    foreach (DataRow row in table.Rows)
        //    {
        //        //---需修改的地方---
        //        //对应的数据添加到对应的对象中
        //        JObject obj1 = new JObject();
        //        obj1.Add("s_ic_no", (JToken)row["s_ic_no"].ToString());
        //        obj1.Add("s_name", (JToken)row["s_name"].ToString());
        //        obj1.Add("s_sex", (JToken)row["s_sex"].ToString());
        //        obj1.Add("d_input_time", (JToken)DateExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_input_time"].ToString())));
        //        obj1.Add("s_office", (JToken)row["s_office"].ToString());
        //        obj1.Add("s_glide", (JToken)row["s_glide"].ToString());
        //        //Opation的值最好是主键，方便通过该值调用相关数据
        //        obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["s_glide"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
        //        JObject obj2 = obj1;
        //        array.Add(obj2);
        //    }

        //    //***************************返回结果传递给前端******************************
        //    object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
        //    return string.Concat((object[])objArray1);
        //}

        #endregion

        #region JObject设置的模板

        /*
           字符串添加对象
           obj1.Add("s_ic_no", (JToken) row["s_ic_no"].ToString());
            
           日期添加对象
            obj1.Add("d_input_time", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_input_time"].ToString())));

           
           超链接添加对象
             obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"add('" + row["s_glide"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a>"));
           
              string[] textArray1 = new string[] { "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-edit (alias)\"></i>编辑</a><a class=\"list\" href=\"javascript:void(0);\" onclick=\"editButton('", row["Id"].ToString(), "');return false;\"><i class=\"fa fa-square-o\"></i>按钮</a>" };
                obj3.Add("Opation", (JToken)string.Concat((string[])textArray1));

             string[] textArray1 = new string[] { "<a href=\"javascript:void(0);\" class=\"blue\" onclick=\"showDoc('", row["Id"].ToString(), "');\">", row["Title"].ToString(), "</a>", user.IsRead(StringExtensions.ToGuid(row["Id"].ToString()), userId) ? "" : "<img style=\"border:0;margin-left:4px;vertical-align:middle;\" src=\"/RoadFlowResources/images/loading/new.png\"/>" };
                string str9 = string.Concat((string[])textArray1);
         
        */
        #endregion

        #endregion

    


    }
}


