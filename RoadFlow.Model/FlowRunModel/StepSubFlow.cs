using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepSubFlow
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        public Guid SubFlowId { get; set; }

        public int SubFlowStrategy { get; set; }

        public int TaskType { get; set; }
    }


}
