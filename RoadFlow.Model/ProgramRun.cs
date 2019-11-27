using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    public class ProgramRun
    {
        // Properties
        public string Button_List { get; set; }

        public string Button_Normal { get; set; }

        public string Button_Toolbar { get; set; }

        [DisplayName("按钮显示位置 0新行 1查询后面")]
        public int ButtonLocation { get; set; }

        [DisplayName("页面脚本")]
        public string ClientScript { get; set; }

        [Required, DisplayName("数据连接ID")]
        public Guid ConnId { get; set; }

        public string CountSql { get; set; }

        [Required, DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        [Required, DisplayName("创建人")]
        public Guid CreateUserId { get; set; }

        public string DefaultSort { get; set; }

        [DisplayName("编辑模式 0，当前窗口 1，弹出层")]
        public int? EditModel { get; set; }

        [DisplayName("导出EXCLE的文件名")]
        public string ExportFileName { get; set; }

        [DisplayName("导出Excel表头")]
        public string ExportHeaderText { get; set; }

        [DisplayName("导出EXCEL模板")]
        public string ExportTemplate { get; set; }

        [DisplayName("表单ID")]
        public string FormId { get; set; }


        /// <summary>
        /// Grid字段设置
        /// </summary>
        public string GridColModels { get; set; }

        /// <summary>
        /// Grid字段名称
        /// </summary>
        public string GridColNames { get; set; }

        /// <summary>
        /// 表头处理
        /// </summary>
        public string GroupHeaders { get; set; }


        [DisplayName("弹出层高度")]
        public string Height { get; set; }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("导入EXCEL数据时的标识字段，每次导入生成一个编号区分")]
        public string InDataNumberFiledName { get; set; }

        [Required, DisplayName("是否显示新增按钮")]
        public int IsAdd { get; set; }

        [DisplayName("是否分页")]
        public int IsPager { get; set; }

        [DisplayName("应用名称"), Required(ErrorMessage = "应用名称不能为空")]
        public string Name { get; set; }




        /// <summary>
        /// 项目按钮
        /// </summary>
        public List<ProgramButton> ProgramButtons { get; set; }

        /// <summary>
        /// 项目导出
        /// </summary>
        public List<ProgramExport> ProgramExports { get; set; }

        /// <summary>
        /// 项目字段
        /// </summary>
        public List<ProgramField> ProgramFields { get; set; }

        /// <summary>
        /// 项目查询
        /// </summary>
        public List<ProgramQuery> ProgramQueries { get; set; }

        /// <summary>
        /// 项目验证
        /// </summary>
        public List<ProgramValidate> ProgramValidates { get; set; }


        /// <summary>
        /// 项目分组
        /// </summary>
        public List<ProgramGroup> ProgramGroups { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [DisplayName("发布时间")]
        public DateTime? PublishTime { get; set; }


        public string QueryData { get; set; }

        public string QueryHtml { get; set; }

        public string QuerySql { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// 选择列
        /// </summary>
        public int SelectColumn { get; set; }

        [Required, DisplayName("查询SQL")]
        public string SqlString { get; set; }

        [Required, DisplayName("状态 0设计中 1已发布 2已作废")]
        public int Status { get; set; }

        [DisplayName("列表表头HTML")]
        public string TableHead { get; set; }

        [DisplayName("列表样式")]
        public string TableStyle { get; set; }

        [Required, DisplayName("分类")]
        public Guid Type { get; set; }

        [DisplayName("弹出层宽度")]
        public string Width { get; set; }


        [DisplayName("是否分组")]
        public int IsGroup { get; set; }


        [DisplayName("是否汇总合计")]
        public int IsFooterrow { get; set; }



        [DisplayName("分组字符串")]
        public Dictionary<string ,List<string>> GridGroups_String { get; set; }
        [DisplayName("分组Bool")]
        public Dictionary<string, List<bool>> GridGroups_Bool { get; set; }



        [DisplayName("排序方式")]
        public string SortWay { get; set; }

    }


}
