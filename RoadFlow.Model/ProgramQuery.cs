using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_ProgramQuery")]
    public class ProgramQuery
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
        [Required(ErrorMessage = "字段不能为空"), DisplayName("字段")]
        public string Field { get; set; }
        [DisplayName("显示名称")]
        public string ShowTitle { get; set; }
        [Required(ErrorMessage = "操作符不能为空"), DisplayName("操作符")]
        public string Operators { get; set; }
        [DisplayName("控件名称")]
        public string ControlName { get; set; }
        [Required(ErrorMessage = "输入类型不能为空"), DisplayName("输入类型 0文本 1日期 2日期范围 3日期时间 4日期时间范围 5下拉选择 6组织机构 7数据字典")]
        public int InputType { get; set; }
        [DisplayName("显示样式")]
        public string ShowStyle { get; set; }
        [Required(ErrorMessage = "显示顺序不能为空"), DisplayName("显示顺序")]
        public int Sort { get; set; }

        [DisplayName("数据来源 0.字符串表达式 1.数据字典 2.SQL")]
        public int? DataSource { get; set; }
        [DisplayName("DataSourceString")]
        public string DataSourceString { get; set; }

        public string DictValue { get; set; }

        // Properties
        [DisplayName("数据来源为SQL时的数据连接ID")]
        public string ConnId { get; set; }

       

        

        

        [DisplayName("组织机构查询时是否将选择转换为人员")]
        public int? IsQueryUsers { get; set; }

        

        public string OrgAttribute { get; set; }

        

       

       

     
    }


}
