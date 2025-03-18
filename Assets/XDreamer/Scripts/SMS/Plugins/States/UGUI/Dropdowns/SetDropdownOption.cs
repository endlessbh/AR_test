using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 设置下拉框选项：设置下拉框选项可用于设置下拉框的当前选择内容
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SetDropdownOption))]
    [Tip("设置下拉框选项可用于设置下拉框的当前选择内容", "The set drop-down box option allows you to set the current selection of the drop-down box")]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    public class SetDropdownOption : BaseSetDropdownOption<SetDropdownOption>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置下拉框选项";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SetDropdownOption))]
        [Tip("设置下拉框选项可用于设置下拉框的当前选择内容", "The set drop-down box option allows you to set the current selection of the drop-down box")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 设置值类型
        /// </summary>
        [Name("设置值类型")]
        [EnumPopup]
        public EDropdownValueType _setValueType = EDropdownValueType.Value;

        /// <summary>
        /// 设置值类型
        /// </summary>
        public EDropdownValueType setValueType => _setValueType;

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        [HideInSuperInspector(nameof(_setValueType), EValidityCheckType.NotEqual, EDropdownValueType.Value)]
        public int _value = 0;

        /// <summary>
        /// 值
        /// </summary>
        public int value => _value;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [HideInSuperInspector(nameof(_setValueType), EValidityCheckType.NotEqual, EDropdownValueType.Text)]
        public string _text = "";

        /// <summary>
        /// 文本
        /// </summary>
        public string text => _text;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (!dropdown) return;
            switch (_setValueType)
            {
                case EDropdownValueType.Value:
                    {
                        dropdown.value = value;
                        break;
                    }
                case EDropdownValueType.Text:
                    {
                        dropdown.SetTextValue(text);
                        break;
                    }
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (!dropdown) return "";
            switch (setValueType)
            {
                case EDropdownValueType.Value: return dropdown.name + ".值=" + value;
                case EDropdownValueType.Text: return dropdown.name + ".文本=" + text;
                default: return dropdown.name;
            }
        }
    }
}
