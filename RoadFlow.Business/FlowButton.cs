using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class FlowButton
    {
        // Fields
        private readonly RoadFlow.Data.FlowButton flowButtonData = new RoadFlow.Data.FlowButton();

        // Methods
        public int Add(RoadFlow.Model.FlowButton flowButton)
        {
            return this.flowButtonData.Add(flowButton);
        }

        public int Delete(RoadFlow.Model.FlowButton[] flowButtons)
        {
            return this.flowButtonData.Delete(flowButtons);
        }

        public RoadFlow.Model.FlowButton Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.FlowButton p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.FlowButton> GetAll()
        {
            return this.flowButtonData.GetAll();
        }


        public string ButtonList()
        {
            StringBuilder builder = new StringBuilder();
            List<RoadFlow.Model.FlowButton> flowbuttons = this.GetAll();
            foreach(RoadFlow.Model.FlowButton  flowbutton in flowbuttons)
            {
                builder.Append("<ul class=\"listulli\" note=\"" + flowbutton.Note + "\" style=\"width:191px;\" title=\"" + flowbutton.Note + "\" val=\"" + flowbutton.Id + "\" onmouseover=\"$(this).removeClass().addClass('listulli1');\" onmouseout=\"if($currentButton==null || $currentButton.get(0)!==this){$(this).removeClass().addClass('listulli');}\" onclick=\"button_click(this);\" ondblclick=\"button_dblclick(this)\">");
                builder.Append("<i style = \"font-size:14px; margin-right:3px;\" class=\"fa " + flowbutton.Ico + "\"></i>");
                builder.Append("<label style = \"\" >"+flowbutton.Title+ "</label>");
                builder.Append("</ul >");
            }
            return builder.ToString();
        }


        public int GetMaxSort()
        {
            List<RoadFlow.Model.FlowButton> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.FlowButton>((IEnumerable<RoadFlow.Model.FlowButton>)all,
               key=>key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.FlowButton flowButton)
        {
            return this.flowButtonData.Update(flowButton);
        }

       
}


}
