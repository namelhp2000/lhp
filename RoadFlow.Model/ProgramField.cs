using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_ProgramField")]
    public class ProgramField
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Required(ErrorMessage = "Id不能为空"), DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "ProgramId不能为空"), DisplayName("ProgramId")]
        public Guid ProgramId { get; set; }

        [DisplayName("字段")]
        public string Field { get; set; }

        [DisplayName("显示标题")]
        public string ShowTitle { get; set; }





        // Properties
        [Required(ErrorMessage = "对齐方式不能为空"), DisplayName("对齐方式")]
        public string Align { get; set; }

        [DisplayName("宽度")]
        public string Width { get; set; }
        [Required(ErrorMessage = "显示类型不能为空"), DisplayName("0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义 100按钮列")]
        public int ShowType { get; set; }
        [DisplayName("格式化字符串")]
        public string ShowFormat { get; set; }

        [DisplayName("自定义字符串")]
        public string CustomString { get; set; }

        [DisplayName("是否可以排序(jqgrid点击列排序)")]
        public string IsSort { get; set; }




        [Required(ErrorMessage = "是否默认排序列不能为空"), DisplayName("是否默认排序列")]
        public int IsDefaultSort { get; set; }


        [Required(ErrorMessage = "排序不能为空"), DisplayName("排序")]
        public int Sort { get; set; }


        [DisplayName("运算类型")]
        public string SummaryType { get; set; }


        [DisplayName("运算显示方式")]
        public string SummaryTpl  { get; set; }

        [DisplayName("格式化")]
        public string Formatter { get; set; }

        [DisplayName("格式化选项")]
        public string FormatOptions { get; set; }

        [Required(ErrorMessage = "是否冻结不能为空"), DisplayName("是否冻结")]
        public int IsFreeze { get; set; }

        [DisplayName("排序方式")]
        public string SortWay { get; set; }

    }



}
