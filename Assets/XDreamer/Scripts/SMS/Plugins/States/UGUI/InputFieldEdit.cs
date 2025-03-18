using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// 输入框编辑:输入框编辑组件是输入框发生输入事件的触发器。发生输入发生时，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(InputFieldEdit))]
    [XCSJ.Attributes.Icon(EIcon.InputField, index = 33606)]
    [Tip("输入框编辑组件是输入框发生输入事件的触发器。发生输入发生时，组件切换为完成态。", "The input box editing component is the trigger for input events in the input box. When an input occurs, the component switches to the completed state.")]
    public class InputFieldEdit : Trigger<InputFieldEdit>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入框编辑";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(InputFieldEdit))]
        [Tip("输入框编辑组件是输入框发生输入事件的触发器。发生输入发生时，组件切换为完成态。", "The input box editing component is the trigger for input events in the input box. When an input occurs, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateInputFieldEdit(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(InputField))]
        [Readonly(EEditorMode.Runtime)]
        public InputField inputField;

        [Name("提交触发")]
        [Tip("勾选，则InputField控件内容编辑时触发；否则，InputField控件内容编辑完成时触发；", "If checked, it will be triggered when inputfield control content is edited; Otherwise, it will be triggered when the content of inputfield control is edited;")]
        public bool changeOrEdit = false;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (inputField)
            {
                if(changeOrEdit)
                    inputField.onEndEdit.AddListener(OnDropdownSwitch);
                else
                    inputField.onValueChanged.AddListener(OnDropdownSwitch);
            }
        }

        public override void OnExit(StateData data)
        {
            if (inputField)
            {
                if (changeOrEdit)
                    inputField.onEndEdit.RemoveListener(OnDropdownSwitch);
                else
                    inputField.onValueChanged.RemoveListener(OnDropdownSwitch);
            }
            base.OnExit(data);
        }

        private void OnDropdownSwitch(string str)
        {
            //if (val == triggerValue)
                finished = true;
        }

        public override bool DataValidity()
        {
            return inputField;
        }

        public override string ToFriendlyString() => inputField ? inputField.name : "";
    }
}
