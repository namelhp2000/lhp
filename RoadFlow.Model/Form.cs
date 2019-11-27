using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Form")]
    public class Form
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("ID"), Key]
        public Guid Id { get; set; }

        [DisplayName("表单名称"), Required]
        public string Name { get; set; }
        [DisplayName("表单分类"), Required]
        public Guid FormType { get; set; }

        [DisplayName("创建人员ID")]
        public Guid CreateUserId { get; set; }

        [DisplayName("创建人员姓名")]
        public string CreateUserName { get; set; }
        [DisplayName("创建时间")]
        public DateTime CreateDate { get; set; }
        [DisplayName("修改时间")]
        public DateTime EditDate { get; set; }


        [DisplayName("表单HTML")]
        public string Html { get; set; }

        [DisplayName("子表json")]
        public string SubtableJSON { get; set; }

        [DisplayName("事件json")]
        public string EventJSON { get; set; }


        // Properties
        [DisplayName("属性json")]
        public string attribute { get; set; }








        [DisplayName("状态：0 保存 1 编译 2作废")]
        public int Status { get; set; }




        [DisplayName("备注")]
        public string Note { get; set; }

        
        [DisplayName("生成后的HTML")]
        public string RunHtml { get; set; }


        [DisplayName("管理人员")]
        public string ManageUser { get; set; }




    }



}
