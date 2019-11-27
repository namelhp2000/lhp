using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepBase
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 自动确认
        /// </summary>
        public int AutoConfirm { get; set; }

        /// <summary>
        /// 回模型
        /// </summary>
        public int BackModel { get; set; }


        public int BackSelectUser { get; set; }



        /// <summary>
        /// 退回步骤Id
        /// </summary>
        public Guid? BackStepId { get; set; }

        /// <summary>
        /// 退回类型
        /// </summary>
        public int BackType { get; set; }

        /// <summary>
        /// 并发模型
        /// </summary>
        public int ConcurrentModel { get; set; }

        /// <summary>
        /// 会签
        /// </summary>
        public int Countersignature { get; set; }

        /// <summary>
        /// 会签百分比
        /// </summary>
        public decimal CountersignaturePercentage { get; set; }


        public Guid? CountersignatureStartStepId { get; set; }


        /// <summary>
        /// 默认的处理程序
        /// </summary>
        public string DefaultHandler { get; set; }

        /// <summary>
        /// 默认的处理程序Sql或者方法
        /// </summary>
        public string DefaultHandlerSqlOrMethod { get; set; }


        /// <summary>
        /// 流类型
        /// </summary>
        public int FlowType { get; set; }

        /// <summary>
        ///处理程序步骤Id
        /// </summary>
        public Guid? HandlerStepId { get; set; }

        /// <summary>
        /// 处理器类型
        /// </summary>
        public string HandlerType { get; set; }

        /// <summary>
        /// 处理模型
        /// </summary>
        public int HanlderModel { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// 选择运行
        /// </summary>
        public int RunSelect { get; set; }

        /// <summary>
        /// 选择范围
        /// </summary>
        public string SelectRange { get; set; }

        public int SendToBackStep { get; set; }




        public int SkipIdenticalUser { get; set; }

        public string SkipMethod { get; set; }

        public int SubFlowStrategy { get; set; }

        public string ValueField { get; set; }

     

     
    }


}
