using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_WorkDate")]
    public class WorkDate
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [DisplayName("WorkDay"), Key]
        public DateTime WorkDay { get; set; }
        // Properties
        public int IsWork { get; set; }

      
    }


}
