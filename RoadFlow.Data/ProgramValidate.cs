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
    public class ProgramValidate
    {
        // Methods
        public int Add(RoadFlow.Model.ProgramValidate[] programValidates)
        {
            if (programValidates.Length == 0)
            {
                return 0;
            }
            using (DataContext context = new DataContext())
            {
                context.Execute("DELETE FROM RF_ProgramValidate WHERE ProgramId='" + Enumerable.First<RoadFlow.Model.ProgramValidate>(programValidates).ProgramId + "'", (DbParameter[])null);
                context.AddRange<RoadFlow.Model.ProgramValidate>(programValidates);
                return context.SaveChanges();
            }
        }

        public List<RoadFlow.Model.ProgramValidate> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramValidate>("SELECT * FROM RF_ProgramValidate WHERE ProgramId={0}", objects);
            }
        }
    }


}
