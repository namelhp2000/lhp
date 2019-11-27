using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class FlowApiSystem
    {
        // Fields
        private readonly RoadFlow.Data.FlowApiSystem flowApiSystemData = new RoadFlow.Data.FlowApiSystem();

        // Methods
        public int Add(RoadFlow.Model.FlowApiSystem appLibrary)
        {
            return this.flowApiSystemData.Add(appLibrary);
        }

        public int Delete(RoadFlow.Model.FlowApiSystem[] flowApiSystems)
        {
            return this.flowApiSystemData.Delete(flowApiSystems);
        }

        public RoadFlow.Model.FlowApiSystem Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.FlowApiSystem p) {
                return p.Id == id;
            });
        }

        public RoadFlow.Model.FlowApiSystem Get(string systemCode)
        {
            if (systemCode.IsNullOrEmpty())
            {
                return null;
            }
            return this.GetAll().Find(delegate (RoadFlow.Model.FlowApiSystem p) {
                return p.SystemCode.EqualsIgnoreCase(systemCode);
            });
        }

        public List<RoadFlow.Model.FlowApiSystem> GetAll()
        {
            return this.flowApiSystemData.GetAll();
        }

        public string GetAllOptions(string value = "")
        {
            StringBuilder builder = new StringBuilder();
            foreach (RoadFlow.Model.FlowApiSystem system in this.GetAll())
            {
                string[] textArray1 = new string[] { "<option", (!value.IsNullOrEmpty() && value.EqualsIgnoreCase(system.Id.ToString())) ? " selected='selected'" : "", " value='", (string)system.Id.ToString(), "'>", (string)system.Name, "(", (string)system.SystemCode, ")</option>" };
                builder.Append(string.Concat((string[])textArray1));
            }
            return builder.ToString();
        }

        public Guid GetIdBySystemCode(string systemCode)
        {
            RoadFlow.Model.FlowApiSystem system = this.Get(systemCode);
            if (system != null)
            {
                return system.Id;
            }
            return Guid.Empty;
        }

        public int GetMaxSort()
        {
            List<RoadFlow.Model.FlowApiSystem> all = this.GetAll();
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.FlowApiSystem>((IEnumerable<RoadFlow.Model.FlowApiSystem>)all,
               key=>key.Sort) + 5);
        }

        public string GetName(Guid id)
        {
            if (!id.IsEmptyGuid())
            {
                RoadFlow.Model.FlowApiSystem system = this.Get(id);
                if (system != null)
                {
                    return system.Name;
                }
            }
            return string.Empty;
        }

        public int Update(RoadFlow.Model.FlowApiSystem appLibrary)
        {
            return this.flowApiSystemData.Update(appLibrary);
        }

        public bool ValidateSystemCode(Guid id, string code)
        {
            List<RoadFlow.Model.FlowApiSystem> all = this.GetAll();
            if (id.IsEmptyGuid())
            {
                return !all.Exists(delegate (RoadFlow.Model.FlowApiSystem p) {
                    return p.SystemCode.EqualsIgnoreCase(code);
                });
            }
            return !all.Exists(delegate (RoadFlow.Model.FlowApiSystem p) {
                return (p.Id != id) && p.SystemCode.EqualsIgnoreCase(code);
            });
        }

       
}




}
