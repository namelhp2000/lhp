using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{
    public class Doc
    {
        // Fields
        private readonly RoadFlow.Data.Doc docData = new RoadFlow.Data.Doc();

        // Methods
        public int Add(RoadFlow.Model.Doc doc, List<RoadFlow.Model.User> users = null)
        {
            return this.docData.Add(doc, users);
        }

        public int Delete(Guid id)
        {
            return this.docData.Delete(id);
        }

        public RoadFlow.Model.Doc Get(Guid id)
        {
            return this.docData.Get(id);
        }

        public DataTable GetPagerList(out int count, int size, int number, Guid userId, string title, string dirId, string date1, string date2, string order, int read)
        {
            return this.docData.GetPagerList(out count, size, number, userId, title, dirId, date1, date2, order, read);
        }

        public string GetRankOptions(int rank)
        {
            return new Dictionary().GetOptionsByCode("system_documentrank", ValueField.Value, ((int)rank).ToString(), false);
        }

        public int Update(RoadFlow.Model.Doc doc, List<RoadFlow.Model.User> users = null)
        {
            return this.docData.Update(doc, users);
        }

        public int UpdateReadCount(RoadFlow.Model.Doc doc)
        {
            return this.docData.UpdateReadCount(doc);
        }
    }


}
