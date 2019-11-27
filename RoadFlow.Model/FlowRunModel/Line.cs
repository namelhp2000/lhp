using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 连接线
    /// </summary>
    public class Line
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 自定义方法
        /// </summary>
        public string CustomMethod { get; set; }

        /// <summary>
        /// 表单Id
        /// </summary>
        public Guid FromId { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 组织表达
        /// </summary>
        public string OrganizeExpression { get; set; }

        /// <summary>
        /// sql条件语句
        /// </summary>
        public string SqlWhere { get; set; }

        /// <summary>
        /// 连线到Id
        /// </summary>
        public Guid ToId { get; set; }

        public int JudgeType { get; set; }

    }


}
