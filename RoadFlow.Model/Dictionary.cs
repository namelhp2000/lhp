using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_Dictionary")]
    public class Dictionary
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("上级ID"), Required(ErrorMessage = "上级Id不能为空")]
        public Guid ParentId { get; set; }

        [DisplayName("标题"), Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }


        // Properties
        [DisplayName("唯一代码")]
        public string Code { get; set; }

        [DisplayName("值")]
        public string Value { get; set; }



        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("其它信息")]
        public string Other { get; set; }

      

        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("0 正常 1 删除")]
        public int Status { get; set; }


        public string Title_en { get; set; }
        public string Title_zh { get; set; }


    }


}
