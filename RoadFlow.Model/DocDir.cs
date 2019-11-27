using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Table("RF_DocDir")]
    public class DocDir
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("上级Id")]
        public Guid ParentId { get; set; }

        [DisplayName("栏目名称")]
        public string Name { get; set; }

        [DisplayName("阅读人员")]
        public string ReadUsers { get; set; }



        [DisplayName("管理人员")]
        public string ManageUsers { get; set; }

       
     

        [DisplayName("发布人员")]
        public string PublishUsers { get; set; }

      

        [DisplayName("排序")]
        public int Sort { get; set; }
    }


}
