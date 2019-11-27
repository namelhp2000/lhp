using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepForm
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        // Properties
        public Guid Id { get; set; }

        public Guid MobileId { get; set; }

        public string MobileName { get; set; }

        public string Name { get; set; }
    }


}
