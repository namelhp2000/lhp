using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DbConnection")]
    public class DbConnection
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("连接名称"), Required(ErrorMessage = "连接名称不能为空")]
        public string Name { get; set; }

        [DisplayName("连接类型"), Required(ErrorMessage = "连接类型不能为空")]
        public string ConnType { get; set; }


        // Properties
        [DisplayName("连接字符串"), Required(ErrorMessage = "连接字符串不能为空")]
        public string ConnString { get; set; }

       
      

       

        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("排序")]
        public int Sort { get; set; }
    }


}
