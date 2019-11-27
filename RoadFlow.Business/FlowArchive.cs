using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{
    public class FlowArchive
    {
        // Fields
        private readonly RoadFlow.Data.FlowArchive flowArchiveData = new RoadFlow.Data.FlowArchive();

        // Methods
        public int Add(RoadFlow.Model.FlowArchive flowArchive)
        {
            return this.flowArchiveData.Add(flowArchive);
        }

        public RoadFlow.Model.FlowArchive Get(Guid id)
        {
            return this.flowArchiveData.Get(id);
        }

        public string GetArchiveComments(Guid id)
        {
            RoadFlow.Model.FlowArchive archive = this.Get(id);
            if (archive != null)
            {
                return archive.Comments;
            }
            return string.Empty;
        }

        public string GetArchiveData(Guid id)
        {
            RoadFlow.Model.FlowArchive archive = this.Get(id);
            if (archive != null)
            {
                return archive.DataJson;
            }
            return string.Empty;
        }

        public DataTable GetPagerData(out int count, int size, int number, string flowId, string stepName, string title, string date1, string date2, string order)
        {
            return this.flowArchiveData.GetPagerData(out count, size, number, flowId, stepName, title, date1, date2, order);
        }
    }


}
