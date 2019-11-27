using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    #region   流程2.8.5
    [Serializable, Table("RF_MailContent")]
    public class MailContent
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }
        // Properties
        [Required(ErrorMessage = "邮件内容不能为空"), Column("Contents"), DisplayName("邮件内容")]
        public string Contents { get; set; }

        [Column("Files"), DisplayName("附件")]
        public string Files { get; set; }

      
    }

    #endregion

}
