using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public class ExecuteResult
    {
        // Methods
        public ExecuteResult()
        {
            this.NextTasks = new List<FlowTask>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 当前任务
        /// </summary>
        public FlowTask CurrentTask { get; set; }

        /// <summary>
        /// 调试消息
        /// </summary>
        public string DebugMessages { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Messages { get; set; }

        /// <summary>
        /// 下个任务列表
        /// </summary>
        public List<FlowTask> NextTasks { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public object Other { get; set; }

        /// <summary>
        /// 自动提交任务
        /// </summary>
        public List<RoadFlow.Model.FlowTask> AutoSubmitTasks { get; set; }
    }


}
