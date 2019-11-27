using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    #region   流程2.8.5

    [Serializable, Table("RF_MailOutBox")]
    public class MailOutBox
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Subject不能为空"), Column("Subject"), DisplayName("Subject")]
        public string Subject { get; set; }

        [Column("SubjectColor"), DisplayName("主题颜色")]
        public string SubjectColor { get; set; }

        [Required(ErrorMessage = "发送人ID不能为空"), Column("UserId"), DisplayName("发送人ID")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "接收人员(组织机构ID字符串)不能为空"), Column("ReceiveUsers"), DisplayName("接收人员(组织机构ID字符串)")]
        public string ReceiveUsers { get; set; }

        [Required(ErrorMessage = "发送时间不能为空"), Column("SendDateTime"), DisplayName("发送时间")]
        public DateTime SendDateTime { get; set; }

        // Properties
        [Required(ErrorMessage = "邮件内容ID不能为空"), Column("ContentsId"), DisplayName("邮件内容ID")]
        public Guid ContentsId { get; set; }

       

     

       

        [Required(ErrorMessage = "0 草稿 1已发送不能为空"), Column("Status"), DisplayName("0 草稿 1已发送")]
        public int Status { get; set; }


        [Column("CarbonCopy"), DisplayName("抄送")]
        public string CarbonCopy { get; set; }

        [Column("SecretCopy"), DisplayName("密送")]
        public string SecretCopy { get; set; }



    }





    #endregion

}
