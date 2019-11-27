using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RoadFlow.Model.FlowRunModel
{
    public class StepCopyFor
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        // Properties
        public int CopyforTime { get; set; }

        // Properties
        public string HandlerType { get; set; }

        public string MemberId { get; set; }

        public string MethodOrSql { get; set; }

        public string Steps { get; set; }
    }


}
