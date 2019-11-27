using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Program")]
    public class Program
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [DisplayName("应用名称"), Required(ErrorMessage = "应用名称不能为空")]
        public string Name { get; set; }
        [Required, DisplayName("分类")]
        public Guid Type { get; set; }
        [Required, DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }
        [DisplayName("发布时间")]
        public DateTime? PublishTime { get; set; }

        [Required, DisplayName("创建人")]
        public Guid CreateUserId { get; set; }
        [Required, DisplayName("查询SQL")]
        public string SqlString { get; set; }

        [Required, DisplayName("是否显示新增按钮")]
        public int IsAdd { get; set; }

        [Required, DisplayName("数据连接ID")]
        public Guid ConnId { get; set; }

        [Required, DisplayName("状态 0设计中 1已发布 2已作废")]
        public int Status { get; set; }

        [DisplayName("表单ID")]
        public string FormId { get; set; }
        [DisplayName("编辑模式 0，当前窗口 1，弹出层 2，弹出窗口")]
        public int? EditModel { get; set; }
        [DisplayName("弹出层宽度")]
        public string Width { get; set; }
        [DisplayName("弹出层高度")]
        public string Height { get; set; }
        // Properties
        [DisplayName("按钮显示位置 0新行 1查询后面")]
        public int ButtonLocation { get; set; }


        [DisplayName("是否分页")]
        public int IsPager { get; set; }
        public int SelectColumn { get; set; }

        public int RowNumber { get; set; }

        [DisplayName("页面脚本")]
        public string ClientScript { get; set; }

        [DisplayName("导出EXCEL模板")]
        public string ExportTemplate { get; set; }
        [DisplayName("导出Excel表头")]
        public string ExportHeaderText { get; set; }
        [DisplayName("导出EXCLE的文件名")]
        public string ExportFileName { get; set; }


        [DisplayName("列表样式")]
        public string TableStyle { get; set; }

        [DisplayName("列表表头HTML")]
        public string TableHead { get; set; }



        [DisplayName("导入EXCEL数据时的标识字段，每次导入生成一个编号区分")]
        public string InDataNumberFiledName { get; set; }

        public string GroupHeaders { get; set; }



        [DisplayName("是否分组")]
        public int IsGroup { get; set; }


        [DisplayName("是否汇总合计")]
        public int IsFooterrow { get; set; }




        /// <summary>
        /// 默认排序
        /// </summary>
        public string DefaultSort { get; set; }











    }


}
