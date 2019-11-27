using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_FlowButton")]
    public class FlowButton
    {
        // Methods
        public FlowButton Clone()
        {
            return (FlowButton)base.MemberwiseClone();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("按钮标题"), Required(ErrorMessage = "按钮标题不能为空")]
        public string Title { get; set; }

        // Properties
        [DisplayName("按钮图标")]
        public string Ico { get; set; }

        [DisplayName("按钮脚本")]
        public string Script { get; set; }

        [DisplayName("备注说明")]
        public string Note { get; set; }

      

        [DisplayName("排序")]
        public int Sort { get; set; }

    }


}
