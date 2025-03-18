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
    /// UGUI滑动条脚本事件类型 
    /// </summary>
    [Name("UGUI滑动条脚本事件类型")]
    public enum EUGUISliderScriptEventType
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
    /// UGUI滑动条脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI滑动条脚本事件函数")]
    public class UGUISliderScriptEventFunction : EnumFunction<EUGUISliderScriptEventType> { }

    /// <summary>
    /// UGUI滑动条脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("UGUI滑动条脚本事件函数集合")]
    public class UGUISliderScriptEventFunctionCollection : EnumFunctionCollection<EUGUISliderScriptEventType, UGUISliderScriptEventFunction> { }

    /// <summary>
    /// UGUI滑动条脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Slider))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUISliderScriptEvent : BaseScriptEvent<EUGUISliderScriptEventType, UGUISliderScriptEventFunction, UGUISliderScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI滑动条脚本事件";

        /// <summary>
        /// 滑动条
        /// </summary>
        public Slider slider { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            slider = gameObject.GetComponent<Slider>();
            if (slider)
            {
                slider.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (slider)
            {
                slider.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUISliderScriptEventType.Start);
        }

        /// <summary>
        /// 当值变化
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(float obj)
        {
            ExecuteScriptEvent(EUGUISliderScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
