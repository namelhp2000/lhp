using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_Menu")]
    public class Menu
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("上级Id"), Required(ErrorMessage = "上级Id不能为空")]
        public Guid ParentId { get; set; }




        // Properties
        [DisplayName("应用程序库Id")]
        public Guid? AppLibraryId { get; set; }
        [DisplayName("菜单名称"), Required(ErrorMessage = "菜单名称不能为空")]
        public string Title { get; set; }

        [DisplayName("URL参数")]
        public string Params { get; set; }

        [DisplayName("图标")]
        public string Ico { get; set; }

        [DisplayName("图标颜色")]
        public string IcoColor { get; set; }

       

      

       
        [DisplayName("排序")]
        public int Sort { get; set; }

        public string Title_en { get; set; }
        public string Title_zh { get; set; }

    }


}
