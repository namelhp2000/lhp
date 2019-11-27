using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_UserFileShare")]
    public class UserFileShare
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        [Key, Required(ErrorMessage = "文件ID不能为空"), Column("FileId"), DisplayName("文件ID")]
        public string FileId { get; set; }
        [Key, Required(ErrorMessage = "人员ID不能为空"), Column("UserId"), DisplayName("人员ID")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "文件名称不能为空"), Column("FileName"), DisplayName("文件名称")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "分享的人员ID不能为空"), Column("ShareUserId"), DisplayName("分享的人员ID")]
        public Guid ShareUserId { get; set; }

        [Required(ErrorMessage = "分享日期不能为空"), Column("ShareDate"), DisplayName("分享日期")]
        public DateTime ShareDate { get; set; }
        // Properties
        [Required(ErrorMessage = "过期时间不能为空"), Column("ExpireDate"), DisplayName("过期时间")]
        public DateTime ExpireDate { get; set; }

       

       

        [Required(ErrorMessage = "是否查看不能为空"), Column("IsView"), DisplayName("是否查看")]
        public int IsView { get; set; }

      

      

       
    }




}
