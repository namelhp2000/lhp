using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// ProgramGroup
    /// </summary>
    [Serializable]
    [Table("RF_ProgramGroup")]
    public class ProgramGroup
    {
       
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Column("Id")][Required(ErrorMessage = "Id不能为空")]
        [DisplayName("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// ProgramId
        /// </summary>
        [Column("ProgramId")][Required(ErrorMessage = "ProgramId不能为空")]
        [DisplayName("ProgramId")]
        public Guid ProgramId { get; set; }

        /// <summary>
        /// 分组字段名
        /// </summary>
        [Column("GroupField")]
        [DisplayName("分组字段名")]
        public String GroupField { get; set; }

        /// <summary>
        /// 是否显示汇总
        /// </summary>
        [Column("GroupSummary")]
        [DisplayName("是否显示汇总")]
        public int IsGroupSummary { get; set; }

        /// <summary>
        /// 是否显示分组列
        /// </summary>
        [Column("GroupColumnShow")]
        [DisplayName("是否显示分组列")]
        public int IsGroupColumnShow { get; set; }

        /// <summary>
        /// 分组表头行设置
        /// </summary>
        [Column("GroupText")]
        [DisplayName("分组表头行设置")]
        public String GroupText { get; set; }

        /// <summary>
        /// 是否折叠分组
        /// </summary>
        [Column("GroupCollapse")]
        [DisplayName("是否折叠分组")]
        public int IsGroupCollapse { get; set; }

        /// <summary>
        /// 分组排序方式
        /// </summary>
        [Column("GroupOrder")]
        [DisplayName("分组排序方式")]
        public String GroupOrder { get; set; }

        /// <summary>
        /// 分组中的数据是否排序
        /// </summary>
        [Column("GroupDataSorted")]
        [DisplayName("分组中的数据是否排序")]
        public int IsGroupDataSorted { get; set; }

        /// <summary>
        /// 显示或隐藏汇总行
        /// </summary>
        [Column("ShowSummaryOnHide")]
        [DisplayName("显示或隐藏汇总行")]
        public int IsShowSummaryOnHide { get; set; }



        /// <summary>
        /// 显示或隐藏汇总行
        /// </summary>
        [Column("Sort")]
        [DisplayName("分组排序")]
        public int Sort { get; set; }



        public override string ToString()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}
    }
}