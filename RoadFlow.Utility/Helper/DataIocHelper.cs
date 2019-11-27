namespace RoadFlow.Utility
{
    public class DataIocHelper
    {

        static DataIocHelper()
        {
            _container = new IocHelper();

        }

        private static IocHelper _container { get; }

        /// <summary>
        /// 针对接口，并且注册周期接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DataIoc<T>(string name = null)
        {
            return RoadFlow.Utility.IocHelper.Create<T>(name);
        }


        /// <summary>
        /// 通过容器设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DataIoc1<T>()
        {
            return _container.ResolveClass<T>();
        }

        /// <summary>
        /// 通过容器设置 类不能是抽象类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DataIoc1<T>(params object[] paramters)
        {
            return _container.ResolveClass<T>(paramters);
        }


        /// <summary>
        /// 通过映射动态获取对应的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FactoryData<T>()
        {
            return ReflectionHelper.CreateInstance<T>(typeof(T));
        }

    }
}
