using Autofac;

namespace RoadFlow.Utility
{
    public class AutofacHelper
    {

        /// <summary>
        /// 容器
        /// </summary>
        public static IContainer Container { get; set; }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return (T)Container?.Resolve(typeof(T));
        }
    }
}
