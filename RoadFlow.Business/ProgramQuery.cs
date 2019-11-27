using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class ProgramQuery
    {
        // Fields
        private readonly RoadFlow.Data.ProgramQuery programQueryData = new RoadFlow.Data.ProgramQuery();

        // Methods
        public int Add(RoadFlow.Model.ProgramQuery programQuery)
        {
            return this.programQueryData.Add(programQuery);
        }

        public int Delete(RoadFlow.Model.ProgramQuery[] programQueries)
        {
            return this.programQueryData.Delete(programQueries);
        }

        public RoadFlow.Model.ProgramQuery Get(Guid id)
        {
            return this.programQueryData.Get(id);
        }

        /// <summary>
        /// 通过项目id获取所有设计项目查询条件
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramQuery> GetAll(Guid programId)
        {
            return Enumerable.ToList<RoadFlow.Model.ProgramQuery>((IEnumerable<RoadFlow.Model.ProgramQuery>)Enumerable.OrderBy<RoadFlow.Model.ProgramQuery, int>((IEnumerable<RoadFlow.Model.ProgramQuery>)this.programQueryData.GetAll(programId), key => key.Sort));
        }

        public int GetMaxSort(Guid programId)
        {
            List<RoadFlow.Model.ProgramQuery> all = this.GetAll(programId);
            if (all.Count() == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.ProgramQuery>((IEnumerable<RoadFlow.Model.ProgramQuery>)all, key => key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.ProgramQuery programQuery)
        {
            return this.programQueryData.Update(programQuery);
        }

    }

}
