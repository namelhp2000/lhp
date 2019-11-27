using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_VoteItem")]
    public class VoteItem
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "VoteId不能为空"), Column("VoteId"), DisplayName("VoteId")]
        public Guid VoteId { get; set; }

        [Required(ErrorMessage = "标题不能为空"), Column("ItemTitle"), DisplayName("标题")]
        public string ItemTitle { get; set; }

        [Required(ErrorMessage = "选择方式 0 不是选择 1 单选 2多选不能为空"), Column("SelectModel"), DisplayName("选择方式 0 不是选择 1 单选 2多选")]
        public int SelectModel { get; set; }

        [Required(ErrorMessage = "排序不能为空"), Column("Sort"), DisplayName("排序")]
        public int Sort { get; set; }

        
    }






}
