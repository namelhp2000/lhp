using RoadFlow.Utility.Domains.Trees;
using RoadFlow.Utility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 树结构帮助类
    /// </summary>
    public static class TreeHelper
    {
        /// <summary>
        /// 建造树结构
        /// </summary>
        /// <param name="allNodes">所有的节点</param>
        /// <returns></returns>
        public static List<T> BuildTree<T>(List<T> allNodes) where T : TreeModel, new()
        {
            List<T> resData = new List<T>();
            var rootNodes = allNodes.Where(x => x.ParentId == "0" || x.ParentId.IsNullOrEmpty()).ToList();
            resData = rootNodes;
            resData.ForEach(aRootNode =>
            {
                if (HaveChildren(allNodes, aRootNode.Id))
                    aRootNode.Children = GetChildren(allNodes, aRootNode);
            });

            return resData;
        }

        /// <summary>
        /// 获取所有子节点
        /// </summary>
        /// <typeparam name="T">树模型（TreeModel或继承它的模型）</typeparam>
        /// <param name="nodes">所有节点列表</param>
        /// <param name="parentNode">父节点Id</param>
        /// <returns></returns>
        private static List<object> GetChildren<T>(List<T> nodes, T parentNode) where T : TreeModel, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties().ToList();
            List<object> resData = new List<object>();
            var children = nodes.Where(x => x.ParentId == parentNode.Id).ToList();
            children.ForEach(aChildren =>
            {
                T newNode = new T();
                resData.Add(newNode);

                //赋值属性
                properties.ForEach(aProperty =>
                {
                    var value = aProperty.GetValue(aChildren, null);
                    aProperty.SetValue(newNode, value);
                });
                //设置深度
                newNode.Level = parentNode.Level + 1;

                if (HaveChildren(nodes, aChildren.Id))
                    newNode.Children = GetChildren(nodes, newNode);
            });

            return resData;
        }

        /// <summary>
        /// 判断当前节点是否有子节点
        /// </summary>
        /// <typeparam name="T">树模型</typeparam>
        /// <param name="nodes">所有节点</param>
        /// <param name="nodeId">当前节点Id</param>
        /// <returns></returns>
        private static bool HaveChildren<T>(List<T> nodes, string nodeId) where T : TreeModel, new()
        {
            return nodes.Exists(x => x.ParentId == nodeId);
        }



        /// <summary>
        /// 更新实体及所有下级节点路径
        /// </summary>
        /// <param name="repository">仓储</param>
        /// <param name="entity">实体</param>
        public static async Task UpdatePathAsync<TEntity, TKey, TParentId>(this ITreeCompactRepository<TEntity, TKey, TParentId> repository, TEntity entity)
            where TEntity : class, ITreeEntity<TEntity, TKey, TParentId>
        {
            var manager = new UpdatePathManager<TEntity, TKey, TParentId>(repository);
            await manager.UpdatePathAsync(entity);
        }

        /// <summary>
        /// 交换排序
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="swapEntity">交换实体</param>
        public static void SwapSort(this ISortId entity, ISortId swapEntity)
        {
            var sortId = entity.SortId;
            entity.SortId = swapEntity.SortId;
            swapEntity.SortId = sortId;
        }

        /// <summary>
        /// 获取缺失的父标识列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// /// <typeparam name="TKey">标识类型</typeparam>
        /// <typeparam name="TParentId">父标识类型</typeparam>
        /// <param name="entities">实体列表</param>
        public static List<string> GetMissingParentIds<TEntity, TKey, TParentId>(this IEnumerable<TEntity> entities) where TEntity : class, ITreeEntity<TEntity, TKey, TParentId>
        {
            var result = new List<string>();
            if (entities == null)
                return result;
            var list = entities.ToList();
            list.ForEach(entity =>
            {
                if (entity == null)
                    return;
                result.AddRange(entity.GetParentIdsFromPath().Select(t => t.SafeString()));
            });
            var ids = list.Select(t => t?.Id.SafeString());
            return result.Except(ids).ToList();
        }



    }
}
