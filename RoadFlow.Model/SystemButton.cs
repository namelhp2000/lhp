using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_SystemButton")]
    public class SystemButton
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [DisplayName("名称"), Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }


        // Properties
        [DisplayName("脚本")]
        public string Events { get; set; }

        [DisplayName("图标")]
        public string Ico { get; set; }

       

       

        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("排序")]
        public int Sort { get; set; }

        public string Name_en { get; set; }
        public string Name_zh { get; set; }


    }



}
