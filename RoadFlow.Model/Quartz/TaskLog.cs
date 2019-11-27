using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// TaskLog
    /// </summary>
    [Serializable]
    [Table("TaskLog")]
    public class TaskLog
    {
       
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Column("Id")][Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// TaskId
        /// </summary>
        [Column("TaskId")]
        [DisplayName("TaskId")]
        public Guid? TaskId { get; set; }

        /// <summary>
        /// 作业名称
        /// </summary>
        [Column("TaskName")]
        [DisplayName("作业名称")]
        public String TaskName { get; set; }

        /// <summary>
        /// 作业开始时间
        /// </summary>
        [Column("BeginDate")]
        [DisplayName("作业开始时间")]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 作业结束时间
        /// </summary>
        [Column("EndDate")]
        [DisplayName("作业结束时间")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 作业信息
        /// </summary>
        [Column("Msg")]
        [DisplayName("作业信息")]
        public String Msg { get; set; }

        public override string ToString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
    }
}