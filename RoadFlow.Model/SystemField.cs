using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_SystemField")]
    public class SystemField
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Required(ErrorMessage = "Id不能为空"), DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "表名不能为空"), DisplayName("系统表名")]
        public string SystemTable { get; set; }

        [DisplayName("表名序号")]
        public int TableId { get; set; }

        [DisplayName("列名")]
        public string FieldName { get; set; }

        [DisplayName("列说明")]
        public string FieldDescribe { get; set; }


        [DisplayName("数据类型")]
        public string FieldType { get; set; }


        [DisplayName("小数位数")]
        public string FieldDecimalDigits { get; set; }

       
        [DisplayName("长度")]
        public int FieldLength { get; set; }

        [DisplayName("标识")]
        public string FieldIdentity { get; set; }


        [DisplayName("主键")]
        public string FieldPrimaryKey { get; set; }

        [DisplayName("允许空")]
        public string FieldAllowEmpty { get; set; }

        [DisplayName("默认值")]
        public string FieldDefaultValue { get; set; }

        [DisplayName("新建时间")]
        public DateTime CreateDate { get; set; }

        //[DisplayName("更新时间")]
        //public DateTime UpdateDate { get; set; }
    }



}
