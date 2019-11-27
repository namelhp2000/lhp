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
    public class ProgramField
    {
        // Methods
        public int Add(RoadFlow.Model.ProgramField programField)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.ProgramField>(programField);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.ProgramField[] programFields)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.ProgramField>(programFields);
                return context.SaveChanges();
            }
        }



        public RoadFlow.Model.ProgramField Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.ProgramField>(objects);
            }
        }

        public List<RoadFlow.Model.ProgramField> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramField>("SELECT * FROM RF_ProgramField WHERE ProgramId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.ProgramField programField)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.ProgramField>(programField);
                return context.SaveChanges();
            }
        }
    }


}
