using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_HomeSet")]
    public class HomeSet
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Required(ErrorMessage = "Id不能为空"), DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "模块类型 0上方信息提示模块 1左边模块 2右边模块不能为空"), DisplayName("模块类型 0上方信息提示模块 1左边模块 2右边模块")]
        public int Type { get; set; }

        [Required(ErrorMessage = "名称不能为空"), DisplayName("名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "显示标题不能为空"), DisplayName("显示标题")]
        public string Title { get; set; }

       


        [Required(ErrorMessage = "数据来源 0:sql,1:c#方法 2:url不能为空"), DisplayName("数据来源 0:sql,1:c#方法 2:url")]
        public int DataSourceType { get; set; }

        [DisplayName("DataSource")]
        public string DataSource { get; set; }

        [DisplayName("图标")]
        public string Ico { get; set; }


        // Properties
        [DisplayName("背景色")]
        public string BackgroundColor { get; set; }

        [DisplayName("字体色")]
        public string FontColor { get; set; }


        [DisplayName("数据连接ID")]
        public Guid? DbConnId { get; set; }

        [DisplayName("连接地址")]
        public string LinkURL { get; set; }

        [DisplayName("使用对象")]
        public string UseOrganizes { get; set; }

        [DisplayName("使用人员")]
        public string UseUsers { get; set; }

        [Required(ErrorMessage = "排序不能为空"), DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("备注")]
        public string Note { get; set; }




      

       
      

      

       

       
    }


}
