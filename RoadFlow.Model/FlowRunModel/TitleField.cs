using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class TitleField
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        public Guid ConnectionId { get; set; }

        public string Field { get; set; }

        public string Table { get; set; }
        public string Value { get; set; }

    }


}
