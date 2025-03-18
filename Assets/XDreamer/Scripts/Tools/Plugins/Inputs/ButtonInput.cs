using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 按钮输入
    /// </summary>
    [Name("按钮输入")]
    [Tool(InteractionCategory.InteractorInput, rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public sealed class ButtonInput : BaseInput<ButtonInput.ButtonCmd>
    {
        /// <summary>
        /// 按钮命令列表
        /// </summary>
        [Name("按钮命令列表")]
        public ButtomCmds _buttonCmds = new ButtomCmds();

        /// <summary>
        /// 命令触发器列表
        /// </summary>
        protected override IEnumerable<ButtonCmd> inputCmds => _buttonCmds._cmds;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (var item in _buttonCmds._cmds)
            {
                item.AddButtonListener();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var item in _buttonCmds._cmds)
            {
                item.RemoveButtonListener();
            }
        }

        /// <summary>
        /// 按钮命令列表
        /// </summary>
        [Serializable]
        public class ButtomCmds : Cmds<ButtonCmd> { }

        /// <summary>
        /// 按钮命令
        /// </summary>
        [Serializable]
        public class ButtonCmd : InputCmd
        {
            /// <summary>
            /// 启用
            /// </summary>
            [Name("启用")]
            [DynamicLabel]
            public bool _enabled = true;

            /// <summary>
            /// 启用
            /// </summary>
            public override bool enabled => _enabled;

            [Name("按钮")]
            [DynamicLabel]
            public Button _button;

            private bool _isTrigger = false;

            /// <summary>
            /// 是否触发
            /// </summary>
            public override bool needInvokeInteract
            {
                get
                {
                    try
                    {
                        return _isTrigger;
                    }
                    finally
                    {
                        _isTrigger = false;
                    }
                }
            }

            /// <summary>
            /// 能否交互
            /// </summary>
            /// <param name="interactData"></param>
            /// <returns></returns>
            public override bool CanInteract(InteractData interactData) => true;

            private Button registerButton;

            /// <summary>
            /// 添加按钮监听
            /// </summary>
            public void AddButtonListener()
            {
                if (_button)
                {
                    registerButton = _button;
                    registerButton.onClick.AddListener(OnButtonClick);
                }
            }

            /// <summary>
            /// 移除按钮监听
            /// </summary>
            public void RemoveButtonListener()
            {
                if (registerButton)
                {
                    registerButton.onClick.RemoveListener(OnButtonClick);
                }
            }

            private void OnButtonClick()
            {
                if (enabled)
                {
                    _isTrigger = true;
                }
            }

            /// <summary>
            /// 获取射线：在按钮上不需要再产生射线
            /// </summary>
            /// <param name="ray"></param>
            /// <returns></returns>
            public override Ray? GetRay()
            {
                return default;
            }
        }
    }
}
