using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Data;
using System.Linq;

namespace RoadFlow.Business
{
    public class TaskOptions
    {
        #region 业务逻辑代码生成区域


        /// <summary>
		/// 调用TaskOptions表Data类
		/// </summary>
		private readonly RoadFlow.Data.TaskOptions dataTaskOptions = new RoadFlow.Data.TaskOptions();


        /// <summary>
		/// 操作所影响的行数
		/// </summary>
		public int  Add(RoadFlow.Model.TaskOptions model)
		{
			return dataTaskOptions.Add(model);
		}



		/// <summary>
		/// 删除
		/// </summary>
		public int Delete(Guid id)
		{
            RoadFlow.Model.TaskOptions getdata = this.Get(id);
			if (getdata != null)
			{
				return dataTaskOptions.Delete(getdata);
			}
			return 0; 
		}


		/// <summary>
		/// 实体类删除
		/// </summary>
		/// <param name="model">TaskOptions表实体删除</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskOptions model)
        {
            return dataTaskOptions.Delete(model);
        }


        /// <summary>
		/// 实体类批量删除
		/// </summary>
		/// <param name="models">TaskOptions列表实体删除</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskOptions[] models)
        {
            return dataTaskOptions.Delete(models);
        }



		
        /// <summary>
		/// 通过字符串id批量删除
		/// </summary>
		public List<RoadFlow.Model.TaskOptions> Delete(string idString)
		{
            List <RoadFlow.Model.TaskOptions> list = new List<RoadFlow.Model.TaskOptions>();
			char[] separator = new char[] { ',' };
			List<RoadFlow.Model.TaskOptions> all = this.GetAll();
			string[] strArray = idString.Split(separator);
			 for (int i = 0; i < strArray.Length; i++)
			 {
				Guid strvalue ;
				if (strArray[i].IsGuid(out strvalue))
				{
					list.Add(all.Find(delegate (RoadFlow.Model.TaskOptions p) {
						return p.Id== strvalue;
					}));
				}
			 }
			dataTaskOptions.Delete(list.ToArray());
			return list;
		}





		/// <summary>
		/// 通过主键获取对应的数据
		/// </summary>
		public RoadFlow.Model.TaskOptions Get(Guid strvalue )
		{
			return this.GetAll().Find(delegate (RoadFlow.Model.TaskOptions p)
			{
				return p.Id == strvalue;
			});
		}



        /// <summary>
		/// 查询所有的数据  
		/// </summary>
		public List<RoadFlow.Model.TaskOptions> GetAll()
		{
			return dataTaskOptions.GetAll();
		}



        /// <summary>
		/// 更新
		/// </summary>
		public int Update(RoadFlow.Model.TaskOptions model)
		{
			return dataTaskOptions.Update(model);
		}



		/// <summary>
		/// 批量更新
		/// </summary>
		public int Update(RoadFlow.Model.TaskOptions[] models)
		{
			return dataTaskOptions.Update(models);
		}



        /// <summary>
		/// 通过传参方式进行综合操作
		/// </summary>
		public int TypeOperation(List<Tuple<RoadFlow.Model.TaskOptions , int>> tuples)
		{
			return dataTaskOptions.TypeOperation(tuples);
		}

		/// <summary>
		/// 通过传参方式进行综合操作
		/// </summary>
		public int TypeOperation(Tuple<RoadFlow.Model.TaskOptions , int> tuple)
		{
			return dataTaskOptions.TypeOperation(tuple);
		}



        #region 页码列表查询方法，需手工进行调整处理 
		/// <summary>
		/// 页码列表查询方法 
		/// </summary>
		/// <param name="count">总数大小</param>
        /// <param name="size">页码大小</param>
        /// <param name="number">页码值</param>
        /// <param name="order">排序</param>
        /// <param name="otherparam">后面参数，通过需求，进行字典添加处理</param>
        /// <returns></returns>
        public DataTable GetWherePagerList(out int count, int size, int number, string order, string condition,string conditionvalue)
        {
            Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();
            #region 把对应的字段循环出来，匹配对应的值，该方式需手工调整处理
            if(!condition.IsNullOrEmpty())
            {
                 dics.Add(new ValueTuple<string, string>(condition,conditionvalue), datatype.stringType);
            }
            /*
            
                dics.Add(new ValueTuple<string, string>("Id",id), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("TaskName",taskname), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("GroupName",groupname), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("Interval",interval), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("ApiUrl",apiurl), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("AuthKey",authkey), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("AuthValue",authvalue), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("Describe",describe), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("RequestType",requesttype), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("LastRunTime",lastruntime), datatype.stringType);
                dics.Add(new ValueTuple<string, string>("Status",status), datatype.stringType);
            */
            #endregion
            return Mapper.GetPagerTemplate.GetWherePagerList(out count, size, number, dics, "  select * from  TaskOptions ", order  );
        }
        #endregion




        /// <summary>
		/// 获取表的条数+1 
		/// </summary>
		public int GetMax()
		{
            List <RoadFlow.Model.TaskOptions> all = GetAll();
			if (all.Count == 0)
			{
				return 1;
			}
			return all.Count+1 ;
		}




        #endregion


        #region 业务其他方法

        #endregion
    }
}