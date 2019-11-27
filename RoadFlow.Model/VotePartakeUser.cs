using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_VotePartakeUser")]
    public class VotePartakeUser
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

        [Required(ErrorMessage = "UserId不能为空"), Column("UserId"), DisplayName("UserId")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "用户姓名不能为空"), Column("UserName"), DisplayName("用户姓名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "用户所在组织不能为空"), Column("UserOrganize"), DisplayName("用户所在组织")]
        public string UserOrganize { get; set; }


        [Required(ErrorMessage = "状态不能为空"), Column("Status"), DisplayName("状态")]
        public int Status { get; set; }

        [Column("SubmitTime"), DisplayName("提交时间")]
        public DateTime? SubmitTime { get; set; }

      

      

       

       
    }






}
