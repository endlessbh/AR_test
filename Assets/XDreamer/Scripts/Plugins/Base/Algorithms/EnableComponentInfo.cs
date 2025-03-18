using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 启用组件信息
    /// </summary>
    [Name("启用组件信息")]
    [Serializable]
    [LanguageFileOutput]
    public class EnableComponentInfo
    {
        /// <summary>
        /// 组件
        /// </summary>
        [Name("组件")]
        [ComponentPopup]
        public Component _component;

        /// <summary>
        /// 启用
        /// </summary>
        [Name("启用")]
        [EnumPopup]
        public EBool _enable = EBool.Yes;

        /// <summary>
        /// 启用
        /// </summary>
        public void Enable()
        {
            _component.XSetEnable(_enable);
        }
    }

    /// <summary>
    /// 基础启用组件信息列表
    /// </summary>
    [LanguageFileOutput]
    public abstract class BaseEnableComponentInfoList
    {
        /// <summary>
        /// 枚举事件
        /// </summary>
        public abstract Enum enumEvent { get; }

        /// <summary>
        /// 信息列表
        /// </summary>
        public abstract List<EnableComponentInfo> infos { get; }
    }

    /// <summary>
    /// 启用组件信息列表
    /// </summary>
    public class EnableComponentInfoList<TEnumEvent> : BaseEnableComponentInfoList
    {
        /// <summary>
        /// 枚举事件
        /// </summary>
        [Name("枚举事件")]
        [EnumPopup]
        public TEnumEvent _enumEvent = default;

        /// <summary>
        /// 枚举事件
        /// </summary>
        public override Enum enumEvent => (Enum)(object)_enumEvent;

        /// <summary>
        /// 启用信息列表
        /// </summary>
        [Name("启用信息列表")]
        [OnlyMemberElements]
        public List<EnableComponentInfo> _infos = new List<EnableComponentInfo>();

        /// <summary>
        /// 信息列表
        /// </summary>
        public override List<EnableComponentInfo> infos => _infos;

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="component"></param>
        /// <param name="active"></param>
        public virtual void Add(Component component, EBool enable)
        {
            if (!component) return;
            var info = _infos.FirstOrNew(i => i._component == component, i =>
            {
                i._component = component;
                _infos.Add(i);
            });
            info._enable = enable;
        }

        /// <summary>
        /// 移除全部
        /// </summary>
        /// <param name="component"></param>
        public virtual void RemoveAll(Component component)
        {
            _infos.RemoveAll(i => i._component == component);
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="enumEvent"></param>
        public virtual bool Enable(TEnumEvent enumEvent)
        {
            if (EqualityComparer<TEnumEvent>.Default.Equals(_enumEvent, enumEvent))
            {
                Enable();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public virtual void Enable()
        {
            foreach (var info in _infos)
            {
                info.Enable();
            }
        }
    }

    /// <summary>
    /// 启用组件事件集合
    /// </summary>
    /// <typeparam name="TEnumEvent"></typeparam>
    /// <typeparam name="TInfoList"></typeparam>
    public class EnableComponentEventSet<TEnumEvent, TInfoList> where TInfoList : EnableComponentInfoList<TEnumEvent>
    {
        /// <summary>
        /// 信息列表
        /// </summary>
        [Name("信息列表")]
        public List<TInfoList> _infoLists = new List<TInfoList>();

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="enumEvent"></param>
        public virtual void Enable(TEnumEvent enumEvent)
        {
            foreach (var info in _infoLists)
            {
                info.Enable(enumEvent);
            }
        }
    }
}
