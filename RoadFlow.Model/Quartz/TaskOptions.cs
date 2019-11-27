using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// TaskOptions
    /// </summary>
    [Serializable]
    [Table("TaskOptions")]
    public class TaskOptions
    {
       
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Column("Id")][Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 作业名称
        /// </summary>
        [Column("TaskName")]
        [DisplayName("作业名称")]
        public String TaskName { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        [Column("GroupName")]
        [DisplayName("分组名称")]
        public String GroupName { get; set; }

        /// <summary>
        /// 间隔
        /// </summary>
        [Column("Interval")]
        [DisplayName("间隔")]
        public String Interval { get; set; }

        /// <summary>
        /// 调用接口
        /// </summary>
        [Column("ApiUrl")]
        [DisplayName("调用接口")]
        public String ApiUrl { get; set; }

        /// <summary>
        /// 验证key
        /// </summary>
        [Column("AuthKey")]
        [DisplayName("验证key")]
        public String AuthKey { get; set; }

        /// <summary>
        /// 验证值
        /// </summary>
        [Column("AuthValue")]
        [DisplayName("验证值")]
        public String AuthValue { get; set; }

        /// <summary>
        /// 描述说明
        /// </summary>
        [Column("Describe")]
        [DisplayName("描述说明")]
        public String Describe { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        [Column("RequestType")]
        [DisplayName("请求类型")]
        public String RequestType { get; set; }

        /// <summary>
        /// 最后运行时间
        /// </summary>
        [Column("LastRunTime")]
        [DisplayName("最后运行时间")]
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        [Column("Status")]
        [DisplayName("运行状态")]
        public Int32? Status { get; set; }


        /// <summary>
        /// 请求类型
        /// </summary>
        [Column("PostData")]
        [DisplayName("Post数据")]
        public String PostData { get; set; }


        public override string ToString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
    }
}