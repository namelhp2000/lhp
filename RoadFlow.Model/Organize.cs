using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_Organize")]
    public class Organize
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("父ID"), Required(ErrorMessage = "父ID不能为空")]
        public Guid ParentId { get; set; }
        [DisplayName("名称"), Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        [DisplayName("类型:1 单位 2 部门 3 岗位")]
        public int Type { get; set; }
        [DisplayName("部门或岗位领导")]
        public string Leader { get; set; }

        // Properties
        [DisplayName("分管领导")]
        public string ChargeLeader { get; set; }

        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("状态  0 正常 1 冻结")]
        public int Status { get; set; }
        [DisplayName("排序")]
        public int Sort { get; set; }

        [DisplayName("这里为了其他系统调用，比如微信")]
        public int IntId { get; set; }




        [DisplayName("科室")]
        public string Office { get; set; }







    }


}
