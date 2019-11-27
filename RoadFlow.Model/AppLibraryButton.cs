using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_AppLibraryButton")]
    public class AppLibraryButton
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }




        // Properties
        [DisplayName("AppLibraryId"), Required(ErrorMessage = "应用程序库ID不能为空")]
        public Guid AppLibraryId { get; set; }




        [DisplayName("按钮库按钮ID")]
        public Guid? ButtonId { get; set; }

        [DisplayName("名称"), Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        [DisplayName("执行脚本")]
        public string Events { get; set; }

        [DisplayName("图标")]
        public string Ico { get; set; }


        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("显示类型 0工具栏按钮 1普通按钮 2列表按键")]
        public int ShowType { get; set; }

        [DisplayName("是否验证权限")]
        public int IsValidateShow { get; set; }

       

       

       
    }


}
