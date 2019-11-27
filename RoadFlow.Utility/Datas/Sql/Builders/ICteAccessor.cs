using RoadFlow.Utility.Datas.Sql.Builders.Core;
using System.Collections.Generic;

namespace RoadFlow.Utility.Datas.Sql.Builders
{
    /// <summary>
    /// 公用表表达式CTE操作访问器
    /// </summary>
    public interface ICteAccessor
    {
        /// <summary>
        /// 公用表表达式CTE集合
        /// </summary>
        List<BuilderItem> CteItems { get; }
    }
}
