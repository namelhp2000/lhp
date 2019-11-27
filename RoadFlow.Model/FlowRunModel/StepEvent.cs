using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepEvent
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        public string BackAfter { get; set; }

        public string BackBefore { get; set; }

        public string SubFlowActivationBefore { get; set; }

        public string SubFlowCompletedBefore { get; set; }

        public string SubmitAfter { get; set; }

        public string SubmitBefore { get; set; }
    }


}
