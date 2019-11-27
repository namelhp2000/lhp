using Newtonsoft.Json;
using RoadFlow.Model.FlowRunModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    public class FlowRun
    {
        // Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        [Key]
        public Guid Id { get; set; }


        // Properties
        public string Color { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid CreateUserId { get; set; }

        public List<Database> Databases { get; set; }

        public int Debug { get; set; }

        public string DebugUserIds { get; set; }

        public string DesignerJSON { get; set; }

        public Guid FirstStepId { get; set; }

        public string Ico { get; set; }

      

        public DateTime? InstallDate { get; set; }

        public Guid? InstallUserId { get; set; }

        public string InstanceManager { get; set; }

        public List<Line> Lines { get; set; }

        public string Manager { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public string RunJSON { get; set; }

        public int Status { get; set; }

        public List<Step> Steps { get; set; }

        public TitleField TitleField { get; set; }

        public Guid Type { get; set; }

        public Guid? SystemId { get; set; }
    }


}
