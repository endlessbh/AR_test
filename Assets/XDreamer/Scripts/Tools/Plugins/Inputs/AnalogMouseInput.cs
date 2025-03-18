using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 鼠标输入命令枚举
    /// </summary>
    public enum EMouseInputCmd
    {
        /// <summary>
        /// 左键按下
        /// </summary>
        [Name("左键按下")]
        LeftPressed,

        /// <summary>
        /// 左键弹起
        /// </summary>
        [Name("左键弹起")]
        LeftRelease,

        /// <summary>
        /// 左键按下弹起切换
        /// </summary>
        [Name("左键按下弹起切换")]
        LeftSwitch,

        /// <summary>
        /// 右键按下
        /// </summary>
        [Name("右键按下")]
        RightPressed,

        /// <summary>
        /// 右键弹起
        /// </summary>
        [Name("右键弹起")]
        RightRelease,

        /// <summary>
        /// 右键按下弹起切换
        /// </summary>
        [Name("右键按下弹起切换")]
        RightSwitch,
    }

    /// <summary>
    /// 鼠标输入命令
    /// </summary>
    [Serializable]
    public class MouseInputCmd : Cmd<EMouseInputCmd> { }

    /// <summary>
    /// 鼠标输入命令列表
    /// </summary>
    [Serializable]
    public class MouseInputCmds : Cmds<EMouseInputCmd, MouseInputCmd> { }

    /// <summary>
    /// 鼠标输入组件提供器
    /// </summary>
    [Serializable]
    public class MouseInputComponentProvider : ComponentProvider<MouseInput> { }

    /// <summary>
    /// 模拟鼠标输入
    /// </summary>
    [Name("模拟鼠标输入")]
    [Tool(InteractionCategory.InteractorInput, rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public class AnalogMouseInput : AnalogRayInput<MouseInput, MouseInputComponentProvider>
    {
        /// <summary>
        /// 鼠标输入命令列表
        /// </summary>
        [Name("鼠标输入命令列表")]
        [OnlyMemberElements]
        public MouseInputCmds _mouseInputCmds = new MouseInputCmds();

        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _mouseInputCmds.cmdNames;

        internal List<AnalogMouseInputSource> analogMouseInputSources = new List<AnalogMouseInputSource>();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _mouseInputCmds.Reset();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var left = leftPressed || leftPressedTmp;
            var right = rightPressed || rightPressedTmp;
            leftPressedTmp = false;
            rightPressedTmp = false;
            foreach (var item in analogMouseInputSources)
            {
                if (item.TryGetMouseInput(out var l, out var r))
                {
                    left |= l;
                    right |= r;
                }
            }
            AnalogInternal(left, right);
        }

        private void AnalogInternal(bool leftPressed, bool rightPressed)
        {
            if (_rayGenerater.TryGetRay(out var ray))
            {
                foreach (var component in _componentProvider.GetComponents())
                {
                    if (component)
                    {
                        component.AnalogMouseInput(this, leftPressed, rightPressed, ray);
                    }
                }
            }
        }

        private bool leftPressedTmp = false;
        private bool rightPressedTmp = false;

        /// <summary>
        /// 模拟鼠标左右键
        /// </summary>
        /// <param name="leftPressed"></param>
        /// <param name="rightPressed"></param>
        public void Analog(bool leftPressed, bool rightPressed)
        {
            this.leftPressedTmp = leftPressed;
            this.rightPressedTmp = rightPressed;
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => true;

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_mouseInputCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EMouseInputCmd.LeftPressed: leftPressed = true; return EInteractResult.Finished;
                    case EMouseInputCmd.LeftRelease: leftPressed = false; return EInteractResult.Finished;
                    case EMouseInputCmd.LeftSwitch: leftPressed = !leftPressed; return EInteractResult.Finished;
                    case EMouseInputCmd.RightPressed: rightPressed = true; return EInteractResult.Finished;
                    case EMouseInputCmd.RightRelease: rightPressed = false; return EInteractResult.Finished;
                    case EMouseInputCmd.RightSwitch: rightPressed = !rightPressed; return EInteractResult.Finished;
                }
            }
            return base.OnInteract(interactData);
        }

        /// <summary>
        /// 左键按下
        /// </summary>
        public bool leftPressed { get; set; } = false;

        /// <summary>
        /// 右键按下
        /// </summary>
        public bool rightPressed { get; set; } = false;
    }

    /// <summary>
    /// 模拟鼠标输入源
    /// </summary>
    [RequireComponent(typeof(AnalogMouseInput))]
    [Tool(InteractionCategory.InteractorInput, nameof(AnalogMouseInput), rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public abstract class AnalogMouseInputSource : InteractProvider
    {
        private AnalogMouseInput _analogMouseInput;

        /// <summary>
        /// 鼠标模拟输入
        /// </summary>
        public AnalogMouseInput analogMouseInput => this.XGetComponent<AnalogMouseInput>(ref _analogMouseInput);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            analogMouseInput.analogMouseInputSources.Add(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            analogMouseInput.analogMouseInputSources.Remove(this);
        }

        /// <summary>
        /// 尝试获取鼠标输入左右键
        /// </summary>
        /// <param name="leftPressed"></param>
        /// <param name="rightPressed"></param>
        /// <returns></returns>
        public abstract bool TryGetMouseInput(out bool leftPressed, out bool rightPressed);


    }
}
