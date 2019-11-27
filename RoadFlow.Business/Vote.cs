using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class Vote
    {
        // Fields
        private readonly RoadFlow.Data.Vote voteData = new RoadFlow.Data.Vote();
        private readonly RoadFlow.Data.VoteItem voteItemData = new RoadFlow.Data.VoteItem();
        private readonly RoadFlow.Data.VoteItemOption voteItemOptionData = new RoadFlow.Data.VoteItemOption();
        private readonly RoadFlow.Data.VotePartakeUser votePartakeUserData = new RoadFlow.Data.VotePartakeUser();
        private readonly RoadFlow.Data.VoteResult voteResultData = new RoadFlow.Data.VoteResult();
        private readonly RoadFlow.Data.VoteResultUser voteResultUserData = new RoadFlow.Data.VoteResultUser();

        // Methods
        public int AddVote(RoadFlow.Model.Vote vote)
        {
            return this.voteData.Add(vote);
        }

        public int AddVoteItem(RoadFlow.Model.VoteItem voteItem)
        {
            return this.voteItemData.Add(voteItem);
        }

        public int AddVoteItemOption(RoadFlow.Model.VoteItemOption voteItemOption)
        {
            return this.voteItemOptionData.Add(voteItemOption);
        }

        public int AddVoteResults(List<RoadFlow.Model.VoteResult> voteItems)
        {
            return this.voteResultData.AddRange(voteItems);
        }

        public int DeleteVote(Guid voteId, bool isDeleteAll = true)
        {
            return this.voteData.Delete(voteId, isDeleteAll);
        }

        public int DeleteVoteItem(Guid itemId)
        {
            return this.voteItemData.Delete(itemId, true);
        }

        public List<RoadFlow.Model.VoteItemOption> GetItemOptions(Guid itemId)
        {
            return this.voteItemOptionData.GetItemOptions(itemId);
        }

        public DataTable GetPartakeUserPagerList(out int count, int size, int number, Guid voteId, string name, string org, string order)
        {
            return this.voteData.GetPartakeUserPagerList(out count, size, number, voteId, name, org, order);
        }

        public List<RoadFlow.Model.VotePartakeUser> GetPartakeUsers(Guid voteID, int status = -1)
        {
            return this.votePartakeUserData.GetPartakeUsers(voteID, status);
        }

        public DataTable GetResultPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return this.voteData.GetResultPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        public RoadFlow.Model.Vote GetVote(Guid id)
        {
            return this.voteData.Get(id);
        }

        public RoadFlow.Model.VoteItem GetVoteItem(Guid id)
        {
            return this.voteItemData.Get(id);
        }

        public int GetVoteItemMaxSort(Guid voteId)
        {
            List<RoadFlow.Model.VoteItem> voteItems = this.GetVoteItems(voteId);
            if (voteItems.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.VoteItem>((IEnumerable<RoadFlow.Model.VoteItem>)voteItems, key => key.Sort) + 5);
        }

        public RoadFlow.Model.VoteItemOption GetVoteItemOption(Guid id)
        {
            return this.voteItemOptionData.Get(id);
        }

        public List<RoadFlow.Model.VoteItemOption> GetVoteItemOptions(Guid voteId)
        {
            return this.voteItemOptionData.GetVoteItemOptions(voteId);
        }

        public List<RoadFlow.Model.VoteItem> GetVoteItems(Guid voteId)
        {
            return this.voteItemData.GetItems(voteId);
        }

        public DataTable GetVotePagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return this.voteData.GetPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        public List<RoadFlow.Model.VoteResult> GetVoteResults(Guid voteId)
        {
            return this.voteResultData.GetVoteResults(voteId);
        }

        public List<RoadFlow.Model.VoteResultUser> GetVoteResultUsers(Guid voteId)
        {
            return this.voteResultUserData.GetVoteResultUsers(voteId);
        }

        public DataTable GetWaitSubmitPagerList(out int count, int size, int number, Guid currentUserId, string topic, string date1, string date2, string order)
        {
            return this.voteData.GetWaitSubmitPagerList(out count, size, number, currentUserId, topic, date1, date2, order);
        }

        public string Publish(Guid voteId)
        {
            RoadFlow.Model.Vote vote = this.GetVote(voteId);
            if (vote == null)
            {
                return "未找到要发布的问卷!";
            }
            if (vote.Status != 0)
            {
                return "该问卷已发布!";
            }
            if (vote.PartakeUsers.IsNullOrWhiteSpace())
            {
                return "该问卷没有设置要参与的人员!";
            }
            Organize organize = new Organize();
            List<RoadFlow.Model.User> allUsers = organize.GetAllUsers(vote.PartakeUsers);
            if (allUsers.Count == 0)
            {
                return "该问卷没有要参与的人员!";
            }
            if (this.GetVoteItems(voteId).Count == 0)
            {
                return "该问卷未设置选题!";
            }
            if (vote.ResultViewUsers.IsNullOrWhiteSpace())
            {
                return "该问卷没有设置结果查看人员!";
            }
            List<RoadFlow.Model.User> list2 = organize.GetAllUsers(vote.ResultViewUsers);
            if (list2.Count == 0)
            {
                return "该问卷没有结果查看人员!";
            }
            User user = new User();
            List<RoadFlow.Model.VotePartakeUser> votePartakeUsers = new List<RoadFlow.Model.VotePartakeUser>();
            foreach (RoadFlow.Model.User user2 in allUsers)
            {
                RoadFlow.Model.VotePartakeUser user1 = new RoadFlow.Model.VotePartakeUser
                {
                    Id = Guid.NewGuid(),
                    UserId = user2.Id,
                    UserName = user2.Name,
                    UserOrganize = user.GetOrganizeMainShowHtml(user2.Id, false),
                    VoteId = voteId,
                    Status = 0
                };
                votePartakeUsers.Add(user1);
            }
            List<RoadFlow.Model.VoteResultUser> voteResultUsers = new List<RoadFlow.Model.VoteResultUser>();
            foreach (RoadFlow.Model.User user3 in list2)
            {
                RoadFlow.Model.VoteResultUser user4 = new RoadFlow.Model.VoteResultUser
                {
                    Id = Guid.NewGuid(),
                    UserId = user3.Id,
                    VoteId = voteId
                };
                voteResultUsers.Add(user4);
            }
            if (this.votePartakeUserData.Add(votePartakeUsers, voteResultUsers) <= 0)
            {
                return "发布失败!";
            }
            return "1";
        }

        public string UnPublish(Guid voteId)
        {
            RoadFlow.Model.Vote vote = this.GetVote(voteId);
            if (vote == null)
            {
                return "未找到该问卷!";
            }
            if (vote.Status > 1)
            {
                return "该问卷已有提交结果,不能取消!";
            }
            if (this.votePartakeUserData.DeleteByVoteId(voteId) <= 0)
            {
                return "取消发布失败!";
            }
            return "1";
        }

        public int UpdateVote(RoadFlow.Model.Vote vote)
        {
            return this.voteData.Update(vote);
        }

        public int UpdateVoteItem(RoadFlow.Model.VoteItem voteItem)
        {
            return this.voteItemData.Update(voteItem);
        }

        public int UpdateVoteItemOption(RoadFlow.Model.VoteItemOption voteItemOption)
        {
            return this.voteItemOptionData.Update(voteItemOption);
        }

    }
}
