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
    public class ProgramGroup
    {
        #region 数据代码生成区域


      

        /// <summary>
		/// 添加记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.ProgramGroup实体类</param>
		/// <returns>操作所影响的行数</returns>
		public int  Add(RoadFlow.Model.ProgramGroup model)
        {
          
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.ProgramGroup>(model);
                return context.SaveChanges();
            }
        }


        /// <summary>
		/// 删除记录
		/// </summary>
		/// <param name="model">RoadFlow.Model.ProgramGroup实体类</param>
		/// <returns></returns>
		public int Delete(RoadFlow.Model.ProgramGroup[] model)
        {
          
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.ProgramGroup>(model);
                return context.SaveChanges();
            }
        }


       




        public RoadFlow.Model.ProgramGroup Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.ProgramGroup>(objects);
            }
        }

        public List<RoadFlow.Model.ProgramGroup> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramGroup>("SELECT * FROM RF_ProgramGroup WHERE ProgramId={0}", objects);
            }
        }



     



		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">RoadFlow.Model.ProgramGroup实体类</param>
		/// <returns></returns>
		public int Update(RoadFlow.Model.ProgramGroup model)
        {
           
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.ProgramGroup>(model);
                return context.SaveChanges();
            }
        }


	  



		

        #endregion

        #region   其他方法区域

        #endregion

            
    }
}