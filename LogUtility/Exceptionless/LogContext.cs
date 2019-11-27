using System;
using RoadFlow.Utility;
using RoadFlow.Utility.Logs.Internal;

namespace LogUtility.Exceptionless {
    /// <summary>
    /// Exceptionless日志上下文
    /// </summary>
    public class LogContext : RoadFlow.Utility.Logs.Core.LogContext {
        /// <summary>
        /// 创建日志上下文信息
        /// </summary>
        protected override LogContextInfo CreateInfo() {
            return new LogContextInfo {
                TraceId = Guid.NewGuid().ToString(),
                Stopwatch = GetStopwatch(),
                Url = WebHelper.Url
            };
        }
    }
}
