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
    /// UGUI滚动条脚本事件类型 
    /// </summary>
    [Name("UGUI滚动条脚本事件类型")]
    public enum EUGUIScrollbarScriptEventType
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
    /// UGUI滚动条脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI滚动条脚本事件函数")]
    public class UGUIScrollbarScriptEventFunction : EnumFunction<EUGUIScrollbarScriptEventType> { }

    /// <summary>
    /// UGUI滚动条脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("UGUI滚动条脚本事件函数集合")]
    public class UGUIScrollbarScriptEventFunctionCollection : EnumFunctionCollection<EUGUIScrollbarScriptEventType, UGUIScrollbarScriptEventFunction> { }

    /// <summary>
    /// UGUI滚动条脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Scrollbar))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIScrollbarScriptEvent : BaseScriptEvent<EUGUIScrollbarScriptEventType, UGUIScrollbarScriptEventFunction, UGUIScrollbarScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI滚动条脚本事件";

        /// <summary>
        /// 滚动条
        /// </summary>
        public Scrollbar scrollbar { get; protected set; }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            scrollbar = gameObject.GetComponent<Scrollbar>();
            if (scrollbar)
            {
                scrollbar.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (scrollbar)
            {
                scrollbar.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIScrollbarScriptEventType.Start);
        }

        /// <summary>
        /// 当值变化
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(float obj)
        {
            ExecuteScriptEvent(EUGUIScrollbarScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
