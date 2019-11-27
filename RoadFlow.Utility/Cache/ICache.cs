using System;

namespace RoadFlow.Utility.Cache
{
    internal interface ICache
    {
        // Methods
        object Get(string key);
        object Insert(string key, object obj);
        object Insert(string key, object obj, DateTime expiry);
        void Remove(string key);
    }



    #region 类型定义

    /// <summary>
    /// 值信息
    /// </summary>
    public struct ValueInfoEntry
    {
        public string Value { get; set; }
        public string TypeName { get; set; }
        public TimeSpan? ExpireTime { get; set; }
        public ExpireType? ExpireType { get; set; }
    }

    #endregion

}
