using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RoadFlow.Data.DataAutoFac
{
     public  interface IApp
    {
        int Add(RoadFlow.Model.AppLibrary appLibrary);
         void ClearCache();
        int Delete(RoadFlow.Model.AppLibrary appLibrary);
         int Delete(RoadFlow.Model.AppLibrary[] appLibrarys);
        List<RoadFlow.Model.AppLibrary> GetAll();
        DataTable GetPagerList(out int count, int size, int number, string title, string address, string typeId, string order);

        int Update(RoadFlow.Model.AppLibrary appLibrary);
    }
}
