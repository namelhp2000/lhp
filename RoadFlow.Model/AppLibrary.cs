using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreeSql.DataAnnotations;

namespace RoadFlow.Model
{
    [FreeSql.DataAnnotations.Table(Name = "RF_AppLibrary")]
    [Serializable, System.ComponentModel.DataAnnotations.Schema.Table("RF_AppLibrary")]
    public class AppLibrary
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }


        [DisplayName("标题"), Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }

     

        //  Properties
        [DisplayName("地址"), Required(ErrorMessage = "地址不能为空")]
        public string Address { get; set; }

        [DisplayName("分类ID"), Required(ErrorMessage = "分类不能为空")]
        public Guid Type { get; set; }

        [DisplayName("打开方式{0-默认,1-弹出模态窗口,2-弹出窗口,3-新窗口}")]
        public int OpenMode { get; set; }

        [DisplayName("弹出窗口宽度")]
        public int? Width { get; set; }


        [DisplayName("弹出窗口高度")]
        public int? Height { get; set; }


        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("唯一标识符，流程应用时为流程ID，表单应用时对应表单ID")]
        public string Code { get; set; }




        public string Title_en { get; set; }
        public string Title_zh { get; set; }










    }


}
