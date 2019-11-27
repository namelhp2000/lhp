using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_ProgramExport")]
    public class ProgramExport
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
        [DisplayName("显示列名")]
        public string ShowTitle { get; set; }



        // Properties
        [Required(ErrorMessage = "对齐方式 0左对齐 1居中 2右对齐不能为空"), DisplayName("对齐方式 0左对齐 1居中 2右对齐")]
        public string Align { get; set; }
        [DisplayName("列宽度")]
        public int? Width { get; set; }

       


        [DisplayName("显示类型 0直接输出 1序号 2日期时间 3数字 4数据字典ID显示标题  5组织机构id显示为名称 6自定义")]
        public int? ShowType { get; set; }
        [DisplayName("单元格类型：0常规 1文本 2数字 3日期时间")]
        public int? DataType { get; set; }

        [DisplayName("格式化字符串")]
        public string ShowFormat { get; set; }

        [DisplayName("自定义字符串")]
        public string CustomString { get; set; }

       
      
      

      

        

    
     

        [Required(ErrorMessage = "显示顺序不能为空"), DisplayName("显示顺序")]
        public int Sort { get; set; }

       
    }


}
