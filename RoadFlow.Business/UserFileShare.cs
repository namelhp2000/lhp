using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class UserFileShare
    {
        // Fields
        private RoadFlow.Data.UserFileShare userFileShareData = new RoadFlow.Data.UserFileShare();

        // Methods
        public int Add(RoadFlow.Model.UserFileShare userFileShare)
        {
            return this.userFileShareData.Add(userFileShare);
        }

        public int Add(IEnumerable<RoadFlow.Model.UserFileShare> userFileShares)
        {
            return this.userFileShareData.Add(userFileShares);
        }

        public int DeleteByFileId(string fileId)
        {
            return this.userFileShareData.DeleteByFileId(fileId);
        }

        public RoadFlow.Model.UserFileShare Get(string fileId, Guid userId)
        {
            return this.userFileShareData.Get(fileId, userId);
        }

        public DataTable GetMySharePagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            return this.userFileShareData.GetMySharePagerList(out count, size, number, userId, fileName, order);
        }



        public DataTable GetMyShareUsers(string fileId, Guid userId)
        {
            return this.userFileShareData.GetMyShareUsers(fileId, userId);
        }

        public DataTable GetShareMyPagerList(out int count, int size, int number, Guid userId, string fileName, string order)
        {
            return this.userFileShareData.GetShareMyPagerList(out count, size, number, userId, fileName, order);
        }

        public bool IsAccess(string fileId, Guid userId, string fileId1 = "")
        {
            RoadFlow.Model.UserFileShare share = this.Get(fileId, userId);
            if ((share == null) && !fileId1.IsNullOrWhiteSpace())
            {
                share = this.Get(fileId1, userId);
            }
            return ((share != null) && (share.ExpireDate > DateTimeExtensions.Now));
        }

        public int Share(string dirs, string members, Guid shareUerId, DateTime expireDate)
        {
            if (dirs.IsNullOrWhiteSpace())
            {
                return 0;
            }
            char[] separator = new char[] { ',' };
            List<RoadFlow.Model.User> allUsers = new Organize().GetAllUsers(members);
            DateTime now = DateTimeExtensions.Now;
            int num = 0;
            foreach (string str in dirs.Split(separator))
            {
                if (!str.IsNullOrWhiteSpace())//不显示自己共享
                {
                    List<RoadFlow.Model.UserFileShare> list2 = new List<RoadFlow.Model.UserFileShare>();
                    foreach (RoadFlow.Model.User user in allUsers)
                    {
                        if (user.Id != shareUerId)
                        {
                            RoadFlow.Model.UserFileShare share = new RoadFlow.Model.UserFileShare
                            {

                                FileId = str,
                                ShareDate = now,
                                FileName = Path.GetFileName(str.DESDecrypt()),
                                ShareUserId = shareUerId,
                                UserId = user.Id,
                                ExpireDate = expireDate,
                                IsView = 0
                            };
                            list2.Add(share);
                        }
                    }
                    num += this.userFileShareData.Share((IEnumerable<RoadFlow.Model.UserFileShare>)list2, str);
                }
            }
            return num;
        }
    }







}
