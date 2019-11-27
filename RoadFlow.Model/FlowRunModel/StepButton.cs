using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepButton
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        public Guid Id { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 显示标题
        /// </summary>
        public string ShowTitle { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }


}
