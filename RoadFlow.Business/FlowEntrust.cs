using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{
    public class FlowEntrust
    {
        // Fields
        private RoadFlow.Data.FlowEntrust flowEntrustData = new RoadFlow.Data.FlowEntrust();

        // Methods
        public int Add(RoadFlow.Model.FlowEntrust flowEntrust)
        {
            return this.flowEntrustData.Add(flowEntrust);
        }

        public int Delete(RoadFlow.Model.FlowEntrust[] flowEntrusts)
        {
            return this.flowEntrustData.Delete(flowEntrusts);
        }

        public RoadFlow.Model.FlowEntrust Get(Guid id)
        {
            return this.flowEntrustData.Get(id);
        }

        public List<RoadFlow.Model.FlowEntrust> GetAll()
        {
            return this.flowEntrustData.GetAll();
        }

        public string GetEntrustUserId(Guid flowId, RoadFlow.Model.User user)
        {
            DateTime now = DateTimeExtensions.Now;
            RoadFlow.Model.FlowEntrust entrust = this.GetAll().Find(delegate (RoadFlow.Model.FlowEntrust p) {
                if (p.UserId == user.Id)
                {
                    if (p.FlowId.HasValue)
                    {
                        Guid? nullable1 = p.FlowId;
                        Guid guid = flowId;
                        if (!(nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == guid) : true) : false))
                        {
                            goto Label_0082;
                        }
                    }
                    if (p.StartTime <= now)
                    {
                        return p.EndTime >= now;
                    }
                }
                Label_0082:
                return false;
            });
            if (entrust != null)
            {
                return entrust.ToUserId;
            }
            return string.Empty;
        }

        public DataTable GetPagerList(out int count, int size, int number, string userId, string date1, string date2, string order)
        {
            return this.flowEntrustData.GetPagerList(out count, size, number, userId, date1, date2, order);
        }

        public int Update(RoadFlow.Model.FlowEntrust flowEntrust)
        {
            return this.flowEntrustData.Update(flowEntrust);
        }
    }


}
