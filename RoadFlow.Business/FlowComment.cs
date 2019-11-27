using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class FlowComment
    {
        // Fields
        private readonly RoadFlow.Data.FlowComment flowCommentData = new RoadFlow.Data.FlowComment();

        // Methods
        public int Add(RoadFlow.Model.FlowComment flowComment)
        {
            return this.flowCommentData.Add(flowComment);
        }

        public int Delete(RoadFlow.Model.FlowComment[] flowComments)
        {
            return this.flowCommentData.Delete(flowComments);
        }

        public RoadFlow.Model.FlowComment Get(Guid id)
        {
            return this.flowCommentData.GetAll().Find(delegate (RoadFlow.Model.FlowComment p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.FlowComment> GetAll()
        {
            return this.flowCommentData.GetAll();
        }

        public List<RoadFlow.Model.FlowComment> GetListByUserId(Guid userId)
        {
            return Enumerable.ToList<RoadFlow.Model.FlowComment>(Enumerable.Distinct<RoadFlow.Model.FlowComment>((IEnumerable<RoadFlow.Model.FlowComment>)this.GetAll().FindAll(delegate (RoadFlow.Model.FlowComment p) {
                if (p.UserId != Guid.Empty)
                {
                    return p.UserId == userId;
                }
                return true;
            }), new RoadFlow.Model.FlowComment()));
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.FlowComment> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.FlowComment>((IEnumerable<RoadFlow.Model.FlowComment>)all, 
                key=>key.Sort) + 5);
        }

        public string GetOptionsByUserId(Guid userId)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.FlowComment comment in this.GetListByUserId(userId))
            {
                string[] textArray1 = new string[] { "<option value=\"", comment.Comments, "\">", comment.Comments, "</option>" };
                builder.Append(string.Concat((string[])textArray1));
            }
            return builder.ToString();
        }

        public int Update(RoadFlow.Model.FlowComment flowComment)
        {
            return this.flowCommentData.Update(flowComment);
        }

      
}


}
