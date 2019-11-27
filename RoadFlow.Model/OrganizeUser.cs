using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_OrganizeUser")]
    public class OrganizeUser
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        [Key]
        public Guid Id { get; set; }
        [DisplayName("组织机构Id"), Required]
        public Guid OrganizeId { get; set; }


        [DisplayName("人员Id"), Required]
        public Guid UserId { get; set; }
        // Properties
      
        [DisplayName("是否主要")]
        public int IsMain { get; set; }

       

        [DisplayName("排序")]
        public int Sort { get; set; }

       
    }


}
