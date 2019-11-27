using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{
    public class DocUser
    {
        // Fields
        private readonly RoadFlow.Data.DocUser docUserData = new RoadFlow.Data.DocUser();

        // Methods
        public int Delete(Guid userId)
        {
            return this.docUserData.Delete(userId);
        }

        public DataTable GetDocReadPagerList(out int count, int size, int number, string docId, string order)
        {
            return this.docUserData.GetDocReadPagerList(out count, size, number, docId, order);
        }

        public bool IsRead(Guid docId, Guid userId)
        {
            return this.docUserData.IsRead(docId, userId);
        }

        public int UpdateIsRead(Guid docId, Guid userId, int isRead)
        {
            return this.docUserData.UpdateIsRead(docId, userId, isRead);
        }
    }



}
