
using Newtonsoft.Json;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;



namespace RoadFlow.Model.FlowRunModel
{
    /// <summary>
    /// 执行
    /// </summary>
    public class Execute
    {

        // Fields
        /// <summary>
        /// 步骤Id、接收用户，完成时间
        /// </summary>
        //[CompilerGenerated]
      // private List<ValueTuple<Guid, List<User>, DateTime?>>  k__BackingField_Steps;


      //  [TupleElementNames(new string[] { "stepId", "stepName", "beforeStepId", "parallelOrSerial", "receiveUsers", "completedTime" }), CompilerGenerated]
       private List<ValueTuple<Guid, string, Guid?, int?, List<User>, DateTime?>> k__BackingField_Steps;



        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        public string Attachment { get; set; }

        // Properties
        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 执行类型
        /// </summary>
        public OperationType ExecuteType { get; set; }

        /// <summary>
        /// 流程Id
        /// </summary>
        public Guid FlowId { get; set; }

        /// <summary>
        /// 组Id
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 实例Id
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// 是否自动提交
        /// </summary>
        public bool IsAutoSubmit { get; set; }

        /// <summary>
        /// 是否标志
        /// </summary>
        public int IsSign { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 其他类型
        /// </summary>
        public int OtherType { get; set; }

        /// <summary>
        /// Json参数
        /// </summary>
        public string ParamsJSON { get; set; }

        /// <summary>
        /// 发送者
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// 步骤Id
        /// </summary>
        public Guid StepId { get; set; }




        // [TupleElementNames(new string[] { "stepId", "stepName", "beforeStepId", "parallelOrSerial", "receiveUsers", "completedTime" })]
        //   [return: TupleElementNames(new string[] { "stepId", "stepName", "beforeStepId", "parallelOrSerial", "receiveUsers", "completedTime" })] 
     //   [param: TupleElementNames(new string[] { "stepId", "stepName", "beforeStepId", "parallelOrSerial", "receiveUsers", "completedTime" })]
        public List<ValueTuple<Guid, string, Guid?, int?, List<User>, DateTime?>> Steps
        {
            [CompilerGenerated]
            get
            {
                return k__BackingField_Steps;
            }
            [CompilerGenerated]
            set
            {
                k__BackingField_Steps = value;
            }
        }



        /// <summary>
        /// 步骤id 、接收用户，完成时间
        /// </summary>
        // [return: TupleElementNames(new string[] { "stepId", "receiveUsers", "completedTime" })]
        // public List<ValueTuple<Guid, List<User>, DateTime?>> Steps {  get;  set; }


        //public List<ValueTuple<Guid, string, Guid?, int?, List<User>, DateTime?>> Steps
        //{
        //    [CompilerGenerated]
        //    get
        //    {
        //        return k__BackingField_Steps;
        //    }
        //    [CompilerGenerated]
        //    set
        //    {
        //        k__BackingField_Steps = value;
        //    }
        //}


        //   public List<ValueTuple<Guid, List<User>, DateTime?>> Steps { [return: TupleElementNames(new string[] { "stepId", "receiveUsers", "completedTime" })] get; [param: TupleElementNames(new string[] { "stepId", "receiveUsers", "completedTime" })] set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

       
    }


}
