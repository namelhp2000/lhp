using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 数据库
    /// </summary>
    public class Database
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 连接Id
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectionName { get; set; }

        public string PrimaryKey { get; set; }

        /// <summary>
        /// 表
        /// </summary>
        public string Table { get; set; }
    }


}
