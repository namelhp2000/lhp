using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 事件参数
    /// </summary>
    public class EventParam
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 流程Id
        /// </summary>
        public Guid FlowId { get; set; }
        public RoadFlow.Model.FlowRun FlowRunModel { get; set; }
        /// <summary>
        /// 组Id
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 实例Id
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public object Other { get; set; }

        /// <summary>
        /// 步骤Id
        /// </summary>
        public Guid StepId { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        public string TaskTitle { get; set; }


      

    }


}
