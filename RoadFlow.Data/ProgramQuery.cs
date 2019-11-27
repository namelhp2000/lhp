using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RoadFlow.Data
{
    public class ProgramQuery
    {
        // Methods
        public int Add(RoadFlow.Model.ProgramQuery programQuery)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.ProgramQuery>(programQuery);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.ProgramQuery[] programQueries)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.ProgramQuery>(programQueries);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.ProgramQuery Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.ProgramQuery>(objects);
            }
        }

        public List<RoadFlow.Model.ProgramQuery> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramQuery>("SELECT * FROM RF_ProgramQuery WHERE ProgramId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.ProgramQuery programQuery)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.ProgramQuery>(programQuery);
                return context.SaveChanges();
            }
        }
    }


}
