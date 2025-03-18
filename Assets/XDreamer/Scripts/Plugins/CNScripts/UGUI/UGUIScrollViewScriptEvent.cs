using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI滚动视图脚本事件类型 
    /// </summary>
    [Name("UGUI滚动视图脚本事件类型")]
    public enum EUGUIScrollViewScriptEventType
    {
        /// <summary>
        /// 值变动时执行
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        /// <summary>
        /// 启动时执行
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI滚动视图脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI滚动视图脚本事件函数")]
    public class UGUIScrollViewScriptEventFunction : EnumFunction<EUGUIScrollViewScriptEventType> { }

    /// <summary>
    /// UGUI滚动视图脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("UGUI滚动视图脚本事件函数集合")]
    public class UGUIScrollViewScriptEventFunctionCollection : EnumFunctionCollection<EUGUIScrollViewScriptEventType, UGUIScrollViewScriptEventFunction> { }

    /// <summary>
    /// UGUI滚动视图脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(ScrollRect))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIScrollViewScriptEvent : BaseScriptEvent<EUGUIScrollViewScriptEventType, UGUIScrollViewScriptEventFunction, UGUIScrollViewScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI滚动视图脚本事件";

        /// <summary>
        /// 滚动视图
        /// </summary>
        public ScrollRect scrollView { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            scrollView = gameObject.GetComponent<ScrollRect>();
            if (scrollView)
            {
                scrollView.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (scrollView)
            {
                scrollView.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIScrollViewScriptEventType.Start);
        }

        /// <summary>
        /// 当值变化
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(Vector2 obj)
        {
            ExecuteScriptEvent(EUGUIScrollViewScriptEventType.OnValueChanged, CommonFun.Vector2ToString(obj));
        }
    }
}
