using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_WorkGroup")]
    public class WorkGroup
    {
        // Properties
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("工作组名称"), Required(ErrorMessage = "工作组名称不能为空")]
        public string Name { get; set; }
        [DisplayName("工作组成员")]
        public string Members { get; set; }
        [DisplayName("备注")]
        public string Note { get; set; }
        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("数字ID，用于微信等其它系统关联")]
        public int IntId { get; set; }

      

      

      

     
    }


}
