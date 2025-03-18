using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入处理器
    /// </summary>
    [Name("输入处理器")]
    [Serializable]
    public class InputHandler
    {
        /// <summary>
        /// 在任意UI上时输入处理规则：鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时的输入处理规则；
        /// </summary>
        [Name("在任意UI上时输入处理规则")]
        [Tip("鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时的输入处理规则；", "Input processing rules when the mouse (gesture) is currently on any GUI (i.e. im GUI) or ugui;")]
        [EnumPopup]
        public ERule _ruleWhenOnAnyUI = ERule.NewInput;

        /// <summary>
        /// 在任意UI上时输入
        /// </summary>
        [Name("在任意UI上时输入")]
        [HideInSuperInspector(nameof(_ruleWhenOnAnyUI), EValidityCheckType.NotEqual, ERule.NewInput)]
        [EnumPopup]
        public EInput _inputWhenOnAnyUI = EInput.VirtualInput;

        /// <summary>
        /// 有触摸时输入处理规则：触摸数大于0时的输入处理规则；
        /// </summary>
        [Name("有触摸时输入处理规则")]
        [Tip("触摸数大于0时的输入处理规则；", "Input processing rules when the number of touches is greater than 0;")]
        [EnumPopup]
        public ERule _ruleWhenHasTouch = ERule.NewInput;

        /// <summary>
        /// 有触摸时输入
        /// </summary>
        [Name("有触摸时输入")]
        [HideInSuperInspector(nameof(_ruleWhenHasTouch), EValidityCheckType.NotEqual, ERule.NewInput)]
        [EnumPopup]
        public EInput _inputWhenHasTouch = EInput.VirtualInput;

        /// <summary>
        /// 输入：默认使用的输入
        /// </summary>
        [Name("输入")]
        [Tip("默认使用的输入", "Default input")]
        [EnumPopup]
        public EInput _input = EInput.XInput;

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <returns></returns>
        public IInput GetInput()
        {
            if (CommonFun.IsOnAnyUI())
            {
                switch (_ruleWhenHasTouch)
                {
                    case ERule.Return: return default;
                    case ERule.NewInput:
                        {
                            return _inputWhenOnAnyUI.GetInput();
                        }
                    case ERule.None:
                    default:
                        {
                            break;
                        }
                }
            }

            if (Input.touchCount > 0)
            {
                switch (_ruleWhenHasTouch)
                {
                    case ERule.Return: return default;
                    case ERule.NewInput:
                        {
                            return _inputWhenHasTouch.GetInput();
                        }
                    case ERule.None:
                    default:
                        {
                            break;
                        }
                }
            }

            return _input.GetInput();
        }

        /// <summary>
        /// 输入处理的规则枚举
        /// </summary>
        [Name("规则")]
        public enum ERule
        {
            /// <summary>
            /// 无：当前不做任何操作，并继续后续的输入处理；
            /// </summary>
            [Name("无")]
            [Tip("当前不做任何操作，并继续后续的输入处理；", "Do not do any operation at present, and continue the subsequent input processing;")]
            None,

            /// <summary>
            /// 返回：逻辑直接返回，不再做任何输入处理；
            /// </summary>
            [Name("返回")]
            [Tip("逻辑直接返回，不再做任何输入处理；", "The logic returns directly without any input processing;")]
            Return,

            /// <summary>
            /// 新输入：使用新的输入做后续的输入处理；
            /// </summary>
            [Name("新输入")]
            [Tip("使用新的输入做后续的输入处理；", "Use the new input for subsequent input processing;")]
            NewInput,
        }
    }

    /// <summary>
    /// 键码设置
    /// </summary>
    [Name("键码设置")]
    [Serializable]
    public class KeyCodesSetting
    {
        /// <summary>
        /// 输入
        /// </summary>
        [Name("输入")]
        public EInput _input = EInput.StandaloneInput;

        /// <summary>
        /// 键码
        /// </summary>
        [Name("键码")]
        public KeyCode _keyCode = KeyCode.None;

        /// <summary>
        /// 输入
        /// </summary>
        public IInput input => _input.GetInput();

        /// <summary>
        /// 是否被按压
        /// </summary>
        /// <returns></returns>
        public bool IsPressed() => input.GetKey(_keyCode);
    }
}
