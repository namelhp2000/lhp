using RoadFlow.Utility;
using System;
using System.Collections.Generic;

namespace RoadFlow.Business
{
    public class ProgramValidate
    {
        // Fields
        private readonly RoadFlow.Data.ProgramValidate programValidateData = new RoadFlow.Data.ProgramValidate();

        // Methods
        public int Add(RoadFlow.Model.ProgramValidate[] ProgramValidates)
        {
            return this.programValidateData.Add(ProgramValidates);
        }

        /// <summary>
        /// 通过项目id获取所有的设计项目验证数据
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        public List<RoadFlow.Model.ProgramValidate> GetAll(Guid programId)
        {
            return this.programValidateData.GetAll(programId);
        }
    }


}
