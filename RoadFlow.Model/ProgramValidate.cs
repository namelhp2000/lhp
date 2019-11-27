using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_ProgramValidate")]
    public class ProgramValidate
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
        [Required(ErrorMessage = "表名不能为空"), DisplayName("表名")]
        public string TableName { get; set; }

        // Properties
        [Required(ErrorMessage = "字段名不能为空"), DisplayName("字段名")]
        public string FieldName { get; set; }

        [DisplayName("字段说明")]
        public string FieldNote { get; set; }

        
    

        

        [Required(ErrorMessage = "验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查不能为空"), DisplayName("验证类型 0不检查 1允许为空,非空时检查 2不允许为空,并检查")]
        public int Validate { get; set; }

        public int Status { get; set; }
    }


}
