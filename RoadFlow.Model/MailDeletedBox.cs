using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    #region   流程2.8.5

    [Serializable, Table("RF_MailDeletedBox")]
    public class MailDeletedBox
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }


        [Required(ErrorMessage = "主题不能为空"), Column("Subject"), DisplayName("主题")]
        public string Subject { get; set; }

        [Column("SubjectColor"), DisplayName("主题颜色")]
        public string SubjectColor { get; set; }

        [Required(ErrorMessage = "用户ID不能为空"), Column("UserId"), DisplayName("用户ID")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "发送人ID不能为空"), Column("SendUserId"), DisplayName("发送人ID")]
        public Guid SendUserId { get; set; }

        [Required(ErrorMessage = "发送时间不能为空"), Column("SendDateTime"), DisplayName("发送时间")]
        public DateTime SendDateTime { get; set; }


        // Properties
        [Required(ErrorMessage = "内容ID不能为空"), Column("ContentsId"), DisplayName("内容ID")]
        public Guid ContentsId { get; set; }

       

        [Required(ErrorMessage = "是否查看不能为空"), Column("IsRead"), DisplayName("是否查看")]
        public int IsRead { get; set; }

      

        [Column("ReadDateTime"), DisplayName("查看时间")]
        public DateTime? ReadDateTime { get; set; }

        [Column("OutBoxId"), DisplayName("发件ID")]
        public Guid OutBoxId { get; set; }








    }



    #endregion


}
