using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class ProgramButton
    {
        // Fields
        private readonly RoadFlow.Data.ProgramButton programButtonData = new RoadFlow.Data.ProgramButton();

        // Methods
        public int Add(RoadFlow.Model.ProgramButton programButton)
        {
            return this.programButtonData.Add(programButton);
        }

        public int Delete(RoadFlow.Model.ProgramButton[] programButtons)
        {
            return this.programButtonData.Delete(programButtons);
        }

        public RoadFlow.Model.ProgramButton Get(Guid id)
        {
            return this.programButtonData.Get(id);
        }

        /// <summary>
        /// 通过程序id获取程序按钮
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramButton> GetAll(Guid programId)
        {
            return Enumerable.ToList<RoadFlow.Model.ProgramButton>((IEnumerable<RoadFlow.Model.ProgramButton>)Enumerable.OrderBy<RoadFlow.Model.ProgramButton, int>((IEnumerable<RoadFlow.Model.ProgramButton>)this.programButtonData.GetAll(programId),key=>key.Sort));
        }

        public int GetMaxSort(Guid programId)
        {
            List<RoadFlow.Model.ProgramButton> all = this.GetAll(programId);
            if (all.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.ProgramButton>((IEnumerable<RoadFlow.Model.ProgramButton>)all, key=>key.Sort) + 5);
        }

        public int Update(RoadFlow.Model.ProgramButton programButton)
        {
            return this.programButtonData.Update(programButton);
        }

     
}




}
