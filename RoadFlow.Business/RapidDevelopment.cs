using RoadFlow.Data.RapidDevelopment;
using RoadFlow.Mapper;
using RoadFlow.Model.RapidDevelopment;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoadFlow.Business
{
    /// <summary>
    /// 快速代码生成，注意点：数据表需要设置主键。
    /// </summary>
    public class RapidDevelopment
    {

        /// <summary>
        /// 关联数据库连接方式
        /// </summary>
        RoadFlow.Business.DbConnection connection = new RoadFlow.Business.DbConnection();
        /// <summary>
        /// 设置存储位置
        /// </summary>
        static RapidDevelopment()
        {
            _contentRootPath = Config.FilePath + "/RapidDevelopment";
        }
        private static string _contentRootPath { get; }




        private RapidHelper _dbHelper { get; set; }

        private Dictionary<string, DbTableInfo> _dbTableInfoDic { get; set; } = new Dictionary<string, DbTableInfo>();
      

        /// <summary>
        /// 获取数据库所有表
        /// </summary>
        /// <param name="linkId">数据库连接Id</param>
        /// <returns></returns>
        public List<DbTableInfo> GetDbTableList(string linkId)
        {
            if (linkId.IsNullOrEmpty())
                return new List<DbTableInfo>();
            else
                return GetTheDbHelper(linkId).GetDbAllTables();
        }



        /// <summary>
        /// 获取对应的数据库帮助类
        /// </summary>
        /// <param name="linkId">数据库连接Id</param>
        /// <returns></returns>
        private RapidHelper GetTheDbHelper(string linkId)
        {
            var theLink = connection.Get(linkId.ToGuid());
            RapidHelper dbHelper =  FactoryDataType.GetDbHelper(theLink.ConnType, theLink.ConnString);

            return dbHelper;
        }




        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="linkId">连接Id</param>
        /// <param name="areaName">区域名</param>
        /// <param name="tables">表列表</param>
        /// <param name="buildType">需要生成类型</param>
        /// <param name="dblink">是否管理其他数据库</param>
        /// <param name="nameSpace">命名空间</param>
        public string BuildCode(string linkId, string areaName, string tables, string buildType,string dblink,string mainNameSpace)
        {
            string message = String.Empty;
            //内部成员初始化
            _dbHelper = GetTheDbHelper(linkId);
            GetDbTableList(linkId).ForEach(aTable =>
            {
                _dbTableInfoDic.Add(aTable.TableName, aTable);
            });

            char[] separator = new char[] { ',' };

            List<string> tableList = new List<string>(tables.Split(separator));
            List<string> buildTypeList = new List<string>(buildType.Split(separator));
            tableList.ForEach(aTable =>
            {
                var tableFieldInfo = _dbHelper.GetDbTableInfo(aTable).Distinct().ToList();

                //无主键退出当前循环
                if(!(tableFieldInfo.Where(p => p.IsKey).Count()>0))
                {
                    if(message.IsNullOrEmpty())
                    {
                        message = aTable;
                    }
                    else
                    {
                        message +=","+ aTable;
                    }
                    return;
                }
              
                //实体层
                if (buildTypeList.Exists(x => x.ToLower() == "entity"))
                {
                    BuildEntity(tableFieldInfo, areaName, aTable, mainNameSpace);
                }
                //数据层
                if (buildTypeList.Exists(x => x.ToLower() == "data"))
                {
                    BuildData(tableFieldInfo, areaName, aTable, linkId, dblink, mainNameSpace);
                }

                //业务层
                if (buildTypeList.Exists(x => x.ToLower() == "business"))
                {
                    BuildBusiness(tableFieldInfo, areaName, aTable, linkId, dblink, mainNameSpace);
                }
                //控制器
                if (buildTypeList.Exists(x => x.ToLower() == "controller"))
                {
                    BuildController(tableFieldInfo, areaName, aTable, mainNameSpace);
                }
                //视图
                if (buildTypeList.Exists(x => x.ToLower() == "view"))
                {
                    BuildView(tableFieldInfo, areaName, aTable, mainNameSpace);
                }
            });
            return message;
        }




        #region 代码生成模块

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="tableInfo">表字段信息</param>
        /// <param name="areaName">区域名</param>
        /// <param name="tableName">表名</param>
        private void BuildEntity(List<TableInfo> tableInfo, string areaName, string tableName,string mainNameSpace)
        {
            if(mainNameSpace.IsNullOrEmpty())
            {
                mainNameSpace = "RoadFlow";
            }
            //设置实体路径
            string entityPath = _contentRootPath.Replace("Coldairarrow.Web", "Coldairarrow.Entity");

            string filePath = Path.Combine(entityPath, $"{mainNameSpace}.Model", areaName, $"{tableName}.cs");
            string nameSpace = areaName.IsNullOrEmpty() ? $"{mainNameSpace}.Model" : $@"{mainNameSpace}.Model.{areaName}";

            _dbHelper.SaveEntityToFile(tableInfo, tableName, _dbTableInfoDic[tableName].Description, filePath, nameSpace);
        }

        /// <summary>
        /// 生成数据
        /// </summary>
        /// <param name="tableInfo">表字段信息</param>
        /// <param name="areaName">区域名</param>
        /// <param name="entityName">表名</param>
        private void BuildData(List<TableInfo> tableInfo, string areaName, string entityName,string linkId,string dblink,string mainNameSpace)
        {
            if (mainNameSpace.IsNullOrEmpty())
            {
                mainNameSpace = "RoadFlow";
            }

            RoadFlow.Model.DbConnection dbConne = connection.Get(linkId.ToGuid());

            //设置是否关联数据库
            string datacontext = dblink == "1"?$@"DataContext context = new DataContext(""{dbConne.ConnType}"",""{dbConne.ConnString}"")":$@"DataContext context = new DataContext()";

            //自增列
            var Identitys = tableInfo.Where(p => p.IsIdentity);
            bool HasIdentity = Identitys.Count() > 0;
            //主键
            var Primarykeys = tableInfo.Where(p => p.IsKey);
            bool HasPrimarykey = Primarykeys.Count() > 0;
            //Data命名空间
            string nameSpace = areaName.IsNullOrEmpty() ? $"{mainNameSpace}.Data" : $@"{mainNameSpace}.Data.{areaName}";
            //Model命名实体类
            string ModelSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Model.{entityName}" : $@"{mainNameSpace}.Model.{areaName}.{entityName}";
            //新增添加对应的返回
            string ReturnsAdd = HasIdentity ? "新增记录的ID" : "操作所影响的行数";
            string HasIdentityMethod = HasIdentity ? "int AddInt" : "int  Add";

            //自增选择对应添加方法
            string HasIdentityMethod1 = HasIdentity ? "AddInt" : "Add";

            string code =
$@"using System;
using System.Collections.Generic;
using System.Text;
using {mainNameSpace}.Utility.Cache;
using {mainNameSpace}.Mapper;
using System.Data.Common;
using {mainNameSpace}.Utility;
using System.Linq;

namespace {nameSpace}
{{
    public class {entityName}
    {{
        #region 数据代码生成区域


        /// <summary>
		/// 设置缓存字段名称
		/// </summary>
		private const string CACHEKEY = ""roadflow_cache_{entityName.ToLower()}"";

        /// <summary>
		/// 清除缓存
		/// </summary>
		public  void ClearCache()
		{{
            IO.Remove(""roadflow_cache_{entityName.ToLower()}"");
        }}


        /// <summary>
		/// 添加记录
		/// </summary>
		/// <param name=""model"">{ModelSpace}实体类</param>
		/// <returns>{ReturnsAdd}</returns>
		public {HasIdentityMethod}({ModelSpace} model)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                context.{HasIdentityMethod1}<{ModelSpace}>(model);
                return context.SaveChanges();
            }}
        }}


        /// <summary>
		/// 删除记录
		/// </summary>
		/// <param name=""model"">{ModelSpace}实体类</param>
		/// <returns></returns>
		public int Delete({ModelSpace} model)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                context.Remove<{ModelSpace}>(model);
                return context.SaveChanges();
            }}
        }}



		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name=""models"">{ModelSpace}列表实体类</param>
		/// <returns></returns>
		public int Delete({ModelSpace}[] models)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                context.RemoveRange<{ModelSpace}>(models);
                return context.SaveChanges();
            }}
        }}


		/// <summary>
		/// 查询所有记录
		/// </summary>
		public List<{ModelSpace}> GetAll()
		{{
            object obj2 = IO.Get(""roadflow_cache_{entityName.ToLower()}"");
			if (obj2 == null)
			{{
				using ({datacontext})
				{{
					List<{ModelSpace}> list = context.QueryAll<{ModelSpace}>(); 
					IO.Insert(""roadflow_cache_{entityName.ToLower()}"", list);
					return list;
				}}
			}}
			return (List<{ModelSpace}>)obj2;
		}}



		/// <summary>
		/// 更新
		/// </summary>
		/// <param name=""model"">{ModelSpace}实体类</param>
		/// <returns></returns>
		public int Update({ModelSpace} model)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                context.Update<{ModelSpace}>(model);
                return context.SaveChanges();
            }}
        }}


	    /// <summary>
		/// 批量更新数据
		/// </summary>
		/// <param name=""models"">{ModelSpace}列表实体类</param>
		/// <returns></returns>
		public int Update({ModelSpace}[] models)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                context.UpdateRange<{ModelSpace}>(models);
                return context.SaveChanges();
            }}
        }}



		/// <summary>
		///多行操作：根据传递参数进行对应操作处理 
		/// </summary>
		/// <param name=""tuples"">通过传递对应的值，进行对应的操作 2代表新增 1代表更新 0代表删除</param>
		/// <returns></returns>
		public int TypeOperation(List<Tuple<{ModelSpace}, int>> tuples)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                foreach (Tuple<{ModelSpace}, int> tuple in tuples)
                {{
                    if (tuple.Item2 == 0)//删除
                    {{
                        context.Remove<{ModelSpace}>(tuple.Item1);
                    }}
                    else if (tuple.Item2 == 1)//更新
                    {{
                        context.Update<{ModelSpace}>(tuple.Item1);
                    }}
                    else if (tuple.Item2 == 2)//新增
                    {{
                        context.Add<{ModelSpace}>(tuple.Item1);
                    }}
                }}
                return context.SaveChanges();
            }}
        }}




		/// <summary>
		///单行操作：根据传递参数进行对应操作处理 
		/// </summary>
		/// <param name=""tuple"">通过传递对应的值，进行对应的操作 2代表新增 1代表更新 0代表删除</param>
		/// <returns></returns>
		public int TypeOperation(Tuple<{ModelSpace}, int> tuple)
        {{
            this.ClearCache();
            using ({datacontext})
            {{
                if (tuple.Item2 == 0)//删除
                {{
                    context.Remove<{ModelSpace}>(tuple.Item1);
                }}
                else if (tuple.Item2 == 1)//更新
                {{
                    context.Update<{ModelSpace}>(tuple.Item1);
                }}
                else if (tuple.Item2 == 2)//新增
                {{
                    context.Add<{ModelSpace}>(tuple.Item1);
                }}
                return context.SaveChanges();
            }}
        }}

        #endregion

       

            
    }}
}}";
            string businessPath = _contentRootPath.Replace("Coldairarrow.Web", "Coldairarrow.Business");
            string filePath = Path.Combine(businessPath, $"{mainNameSpace}.Data", areaName, $"{entityName}.cs");

            FileHelper.WriteTxt(code, filePath, FileMode.Create);
        }

        /// <summary>
        /// 生成业务逻辑代码
        /// </summary>
        /// <param name="areaName">区域名</param>
        /// <param name="entityName">实体名</param>
        private void BuildBusiness(List<TableInfo> tableInfo, string areaName, string entityName, string linkId, string dblink,string mainNameSpace)
        {
            if (mainNameSpace.IsNullOrEmpty())
            {
                mainNameSpace = "RoadFlow";
            }

            RoadFlow.Model.DbConnection dbConne = connection.Get(linkId.ToGuid());

            //设置是否关联数据库
            string datacontext = dblink == "1" ? $@",""{dbConne.ConnType}"",""{dbConne.ConnString}""" : " ";

            //自增列
            var Identitys = tableInfo.Where(p => p.IsIdentity);
            bool HasIdentity = Identitys.Count() > 0;
            //主键
            var Primarykeys = tableInfo.Where(p => p.IsKey);
            bool HasPrimarykey = Primarykeys.Count() > 0;

            //Business命名空间
            string nameSpace = areaName.IsNullOrEmpty() ? $"{mainNameSpace}.Business" : $@"{mainNameSpace}.Business.{areaName}";

            //Data命名实体空间
            string DataSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Data.{entityName}" : $@"{mainNameSpace}.Data.{areaName}.{entityName}";

            //Model命名实体类
            string ModelSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Model.{entityName}" : $@"{mainNameSpace}.Model.{areaName}.{entityName}";
            //新增添加对应的返回
            string ReturnsAdd = HasIdentity ? "新增记录的ID" : "操作所影响的行数";
            string HasIdentityMethod = HasIdentity ? "AddInt" :"Add";
            string HasIdentityMethodType = HasIdentity ? "int" : _dbHelper.DbTypeStr_To_CsharpType(Primarykeys.First().Type).Name;
            string dicsField = "";
            foreach (var field in tableInfo)
            {
                dicsField += $@"
                dics.Add(new ValueTuple<string, string>(""{field.Name}"",{field.Name.ToLower()}), datatype.stringType);";
            }
            //字符串通过id批量删除
            string deleteIsString = HasIdentityMethodType== "String" ? $@"
        /// <summary>
		/// 通过字符串id批量删除
		/// </summary>
		public List<{ModelSpace}> DeleteIdString(string idString)
		{{
            List <{ModelSpace}> list = new List<{ModelSpace}>();
			char[] separator = new char[] {{ ',' }};
			List<{ModelSpace}> all = this.GetAll();
			string[] strArray = idString.Split(separator);
			 for (int i = 0; i < strArray.Length; i++)
			 {{
					list.Add(all.Find(delegate ({ModelSpace} p) {{
						return p.{Primarykeys.Last().Name}== strArray[i];
					}}));	
			 }}
			data{entityName}.Delete(list.ToArray());
			return list;
		}}

" : $@"
        /// <summary>
		/// 通过字符串id批量删除
		/// </summary>
		public List<{ModelSpace}> Delete(string idString)
		{{
            List <{ModelSpace}> list = new List<{ModelSpace}>();
			char[] separator = new char[] {{ ',' }};
			List<{ModelSpace}> all = this.GetAll();
			string[] strArray = idString.Split(separator);
			 for (int i = 0; i < strArray.Length; i++)
			 {{
				{HasIdentityMethodType} strvalue ;
				if (strArray[i].Is{HasIdentityMethodType.ToFirstUpperStr()}(out strvalue))
				{{
					list.Add(all.Find(delegate ({ModelSpace} p) {{
						return p.{Primarykeys.Last().Name}== strvalue;
					}}));
				}}
			 }}
			data{entityName}.Delete(list.ToArray());
			return list;
		}}

";

            string code =
$@"using System;
using System.Collections.Generic;
using System.Text;
using {mainNameSpace}.Utility;
using System.Data;
using System.Linq;

namespace {nameSpace}
{{
    public class {entityName}
    {{
        #region 业务逻辑代码生成区域


        /// <summary>
		/// 调用{entityName}表Data类
		/// </summary>
		private readonly {DataSpace} data{entityName} = new {DataSpace}();


        /// <summary>
		/// {ReturnsAdd}
		/// </summary>
		public int  {HasIdentityMethod}({ModelSpace} model)
		{{
			return data{entityName}.{HasIdentityMethod}(model);
		}}



		/// <summary>
		/// 删除
		/// </summary>
		public int Delete({HasIdentityMethodType} id)
		{{
            {ModelSpace} getdata = this.Get(id);
			if (getdata != null)
			{{
				return data{entityName}.Delete(getdata);
			}}
			return 0; 
		}}


		/// <summary>
		/// 实体类删除
		/// </summary>
		/// <param name=""model"">{entityName}表实体删除</param>
		/// <returns></returns>
		public int Delete({ModelSpace} model)
        {{
            return data{entityName}.Delete(model);
        }}


        /// <summary>
		/// 实体类批量删除
		/// </summary>
		/// <param name=""models"">{entityName}列表实体删除</param>
		/// <returns></returns>
		public int Delete({ModelSpace}[] models)
        {{
            return data{entityName}.Delete(models);
        }}



		{deleteIsString}



		/// <summary>
		/// 通过主键获取对应的数据
		/// </summary>
		public {ModelSpace} Get({HasIdentityMethodType} strvalue )
		{{
			return this.GetAll().Find(delegate ({ModelSpace} p)
			{{
				return p.{Primarykeys.First().Name} == strvalue;
			}});
		}}



        /// <summary>
		/// 查询所有的数据  
		/// </summary>
		public List<{ModelSpace}> GetAll()
		{{
			return data{entityName}.GetAll();
		}}



        /// <summary>
		/// 更新
		/// </summary>
		public int Update({ModelSpace} model)
		{{
			return data{entityName}.Update(model);
		}}



		/// <summary>
		/// 批量更新
		/// </summary>
		public int Update({ModelSpace}[] models)
		{{
			return data{entityName}.Update(models);
		}}



        /// <summary>
		/// 通过传参方式进行综合操作
		/// </summary>
		public int TypeOperation(List<Tuple<{ModelSpace} , int>> tuples)
		{{
			return data{entityName}.TypeOperation(tuples);
		}}

		/// <summary>
		/// 通过传参方式进行综合操作
		/// </summary>
		public int TypeOperation(Tuple<{ModelSpace} , int> tuple)
		{{
			return data{entityName}.TypeOperation(tuple);
		}}



        #region 页码列表查询方法，需手工进行调整处理 
		/// <summary>
		/// 页码列表查询方法 
		/// </summary>
		/// <param name=""count"">总数大小</param>
        /// <param name=""size"">页码大小</param>
        /// <param name=""number"">页码值</param>
        /// <param name=""order"">排序</param>
        /// <param name=""otherparam"">后面参数，通过需求，进行字典添加处理</param>
        /// <returns></returns>
        public DataTable GetWherePagerList(out int count, int size, int number, string order, string condition,string conditionvalue)
        {{
            Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();
            #region 把对应的字段循环出来，匹配对应的值，该方式需手工调整处理
            if(!condition.IsNullOrEmpty())
            {{
                 dics.Add(new ValueTuple<string, string>(condition,conditionvalue), datatype.stringType);
            }}
            /*
            {dicsField}
            */
            #endregion
            return Mapper.GetPagerTemplate.GetWherePagerList(out count, size, number, dics, ""  select * from  {entityName} "", order {datacontext});
        }}
        #endregion




        /// <summary>
		/// 获取表的条数+1 
		/// </summary>
		public int GetMax()
		{{
            List <{ModelSpace}> all = GetAll();
			if (all.Count == 0)
			{{
				return 1;
			}}
			return all.Count+1 ;
		}}




        #endregion


       
    }}
}}";
            string businessPath = _contentRootPath.Replace("Coldairarrow.Web", "Coldairarrow.Business");
            string filePath = Path.Combine(businessPath, $"{mainNameSpace}.Business", areaName, $"{entityName}.cs");

            FileHelper.WriteTxt(code, filePath, FileMode.Create);
        }




        /// <summary>
        /// 生成控制器代码
        /// </summary>
        /// <param name="areaName">区域名</param>
        /// <param name="entityName">实体名</param>
        private void BuildController(List<TableInfo> tableInfo, string areaName, string entityName,string mainNameSpace)
        {
            if (mainNameSpace.IsNullOrEmpty())
            {
                mainNameSpace = "RoadFlow";
            }
            //自增列
            var Identitys = tableInfo.Where(p => p.IsIdentity);
            bool HasIdentity = Identitys.Count() > 0;
            //主键
            var Primarykeys = tableInfo.Where(p => p.IsKey);
            bool HasPrimarykey = Primarykeys.Count() > 0;

            //是否为空
            var IsNullable = tableInfo.Where(p => p.IsNullable==false);
            bool HasIsNullable = IsNullable.Count() > 0;

            //控制器命名空间
            string AreaSpace = areaName.IsNullOrEmpty() ? "RapidDevelopment" : areaName;

            //Business命名空间
            string BusinessSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Business.{entityName}" : $@"{mainNameSpace}.Business.{areaName}.{entityName}";

            //业务命名空间
            string Business = $@"{mainNameSpace}.Business.";    //areaName.IsNullOrEmpty() ? $@"RoadFlow.Business." : $@"RoadFlow.Business.";

            //自增选择对应添加方法
            string HasIdentityMethod = HasIdentity ? "AddInt" : "Add";


            //Model命名实体类
            string ModelSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Model.{entityName}" : $@"{mainNameSpace}.Model.{areaName}.{entityName}";

            //自增或者主键类型
            string HasIdentityMethodType = HasIdentity ? "int" : _dbHelper.DbTypeStr_To_CsharpType(Primarykeys.First().Type).Name;

            //新增时，需要设置默认值的调整
            var addField = "";
           
            foreach (var field in Primarykeys)
            {
                if (_dbHelper.DbTypeStr_To_CsharpType(field.Type).Name.Contains("Guid"))
                {
                    addField += $@"
                {entityName}model1.{field.Name} = Guid.NewGuid();";
                }
                //else if (_dbHelper.DbTypeStr_To_CsharpType(field.Type).Name == "DateTime")
                //{
                //    addField += $@"
                //{entityName}model1.{field.Name} = DateExtensions.Now;";
                //}
                //else
                //{
                //    addField += $@"
                //{entityName}model1.{field.Name} = """";";
                //}
            }


            //查询列表显示
            var searchField = "";
            foreach(var field in tableInfo)
            {
                if(_dbHelper.DbTypeStr_To_CsharpType(field.Type).Name == "DateTime")
                {
                    searchField += $@" 
                obj1.Add(""{field.Name}"", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row[""{field.Name}""].ToString())));";
                }
                else
                {
                    searchField += $@" 
                obj1.Add(""{field.Name}"", (JToken)row[""{field.Name}""].ToString());";
                }
            }

            //前端设置id复选框使用
            string idKeyObject = Primarykeys.Last().Name != "id"? $@"obj1.Add(""id"", (JToken)row[""{ Primarykeys.Last().Name}""].ToString());" : "";
           




            string code =
