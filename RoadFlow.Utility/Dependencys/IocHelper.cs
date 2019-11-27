using Microsoft.Extensions.DependencyInjection;
using RoadFlow.Utility.Dependencys;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RoadFlow.Utility
{
    /// <summary>
    /// Ioc容器帮助类
    /// </summary>
    public class IocHelper
    {
        #region 私有成员

        private string _lastName { get; } = "Last";
        private ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>> _mapping { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>>();

        #endregion

        #region 注册类型

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <typeparam name="TFrom">定义类型</typeparam>
        /// <typeparam name="TTo">实现类型</typeparam>
        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            RegisterType(typeof(TFrom), typeof(TTo), null);
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="name">注册名</param>
        /// <typeparam name="TFrom">定义类型</typeparam>
        /// <typeparam name="TTo">实现类型</typeparam>
        public void RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            RegisterType(typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="typeFrom">定义类型</param>
        /// <param name="typeTo">实现类型</param>
        /// <param name="name">注册名</param>
        public void RegisterType(Type typeFrom, Type typeTo, string name)
        {
            ConcurrentDictionary<string, Type> typeMapping = null;
            if (!_mapping.ContainsKey(typeFrom))
            {
                typeMapping = new ConcurrentDictionary<string, Type>();
                _mapping[typeFrom] = typeMapping;
            }
            else
                typeMapping = _mapping[typeFrom];

            if (name.IsNullOrEmpty())
                name = _lastName;

            typeMapping[name] = typeTo;
        }


        public void RegisterType(Type typeFrom, string name)
        {
            ConcurrentDictionary<string, Type> typeMapping = null;
            if (!_mapping.ContainsKey(typeFrom))
            {
                typeMapping = new ConcurrentDictionary<string, Type>();
                _mapping[typeFrom] = typeMapping;
            }
            else
                typeMapping = _mapping[typeFrom];

            if (name.IsNullOrEmpty())
                name = _lastName;

            typeMapping[name] = typeFrom;
        }


        #endregion

        #region 获取类型


        #region 构建类的容器方式   注册+实例

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public T ResolveClass<T>()
        {
            RegisterType(typeof(T), typeof(T).Name);
            return (T)Resolve(typeof(T), typeof(T).Name);
        }


        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public T ResolveClass<T>(params object[] paramters)
        {
            RegisterType(typeof(T), typeof(T).Name);
            return (T)Resolve(typeof(T), typeof(T).Name, paramters);
        }




        #endregion 

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), null);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="paramters">构造参数</param>
        /// <returns></returns>
        public T Resolve<T>(params object[] paramters)
        {
            return (T)Resolve(typeof(T), null, paramters);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">注册名</param>
        /// <returns></returns>
        public T Resolve<T>(string name)
        {
            return (T)Resolve(typeof(T), name);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">注册名</param>
        /// <param name="paramters">构造参数</param>
        /// <returns></returns>
        public T Resolve<T>(string name, params object[] paramters)
        {
            return (T)Resolve(typeof(T), name, paramters);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="typeFrom">对象类型</param>
        /// <param name="name">注册名</param>
        /// <param name="paramters">构造参数</param>
        /// <returns></returns>
        public object Resolve(Type typeFrom, string name, params object[] paramters)
        {
            if (!_mapping.ContainsKey(typeFrom))
                throw new Exception("该类型未注册！");
            var typeMapping = _mapping[typeFrom];
            if (name.IsNullOrEmpty())
                name = _lastName;
            if (!typeMapping.ContainsKey(name))
                throw new Exception("该类型实现名未注册！");

            return Activator.CreateInstance(typeMapping[name], paramters);
        }

        #endregion

        #region  容器设置
        /// <summary>
        /// 默认容器
        /// </summary>
        internal static readonly Container DefaultContainer = new Container();

        /// <summary>
        /// 创建容器
        /// </summary>
        /// <param name="configs">依赖配置</param>
        public static IContainer CreateContainer(params IConfig[] configs)
        {
            var container = new Container();
            container.Register(null, builder => builder.EnableAop(), configs);
            return container;
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">服务名称</param>
        public static List<T> CreateList<T>(string name = null)
        {
            return DefaultContainer.CreateList<T>(name);
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="type">对象类型</param>
        /// <param name="name">服务名称</param>
        public static List<T> CreateList<T>(Type type, string name = null)
        {
            return ((IEnumerable<T>)DefaultContainer.CreateList(type, name)).ToList();
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">服务名称</param>
        public static T Create<T>(string name = null)
        {
            return DefaultContainer.Create<T>(name);
        }




        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="type">对象类型</param>
        /// <param name="name">服务名称</param>
        public static T Create<T>(Type type, string name = null)
        {
            return (T)DefaultContainer.Create(type, name);
        }

        /// <summary>
        /// 作用域开始
        /// </summary>
        public static IScope BeginScope()
        {
            return DefaultContainer.BeginScope();
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="configs">依赖配置</param>
        public static void Register(params IConfig[] configs)
        {
            DefaultContainer.Register(null, builder => builder.EnableAop(), configs);
        }

        /// <summary>
        /// 注册依赖
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configs">依赖配置</param>
        public static IServiceProvider Register(IServiceCollection services, params IConfig[] configs)
        {
            return DefaultContainer.Register(services, builder => builder.EnableAop(), configs);
        }

        /// <summary>
        /// 释放容器
        /// </summary>
        public static void Dispose()
        {
            DefaultContainer.Dispose();
        }
        #endregion


    }
}
