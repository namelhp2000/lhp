using RoadFlow.Utility;
using System;
using System.Collections.Generic;

namespace RoadFlow.Business
{
    public class UserShortcut
    {
        // Fields
        private readonly RoadFlow.Data.UserShortcut userShortcutData = new RoadFlow.Data.UserShortcut();

        // Methods
        public int Add(RoadFlow.Model.UserShortcut[] userShortcuts, Guid userId)
        {
            return this.userShortcutData.Add(userShortcuts, userId);
        }

        public int Delete(Guid userId)
        {
            return this.userShortcutData.Delete(userId);
        }

        public RoadFlow.Model.UserShortcut Get(Guid id)
        {
            return this.userShortcutData.GetAll().Find(delegate (RoadFlow.Model.UserShortcut p) {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.UserShortcut> GetAll()
        {
            return this.userShortcutData.GetAll();
        }

        public List<RoadFlow.Model.UserShortcut> GetListByMenuId(Guid menuId)
        {
            return this.userShortcutData.GetListByMenuId(menuId);
        }

        public List<RoadFlow.Model.UserShortcut> GetListByUserId(Guid userId)
        {
            return this.userShortcutData.GetListByUserId(userId);
        }

        public int Update(RoadFlow.Model.UserShortcut[] userShortcuts)
        {
            return this.userShortcutData.Update(userShortcuts);
        }
    }


    



}