$@"using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using {mainNameSpace}.Utility;
using System.Data;

namespace WebCore.Areas.{AreaSpace}.Controllers
{{
    [Area(""{AreaSpace}"")]
    public class {entityName}Controller : Controller
    {{
        #region 代码生成控制器

        /// <summary>
        /// 批量删除{entityName}
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string DeleteBatch()
        {{
            string str = base.Request.Forms(""ids"");
            List<{ModelSpace}> list = new {BusinessSpace}().Delete(str);
            {Business}Log.Add(""批量删除了{entityName}表库"", JsonConvert.SerializeObject(list), LogType.系统管理, """", """", """", """", """", """", """", """");
            return ""批量删除成功!"";
        }}


        /// <summary>
        /// 删除{entityName}
        /// </summary>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string Delete()
        {{
            {HasIdentityMethodType} guid;
            if (!StringExtensions.Is{HasIdentityMethodType.ToFirstUpperStr()}(base.Request.Querys(""id""), out guid))
            {{
                return ""{Primarykeys.Last().Name}错误"";
            }}
            {ModelSpace} {entityName.ToLower()}model = new {BusinessSpace}().Get(guid);
            new {BusinessSpace}().Delete(guid);
            {Business}Log.Add(""删除了{entityName}表库"", JsonConvert.SerializeObject({entityName.ToLower()}model), LogType.系统管理, """", """", """", """", """", """", """", """");
            return ""删除成功"";
        }}



        /// <summary>
        /// 保存
        /// </summary>
        /// <param name=""model""></param>
        /// <returns></returns>
        [Validate, ValidateAntiForgeryToken]
        public string Save({ModelSpace} model)
        {{
            {HasIdentityMethodType} guid;
            if (!base.ModelState.IsValid)
            {{
                return Tools.GetValidateErrorMessag(base.ModelState);
            }}
            {BusinessSpace} {entityName.ToLower()}business = new {BusinessSpace}();
            if (StringExtensions.Is{HasIdentityMethodType.ToFirstUpperStr()}(base.Request.Querys(""id""), out guid))
            {{
                {ModelSpace} {entityName.ToLower()}model = {entityName.ToLower()}business.Get(guid);
                string oldContents = ({entityName.ToLower()}model == null) ? """" : {entityName.ToLower()}model.ToString();
                {entityName.ToLower()}business.Update(model);
                {Business}Log.Add(""修改了 -{entityName}表 "", """", LogType.系统管理, oldContents, model.ToString(), """", """", """", """", """", """");
            }}
            else
            {{
                {entityName.ToLower()}business.{HasIdentityMethod}(model);
                {Business}Log.Add(""添加了 -{entityName}表"", model.ToString(), LogType.系统管理, """", """", """", """", """", """", """", """");
            }}
            return ""保存成功!"";
        }}











        #region Index控制器视图

        /// <summary>
        /// Index视图
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Index()
        {{
            //Url查询获取
            base.ViewData[""appId""] = base.Request.Querys(""appid"");
            base.ViewData[""tabId""] = base.Request.Querys(""tabid"");
            string[] textArray1 = new string[] {{ ""appid="", base.Request.Querys(""appid""), ""&tabid="", base.Request.Querys(""tabid"") }};
            base.ViewData[""query""] = string.Concat((string[])textArray1);
            return View();
        }}



        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string Query()
        {{
            //总数大小
            int count;
            //查询条件
            string condition = base.Request.Forms(""condition"");
            //查询值
            string conditionValue = base.Request.Forms(""conditionvalue"");
            //排序字段
            string orderField = base.Request.Forms(""sidx"");
            //排序方式
            string orderWay = base.Request.Forms(""sord"");
            //页码大小
            int pageSize = Tools.GetPageSize();
            //页码值
            int pageNumber = Tools.GetPageNumber();
            //排序是否是asc方式
            bool flag = ""asc"".EqualsIgnoreCase(orderWay);
            //排序字符串
            string order = (orderField.IsNullOrEmpty() ? ""{Primarykeys.Last().Name}"" : orderField) + ""  "" + (orderWay.IsNullOrEmpty() ? ""DESC"" : orderWay);
           
            //抓取数据转table
            DataTable table = new {BusinessSpace}().GetWherePagerList(out count, pageSize, pageNumber, order, condition, conditionValue);
            JArray array = new JArray();


            {ModelSpace} {entityName.ToLower()}model = new {ModelSpace}();
            foreach (DataRow row in table.Rows)
            {{
                
                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();
                //根据需求调整显示列表
                {idKeyObject}
                {searchField}
                obj1.Add(""Opation"", (JToken)(""<a class=\""list\"" href=\""javascript:void(0);\"" onclick=\""add('"" + row[""{Primarykeys.Last().Name}""].ToString() + ""');return false;\""><i class=\""fa fa-edit (alias)\""></i>编辑</a>  <a class=\""list\"" href=\""javascript:void(0);\"" onclick=\""del('"" + row[""{Primarykeys.Last().Name}""].ToString() + ""');return false;\""><i class=\""fa fa-remove (alias)\""></i>删除</a>""));
                JObject obj2 = obj1;
                array.Add(obj2);
            }}
            object[] objArray1 = new object[] {{ ""{{\""userdata\"":{{\""total\"":"", (int)count, "",\""pagesize\"":"", (int)pageSize, "",\""pagenumber\"":"", (int)pageNumber, ""}},\""rows\"":"", array.ToString(), ""}}"" }};
            return string.Concat((object[]) objArray1);
        }}


         #endregion


        #region 编辑控制器视图    

        /// <summary>
        /// 编辑视图      **********新增模块设置默认值，需要根据需求进行手工调整***************  
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult Edit()
        {{

            {HasIdentityMethodType} guid;
            string str = base.Request.Querys(""id"");
            //实体数据
            {ModelSpace} {entityName}model = null;
            //修改
            if (StringExtensions.Is{HasIdentityMethodType.ToFirstUpperStr()}(str, out guid))  
            {{
                {entityName}model = new {BusinessSpace}().Get(guid);
            }}
            //新增
            if ({entityName}model == null)
            {{
                {ModelSpace} {entityName}model1 = new {ModelSpace}();
                //新增添加需要根据实际调整 增加主键Guid设置
                {addField}
                {entityName}model = {entityName}model1;
            }}
            base.ViewData[""queryString""] = base.Request.UrlQuery();
            base.ViewData[""pageSize""] = base.Request.Querys(""pagesize"");
            base.ViewData[""pageNumber""] = base.Request.Querys(""pagenumber"");
           
            return this.View({entityName}model);
        }}








       





        #endregion


        #endregion 代码生成器


        #region 其他方法控制器


        #endregion
    }}
}}";
            string filePath = Path.Combine(_contentRootPath,"Areas.Controllers" ,$@"Areas.{AreaSpace}.Controllers", $"{entityName}Controller.cs");

            FileHelper.WriteTxt(code, filePath, FileMode.Create);
        }

        /// <summary>
        /// 生成视图
        /// </summary>
        /// <param name="tableInfoList">表字段信息</param>
        /// <param name="areaName">区域名</param>
        /// <param name="entityName">实体名</param>
        private void BuildView(List<TableInfo> tableInfo, string areaName, string entityName,string mainNameSpace)
        {

            //生成Index页面
            StringBuilder searchConditionSelectHtml = new StringBuilder();
            StringBuilder colNamesBuilder = new StringBuilder();  //字段列名设置
            StringBuilder tableColsBuilder = new StringBuilder();
            StringBuilder formRowBuilder = new StringBuilder();

            if (mainNameSpace.IsNullOrEmpty())
            {
                mainNameSpace = "RoadFlow";
            }

            //自增列
            var Identitys = tableInfo.Where(p => p.IsIdentity);
            bool HasIdentity = Identitys.Count() > 0;
            //主键
            var Primarykeys = tableInfo.Where(p => p.IsKey);
            bool HasPrimarykey = Primarykeys.Count() > 0;


            //控制器命名空间
            string AreaSpace = areaName.IsNullOrEmpty() ? "RapidDevelopment" : areaName;
            //Model命名实体类
            string ModelSpace = areaName.IsNullOrEmpty() ? $@"{mainNameSpace}.Model.{entityName}" : $@"{mainNameSpace}.Model.{areaName}.{entityName}";

            var formHeight = tableInfo.Where(x => x.Name != "Id").Count() * 2;
            if (formHeight > 8)
                formHeight = 8;
            tableInfo.Where(x => x.Name != Primarykeys.Last().Name ).ForEach((aField, index) =>
            {
                //搜索的下拉选项
                Type fieldType = _dbHelper.DbTypeStr_To_CsharpType(aField.Type);
                //查询条件是Type是字符串格式
                if (fieldType == typeof(string))
                {
                    string newOption = $@"
                    <option value=""{aField.Name}"">{aField.Description}</option>";
                    searchConditionSelectHtml.Append(newOption);
                }

                //数据表格列
                string newCol = $@"
                {{ name: '{aField.Name}', index: '{aField.Name}', width: '20%' }},";
                tableColsBuilder.Append(newCol);


                //数据列名
                string newcolName = $@"'{aField.Description}',";
                colNamesBuilder.Append(newcolName);



                //Form页面中的Html

                #region 设置编辑页面格式显示
                string newFormRow = "";
                if (_dbHelper.DbTypeStr_To_CsharpType(aField.Type).Name == "DateTime")
                {
                    string validatevalue = aField.IsNullable ? "" : $@"validate = ""empty"" errmsg = ""{aField.Description}不能为空!"" ";
                    newFormRow += $@"
        <tr>
            <th style=""width: 100px;"">{aField.Description}：</th>
            <td>
                <input type = ""text"" name=""{aField.Name}""  id=""{aField.Name}"" class=""mycalendar"" style=""width:75%"" istime=""1"" {validatevalue} value=""@Model.{aField.Name}"" />
            </td>
        </tr>";
                }
                else if(_dbHelper.DbTypeStr_To_CsharpType(aField.Type).Name.ToLower().Contains("int"))
                {
                    string validatevalue = aField.IsNullable ? "" : $@"validate = ""empty"" errmsg = ""{aField.Description}不能为空!"" ";
                    newFormRow += $@"
        <tr>
            <th style=""width: 100px;"">{aField.Description}：</th>
            <td>
                <input type = ""text"" name=""{aField.Name}""  id=""{aField.Name}"" class=""mytext""  {validatevalue} value=""@Model.{aField.Name}"" />
            </td>
        </tr>";
                }
                else if (aField.Name.ToLower().Contains("note"))
                {
                    string validatevalue = aField.IsNullable ? "" : $@"validate = ""empty"" errmsg = ""{aField.Description}不能为空!"" ";
                    newFormRow += $@"
        <tr>
            <th style=""width: 100px;"">{aField.Description}：</th>
            <td>
                <textarea class=""mytext"" name=""{aField.Name}"" id=""{aField.Name}"" {validatevalue} rows=""1"" cols=""1"" style=""width:90%; height:50px;"" >@Model.{aField.Name}</textarea>
            </td>
        </tr>";
                }
                else
                {
                    string validatevalue = aField.IsNullable ? "" : $@"validate = ""empty"" errmsg = ""{aField.Description}不能为空!"" ";
                    newFormRow += $@"
        <tr>
            <th style=""width: 100px;"">{aField.Description}：</th>
            <td>
                <input type = ""text"" name=""{aField.Name}""  id=""{aField.Name}"" class=""mytext"" style=""width:75%""  {validatevalue} value=""@Model.{aField.Name}"" />
            </td>
        </tr>";
                }  
                formRowBuilder.Append(newFormRow);
                #endregion

            });


        





            #region  Index 代码生成
            string indexHtml =
$@"@{{
    ViewBag.Title = ""{entityName}Index"";
}} 

<form method=""post"">
    @Html.AntiForgeryToken()
    <div class=""querybar"">
        <table cellpadding = ""0"" cellspacing=""1"" border=""0"" width=""100%"">
            <tr>
                <td>
                    <label>查询类别</label> 
                    <select class=""myselect2"" id=""condition"" name=""condition"" style=""width: 210px; "">
                    <option value="""">请选择</option>
                    {searchConditionSelectHtml.ToString()}
                    </select>
                    <input type=""text"" class=""mytext"" name=""keyword""  id=""keyword""  placeholder=""请输入关键字"">
                    <input type = ""button"" onclick=""query(null, 1);"" name=""Search"" value=""&nbsp;&nbsp;查&nbsp;询&nbsp;&nbsp;"" class=""mybutton"" />
                    <input type = ""button"" onclick=""add(); return false;"" value=""添加"" class=""mybutton"" />
                    <input type = ""button"" onclick=""return delBatch();"" value=""删除所选"" class=""mybutton"" />
                </td>
            </tr>
        </table>
    </div>
    <table id = ""listtable"" ></table >
    <div class=""buttondiv""></div>
</form>


<script type = ""text/javascript"" >
    var appid = '@ViewBag.appId';
    var iframeid = '@ViewBag.tabId';
    var dialog = top.mainDialog || new RoadUI.Window();
    var curPageSize;
    var curPageNumber;
    $(function () {{
        $(""#listtable"").jqGrid({{
            url: ""Query?@Html.Raw(ViewBag.query)"",
            postData: {{ appid: appid }},
            mtype: 'POST',
            datatype: ""json"",
            colNames: [{colNamesBuilder.ToString()}'编辑'],
            colModel: [
                {tableColsBuilder.ToString()}
                {{name: 'Opation', index: 'Opation', sortable: false, width: 100}},
            ],
            sortname: ""{Primarykeys.Last().Name}"",
            sortorder: ""desc"",
            height: '100%',
            width: $(window).width(),
            multiselect: true,
            loadComplete: function() {{
                var gridObj = $(""#listtable"");
                var records = gridObj.getGridParam(""userData"");
                curPageSize = records.pagesize;
                curPageNumber = records.pagenumber;
                    $("".buttondiv"").html(RoadUI.Core.getPager1(records.total, records.pagesize, records.pagenumber, ""query""));
                    //鼠标移动背景颜色变化
                    $(""#listtable tr"").mouseenter(function() {{
                        $(this).css(""background"", ""lightblue"");
                        }});
                    $(""#listtable tr"").mouseleave(function() {{
                        $(this).css(""background"", ""white"");
                        }});
            }}
        }});
           
    }});
    
    //设置显示屏幕宽度
    $(window).resize(function () {{
        $(""#listtable"").setGridWidth($(window).width());
    }});

    //查询功能
    function query(size, number)
    {{
            //查询数据获取
            var data = {{
                    condition: $(""#condition"").val(),conditionvalue: $(""#keyword"").val(), pagesize: size, pagenumber: number
                }};
            $(""#listtable"").setGridParam({{ postData: data }}).trigger(""reloadGrid"");
    }}

    //添加与编辑方法
    function add(id)
    {{
        dialog.open({{
                id: ""window_"" + appid.replaceAll('-', ''),
                title: (id && id.length > 0 ? ""编辑"" : ""添加""),
                width: 700,
                height: 420,
                url: 'Edit?id=' + (id || """") + ""&pagesize="" + curPageSize + ""&pagenumber="" + curPageNumber + '&@Html.Raw(ViewBag.query)',
                opener: window,
                openerid: iframeid
            }});
    }}

    //批量删除
    function delBatch()
    {{
        var rowIds = $(""#listtable"").jqGrid('getGridParam', 'selarrrow');
        if (rowIds.length == 0)
        {{
            alert(""您没有选择要删除的项!"");
            return false;
        }}
        if (confirm('您真的要删除所选吗?'))
        {{
            $.ajax({{
                url: ""DeleteBatch?@Html.Raw(ViewBag.query)"",
                type: ""post"",
                data: {{
                            ""ids"": rowIds.join(','),
                            ""__RequestVerificationToken"": $(""input[name='__RequestVerificationToken']"").val()
                        }},
                success: function(txt) {{
                    alert(txt);
                    query(curPageSize, curPageNumber);
                }}
            }});
        }}
    }}


    //删除
    function del(id)
    {{
        if (confirm('您真的要删除吗?'))
        {{
            $.ajax({{
                url: ""Delete?id=""+id+""&@Html.Raw(ViewBag.query)"",
                type: ""post"",
                data: {{
                            ""__RequestVerificationToken"": $(""input[name='__RequestVerificationToken']"").val()
                        }},
                success: function(txt) {{
                    alert(txt);
                    query(curPageSize, curPageNumber);
                }}
            }});
        }}
    }}

    </script>
";
            string indexPath = Path.Combine(_contentRootPath, "Areas", AreaSpace, "Views", entityName, "Index.cshtml");

            FileHelper.WriteTxt(indexHtml, indexPath, FileMode.Create);

            #endregion


            #region Edit代码生成

            //生成Edit页面
            string EditHtml =
$@"@{{
    ViewBag.Title = ""{entityName}Edit"";
}}
@model {ModelSpace}
<form method=""post"">
    @Html.AntiForgeryToken()
    <br/>
    <input type = ""hidden"" name = ""{Primarykeys.Last().Name}"" value = ""@Model.{Primarykeys.Last().Name}"" />
    <table cellpadding = ""0"" cellspacing = ""1"" border = ""0"" width = ""97%"" class=""formtable"" >
        {formRowBuilder.ToString()}
    </table>
    <div class=""buttondiv"" >
        <input type = ""button"" value=""确定保存"" class=""mybutton"" onclick=""saveform(this);"" />
        <input type = ""button"" class=""mybutton"" value=""取消关闭"" style=""margin-left: 5px;"" onclick=""new RoadUI.Window().close();"" />
    </div>
