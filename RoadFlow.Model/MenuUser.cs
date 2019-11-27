using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_MenuUser")]
    public class MenuUser
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("菜单ID"), Required(ErrorMessage = "菜单ID不能为空")]
        public Guid MenuId { get; set; }
        [DisplayName("使用对象（组织机构ID）")]
        public string Organizes { get; set; }

        [DisplayName("使用人员，人员ID")]
        public string Users { get; set; }

        // Properties
        [DisplayName("可使用的按钮")]
        public string Buttons { get; set; }

       

       

        [DisplayName("参数")]
        public string Params { get; set; }

       
    }


}
