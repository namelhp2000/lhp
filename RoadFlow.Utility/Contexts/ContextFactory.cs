

namespace RoadFlow.Utility.Contexts
{
    /// <summary>
    /// 上下文工厂
    /// </summary>
    public static class ContextFactory
    {
        /// <summary>
        /// 创建上下文
        /// </summary>
        public static IContext Create()
        {
            if (WebHelper.HttpContext == null)
                return NullContext.Instance;
            return new WebContext();
        }
    }
}
