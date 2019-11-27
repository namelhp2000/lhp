using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Data
{
    public class FlowDynamic
    {
        // Methods
        public int Add(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.FlowDynamic>(flowDynamic);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.FlowDynamic>(flowDynamic);
                return context.SaveChanges();
            }
        }

        public int Delete(Guid stepId, Guid groupId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { stepId, groupId };
                context.Execute("DELETE FROM RF_FlowDynamic WHERE StepId={0} AND GroupId={1}", objects);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.FlowDynamic Get(Guid StepId, Guid groupId)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { StepId, groupId };
                return context.Find<RoadFlow.Model.FlowDynamic>(objects);
            }
        }

        public int Update(RoadFlow.Model.FlowDynamic flowDynamic)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.FlowDynamic>(flowDynamic);
                return context.SaveChanges();
            }
        }
    }




}