</form>
<script type = ""text/javascript"" >
    var win = new RoadUI.Window();

    $(window).load(function () {{

    }});
    function saveform(but)
    {{
        var f = document.forms[0];
        if (new RoadUI.Validate().validateForm(f))
        {{
            var o = RoadUI.Core.serializeForm($(f));
            $(but).prop(""disabled"", true);
            $.ajax({{
                    url: ""Save"" + ""@Html.Raw(ViewBag.queryString)"", 
                    data: o,
                    type: ""post"", 
                    success: function(text) {{
                        if (RoadUI.Core.checkLogin(text, false))
                        {{
                            alert(text);
                        }}
                        $(but).prop(""disabled"", false);
                        win.reloadOpener(undefined, undefined, ""query('@ViewBag.pageSize','@ViewBag.pageNumber')"");
                        win.close();
                }}
            }});
    }}
}}
</script>
";
            string formPath = Path.Combine(_contentRootPath, "Areas", AreaSpace, "Views", entityName, "Edit.cshtml");

            FileHelper.WriteTxt(EditHtml, formPath, FileMode.Create);

            #endregion



            #region 新增区域需要添加的ViewStart视图
            string ViewStartPath = Path.Combine(_contentRootPath, "Areas", AreaSpace, "Views", "_ViewStart.cshtml");

            if (!areaName.IsNullOrEmpty())
            {
                var ViewStartCode = $@"
@{{
    Layout = ""_Layout"";
}}";
                FileHelper.WriteTxt(ViewStartCode, ViewStartPath, FileMode.Create);
            }
            #endregion

        }


        #endregion

    }
}
