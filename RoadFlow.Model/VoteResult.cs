using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_VoteResult")]
    public class VoteResult
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "投票ID不能为空"), Column("VoteId"), DisplayName("投票ID")]
        public Guid VoteId { get; set; }




        [Required(ErrorMessage = "选题ID不能为空"), Column("ItemId"), DisplayName("选题ID")]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "用户ID不能为空"), Column("UserId"), DisplayName("用户ID")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "选项ID不能为空"), Column("OptionId"), DisplayName("选项ID")]
        public Guid OptionId { get; set; }

        [Column("OptionOther"), DisplayName("其它说明")]
        public string OptionOther { get; set; }

      

       
    }






}
