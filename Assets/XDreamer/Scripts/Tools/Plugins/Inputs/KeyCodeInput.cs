using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginTools.Inputs.KeyCodeInput;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 键码输入
    /// </summary>
    [Name("键码输入")]
    [Tool(InteractionCategory.InteractorInput, rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public sealed class KeyCodeInput : RayInput<KeyCodeCmd, KeyCodeCmds>
    {
        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="keyCode">键码</param>
        /// <param name="pressed">按下</param>
        /// <param name="rayOrgin">射线原点</param>
        /// <param name="rayDirection">射线方向</param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, KeyCode keyCode, bool pressed, Vector3 rayOrgin, Vector3 rayDirection)
        {
            AnalogKeyCodeInput(sender, keyCode, pressed, new Ray(rayOrgin, rayDirection));
        }

        /// <summary>
        /// 模拟键盘输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="keyCode">键码</param>
        /// <param name="pressed">按下</param>
        /// <param name="ray">射线</param>
        public void AnalogKeyCodeInput(UnityEngine.Object sender, KeyCode keyCode, bool pressed, Ray? ray = null)
        {
            if (!sender) return;

            analogKeyCodeDatas.Add((sender, keyCode, pressed, ray));
        }

        private List<(UnityEngine.Object, KeyCode, bool, Ray?)> analogKeyCodeDatas = new List<(UnityEngine.Object, KeyCode, bool, Ray?)>();

        /// <summary>
        /// 检测命令集合是否执行
        /// </summary>
        protected override void OnCheckCmds()
        {
            base.OnCheckCmds();

            analogKeyCodeDatas.Clear();
        }

        /// <summary>
        /// 检测命令是否执行
        /// </summary>
        /// <param name="rayCmd"></param>
        protected override void OnCheckCmd(KeyCodeCmd rayCmd)
        {
            base.OnCheckCmd(rayCmd);

            if (rayCmd.CanAnalogInput())
            {
                foreach (var data in analogKeyCodeDatas)
                {
                    if (rayCmd.TryHandleAnalogInput(data, this, out var rayInteractData))
                    {
                        TryInteract(rayInteractData, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 创建交互数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override InteractData CreateInteractData(string cmdName, params InteractObject[] interactables)
        {
            return new KeyCodeRayInteractData(cmdName, this, interactables);
        }

        /// <summary>
        /// 创建键码射线交互数据
        /// </summary>
        /// <param name="cmdTrigger"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override RayInteractData CreateRayInteractData(UnityEngine.Object sender, KeyCodeCmd keyCodeCmd, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables)
        {
            return new KeyCodeRayInteractData(keyCodeCmd._keyCode, keyCodeCmd.sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables);
        }

        /// <summary>
        /// 键码射线交互数据
        /// </summary>
        public class KeyCodeRayInteractData : RayInteractData
        {
            /// <summary>
            /// 键码
            /// </summary>
            public KeyCode keyCode { get; private set; }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            private KeyCodeRayInteractData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public KeyCodeRayInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables) { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="keyCode"></param>
            /// <param name="sender"></param>
            /// <param name="ray"></param>
            /// <param name="raycastHit"></param>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public KeyCodeRayInteractData(KeyCode keyCode, UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables)
            {
                this.keyCode = keyCode;
            }

            /// <summary>
            /// 创建实例
            /// </summary>
            /// <returns></returns>
            protected override InteractData CreateInstance() => new KeyCodeRayInteractData();

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="interactData"></param>
            public override void CopyTo(InteractData interactData)
            {
                base.CopyTo(interactData);

                if (interactData is KeyCodeRayInteractData keyCodeRayInteractData)
                {
                    keyCodeRayInteractData.keyCode = keyCode;
                }
            }
        }

        /// <summary>
        /// 键码命令列表
        /// </summary>
        [Serializable]
        public class KeyCodeCmds : Cmds<KeyCodeCmd> { }

        /// <summary>
        /// 键码命令
        /// </summary>
        [Serializable]
        public class KeyCodeCmd : RayCmd
        {
            [Name("键码")]
            public KeyCode _keyCode = KeyCode.KeypadEnter;

            /// <summary>
            /// 按下
            /// </summary>
            protected override bool Pressed() => XInput.GetKeyDown(_keyCode);

            /// <summary>
            /// 保持
            /// </summary>
            protected override bool Keep() => XInput.GetKey(_keyCode);

            /// <summary>
            /// 弹起
            /// </summary>
            protected override bool Release() => XInput.GetKeyUp(_keyCode);

            #region 模拟输入

            private Dictionary<UnityEngine.Object, AnalogCmd> analogCmdMap = new Dictionary<UnityEngine.Object, AnalogCmd>();

            /// <summary>
            /// 尝试处理交互输入
            /// </summary>
            /// <param name="data"></param>
            /// <param name="keyCodeInput"></param>
            /// <param name="rayInteractData"></param>
            /// <returns></returns>
            public bool TryHandleAnalogInput((UnityEngine.Object, KeyCode, bool, Ray?) data, KeyCodeInput keyCodeInput, out RayInteractData rayInteractData)
            {
                // 是否匹配键码输入
                if (_keyCode == data.Item2)
                {
                    var sender = data.Item1;
                    var analogCmd = GetAnalogCmd(sender);

                    // 更新模拟命令对象的按压状态
                    var ray = data.Item4;
                    analogCmd.SetPressState(data.Item3, ray);

                    // 判断是否需要执行交互
                    if (analogCmd.NeedInvokeInteract(this))
                    {
                        rayInteractData = keyCodeInput.CreateRayInteractData(sender, this, cmdName, ray);

                        return analogCmd.CanInteract(rayInteractData, this);
                    }
                }

                rayInteractData = default;
                return false;
            }

            private AnalogCmd GetAnalogCmd(UnityEngine.Object sender)
            {
                if (!analogCmdMap.TryGetValue(sender, out var analogCmd))
                {
                    analogCmdMap[sender] = analogCmd = new AnalogCmd();
                }
                return analogCmd;
            }

            #endregion
        }
    }
}
