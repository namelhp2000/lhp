using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_Vote")]
    public class Vote
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }



        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "主题不能为空"), Column("Topic"), DisplayName("主题")]
        public string Topic { get; set; }



        [Required(ErrorMessage = "发起人不能为空"), Column("CreateUserId"), DisplayName("发起人")]
        public Guid CreateUserId { get; set; }


        // Properties
        [Required(ErrorMessage = "创建时间不能为空"), Column("CreateTime"), DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }


        [Required(ErrorMessage = "参与人员不能为空"), Column("PartakeUsers"), DisplayName("参与人员")]
        public string PartakeUsers { get; set; }


        [Column("ResultViewUsers"), DisplayName("结果查看人员")]
        public string ResultViewUsers { get; set; }




        [Required(ErrorMessage = "结束时间不能为空"), Column("EndTime"), DisplayName("结束时间")]
        public DateTime? EndTime { get; set; }

    

        [Column("Note"), DisplayName("备注")]
        public string Note { get; set; }


        [Required(ErrorMessage = "状态不能为空"), Column("Status"), DisplayName("状态")]
        public int Status { get; set; }




        [Column("PublishTime"), DisplayName("发布时间")]
        public DateTime? PublishTime { get; set; }



        [Column("Anonymous"), DisplayName("是否匿名")]
        public int Anonymous { get; set; }


    }






}
