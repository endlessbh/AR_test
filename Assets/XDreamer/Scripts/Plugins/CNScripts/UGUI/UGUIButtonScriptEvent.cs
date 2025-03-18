using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI按钮脚本事件类型 
    /// </summary>
    [Name("UGUI按钮脚本事件类型")]
    public enum EUGUIButtonScriptEventType
    {
        /// <summary>
        /// 选择时
        /// </summary>
        [Name("点击时执行")]
        OnClick,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI按钮脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI按钮脚本事件函数")]
    public class UGUIButtonScriptEventFunction : EnumFunction<EUGUIButtonScriptEventType> { }

    /// <summary>
    /// UGUI按钮脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("UGUI按钮脚本事件函数集合")]
    public class UGUIButtonScriptEventFunctionCollection : EnumFunctionCollection<EUGUIButtonScriptEventType, UGUIButtonScriptEventFunction> { }

    /// <summary>
    /// UGUI按钮脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Button))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    public class UGUIButtonScriptEvent : BaseScriptEvent<EUGUIButtonScriptEventType, UGUIButtonScriptEventFunction, UGUIButtonScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI按钮脚本事件";

        /// <summary>
        /// 按钮
        /// </summary>
        public Button button { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            button = gameObject.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(this.OnClick);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (button)
            {
                button.onClick.RemoveListener(this.OnClick);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EUGUIButtonScriptEventType.Start);
        }

        /// <summary>
        /// 当点击
        /// </summary>
        public void OnClick()
        {
            //Log.Info(gameObject.name);
            ExecuteScriptEvent(EUGUIButtonScriptEventType.OnClick);
        }
    }
}
