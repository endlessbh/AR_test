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
    /// UGUI下拉选择框脚本事件类型 
    /// </summary>
    [Name("UGUI下拉选择框脚本事件类型")]
    public enum EUGUIDropdownScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI下拉选择框脚本事件函数
    /// </summary>
    [Name("UGUI下拉选择框脚本事件函数")]
    [Serializable]
    public class UGUIDropdownScriptEventFunction : EnumFunction<EUGUIDropdownScriptEventType> { }

    /// <summary>
    /// UGUI下拉选择框脚本事件函数集合
    /// </summary>
    [Name("UGUI下拉选择框脚本事件函数集合")]
    [Serializable]
    public class UGUIDropdownScriptEventFunctionCollection : EnumFunctionCollection<EUGUIDropdownScriptEventType, UGUIDropdownScriptEventFunction> { }

    /// <summary>
    /// UGUI下拉选择框脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Dropdown))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIDropdownScriptEvent : BaseScriptEvent<EUGUIDropdownScriptEventType, UGUIDropdownScriptEventFunction, UGUIDropdownScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI下拉选择框脚本事件";

        /// <summary>
        /// 下拉框
        /// </summary>
        public Dropdown dropdown { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            dropdown = gameObject.GetComponent<Dropdown>();
            if (dropdown)
            {
                dropdown.onValueChanged.AddListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveListener(this.OnValueChanged);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIDropdownScriptEventType.Start);
        }

        /// <summary>
        /// 当值变换后
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(int obj)
        {
            ExecuteScriptEvent(EUGUIDropdownScriptEventType.OnValueChanged, obj.ToString());
        }
    }
}
