using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Flow")]
    public class Flow
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [DisplayName("名称"), Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }
        [DisplayName("分类"), Required(ErrorMessage = "分类不能为空")]
        public Guid FlowType { get; set; }
        [DisplayName("管理人员")]
        public string Manager { get; set; }

        [DisplayName("实例管理人员")]
        public string InstanceManager { get; set; }




        // Properties
        [DisplayName("创建日期")]
        public DateTime CreateDate { get; set; }





        [DisplayName("创建人员")]
        public Guid CreateUser { get; set; }




        [DisplayName("设计时JSON")]
        public string DesignerJSON { get; set; }


        [DisplayName("运行时JSON")]
        public string RunJSON { get; set; }




        [DisplayName("安装日期")]
        public DateTime? InstallDate { get; set; }

        [DisplayName("安装人员")]
        public Guid? InstallUser { get; set; }


        [DisplayName("状态 0设计中 1已安装 2已卸载 3已删除")]
        public int Status { get; set; }



        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("所属系统Id")]
        public Guid? SystemId { get; set; }



    }


}
