using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_VoteItemOption")]
    public class VoteItemOption
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


        [Required(ErrorMessage = "ItemId不能为空"), Column("ItemId"), DisplayName("ItemId")]
        public Guid ItemId { get; set; }


        [Required(ErrorMessage = "选项标题不能为空"), Column("OptionTitle"), DisplayName("选项标题")]
        public string OptionTitle { get; set; }



        [Required(ErrorMessage = "输入类型不能为空"), Column("IsInput"), DisplayName("输入类型")]
        public int IsInput { get; set; }



        [Column("InputStyle"), DisplayName("输入框样式")]
        public string InputStyle { get; set; }

      
        

      

        [Required(ErrorMessage = "排序不能为空"), Column("Sort"), DisplayName("排序")]
        public int Sort { get; set; }

       
    }





}
