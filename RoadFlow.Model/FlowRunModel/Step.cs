using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 步骤
    /// </summary>
    public class Step
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        /// <summary>
        /// 档案
        /// </summary>
        public int Archives { get; set; }

        public int Attachment { get; set; }


        /// <summary>
        /// 显示消息
        /// </summary>
        public string BackShowMessage { get; set; }

        /// <summary>
        /// 评论显示
        /// </summary>
        public int CommentDisplay { get; set; }


        public int DataEditModel { get; set; }

        public int Dynamic { get; set; }


        public string DynamicField { get; set; }


        /// <summary>
        /// 过期的执行模式
        /// </summary>
        public int ExpiredExecuteModel { get; set; }

        /// <summary>
        /// 过期的提示
        /// </summary>
        public int ExpiredPrompt { get; set; }
        public decimal ExpiredPromptDays { get; set; }


        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// X位置
        /// </summary>
        public decimal Position_X { get; set; }

        /// <summary>
        /// Y位置
        /// </summary>
        public decimal Position_Y { get; set; }

        /// <summary>
        /// 运行默认成员
        /// </summary>
        public string RunDefaultMembers { get; set; }

        /// <summary>
        /// 发送设置工作
        /// </summary>
        public int SendSetWorkTime { get; set; }

        /// <summary>
        /// 发送信息显示
        /// </summary>
        public string SendShowMessage { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        public int SignatureType { get; set; }

        /// <summary>
        /// 基本步骤
        /// </summary>
        public StepBase StepBase { get; set; }

        /// <summary>
        /// 步骤按钮
        /// </summary>
        public List<StepButton> StepButtons { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public StepCopyFor StepCopyFor { get; set; }

        /// <summary>
        /// 步骤事件
        /// </summary>
        public StepEvent StepEvent { get; set; }

        /// <summary>
        /// 步骤字段状态
        /// </summary>
        public List<StepFieldStatus> StepFieldStatuses { get; set; }

        /// <summary>
        /// 步骤表单
        /// </summary>
        public StepForm StepForm { get; set; }

        /// <summary>
        /// 步骤子流程
        /// </summary>
        public StepSubFlow StepSubFlow { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 工作时间
        /// </summary>
        public decimal WorkTime { get; set; }


        public int BatchExecute { get; set; }



    }


}
