using RoadFlow.Utility.Cache;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RoadFlow.Business
{


 
    public class MenuUser
    {
        // Fields
        private readonly RoadFlow.Data.MenuUser menuUserData = new RoadFlow.Data.MenuUser();

        // Methods
        public int Add(RoadFlow.Model.MenuUser menuUser)
        {
            return this.menuUserData.Add(menuUser);
        }

        public int Delete(Guid id)
        {
            return this.menuUserData.Delete(this.Get(id));
        }

        public int DeleteByMenuId(Guid menuId)
        {
            return this.menuUserData.Delete(this.GetListByMenuId(menuId).ToArray());
        }

        public RoadFlow.Model.MenuUser Get(Guid id)
        {
            return this.GetAll().Find(delegate (RoadFlow.Model.MenuUser p)
            {
                return p.Id == id;
            });
        }

        public List<RoadFlow.Model.MenuUser> GetAll()
        {
            bool flag;
            List<RoadFlow.Model.MenuUser> all = this.menuUserData.GetAll(out flag);
            if (!flag)
            {
                this.UpdateAllUseUser(all);
            }
            return all;
        }

        public List<RoadFlow.Model.MenuUser> GetListByMenuId(Guid menuId)
        {
            return this.GetAll().FindAll(delegate (RoadFlow.Model.MenuUser p)
            {
                return p.MenuId == menuId;
            });
        }

        public int Update(RoadFlow.Model.MenuUser menuUser)
        {
            return this.menuUserData.Update(menuUser);
        }

        public int Update(RoadFlow.Model.MenuUser[] menuUsers, string orgId)
        {
            return this.menuUserData.Update(menuUsers, orgId);
        }



        public void UpdateAllUseUser(List<RoadFlow.Model.MenuUser> menuUsers = null)
        {
            if (menuUsers == null)
            {
                bool flag;
                menuUsers = this.menuUserData.GetAll(out flag);
            }
            Organize organize = new Organize();
            foreach (RoadFlow.Model.MenuUser user in menuUsers)
            {
                user.Users = organize.GetAllUsersId(user.Organizes);
            }
            IO.Insert("roadflow_cache_menuuser", menuUsers);
        }





        #region 2.8.9方法

        [CompilerGenerated]
        private void UpdateAllUseUserAsyncb__11_0()
        {
            this.UpdateAllUseUser(null);
        }



        [AsyncStateMachine((typeof(UpdateAllUseUserAsyncd__11)))]
        public void UpdateAllUseUserAsync()
        {
            UpdateAllUseUserAsyncd__11 d__=new UpdateAllUseUserAsyncd__11();
            d__.s_4__this = this;
            d__.s_t__builder = AsyncVoidMethodBuilder.Create();
            d__.s_1__state = -1;
            d__.s_t__builder.Start<UpdateAllUseUserAsyncd__11>(ref d__);
        }

        // Nested Types
        [CompilerGenerated]
        private struct UpdateAllUseUserAsyncd__11 : IAsyncStateMachine
        {
            // Fields
            public int s_1__state;
            public MenuUser s_4__this;
            public AsyncVoidMethodBuilder s_t__builder;
            private TaskAwaiter s_u__1;

            // Methods
             void IAsyncStateMachine.MoveNext()
            {
                int num = this.s_1__state;
                MenuUser user = this.s_4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        awaiter = Task.Run(new Action(user.UpdateAllUseUserAsyncb__11_0)).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.s_1__state = num = 0;
                            this.s_u__1 = awaiter;
                            this.s_t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, MenuUser.UpdateAllUseUserAsyncd__11>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.s_u__1;
                        this.s_u__1 = new TaskAwaiter();
                        this.s_1__state = num = -1;
                    }
                    awaiter.GetResult();
                }
                catch (Exception exception)
                {
                    this.s_1__state = -2;
                    this.s_t__builder.SetException(exception);
                    return;
                }
                this.s_1__state = -2;
                this.s_t__builder.SetResult();
            }

            [DebuggerHidden]
             void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.s_t__builder.SetStateMachine(stateMachine);
            }

          







            #endregion











          
        }






    
    }
}
