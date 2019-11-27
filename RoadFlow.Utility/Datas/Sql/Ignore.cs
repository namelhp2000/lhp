using System;

namespace RoadFlow.Utility.Datas.Sql.Matedatas
{
    /// <summary>
    /// 忽略生成列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
