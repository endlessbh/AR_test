using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.PluginXGUI.States
{
    /// <summary>
    /// 显示对话框
    /// </summary>
    [ComponentMenu("XGUI/" + Title, typeof(XGUIManager))]
    [Name(Title, nameof(ShowDialogBox))]
    [Tip("", "")]
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [Owner(typeof(XGUIManager))]
    public class ShowDialogBox : Trigger<ShowDialogBox>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "显示对话框";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("XGUI", typeof(XGUIManager))]
        [StateComponentMenu("XGUI/" + Title, typeof(XGUIManager))]
        [Name(Title, nameof(ShowDialogBox))]
        [XCSJ.Attributes.Icon(EIcon.Chat)]
        public static State CreateButtonClick(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 使用全局对话框
        /// </summary>
        [Name("使用全局对话框")]
        public bool _useGlobalDialog = true;

        /// <summary>
        /// 对话框
        /// </summary>
        [Name("对话框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_useGlobalDialog), EValidityCheckType.True)]
        public DialogBox _dialogBox;

        private DialogBox dialogBoxBak;


        [Name("标题")]
        public StringPropertyValue _title = new StringPropertyValue();

        [Name("图标")]
        public SpritePropertyValue _icon = new SpritePropertyValue();

        [Name("描述")]
        public StringPropertyValue_TextArea _content = new StringPropertyValue_TextArea();

        public enum ETriggerRule
        {
            [Name("无")]
            None,

            [Name("结果相同")]
            ResultEqual,

            [Name("结果不相同")]
            ResultNotEqual,
        }

        [Name("触发规则")]
        [EnumPopup]
        public ETriggerRule _triggerRule = ETriggerRule.ResultEqual;

        [Name("结果")]
        public StringPropertyValue _result = new StringPropertyValue(nameof(DialogBox.Yes));

        private void OnDialogResult(DialogBox dialogBox, string result)
        {
            if (finished) return;
            if (dialogBox != dialogBoxBak) return;

            switch (_triggerRule)
            {
                case ETriggerRule.ResultEqual:
                    {
                        finished = result == _result.GetValue();
                        break;
                    }
                case ETriggerRule.ResultNotEqual:
                    {
                        finished = result != _result.GetValue();
                        break;
                    }
            }
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            DialogBox.onResult += OnDialogResult;
            if (_useGlobalDialog)
            {
                dialogBoxBak = XGUIHelper.dialogBox;
            }
            else
            {
                dialogBoxBak = _dialogBox;
            }
            if (dialogBoxBak)
            {
                dialogBoxBak.Show(_title.GetValue(), _content.GetValue(), _icon.GetValue());
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            DialogBox.onResult -= OnDialogResult;
        }

        public override bool DataValidity()
        {
            return _useGlobalDialog ? XGUIHelper.dialogBox : _dialogBox;
        }

        public override string ToFriendlyString() => _result.ToFriendlyString();
    }
}
