using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowTask")]
    public class FlowTask : IEqualityComparer<FlowTask>
    {
        // Methods
        public FlowTask Clone()
        {
            return (FlowTask)base.MemberwiseClone();
        }

        public bool Equals(FlowTask u1, FlowTask u2)
        {
            return (u1.Id == u2.Id);
        }

        public int GetHashCode(FlowTask u)
        {
            return u.GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }



        [DisplayName("任务ID"), Key]
        public Guid Id { get; set; }
        [DisplayName("上一任务ID")]
        public Guid PrevId { get; set; }

        [DisplayName("上一步骤ID")]
        public Guid PrevStepId { get; set; }
        [DisplayName("流程ID")]
        public Guid FlowId { get; set; }

        [DisplayName("流程名称")]
        public string FlowName { get; set; }
        [DisplayName("步骤ID")]
        public Guid StepId { get; set; }

        [DisplayName("步骤名称")]
        public string StepName { get; set; }

        [DisplayName("对应业务表主键值")]
        public string InstanceId { get; set; }
        [DisplayName("分组ID")]
        public Guid GroupId { get; set; }








     
      
        [DisplayName("任务类型 0常规 1指派 2委托 3转交 4退回 5抄送 6前加签 7后加签 8并签 9跳转")]
        public int TaskType { get; set; }

        [DisplayName("任务标题")]
        public string Title { get; set; }

        [DisplayName("发送人ID(如果是兼职岗位R_关系表ID)")]
        public Guid SenderId { get; set; }

        [DisplayName("发送人姓名")]
        public string SenderName { get; set; }

        [DisplayName("接收人ID")]
        public Guid ReceiveId { get; set; }

        [DisplayName("接收人姓名")]
        public string ReceiveName { get; set; }

        [DisplayName("接收时间")]
        public DateTime ReceiveTime { get; set; }

        [DisplayName("打开时间")]
        public DateTime? OpenTime { get; set; }
        [DisplayName("要求完成时间")]
        public DateTime? CompletedTime { get; set; }

        [DisplayName("实际完成时间")]
        public DateTime? CompletedTime1 { get; set; }

        [DisplayName("处理意见")]
        public string Comments { get; set; }

        [DisplayName("是否签章")]
        public int IsSign { get; set; }

        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("子流程实例分组ID")]
        public string SubFlowGroupId { get; set; }


        [DisplayName("是否超时自动提交 0否 1是 2自动提交失败")]
        public int IsAutoSubmit { get; set; }


        // Properties
        [DisplayName("附件")]
        public string Attachment { get; set; }







        [DisplayName("任务状态 -1等待中 0未处理 1处理中 2已完成")]
        public int Status { get; set; }



        [DisplayName("任务顺序")]
        public int Sort { get; set; }

        [DisplayName("处理类型 处理类型 -1等待中 0未处理 1处理中 2已完成 3已退回 4他人已处理 5他人已退回 6已转交 7已委托 8已阅知 9已指派 10已跳转 11已终止 12他人已终止 13已加签")]
        public int ExecuteType { get; set; }

          [DisplayName("接收人所在机构ID（如果是兼职人员的情况下这里有值）")]
        public Guid? ReceiveOrganizeId { get; set; }



        [DisplayName("一个步骤内的处理顺序(选择人员顺序处理时的处理顺序)")]
        public int StepSort { get; set; }


        [DisplayName("如果是委托任务，这里记录委托人员ID")]
        public Guid? EntrustUserId { get; set; }

        [DisplayName("其它类型")]
        public int OtherType { get; set; }








        [DisplayName("指定的后续步骤处理人")]
        public string NextStepsHandle { get; set; }



        [DisplayName("原步骤ID(动态步骤的原步骤ID)")]
        public Guid? BeforeStepId { get; set; }




        [DisplayName("提醒时间")]
        public DateTime? RemindTime { get; set; }



        [DisplayName("是否可以批量提交")]
        public int? IsBatch { get; set; }







    }


}
