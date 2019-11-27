using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_Log")]
    public class Log
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
        [DisplayName("类型"), Required(ErrorMessage = "类型不能为空")]
        public string Type { get; set; }
        [DisplayName("写入时间"), Required(ErrorMessage = "写入时间不能为空"), DataType(DataType.DateTime)]
        public DateTime WriteTime { get; set; }
        [DisplayName("用户ID")]
        public Guid? UserId { get; set; }

        [DisplayName("用户姓名")]
        public string UserName { get; set; }
        [DisplayName("IP")]
        public string IPAddress { get; set; }


        public string Referer { get; set; }
        [DisplayName("发生URL")]
        public string URL { get; set; }
        [DisplayName("内容")]
        public string Contents { get; set; }

        [DisplayName("其它")]
        public string Others { get; set; }

        [DisplayName("更改后")]
        public string NewContents { get; set; }

        [DisplayName("更改前")]
        public string OldContents { get; set; }


        // Properties
        [DisplayName("浏览器信息")]
        public string BrowseAgent { get; set; }



        [DisplayName("城市地址")]
        public string CityAddress { get; set; }

















    }


}
