using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_VoteResultUser")]
    public class VoteResultUser
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        [Key, Required(ErrorMessage = "Id不能为空"), Column("Id"), DisplayName("Id")]
        public Guid Id { get; set; }


        [Required(ErrorMessage = "VoteId不能为空"), Column("VoteId"), DisplayName("VoteId")]
        public Guid VoteId { get; set; }

        [Required(ErrorMessage = "UserId不能为空"), Column("UserId"), DisplayName("UserId")]
        public Guid UserId { get; set; }

      
    }





}
