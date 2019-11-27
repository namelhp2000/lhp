using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_FlowApiSystem")]
    public class FlowApiSystem
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "系统名称不能为空"), Column("Name"), DisplayName("系统名称")]
        public string Name { get; set; }


        [Required(ErrorMessage = "系统标识不能为空"), Column("SystemCode"), DisplayName("系统标识")]
        public string SystemCode { get; set; }


        [Required(ErrorMessage = "调用IP不能为空"), Column("SystemIP"), DisplayName("调用IP")]
        public string SystemIP { get; set; }

        [Column("Note"), DisplayName("备注")]
        public string Note { get; set; }

        [Required(ErrorMessage = "排序不能为空"), Column("Sort"), DisplayName("排序")]
        public int Sort { get; set; }

       

       
    }

}

