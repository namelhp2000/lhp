using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_Doc")]
    public class Doc
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }
        [DisplayName("栏目Id")]
        public Guid DirId { get; set; }
        [DisplayName("栏目名称")]
        public string DirName { get; set; }
        [DisplayName("标题"), Required(ErrorMessage = "标题不能为空")]
        public string Title { get; set; }
        [DisplayName("来源")]
        public string Source { get; set; }
        // Properties
        [DisplayName("内容"), Required(ErrorMessage = "内容不能为空")]
        public string Contents { get; set; }
        [DisplayName("相关附件")]
        public string Files { get; set; }

        [DisplayName("添加时间")]
        public DateTime WriteTime { get; set; }

        [DisplayName("添加人员Id")]
        public Guid WriteUserID { get; set; }

        [DisplayName("添加人员姓名")]
        public string WriteUserName { get; set; }

        [DisplayName("最后修改时间")]
        public DateTime? EditTime { get; set; }







        [DisplayName("修改用户ID")]
        public Guid? EditUserID { get; set; }

        [DisplayName("修改人姓名")]
        public string EditUserName { get; set; }

        [DisplayName("阅读人员")]
        public string ReadUsers { get; set; }


        [DisplayName("阅读次数")]
        public int ReadCount { get; set; }




        [DisplayName("文档等级 0普通 1重要 2非常重要")]
        public int DocRank { get; set; }

      
       
       
       

     

       
    }



}
