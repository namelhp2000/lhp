using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace RoadFlow.Data
{
    public class ProgramExport
    {
        // Methods
        public int Add(RoadFlow.Model.ProgramExport programExport)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.ProgramExport>(programExport);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.ProgramExport[] programExports)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.ProgramExport>(programExports);
                return context.SaveChanges();
            }
        }

        public int DeleteAndAdd(RoadFlow.Model.ProgramExport[] programExports)
        {
            if (programExports.Length == 0)
            {
                return 0;
            }
            using (DataContext context = new DataContext())
            {
                context.Execute("DELETE FROM RF_ProgramExport WHERE ProgramId='" + Enumerable.First<RoadFlow.Model.ProgramExport>(programExports).ProgramId + "'", (DbParameter[])null);
                context.AddRange<RoadFlow.Model.ProgramExport>(programExports);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.ProgramExport Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.ProgramExport>(objects);
            }
        }

        public List<RoadFlow.Model.ProgramExport> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramExport>("SELECT * FROM RF_ProgramExport WHERE ProgramId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.ProgramExport programExport)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.ProgramExport>(programExport);
                return context.SaveChanges();
            }
        }
    }


}
