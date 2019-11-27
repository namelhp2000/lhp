using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class ProgramField
    {
        // Fields
        private readonly RoadFlow.Data.ProgramField programFieldData = new RoadFlow.Data.ProgramField();

        // Methods
        public int Add(RoadFlow.Model.ProgramField programField)
        {
            return this.programFieldData.Add(programField);
        }

        public int Delete(RoadFlow.Model.ProgramField[] programFields)
        {
            return this.programFieldData.Delete(programFields);
        }

        public RoadFlow.Model.ProgramField Get(Guid id)
        {
            return this.programFieldData.Get(id);
        }


        /// <summary>
        /// 通过项目id获取所有项目字段
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramField> GetAll(Guid programId)
        {
            return Enumerable.ToList<RoadFlow.Model.ProgramField>((IEnumerable<RoadFlow.Model.ProgramField>)Enumerable.OrderBy<RoadFlow.Model.ProgramField, int>((IEnumerable<RoadFlow.Model.ProgramField>)this.programFieldData.GetAll(programId), key => key.Sort));
        }

        public string GetFieldOptions(Guid connId, string sql, string value, List<string> removeValue = null)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in new DbConnection().GetFieldsBySql(connId, sql, null))
            {
                if (((removeValue == null) || value.EqualsIgnoreCase(str)) || !removeValue.ContainsIgnoreCase(str))
                {
                    string[] textArray1 = new string[] { "<option value=\"", str, "\"", str.EqualsIgnoreCase(value) ? " selected=\"selected\"" : "", ">", str, "</option>" };
                    builder.Append(string.Concat((string[])textArray1));
                }
            }
            return builder.ToString();
        }

        public int GetMaxSort(Guid programId)
        {
            List<RoadFlow.Model.ProgramField> all = this.GetAll(programId);
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.ProgramField>((IEnumerable<RoadFlow.Model.ProgramField>)all, key => key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.ProgramField programField)
        {
            return this.programFieldData.Update(programField);
        }
    }
}
