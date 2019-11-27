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
    public class TaskOptions
    {
        #region 数据代码生成区域


        /// <summary>
		/// 设置缓存字段名称
		/// </summary>
		private const string CACHEKEY = "roadflow_cache_taskoptions";

        /// <summary>
		/// 清除缓存
		/// </summary>
		public  void ClearCache()
		{
            IO.Remove("roadflow_cache_taskoptions");
        }


        /// <summary>
		/// 添加记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskOptions实体类</param>
		/// <returns>操作所影响的行数</returns>
		public int  Add(RoadFlow.Model.TaskOptions model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.TaskOptions>(model);
                return context.SaveChanges();
            }
        }


        /// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskOptions实体类</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskOptions model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.TaskOptions>(model);
                return context.SaveChanges();
            }
        }



		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="models">RoadFlow.Model.TaskOptions列表实体类</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.TaskOptions[] models)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.TaskOptions>(models);
                return context.SaveChanges();
            }
        }


		/// <summary>
		/// 查询所有记录
		/// </summary>
		public List<RoadFlow.Model.TaskOptions> GetAll()
		{
            object obj2 = IO.Get("roadflow_cache_taskoptions");
			if (obj2 == null)
			{
				using (DataContext context = new DataContext())
				{
					List<RoadFlow.Model.TaskOptions> list = context.QueryAll<RoadFlow.Model.TaskOptions>(); 
					IO.Insert("roadflow_cache_taskoptions", list);
					return list;
				}
			}
			return (List<RoadFlow.Model.TaskOptions>)obj2;
		}



		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">RoadFlow.Model.TaskOptions实体类</param>
		/// <returns></returns>
		public int Update(RoadFlow.Model.TaskOptions model)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.TaskOptions>(model);
                return context.SaveChanges();
            }
        }


	    /// <summary>
		/// 批量更新数据
		/// </summary>
		/// <param name="models">RoadFlow.Model.TaskOptions列表实体类</param>
		/// <returns></returns>
		public int Update(RoadFlow.Model.TaskOptions[] models)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                context.UpdateRange<RoadFlow.Model.TaskOptions>(models);
                return context.SaveChanges();
            }
        }



		/// <summary>
		///多行操作：根据传递参数进行对应操作处理 
		/// </summary>
		/// <param name="tuples">通过传递对应的值，进行对应的操作 2代表新增 1代表更新 0代表删除</param>
		/// <returns></returns>
		public int TypeOperation(List<Tuple<RoadFlow.Model.TaskOptions, int>> tuples)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                foreach (Tuple<RoadFlow.Model.TaskOptions, int> tuple in tuples)
                {
                    if (tuple.Item2 == 0)//删除
                    {
                        context.Remove<RoadFlow.Model.TaskOptions>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 1)//更新
                    {
                        context.Update<RoadFlow.Model.TaskOptions>(tuple.Item1);
                    }
                    else if (tuple.Item2 == 2)//新增
                    {
                        context.Add<RoadFlow.Model.TaskOptions>(tuple.Item1);
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
		public int TypeOperation(Tuple<RoadFlow.Model.TaskOptions, int> tuple)
        {
            this.ClearCache();
            using (DataContext context = new DataContext())
            {
                if (tuple.Item2 == 0)//删除
                {
                    context.Remove<RoadFlow.Model.TaskOptions>(tuple.Item1);
                }
                else if (tuple.Item2 == 1)//更新
                {
                    context.Update<RoadFlow.Model.TaskOptions>(tuple.Item1);
                }
                else if (tuple.Item2 == 2)//新增
                {
                    context.Add<RoadFlow.Model.TaskOptions>(tuple.Item1);
                }
                return context.SaveChanges();
            }
        }

        #endregion

        #region   其他方法区域

        #endregion

            
    }
}