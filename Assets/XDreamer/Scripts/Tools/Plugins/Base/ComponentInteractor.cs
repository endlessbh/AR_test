using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 组件查找规则
    /// </summary>
    public enum EComponentSearchRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 列表中
        /// </summary>
        [Name("列表中")]
        InList,

        /// <summary>
        /// 所有启用的
        /// </summary>
        [Name("所有启用的")]
        AllEnabled,

        /// <summary>
        /// 在子级中所有启用的
        /// </summary>
        [Name("在子级中所有启用的")]
        AllEnabledInChildren,
    }

    /// <summary>
    /// 组件提供器：用于查找场景中所有某类型组件（包括所有启用、子级中启用、序列化列表存储指定）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentProvider<T> where T : Component
    {
        /// <summary>
        /// 组件查找规则
        /// </summary>
        [Name("组件查找规则")]
        [EnumPopup]
        public EComponentSearchRule _componentSearchRule = EComponentSearchRule.AllEnabledInChildren;

        /// <summary>
        /// 组件列表
        /// </summary>
        [Name("组件列表")]
        [HideInSuperInspector(nameof(_componentSearchRule), EValidityCheckType.NotEqual, EComponentSearchRule.InList)]
        public List<T> _components = new List<T>();

        private List<T> enabledComponents = new List<T>();

        private List<T> enabledComponentsInChildren = new List<T>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComponentProvider() { }

        /// <summary>
        /// 拥有者启用
        /// </summary>
        /// <param name="owner"></param>
        internal void OnOwnerEnable(Component owner)
        {
            enabledComponents.AddRange(ComponentCache.GetComponents<T>().Where(c => c && c.GetComponentEnabled()));
            enabledComponentsInChildren.AddRange(owner.GetComponentsInChildren<T>(true));
        }

        /// <summary>
        /// 拥有者禁用
        /// </summary>
        /// <param name="owner"></param>
        internal void OnOwnerDisnable(Component owner)
        {
            enabledComponents.Clear();
            enabledComponentsInChildren.Clear();
        }

        /// <summary>
        /// MB组件启用回调
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="owner"></param>
        internal void OnMBEnable(MB mb, Component owner)
        {
            if (mb is T component)
            {
                enabledComponents.AddWithDistinct(component);

                if (component.transform.IsChildOf(owner.transform))
                {
                    enabledComponentsInChildren.AddWithDistinct(component);
                }
            }
        }

        /// <summary>
        /// MB组件禁用回调
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="owner"></param>
        internal void OnMBDisable(MB mb, Component owner)
        {
            if (mb is T component)
            {
                enabledComponents.Remove(component);

                if (component.transform.IsChildOf(owner.transform))
                {
                    enabledComponentsInChildren.Remove(component);
                }
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetComponents()
        {
            switch (_componentSearchRule)
            {
                case EComponentSearchRule.InList: return _components;
                case EComponentSearchRule.AllEnabled: return enabledComponents;
                case EComponentSearchRule.AllEnabledInChildren: return enabledComponentsInChildren;
                default: return Enumerable.Empty<T>();
            }
        }
    }

    /// <summary>
    /// 组件交互器：使用组件提供器提供的组件列表执行交互
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TComponentProvider"></typeparam>
    public abstract class ComponentInteractor<TComponent, TComponentProvider> : Interactor
        where TComponent : Component, new()
        where TComponentProvider : ComponentProvider<TComponent>, new()
    {
        public TComponentProvider _componentProvider = new TComponentProvider();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _componentProvider.OnOwnerEnable(this);

            onEnable += OnMBEnable;
            onDisable += OnMBDisable;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            onEnable -= OnMBEnable;
            onDisable -= OnMBDisable;

            _componentProvider.OnOwnerDisnable(this);
        }

        private void OnMBEnable(MB mb) => _componentProvider.OnMBEnable(mb, this);

        private void OnMBDisable(MB mb) => _componentProvider.OnMBDisable(mb, this);
    }
}
