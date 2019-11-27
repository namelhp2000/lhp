using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{
    public class MessageUser
    {
        // Fields
        private readonly RoadFlow.Data.MessageUser messageUserData = new RoadFlow.Data.MessageUser();

        // Methods
        public DataTable GetReadUserList(out int count, int size, int number, string messageId, string order)
        {
            return this.messageUserData.GetReadUserList(out count, size, number, messageId, order);
        }

        public int UpdateIsRead(Guid messageId, Guid userId)
        {
            return this.messageUserData.UpdateIsRead(messageId, userId);
        }


        public int DeleteSerd(Guid messageId, Guid userId)
        {
            return this.messageUserData.DeleteSerd(messageId, userId);
        }

    }


}
