using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoadFlow.Business
{
    public class AppLibraryButton
    {
        // Fields
        private readonly RoadFlow.Data.AppLibraryButton appLibraryButtonData = new RoadFlow.Data.AppLibraryButton();

        // Methods
        public int Add(RoadFlow.Model.AppLibraryButton appLibraryButton)
        {
            return this.appLibraryButtonData.Add(appLibraryButton);
        }

        public int Delete(Guid id)
        {
            return this.appLibraryButtonData.Delete(this.Get(id));
        }

        public int DeleteByApplibraryId(Guid applibraryId)
        {
            List<RoadFlow.Model.AppLibraryButton> listByApplibraryId = this.GetListByApplibraryId(applibraryId);
            return this.appLibraryButtonData.Delete(listByApplibraryId.ToArray());
        }

        public RoadFlow.Model.AppLibraryButton Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.AppLibraryButton p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.AppLibraryButton> GetAll()
        {
            return this.appLibraryButtonData.GetAll();
        }

        public List<RoadFlow.Model.AppLibraryButton> GetListByApplibraryId(Guid applibraryId)
        {
            return Enumerable.ToList<RoadFlow.Model.AppLibraryButton>((IEnumerable<RoadFlow.Model.AppLibraryButton>)Enumerable.OrderBy<RoadFlow.Model.AppLibraryButton, int>((IEnumerable<RoadFlow.Model.AppLibraryButton>)this.GetAll().FindAll(delegate (RoadFlow.Model.AppLibraryButton p) {
                return p.AppLibraryId == applibraryId;
            }), key=>key.Sort));
        }

        public int Update(RoadFlow.Model.AppLibraryButton appLibraryButton)
        {
            return this.appLibraryButtonData.Update(appLibraryButton);
        }

        public int Update(List<Tuple<RoadFlow.Model.AppLibraryButton, int>> tuples)
        {
            return this.appLibraryButtonData.Update(tuples);
        }

     
}


}
