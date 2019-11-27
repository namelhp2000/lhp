using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class ProgramExport
    {
        // Fields
        private readonly RoadFlow.Data.ProgramExport programExportData = new RoadFlow.Data.ProgramExport();

        // Methods
        public int Add(RoadFlow.Model.ProgramExport programExport)
        {
            return this.programExportData.Add(programExport);
        }

        public int Delete(RoadFlow.Model.ProgramExport[] programExports)
        {
            return this.programExportData.Delete(programExports);
        }

        public int DeleteAndAdd(RoadFlow.Model.ProgramExport[] programExports)
        {
            return this.programExportData.DeleteAndAdd(programExports);
        }

        public RoadFlow.Model.ProgramExport Get(Guid id)
        {
            return this.programExportData.Get(id);
        }

        /// <summary>
        /// 通过项目id获取设计所有导出数据
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramExport> GetAll(Guid programId)
        {
            return Enumerable.ToList<RoadFlow.Model.ProgramExport>((IEnumerable<RoadFlow.Model.ProgramExport>)Enumerable.OrderBy<RoadFlow.Model.ProgramExport, int>((IEnumerable<RoadFlow.Model.ProgramExport>)this.programExportData.GetAll(programId), key => key.Sort));
        }

        public int GetMaxSort(Guid programId)
        {
            List<RoadFlow.Model.ProgramExport> all = this.GetAll(programId);
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.ProgramExport>((IEnumerable<RoadFlow.Model.ProgramExport>)all, key => key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.ProgramExport programExport)
        {
            return this.programExportData.Update(programExport);
        }


    }
}
