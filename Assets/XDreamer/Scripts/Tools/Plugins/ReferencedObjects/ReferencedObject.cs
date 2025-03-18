using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.ReferencedObjects
{
    /// <summary>
    /// 引用的对象
    /// </summary>
    [Name("引用的对象")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class ReferencedObject : InteractProvider
    {
        /// <summary>
        /// 创建者；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        public UnityEngine.Object creater
        {
            get => _owners.Count > 0 ? _owners[0] : null;
            private set
            {
                if (_owners.Count == 0)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }
                    AddOwner(value);
                }
                else if (_owners[0] != value)
                {
                    Debug.LogWarningFormat("引用的游戏对象[{0}]被对象[{1}]({2})创建,无法修改为被对象[{3}]({4})创建",
                        CommonFun.ObjectToString(gameObject),
                        CommonFun.ObjectToString(_owners[0]),
                        CommonFun.Name(_owners[0]?.GetType()),
                        CommonFun.ObjectToString(value),
                        CommonFun.Name(value?.GetType()));
                }
            }
        }

        /// <summary>
        /// 拥有者列表:标识拥有当前引用的对象的使用权限的对象
        /// </summary>
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [Name("拥有者列表")]
        [Tip("标识拥有当前引用的对象的使用权限的对象列表", "Identifies the list of objects that have permission to use the currently referenced object")]
        public List<UnityEngine.Object> _owners = new List<UnityEngine.Object>();

        /// <summary>
        /// 拥有者列表
        /// </summary>
        public List<UnityEngine.Object> owners => _owners;

        /// <summary>
        /// 增加拥有者：去重添加；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="owner"></param>
        public void AddOwner(UnityEngine.Object owner)
        {
            if (!owner) return;
            this.XModifyProperty(() =>
            {
                _owners.AddWithDistinct(owner);
            });
        }

        /// <summary>
        /// 移除拥有者；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="owner"></param>
        public void RemoveOwner(UnityEngine.Object owner)
        {
            if (!owner) return;
            this.XModifyProperty(() =>
            {
                _owners.Remove(owner);
            });
        }

        /// <summary>
        /// 检测当前对象的拥有者列表中是否存在指定的拥有者
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool HasOwner(UnityEngine.Object owner)
        {
            if (!owner) return false;
            return _owners.Any(o => o == owner);
        }

        /// <summary>
        /// 有任意拥有者
        /// </summary>
        /// <returns></returns>
        public bool HasAnyOnwer() => _owners.Any(o => o);

        /// <summary>
        /// 检查当前对象是否被拥有者占有，通过<see cref="IOwnReferencedObject"/>接口检测
        /// </summary>
        /// <param name="owner">拥有者对象：需要实现接口<see cref="IOwnReferencedObject"/></param>
        /// <returns></returns>
        public bool OwnBy(UnityEngine.Object owner)

        {
            if (!owner) return false;
            if (owner is IOwnReferencedObject own)
            {
                var gameObject = this.gameObject;
                var transform = this.transform;
                return own.OwnReferencedObject(o => o == this || o == gameObject || o == transform);
            }
            return false;
        }

        #region 创建

        /// <summary>
        /// 创建引用的对象；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">父级变换</param>
        /// <param name="owner">拥有则</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static T Create<T>(Transform parent, UnityEngine.Object owner, string name = null) where T : ReferencedObject
        {
            if (!parent || !owner) return null;
            var newName = CommonFun.GetGameObjectUniqueName(parent.gameObject, name ?? owner.name, parent.childCount);

            var go = UnityObjectHelper.CreateGameObject(newName);
            go.XSetParent(parent);

            var tc = go.XAddComponent<T>();
            tc.creater = owner;

            return tc;
        }

        #endregion
    }

    /// <summary>
    /// 拥有引用的对象的接口
    /// </summary>
    public interface IOwnReferencedObject
    {
        /// <summary>
        /// 检测是否拥有引用的对象
        /// </summary>
        /// <param name="ownFunc"></param>
        /// <returns></returns>
        bool OwnReferencedObject(Func<UnityEngine.Object, bool> ownFunc);
    }
}
