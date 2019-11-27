using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_MessageUser")]
    public class MessageUser
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        [Required(ErrorMessage = "MessageId不能为空"), DisplayName("MessageId"), Key, Column(Order = 1)]
        public Guid MessageId { get; set; }

        [Required(ErrorMessage = "UserId不能为空"), DisplayName("UserId"), Key, Column(Order = 2)]
        public Guid UserId { get; set; }


        // Properties
        [Required(ErrorMessage = "IsRead不能为空"), DisplayName("IsRead")]
        public int IsRead { get; set; }

      

        [DisplayName("ReadTime")]
        public DateTime? ReadTime { get; set; }

        
    }


}
