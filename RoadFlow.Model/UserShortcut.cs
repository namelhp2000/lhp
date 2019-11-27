using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_UserShortcut")]
    public class UserShortcut
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [Required(ErrorMessage = "Id不能为空"), DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "菜单ID不能为空"), DisplayName("菜单ID")]
        public Guid MenuId { get; set; }

        [Required(ErrorMessage = "用户ID不能为空"), DisplayName("用户ID")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "排序不能为空"), DisplayName("排序")]
        public int Sort { get; set; }

      
    }



}
