using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class DocDir
    {
        // Fields
        private readonly RoadFlow.Data.DocDir docDirData = new RoadFlow.Data.DocDir();

        // Methods
        public int Add(RoadFlow.Model.DocDir docDir)
        {
            return this.docDirData.Add(docDir);
        }

        private void AddChilds(RoadFlow.Model.DocDir docDir, List<RoadFlow.Model.DocDir> docDirs, List<RoadFlow.Model.DocDir> all)
        {
            foreach (RoadFlow.Model.DocDir dir in all.FindAll(delegate (RoadFlow.Model.DocDir p) {
                return p.ParentId == docDir.Id;
            }))
            {
                docDirs.Add(dir);
                this.AddChilds(dir, docDirs, all);
            }
        }

        private void AddParent(List<RoadFlow.Model.DocDir> docDirs, List<RoadFlow.Model.DocDir> all, Guid parentId)
        {
            RoadFlow.Model.DocDir dir = all.Find(delegate (RoadFlow.Model.DocDir p) {
                return p.Id == parentId;
            });
            if (dir != null)
            {
                docDirs.Add(dir);
                this.AddParent(docDirs, all, dir.ParentId);
            }
        }

        public int Delete(RoadFlow.Model.DocDir docDir)
        {
            List<RoadFlow.Model.DocDir> allChilds = this.GetAllChilds(docDir.Id, true);
            return this.docDirData.Delete(allChilds.ToArray());
        }

        public RoadFlow.Model.DocDir Get(Guid id)
        {
            List<RoadFlow.Model.DocDir> all = this.GetAll();
            if (all.Count != 0)
            {
                return all.Find(delegate (RoadFlow.Model.DocDir p) {
                    return p.Id == id;
                });
            }
            return null;
        }

        public List<RoadFlow.Model.DocDir> GetAll()
        {
            return this.docDirData.GetAll();
        }

        public List<RoadFlow.Model.DocDir> GetAllChilds(Guid id, bool isMe = true)
        {
            List<RoadFlow.Model.DocDir> docDirs = new List<RoadFlow.Model.DocDir>();
            List<RoadFlow.Model.DocDir> all = this.GetAll();
            if (all.Count!= 0)
            {
                RoadFlow.Model.DocDir docDir = all.Find(delegate (RoadFlow.Model.DocDir p) {
                    return p.Id == id;
                });
                if (docDir == null)
                {
                    return docDirs;
                }
                if (isMe)
                {
                    docDirs.Add(docDir);
                }
                this.AddChilds(docDir, docDirs, all);
            }
            return docDirs;
        }

        public List<Guid> GetAllChildsId(Guid id, bool isMe = true)
        {
            List<Guid> list = new List<Guid>();
            foreach (RoadFlow.Model.DocDir dir in this.GetAllChilds(id, isMe))
            {
                list.Add(dir.Id);
            }
            return list;
        }

        public string GetAllParentNames(Guid dirId, bool hasMe = true, bool hasRoot = true, string split = @"\")
        {
            StringBuilder builder = new StringBuilder();
            List<RoadFlow.Model.DocDir> list1 = this.GetAllParents(dirId, hasMe, hasRoot);
            list1.Reverse();
            foreach (RoadFlow.Model.DocDir dir in list1)
            {
                builder.Append(dir.Name);
                builder.Append(split);
            }
            return builder.ToString().TrimEnd(split.ToCharArray());
        }

        public List<RoadFlow.Model.DocDir> GetAllParents(Guid dirId, bool hasMe = true, bool hasRoot = true)
        {
            List<RoadFlow.Model.DocDir> all = this.GetAll();
            List<RoadFlow.Model.DocDir> docDirs = new List<RoadFlow.Model.DocDir>();
            RoadFlow.Model.DocDir dir = all.Find(delegate (RoadFlow.Model.DocDir p) {
                return p.Id == dirId;
            });
            if (dir != null)
            {
                if (hasMe)
                {
                    docDirs.Add(dir);
                }
                this.AddParent(docDirs, all, dir.ParentId);
                if (hasRoot)
                {
                    return docDirs;
                }
                docDirs.RemoveAll(key=> (key.ParentId == Guid.Empty));
            }
            return docDirs;
        }

        /// <summary>
        /// 获取当前id的所有子节点
        /// </summary>
        /// <param name="id">当前id</param>
        /// <returns></returns>
        public List<RoadFlow.Model.DocDir> GetChilds(Guid id)
        {
            List<RoadFlow.Model.DocDir> all = this.GetAll();
            if (all.Count == 0)
            {
                return new List<RoadFlow.Model.DocDir>();
            }
            return all.FindAll(delegate (RoadFlow.Model.DocDir p) {
                return p.ParentId == id;
            });
        }

        /// <summary>
        /// 根据id与用户id获取匹配所有的子节点
        /// </summary>
        /// <param name="id">当前id</param>
        /// <param name="userId">对应用户id</param>
        /// <returns></returns>
        public List<RoadFlow.Model.DocDir> GetDisplayChilds(Guid id, Guid userId)
        {
            List<RoadFlow.Model.DocDir> list = new List<RoadFlow.Model.DocDir>();
            foreach (RoadFlow.Model.DocDir dir in this.GetChilds(id))
            {
                if (this.IsDisplay(dir, userId))
                {
                    list.Add(dir);
                }
            }
            return list;
        }

        public int GetMaxSort(Guid id)
        {
            List<RoadFlow.Model.DocDir> childs = this.GetChilds(id);
            if (childs.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.DocDir>((IEnumerable<RoadFlow.Model.DocDir>)childs,
              key=>key.Sort) + 5);
        }

        public string GetName(Guid id)
        {
            RoadFlow.Model.DocDir dir = this.Get(id);
            if (dir != null)
            {
                return dir.Name;
            }
            return string.Empty;
        }

        public List<RoadFlow.Model.DocDir> GetReadDirs(Guid userId)
        {
            List<RoadFlow.Model.DocDir> list = new List<RoadFlow.Model.DocDir>();
            foreach (RoadFlow.Model.DocDir dir in this.GetAll())
            {
                if (this.IsRead(dir, userId))
                {
                    list.Add(dir);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据文件id与用户id获取刷新json树
        /// </summary>
        /// <param name="docDirId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRefreshJson(Guid docDirId, Guid userId)
        {
            JArray array = new JArray();
            foreach (RoadFlow.Model.DocDir dir in this.GetDisplayChilds(docDirId, userId))
            {
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)dir.Id);
                obj1.Add("parentID", (JToken)dir.ParentId);
                obj1.Add("title", (JToken)dir.Name);
                obj1.Add("type", 0);
                obj1.Add("ico", "");
                obj1.Add("color", "");
                obj1.Add("hasChilds", (JToken)this.GetDisplayChilds(dir.Id, userId).Count);
                JObject obj2 = obj1;
                array.Add(obj2);
            }
            return array.ToString();
        }

        public RoadFlow.Model.DocDir GetRoot()
        {
            return this.docDirData.GetRoot();
        }

        /// <summary>
        /// 根据用户id获取文件夹树
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetTreeJson(Guid userId)
        {
            JArray array = new JArray();

            foreach (RoadFlow.Model.DocDir dir in this.GetDisplayChilds(Guid.Empty, userId))
            {
                List<RoadFlow.Model.DocDir> displayChilds = this.GetDisplayChilds(dir.Id, userId);
                JObject obj1 = new JObject();
                obj1.Add("id", (JToken)dir.Id);
                obj1.Add("parentID", (JToken)dir.ParentId);
                obj1.Add("title", (JToken)dir.Name);
                obj1.Add("type", 0);
                obj1.Add("ico", "");
                obj1.Add("color", "");
                obj1.Add("hasChilds", (JToken)displayChilds.Count);
                JObject obj2 = obj1;
                JArray array2 = new JArray();
                foreach (RoadFlow.Model.DocDir dir2 in displayChilds)
                {
                    JObject obj4 = new JObject();
                    obj4.Add("id", (JToken)dir2.Id);
                    obj4.Add("parentID", (JToken)dir2.ParentId);
                    obj4.Add("title", (JToken)dir2.Name);
                    obj4.Add("type", 0);
                    obj4.Add("ico", "");
                    obj4.Add("color", "");
                    obj4.Add("hasChilds", (JToken)this.GetDisplayChilds(dir2.Id, userId).Count);
                    JObject obj3 = obj4;
                    array2.Add(obj3);
                }
                obj2.Add("childs", array2);
                array.Add(obj2);
            }
            return array.ToString();
        }

        public bool HasDoc(Guid dirId)
        {
            List<Guid> allChildsId = this.GetAllChildsId(dirId, true);
            return this.docDirData.HasDoc(allChildsId);
        }

        /// <summary>
        /// 根据用户id显示对应的文件夹
        /// </summary>
        /// <param name="docDir">文件夹</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public bool IsDisplay(RoadFlow.Model.DocDir docDir, Guid userId)
        {
            if (docDir != null)
            {
                if ((this.IsRead(docDir, userId) || this.IsManage(docDir, userId)) || this.IsPublish(docDir, userId))
                {
                    return true;
                }
                foreach (RoadFlow.Model.DocDir dir in this.GetAllChilds(docDir.Id, false))
                {
                    if ((this.IsRead(dir, userId) || this.IsManage(dir, userId)) || this.IsPublish(dir, userId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsManage(RoadFlow.Model.DocDir docDir, Guid userId)
        {
            if (docDir == null)
            {
                return false;
            }
            return new User().Exists(userId.ToString(), docDir.ManageUsers);
        }

        public bool IsPublish(RoadFlow.Model.DocDir docDir, Guid userId)
        {
            if (docDir == null)
            {
                return false;
            }
            return new User().Exists(userId.ToString(), docDir.PublishUsers);
        }

        public bool IsRead(RoadFlow.Model.DocDir docDir, Guid userId)
        {
            if (docDir == null)
            {
                return false;
            }
            if (!docDir.ReadUsers.IsNullOrWhiteSpace())
            {
                return new User().Exists(userId.ToString(), docDir.ReadUsers);
            }
            return true;
        }

        public int Update(RoadFlow.Model.DocDir docDir)
        {
            return this.docDirData.Update(docDir);
        }

      
}


}
