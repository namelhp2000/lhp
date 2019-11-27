using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class ProgramGroup
    {
        #region 业务逻辑代码生成区域


        /// <summary>
		/// 调用ProgramGroup表Data类
		/// </summary>
		private readonly RoadFlow.Data.ProgramGroup dataProgramGroup = new RoadFlow.Data.ProgramGroup();


        /// <summary>
		/// 操作所影响的行数
		/// </summary>
		public int  Add(RoadFlow.Model.ProgramGroup model)
		{
			return dataProgramGroup.Add(model);
		}



	
		

        public int Delete(RoadFlow.Model.ProgramGroup[] programGroups)
        {
            return this.dataProgramGroup.Delete(programGroups);
        }






        /// <summary>
        /// 通过主键获取对应的数据
        /// </summary>
        public RoadFlow.Model.ProgramGroup Get(Guid strvalue )
		{
			return dataProgramGroup.Get(strvalue);
        }


        /// <summary>
        /// 通过项目id获取所有项目字段分组
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramGroup> GetAll(Guid programId)
        {
            return Enumerable.ToList<RoadFlow.Model.ProgramGroup>((IEnumerable<RoadFlow.Model.ProgramGroup>)Enumerable.OrderBy<RoadFlow.Model.ProgramGroup, int>((IEnumerable<RoadFlow.Model.ProgramGroup>)this.dataProgramGroup.GetAll(programId),  key=>key.Sort));
        }

        public string GetFieldOptions(Guid connId, string sql, string value, List<string> removeValue = null)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in new DbConnection().GetFieldsBySql(connId, sql, null))
            {
                if (((removeValue == null) || value.EqualsIgnoreCase(str)) || !removeValue.ContainsIgnoreCase(str))
                {
                    string[] textArray1 = new string[] { "<option value=\"", str, "\"", str.EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">", str, "</option>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            return builder.ToString();
        }

        public int GetMaxSort(Guid programId)
        {
            List<RoadFlow.Model.ProgramGroup> all = this.GetAll(programId);
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.ProgramGroup>((IEnumerable<RoadFlow.Model.ProgramGroup>)all,key=>key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.ProgramGroup programField)
        {
            return this.dataProgramGroup.Update(programField);
        }


        #endregion
  
    }
}