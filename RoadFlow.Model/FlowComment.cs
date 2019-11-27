using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowComment")]
    public class FlowComment : IEqualityComparer<FlowComment>
    {
        // Methods
        public bool Equals(FlowComment u1, FlowComment u2)
        {
            return (u1.Comments == u2.Comments);
        }

        public int GetHashCode(FlowComment u)
        {
            return u.Comments.GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("意见使用人")]
        public Guid UserId { get; set; }


        // Properties
        [DisplayName("类型 0用户添加 1管理员添加")]
        public int AddType { get; set; }

        [DisplayName("意见"), Required(ErrorMessage = "意见不能为空")]
        public string Comments { get; set; }

      

        [DisplayName("排序")]
        public int Sort { get; set; }

    }


}
