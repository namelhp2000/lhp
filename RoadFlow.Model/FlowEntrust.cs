using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowEntrust")]
    public class FlowEntrust
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [DisplayName("委托人")]
        public Guid UserId { get; set; }
        [DisplayName("开始时间")]
        public DateTime StartTime { get; set; }


        // Properties
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }

        [DisplayName("委托流程ID,为空表示所有流程")]
        public Guid? FlowId { get; set; }

        [DisplayName("被委托人")]
        public string ToUserId { get; set; }

        [DisplayName("设置时间")]
        public DateTime WriteTime { get; set; }

        [DisplayName("备注说明")]
        public string Note { get; set; }

       

      

       

        
    }


}
