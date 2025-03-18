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
    /// UGUI文本输入框脚本事件类型 
    /// </summary>
    [Name("UGUI文本输入框脚本事件类型")]
    public enum EUGUIInputFieldScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("值变动时执行")]
        OnValueChanged,

        /// <summary>
        /// 完成修改时
        /// </summary>
        [Name("完成修改时执行")]
        EndEdit,

        /// <summary>
        /// 完成修改时
        /// </summary>
        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI文本输入框脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI文本输入框脚本事件函数")]
    public class UGUIInputFieldScriptEventFunction : EnumFunction<EUGUIInputFieldScriptEventType> { }

    /// <summary>
    /// UGUI文本输入框脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("UGUI文本输入框脚本事件函数集合")]
    public class UGUIInputFieldScriptEventFunctionCollection : EnumFunctionCollection<EUGUIInputFieldScriptEventType, UGUIInputFieldScriptEventFunction> { }

    /// <summary>
    /// UGUI文本输入框脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(InputField))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIInputFieldScriptEvent : BaseScriptEvent<EUGUIInputFieldScriptEventType, UGUIInputFieldScriptEventFunction, UGUIInputFieldScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标记
        /// </summary>
        public const string Title = "UGUI文本输入框脚本事件";

        /// <summary>
        /// 输入框
        /// </summary>
        public InputField inputField { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            inputField = gameObject.GetComponent<InputField>();
            if (inputField)
            {
                inputField.onValueChanged.AddListener(this.OnValueChanged);
                inputField.onEndEdit.AddListener(this.EndEdit);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (inputField)
            {
                inputField.onValueChanged.RemoveListener(this.OnValueChanged);
                inputField.onEndEdit.RemoveListener(this.EndEdit);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIInputFieldScriptEventType.Start);
        }

        /// <summary>
        /// 当值变化
        /// </summary>
        /// <param name="obj"></param>
        public void OnValueChanged(string obj)
        {
            ExecuteScriptEvent(EUGUIInputFieldScriptEventType.OnValueChanged, obj.ToString());
        }

        /// <summary>
        /// 完成修改
        /// </summary>
        /// <param name="obj"></param>
        public void EndEdit(string obj)
        {
            ExecuteScriptEvent(EUGUIInputFieldScriptEventType.EndEdit, obj.ToString());
        }
    }
}
