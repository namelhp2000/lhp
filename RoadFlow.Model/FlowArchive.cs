using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowArchive")]
    public class FlowArchive
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [Required, DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [Required, DisplayName("流程ID")]
        public Guid FlowId { get; set; }
        [Required, DisplayName("步骤")]
        public Guid StepId { get; set; }
        [Required, DisplayName("流程名称")]
        public string FlowName { get; set; }
        [Required, DisplayName("步骤名称")]
        public string StepName { get; set; }
        [Required, DisplayName("任务ID")]
        public Guid TaskId { get; set; }
        [Required, DisplayName("组")]
        public Guid GroupId { get; set; }
        [Required, DisplayName("实例ID")]
        public string InstanceId { get; set; }
        [Required, DisplayName("标题")]
        public string Title { get; set; }
        [Required, DisplayName("用户ID")]
        public string UserId { get; set; }
        [Required, DisplayName("用户名称")]
        public string UserName { get; set; }

        [Required, DisplayName("数据")]
        public string DataJson { get; set; }


        // Properties
        [DisplayName("处理意见HTML")]
        public string Comments { get; set; }

        [Required, DisplayName("写入时间")]
        public DateTime WriteTime { get; set; }


        public string FormHtml { get; set; }





















    }



}
