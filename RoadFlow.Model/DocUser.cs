using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DocUser")]
    public class DocUser
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [DisplayName("文档id"), Key]
        public Guid DocId { get; set; }

        [DisplayName("人员id"), Key]
        public Guid UserId { get; set; }

        [DisplayName("是否已读")]
        public int IsRead { get; set; }


        [DisplayName("读取时间")]
        public int ReadTime { get; set; }

    }


}
