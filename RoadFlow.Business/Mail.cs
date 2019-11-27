using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
   

    public class Mail
    {
        // Fields
        private readonly RoadFlow.Data.MailContent mailContentData = new RoadFlow.Data.MailContent();
        private readonly RoadFlow.Data.MailDeletedBox mailDeletedBoxData = new RoadFlow.Data.MailDeletedBox();
        private readonly RoadFlow.Data.MailInBox mailInBoxData = new RoadFlow.Data.MailInBox();
        private readonly RoadFlow.Data.MailOutBox mailOutBoxData = new RoadFlow.Data.MailOutBox();

        // Methods
        public int AddMailContent(RoadFlow.Model.MailContent mailContent)
        {
            return this.mailContentData.Add(mailContent);
        }

        public int AddMailDeletedBox(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            return this.mailDeletedBoxData.Add(mailDeletedBox);
        }

        public int AddMailInBox(RoadFlow.Model.MailInBox mailInBox)
        {
            return this.mailInBoxData.Add(mailInBox);
        }

        public int AddMailOutBox(RoadFlow.Model.MailOutBox mailOutBox)
        {
            return this.mailOutBoxData.Add(mailOutBox);
        }

        public int DeleteMailContent(RoadFlow.Model.MailContent mailContent)
        {
            return this.mailContentData.Delete(mailContent);
        }

        public int DeleteMailDeletedBox(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            return this.mailDeletedBoxData.Delete(mailDeletedBox);
        }

        public int DeleteMailInBox(RoadFlow.Model.MailInBox mailInBox)
        {
            return this.mailInBoxData.Delete(mailInBox);
        }

        public int DeleteMailInBox(Guid id, int status)
        {
            return this.mailInBoxData.Delete(id, status);
        }

        public int DeleteMailOutBox(RoadFlow.Model.MailOutBox mailOutBox)
        {
            return this.mailOutBoxData.Delete(mailOutBox);
        }

        public int DeleteMailOutBox(Guid id)
        {
            return this.mailOutBoxData.Delete(id);
        }

        public RoadFlow.Model.MailContent GetMailContent(Guid id)
        {
            return this.mailContentData.Get(id);
        }

        public RoadFlow.Model.MailDeletedBox GetMailDeletedBox(Guid id)
        {
            return this.mailDeletedBoxData.Get(id);
        }

        public DataTable GetMailDeletedBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            return this.mailDeletedBoxData.GetPagerList(out count, size, number, currentUserId, subject, userId, date1, date2, order);
        }

        public RoadFlow.Model.MailInBox GetMailInBox(Guid id)
        {
            return this.mailInBoxData.Get(id);
        }

        public DataTable GetMailInBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string userId, string date1, string date2, string order)
        {
            return this.mailInBoxData.GetPagerList(out count, size, number, currentUserId, subject, userId, date1, date2, order);
        }

        public RoadFlow.Model.MailOutBox GetMailOutBox(Guid id)
        {
            return this.mailOutBoxData.Get(id);
        }

        public DataTable GetMailOutBoxPagerList(out int count, int size, int number, Guid currentUserId, string subject, string date1, string date2, string order, int status)
        {
            return this.mailOutBoxData.GetPagerList(out count, size, number, currentUserId, subject, date1, date2, order, status);
        }

        public bool IsWithdraw(Guid id)
        {
            return this.MailInBoxAllNoRead(id);
        }

        public bool MailInBoxAllNoRead(Guid outBoxId)
        {
            return this.mailInBoxData.AllNoRead(outBoxId);
        }

        public int RecoveryMailDeletedBox(Guid id)
        {
            RoadFlow.Model.MailDeletedBox mailDeletedBox = this.GetMailDeletedBox(id);
            if (mailDeletedBox == null)
            {
                return 0;
            }
            return this.mailDeletedBoxData.Recovery(mailDeletedBox);
        }

        public int Send(RoadFlow.Model.MailOutBox mailOutBox, RoadFlow.Model.MailContent mailContent, bool isAdd)
        {
            Organize organize = new Organize();
            return this.mailOutBoxData.Send(mailOutBox, mailContent, organize.GetAllUsers(mailOutBox.ReceiveUsers), organize.GetAllUsers(mailOutBox.CarbonCopy), organize.GetAllUsers(mailOutBox.SecretCopy), isAdd);

           // return this.mailOutBoxData.Send(mailOutBox, mailContent, new Organize().GetAllUsers(mailOutBox.ReceiveUsers), isAdd);
        }

        public int UpdateIsRead(Guid id, int status, bool isUpdateDate = false)
        {
            return this.mailInBoxData.UpdateIsRead(id, status, isUpdateDate);
        }

        public int UpdateMailContent(RoadFlow.Model.MailContent mailContent)
        {
            return this.mailContentData.Update(mailContent);
        }

        public int UpdateMailDeletedBox(RoadFlow.Model.MailDeletedBox mailDeletedBox)
        {
            return this.mailDeletedBoxData.Update(mailDeletedBox);
        }

        public int UpdateMailInBox(RoadFlow.Model.MailInBox mailInBox)
        {
            return this.mailInBoxData.Update(mailInBox);
        }

        public int UpdateMailOutBox(RoadFlow.Model.MailOutBox mailOutBox)
        {
            return this.mailOutBoxData.Update(mailOutBox);
        }

        public bool Withdraw(Guid id)
        {
            return this.mailOutBoxData.Withdraw(id);
        }
    }






}
