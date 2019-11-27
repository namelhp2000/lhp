using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore.Areas.Controllers.Areas.Quartz.Controllers
{
    public class HttpResultful : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            RoadFlow.Business.TaskOptions task = new RoadFlow.Business.TaskOptions();
            RoadFlow.Business.TaskLog logs = new RoadFlow.Business.TaskLog();
            DateTime dateTime = DateTime.Now;
            RoadFlow.Model.TaskOptions taskOptions = context.GetTaskOptions();
            string httpMessage = "";
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            if (taskOptions == null)
            {
                RoadFlow.Model.TaskLog logmodel1 = new RoadFlow.Model.TaskLog()
                {
                    Id = Guid.NewGuid(),
                     BeginDate=DateTime.Now,
                    TaskName = trigger.Name,
                    Msg = "未到找作业或可能被移除"

                };
                logs.Add(logmodel1);
              
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(taskOptions.ApiUrl) || taskOptions.ApiUrl == "/")
            {

                RoadFlow.Model.TaskLog logmodel1 = new RoadFlow.Model.TaskLog()
                {
                    Id = Guid.NewGuid(),
                    TaskName = trigger.Name,
                    TaskId=taskOptions.Id,
                     BeginDate=DateTime.Now,
                    Msg = "未配置url",

                };
                logs.Add(logmodel1);
              
                return Task.CompletedTask;
            }

            try
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(taskOptions.AuthKey)
                    && !string.IsNullOrEmpty(taskOptions.AuthValue))
                {
                    header.Add(taskOptions.AuthKey.Trim(), taskOptions.AuthValue.Trim());
                }

                if (taskOptions.RequestType?.ToLower() == "get")
                {
                    httpMessage = HttpHelper.HttpGetAsync(taskOptions.ApiUrl, header).Result;
                }
                else
                {
                    string postData = String.Empty;
                    if (!taskOptions.PostData.IsNullOrWhiteSpace())
                    {
                        try
                        {
                            JObject obj2 = null;
                            obj2 = JObject.Parse(taskOptions.PostData);
                            postData = obj2.ToString(0, Array.Empty<JsonConverter>());//taskOptions.PostData;//.ToJson();
                        }
                        catch (Exception ex1)
                        {

                        }
                      
                    }
                    httpMessage = HttpHelper.HttpPostAsync(taskOptions.ApiUrl, postData, null, 60, header).Result;
                }
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
            }

            try
            {
                RoadFlow.Model.TaskLog logmodel1 = new RoadFlow.Model.TaskLog()
                {
                    Id = Guid.NewGuid(),
                     TaskId= taskOptions.Id,
                      BeginDate=taskOptions.LastRunTime,
                      EndDate=DateTime.Now,
                    TaskName = taskOptions.TaskName,
                    Msg = $"{(string.IsNullOrEmpty(httpMessage) ? "OK" : httpMessage)}\r\n"

            };
                logs.Add(logmodel1);

                taskOptions.LastRunTime = DateTime.Now;
                task.Update(taskOptions);
            }
            catch (Exception)
            {
            }
            //输出到控制台上的信息
            Console.Out.WriteLineAsync(trigger.FullName + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") + " " + httpMessage);
            return Task.CompletedTask;
        }
    }
}
