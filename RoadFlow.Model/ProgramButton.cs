using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_ProgramButton")]
    public class ProgramButton
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }



        [Required, DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [Required, DisplayName("程序ID")]
        public Guid ProgramId { get; set; }
        // Properties
        [DisplayName("系统按钮库ID")]
        public Guid? ButtonId { get; set; }

        [Required, DisplayName("名称")]
        public string ButtonName { get; set; }




        [DisplayName("脚本")]
        public string ClientScript { get; set; }

        [DisplayName("图标")]
        public string Ico { get; set; }
        [Required, DisplayName("显示类型 0工具栏按钮 1普通按钮 2列表按键")]
        public int ShowType { get; set; }

        [Required, DisplayName("序号")]
        public int Sort { get; set; }

        [Required, DisplayName("是否验证权限")]
        public int IsValidateShow { get; set; }

        

     
    }


}
