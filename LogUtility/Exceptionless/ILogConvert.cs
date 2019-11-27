using RoadFlow.Utility.Model;
using System.Collections.Generic;

namespace LogUtility.Exceptionless {
    /// <summary>
    /// 日志转换器
    /// </summary>
    public interface ILogConvert {
        /// <summary>
        /// 转换
        /// </summary>
        List<Item> To();
    }
}
