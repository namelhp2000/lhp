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
    #region 流程2.8.5

    public class MailContent
    {
        // Methods
        public int Add(RoadFlow.Model.MailContent mailContent)
        {
            using (DataContext context = new DataContext())
            {
                context.Add<RoadFlow.Model.MailContent>(mailContent);
                return context.SaveChanges();
            }
        }

        public int Delete(RoadFlow.Model.MailContent mailContent)
        {
            using (DataContext context = new DataContext())
            {
                context.Remove<RoadFlow.Model.MailContent>(mailContent);
                return context.SaveChanges();
            }
        }

        public RoadFlow.Model.MailContent Get(Guid id)
        {
            using (DataContext context = new DataContext())
            {
                object[] objects = new object[] { id };
                return context.Find<RoadFlow.Model.MailContent>(objects);
            }
        }

        public int Update(RoadFlow.Model.MailContent mailContent)
        {
            using (DataContext context = new DataContext())
            {
                context.Update<RoadFlow.Model.MailContent>(mailContent);
                return context.SaveChanges();
            }
        }
    }


  



    #endregion


}
