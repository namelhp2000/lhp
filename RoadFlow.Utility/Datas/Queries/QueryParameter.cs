using RoadFlow.Utility.Domains.Repositories;
using RoadFlow.Utility.Ui.Attributes;

namespace RoadFlow.Utility.Datas.Queries
{
    /// <summary>
    /// 查询参数
    /// </summary>
    [Model("queryParam")]
    public class QueryParameter : Pager, IQueryParameter
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }
    }
}
