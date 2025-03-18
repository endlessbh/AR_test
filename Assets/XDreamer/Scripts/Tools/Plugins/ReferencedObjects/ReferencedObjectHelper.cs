using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.ReferencedObjects
{
    /// <summary>
    /// 引用的目录对象辅助类
    /// </summary>
    public static class ReferencedObjectHelper
    {
        /// <summary>
        /// 获取默认父级
        /// </summary>
        /// <returns></returns>
        public static Transform GetDefaultParent() => ReferencedSet.GetFirstInstanceTransform();

        /// <summary>
        /// 获取拥有者可操作的引用的目录对象
        /// </summary>
        /// <param name="owner">拥有者</param>
        /// <param name="members">拥有者所具有的成员组件；用于搜索引用的目录对象时使用</param>
        /// <param name="categoryName">目录名称</param>
        /// <returns></returns>
        private static ReferencedCategory GetCategory(UnityEngine.Object owner, List<Transform> members, string categoryName = null)
        {
            if (!owner) return null;

            //从全局引用目录中查找对应的拥有者
            var components = ComponentCache.Get(typeof(ReferencedCategory), true)?.components;
            if (components != null
                && components.FirstOrDefault(c => c is ReferencedCategory rc && rc.HasOwner(owner)) is ReferencedCategory component
                && component)
            {
                return component;
            }

            //已有实际引用对象但是未添加引用目录组件的情况
            var setTransform = GetDefaultParent();
            if (members != null && members.Count > 0)
            {
                foreach (Transform categoryTransform in setTransform)
                {
                    foreach (Transform objectTransform in categoryTransform)
                    {
                        if (members.Any(m => m == objectTransform))
                        {
                            var category = categoryTransform.XGetOrAddComponent<ReferencedCategory>();
                            category.AddOwner(owner);
                            return category;
                        }
                    }
                }
            }

            //创建新的目录
            return ReferencedObject.Create<ReferencedCategory>(setTransform, owner, categoryName);
        }

        /// <summary>
        /// 同步位置；将变换列表中变换对象的位置同步为位置列表指向的位置；
        /// </summary>
        /// <param name="owner">变换列表的拥有者</param>
        /// <param name="transforms">变换列表：期望同步位置的变换，变换对象列表长度可能发生调整；</param>
        /// <param name="positions">位置列表：变换列表的新世界坐标列表；通过索引下标与变换列表一一对应；会根据本列长度动态调整变换列表长度</param>
        /// <param name="categoryName">目录名称</param>
        public static void SynchronousPosition(UnityEngine.Object owner, List<Transform> transforms, List<Vector3> positions, string categoryName = null)
        {
            if (!owner || positions == null || transforms == null) return;
            _SynchronousPosition(owner, transforms, positions, categoryName);
        }

        private static void _SynchronousPosition(UnityEngine.Object owner, List<Transform> transforms, List<Vector3> positions, string categoryName)
        {
            var category = GetCategory(owner, transforms, categoryName);
            if (!category) return;

            _SynchronousPosition(owner, category, transforms, positions);
        }

        private static void _SynchronousPosition(UnityEngine.Object owner, ReferencedCategory category, List<Transform> transforms, List<Vector3> positions)
        {
            _SynchronousData(owner, category, transforms, positions, (i, p, t) =>
            {
                t.XSetPosition(p);
            });
        }

        private static void _SynchronousData(UnityEngine.Object owner, ReferencedCategory category, List<Transform> transforms, List<Vector3> positions, Action<int, Vector3, Transform> action)
        {
            if (!owner || !category || positions == null || transforms == null) return;

            //移除多余的
            while (transforms.Count > positions.Count && transforms.Count > 0)
            {
                var i = transforms.Count - 1;
                var last = transforms[i];

                owner.XModifyProperty(() =>
                {
                    transforms.RemoveAt(i);
                });

                category.Delete(owner, last);
            }

            //添加不足的
            while (transforms.Count < positions.Count)
            {
                owner.XModifyProperty(() =>
                {
                    transforms.Add(ReferencedObject.Create<ReferencedObject>(category.transform, owner).transform);
                });
            }

            //坐标同步
            for (int i = 0; i < transforms.Count; ++i)
            {
                var t = transforms[i];
                if (!t)
                {
                    owner.XModifyProperty(() =>
                    {
                        transforms[i] = t = ReferencedObject.Create<ReferencedObject>(category.transform, owner).transform;
                    });
                }
                else
                {
                    var temp = t.XGetOrAddComponent<ReferencedObject>();
                    temp.AddOwner(owner);
                }

                action?.Invoke(i, positions[i], t);
            }
        }
    }
}
