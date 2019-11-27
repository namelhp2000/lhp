using Quartz;
using RoadFlow.Utility;
using RoadFlow.Utility.Dependencys;
using RoadFlow.Utility.Schedulers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qz = Quartz;

namespace SchedulersUtility
{
    /// <summary>
    /// Quartz作业
    /// </summary>
    public abstract class JobBase : RoadFlow.Utility.Schedulers.IJob, Qz.IJob
    {
        /// <summary>
        /// 作业名称
        /// </summary>
        private readonly string _jobName;
        /// <summary>
        /// 触发器名称
        /// </summary>
        private readonly string _triggerName;
        /// <summary>
        /// 组名称
        /// </summary>
        private readonly string _groupName;

        /// <summary>
        /// 初始化
        /// </summary>
        protected JobBase()
        {
            _jobName = IdHelper.Guid();
            _triggerName = IdHelper.Guid();
            _groupName = IdHelper.Guid();
        }

        /// <summary>
        /// 获取作业名称
        /// </summary>
        public virtual string GetJobName()
        {
            return _jobName;
        }

        /// <summary>
        /// 获取触发器名称
        /// </summary>
        public virtual string GetTriggerName()
        {
            return _triggerName;
        }

        /// <summary>
        /// 获取组名称
        /// </summary>
        public virtual string GetGroupName()
        {
            return _groupName;
        }

        /// <summary>
        /// 获取Cron表达式
        /// </summary>
        public virtual string GetCron()
        {
            return null;
        }

        /// <summary>
        /// 获取重复执行次数，默认返回null，表示持续重复执行
        /// </summary>
        public virtual int? GetRepeatCount()
        {
            return null;
        }

        /// <summary>
        /// 获取开始执行时间
        /// </summary>
        public virtual DateTimeOffset? GetStartTime()
        {
            return null;
        }

        /// <summary>
        /// 获取结束执行时间
        /// </summary>
        public virtual DateTimeOffset? GetEndTime()
        {
            return null;
        }

        /// <summary>
        /// 获取重复执行间隔时间
        /// </summary>
        public virtual TimeSpan? GetInterval()
        {
            return null;
        }

        /// <summary>
        /// 获取重复执行间隔时间，单位：小时
        /// </summary>
        public virtual int? GetIntervalInHours()
        {
            return null;
        }

        /// <summary>
        /// 获取重复执行间隔时间，单位：分
        /// </summary>
        public virtual int? GetIntervalInMinutes()
        {
            return null;
        }

        /// <summary>
        /// 获取重复执行间隔时间，单位：秒
        /// </summary>
        public virtual int? GetIntervalInSeconds()
        {
            return null;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context">执行上下文</param>
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = IocHelper.BeginScope())
            {
                await Execute(context, scope);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="scope">作用域</param>
        protected abstract Task Execute(IJobExecutionContext context, IScope scope);
    }
}
