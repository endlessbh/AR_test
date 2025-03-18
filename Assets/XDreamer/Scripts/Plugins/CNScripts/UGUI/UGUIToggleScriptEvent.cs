using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI切换脚本事件类型 
    /// </summary>
    [Name("UGUI切换脚本事件类型")]
    public enum EUGUIToggleScriptEventType
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
    /// UGUI切换脚本事件函数
    /// </summary>
    [Name("UGUI切换脚本事件函数")]
    [Serializable]
    public class UGUIToggleScriptEventFunction : EnumFunction<EUGUIToggleScriptEventType> { }

    /// <summary>
    /// UGUI切换脚本事件函数集合
    /// </summary>
    [Name("UGUI切换脚本事件函数集合")]
    [Serializable]
    public class UGUIToggleScriptEventFunctionCollection : EnumFunctionCollection<EUGUIToggleScriptEventType, UGUIToggleScriptEventFunction> { }

    /// <summary>
    /// UGUI切换脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Toggle))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIToggleScriptEvent : BaseScriptEvent<EUGUIToggleScriptEventType, UGUIToggleScriptEventFunction, UGUIToggleScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI切换脚本事件";

        /// <summary>
        /// 切换
        /// </summary>
        public Toggle toggle { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            toggle = gameObject.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIToggleScriptEventType.Start);
        }

        /// <summary>
        /// 当值变化
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(bool obj)
        {
            ExecuteScriptEvent(EUGUIToggleScriptEventType.OnValueChanged, ScriptOption.ReturnValueFlag + obj.ToString());
        }
    }
}
