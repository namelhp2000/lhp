using AspectCore.DynamicProxy.Parameters;
using RoadFlow.Utility.Aspects.Base;
using System;
using System.Threading.Tasks;

namespace RoadFlow.Utility.Aspects
{
    /// <summary>
    /// 验证不能为空
    /// </summary>
    public class NotEmptyAttribute : ParameterInterceptorBase
    {
        /// <summary>
        /// 执行
        /// </summary>
        public override Task Invoke(ParameterAspectContext context, ParameterAspectDelegate next)
        {
            if (string.IsNullOrWhiteSpace(context.Parameter.Value.SafeString()))
                throw new ArgumentNullException(context.Parameter.Name);
            return next(context);
        }
    }
}
