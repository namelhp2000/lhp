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
    public class ProgramButton
    {
        // Methods
        public int Add(RoadFlow.Model.ProgramButton programButton)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.ProgramButton>(programButton);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.ProgramButton[] programButtons)
        {
            using (DataContext context = new DataContext())
            {
                context.RemoveRange<RoadFlow.Model.ProgramButton>(programButtons);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.ProgramButton Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.ProgramButton>(objects);
            }
        }

        public List<RoadFlow.Model.ProgramButton> GetAll(Guid programId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { programId };
                return context.Query<RoadFlow.Model.ProgramButton>("SELECT * FROM RF_ProgramButton WHERE ProgramId={0}", objects);
            }
        }

        public int Update(RoadFlow.Model.ProgramButton programButton)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.ProgramButton>(programButton);
                return context.SaveChanges();
            }
        }
    }


}
