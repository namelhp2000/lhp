using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using System.Data.Common;
using RoadFlow.Utility;
using System.Linq;

namespace RoadFlow.Data
{
    public class TaskLog
    {
        #region 数据代码生成区域


        /// <summary>
		/// 设置缓存字段名称
		/// </summary>
		private const string CACHEKEY = "roadflow_cache_tasklog";

        /// <summary>
		/// 清除缓存
		/// </summary>
		public  void ClearCache()
		{
            IO.Remove("roadflow_cache_tasklog");
        }


        /// <summary>
		/// 添加记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskLog实体类</param>
		/// <returns>操作所影响的行数</returns>
		public int  Add(RoadFlow.Model.TaskLog model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.TaskLog>(model);
                return context.SaveChanges();
            }
        }


        /// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskLog实体类</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskLog model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.TaskLog>(model);
                return context.SaveChanges();
            }
        }



		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="models">RoadFlow.Model.TaskLog列表实体类</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskLog[] models)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.TaskLog>(models);
                return context.SaveChanges();
            }
        }


		/// <summary>
		/// 查询所有记录
		/// </summary>
		public List<RoadFlow.Model.TaskLog> GetAll()
		{
            object obj2 = IO.Get("roadflow_cache_tasklog");
			if (obj2 == null)
			{
				using (DataContext context = new DataContext())
				{
					List<RoadFlow.Model.TaskLog> list = context.QueryAll<RoadFlow.Model.TaskLog>(); 
					IO.Insert("roadflow_cache_tasklog", list);
					return list;
				}
			}
			return (List<RoadFlow.Model.TaskLog>)obj2;
		}



		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskLog实体类</param>
		/// <returns></returns>
		public int Update(RoadFlow.Model.TaskLog model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.TaskLog>(model);
                return context.SaveChanges();
            }
        }


	    /// <summary>
		/// 批量更新数据
		/// </summary>
		/// <param name="models">RoadFlow.Model.TaskLog列表实体类</param>
		/// <returns></returns>
		public int Update(RoadFlow.Model.TaskLog[] models)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.TaskLog>(models);
                return context.SaveChanges();
            }
        }



		/// <summary>
		///多行操作：根据传递参数进行对应操作处理 
		/// </summary>
		/// <param name="tuples">通过传递对应的值，进行对应的操作 2代表新增 1代表更新 0代表删除</param>
		/// <returns></returns>
		public int TypeOperation(List<Tuple<RoadFlow.Model.TaskLog, int>> tuples)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                foreach (Tuple<RoadFlow.Model.TaskLog, int> tuple in tuples)
                {
                    if (tuple.Item2 == 0)//删除
                    {
                        context.Remove<RoadFlow.Model.TaskLog>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 1)//更新
                    {
                        context.Update<RoadFlow.Model.TaskLog>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 2)//新增
                    {
                        context.Add<RoadFlow.Model.TaskLog>(tuple.Item1);
                    }
                }
                return context.SaveChanges();
            }
        }




		/// <summary>
		///单行操作：根据传递参数进行对应操作处理 
		/// </summary>
		/// <param name="tuple">通过传递对应的值，进行对应的操作 2代表新增 1代表更新 0代表删除</param>
		/// <returns></returns>
		public int TypeOperation(Tuple<RoadFlow.Model.TaskLog, int> tuple)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                if (tuple.Item2 == 0)//删除
                {
                    context.Remove<RoadFlow.Model.TaskLog>(tuple.Item1);
                }
                else if (tuple.Item2 == 1)//更新
                {
                    context.Update<RoadFlow.Model.TaskLog>(tuple.Item1);
                }
                else if (tuple.Item2 == 2)//新增
                {
                    context.Add<RoadFlow.Model.TaskLog>(tuple.Item1);
                }
                return context.SaveChanges();
            }
        }

        #endregion

        #region   其他方法区域

        #endregion

            
    }
}