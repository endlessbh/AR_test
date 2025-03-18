using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Base;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.PluginTools.Inputs
{
    /// <summary>
    /// 鼠标输入
    /// </summary>
    [Name("鼠标输入")]
    [Tool(InteractionCategory.InteractorInput, rootType = typeof(ToolsManager), index = InteractionCategory.InteractInputIndex)]
    public sealed class MouseInput : RayInput<MouseCmd, MouseCmds>
    {
        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="mouseButton">鼠标按键</param>
        /// <param name="pressed">按下</param>
        /// <param name="rayOrgin">射线原点</param>
        /// <param name="rayDirection">射线方向</param>
        public void AnalogMouseInput(UnityEngine.Object sender, bool leftPressed, bool rightPressed, Vector3 rayOrgin, Vector3 rayDirection)
        {
            AnalogMouseInput(sender, leftPressed, rightPressed, new Ray(rayOrgin, rayDirection));
        }

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="mouseButton">鼠标按键</param>
        /// <param name="pressed">按下</param>
        /// <param name="ray">射线</param>
        public void AnalogMouseInput(UnityEngine.Object sender, bool leftPressed, bool rightPressed, Ray? ray = null)
        {
            if (!sender) return;

            analogMouseDatas.Add((sender, leftPressed, rightPressed, ray));
        }

        private List<(UnityEngine.Object, bool, bool, Ray?)> analogMouseDatas = new List<(UnityEngine.Object, bool, bool, Ray?)>();

        /// <summary>
        /// 检查命令回调
        /// </summary>
        protected override void OnCheckCmds()
        {
            base.OnCheckCmds();

            analogMouseDatas.Clear();
        }

        /// <summary>
        /// 检查命令
        /// </summary>
        /// <param name="rayCmd"></param>
        protected override void OnCheckCmd(MouseCmd rayCmd)
        {
            base.OnCheckCmd(rayCmd);

            if (rayCmd.CanAnalogInput())
            {
                foreach (var data in analogMouseDatas)
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
            return new MouseRayInteractData(cmdName, this, interactables);
        }

        /// <summary>
        /// 创建鼠标射线交互数据
        /// </summary>
        /// <param name="rayCmd"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override RayInteractData CreateRayInteractData(UnityEngine.Object sender, MouseCmd rayCmd, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables)
        {
            return new MouseRayInteractData(rayCmd._mouseButton, XInput.mousePosition, sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, interactor, interactables);
        }

        /// <summary>
        /// 鼠标射线交互数据
        /// </summary>
        public class MouseRayInteractData : RayInteractData
        {
            /// <summary>
            /// 鼠标按钮
            /// </summary>
            public EMouseButton mouseButton { get; private set; }

            /// <summary>
            /// 鼠标位置
            /// </summary>
            public Vector2 mousePosition { get; private set; }

            public MouseRayInteractData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public MouseRayInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables) { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="cmdName"></param>
            /// <param name="parent"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public MouseRayInteractData(string cmdName, InteractData parent, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, parent, interactor, interactables) { }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="mouseButton"></param>
            /// <param name="mousePosition"></param>
            /// <param name="sender"></param>
            /// <param name="ray"></param>
            /// <param name="raycastHit"></param>
            /// <param name="cmdName"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public MouseRayInteractData(EMouseButton mouseButton, Vector2 mousePosition, UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(mouseButton, mousePosition, sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, null, interactor, interactables)
            {
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="mouseButton"></param>
            /// <param name="mousePosition"></param>
            /// <param name="sender"></param>
            /// <param name="ray"></param>
            /// <param name="raycastHit"></param>
            /// <param name="cmdName"></param>
            /// <param name="parent"></param>
            /// <param name="interactor"></param>
            /// <param name="interactables"></param>
            public MouseRayInteractData(EMouseButton mouseButton, Vector2 mousePosition, UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractData parent, InteractObject interactor, params InteractObject[] interactables) : base(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, parent, interactor, interactables)
            {
                this.mouseButton = mouseButton;
                this.mousePosition = mousePosition;
            }

            /// <summary>
            /// 创建实例
            /// </summary>
            /// <returns></returns>
            protected override InteractData CreateInstance() => new MouseRayInteractData();

            /// <summary>
            /// 复制
            /// </summary>
            /// <param name="interactData"></param>
            public override void CopyTo(InteractData interactData)
            {
                base.CopyTo(interactData);

                if (interactData is MouseRayInteractData mouseRayInteractData)
                {
                    mouseRayInteractData.mouseButton = mouseButton;
                    mouseRayInteractData.mousePosition = mousePosition;
                }
            }
        }

        /// <summary>
        /// 鼠标命令列表
        /// </summary>
        [Serializable]
        public class MouseCmds : Cmds<MouseCmd> { }

        /// <summary>
        /// 鼠标命令
        /// </summary>
        [Serializable]
        public class MouseCmd : RayCmd
        {
            /// <summary>
            /// 鼠标按键
            /// </summary>
            [Name("鼠标按键")]
            [EnumPopup]
            public EMouseButton _mouseButton = EMouseButton.Left;

            /// <summary>
            /// 按下
            /// </summary>
            protected override bool Pressed() => _mouseButton.GetMouseButtonDown(XInput.input);

            /// <summary>
            /// 保持
            /// </summary>
            protected override bool Keep() => _mouseButton.GetMouseButton(XInput.input);

            /// <summary>
            /// 弹起
            /// </summary>
            protected override bool Release() => _mouseButton.GetMouseButtonUp(XInput.input);

            #region 模拟输入处理

            private Dictionary<UnityEngine.Object, AnalogCmd> analogCmdMap = new Dictionary<UnityEngine.Object, AnalogCmd>();

            /// <summary>
            /// 尝试处理模拟输入
            /// </summary>
            /// <param name="data"></param>
            /// <param name="mouseInput"></param>
            /// <param name="rayInteractData"></param>
            /// <returns></returns>
            public bool TryHandleAnalogInput((UnityEngine.Object, bool, bool, Ray?) data, MouseInput mouseInput, out RayInteractData rayInteractData)
            {
                // 是否匹配鼠标输入
                if (CanHandleAnalogInput())
                {
                    var sender = data.Item1;
                    var analogCmd = GetAnalogCmd(sender);

                    // 更新模拟命令对象的按压状态
                    var ray = data.Item4;
                    analogCmd.SetPressState(GetAnalogInputResult(data.Item2, data.Item3), ray);

                    // 判断是否需要执行交互
                    if (analogCmd.NeedInvokeInteract(this))
                    {
                        rayInteractData = mouseInput.CreateRayInteractData(sender, this, this.cmdName, ray);

                        return analogCmd.CanInteract(rayInteractData, this);
                    }
                }

                rayInteractData = default;
                return false;
            }

            private bool CanHandleAnalogInput()
            {
                return _mouseButton.GetMouseButtonInput(mouseButtonIndex =>
                {
                    switch (mouseButtonIndex)
                    {
                        case 0:
                        case 1: return true;
                        default: return false;
                    }
                });
            }

            private AnalogCmd GetAnalogCmd(UnityEngine.Object sender)
            {
                if (!analogCmdMap.TryGetValue(sender, out var analogCmd))
                {
                    analogCmdMap[sender] = analogCmd = new AnalogCmd();
                }
                return analogCmd;
            }

            private bool GetAnalogInputResult(bool leftPressed, bool rightPressed)
            {
                return _mouseButton.GetMouseButtonInput(mouseButtonIndex =>
                {
                    switch (mouseButtonIndex)
                    {
                        case 0: return leftPressed;
                        case 1: return rightPressed;
                        default: return false;
                    }
                });
            }

            #endregion
        }
    }

    /// <summary>
    /// 刚体记录器
    /// </summary>
    public class MouseInputRecorder : Recorder<MouseInput, MouseInputRecorder.Info>
    {
        /// <summary>
        /// 记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.GetComponent<MouseInput>());
        }

        /// <summary>
        /// 批量记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        public class Info : ISingleRecord<MouseInput>
        {
            /// <summary>
            /// 刚体
            /// </summary>
            [Json(exportString = true)]
            public MouseInput mouseInput;

            /// <summary>
            /// 组件
            /// </summary>
            [Json(exportString = true)]
            public Component component { get => mouseInput; set => mouseInput = value as MouseInput; }

            private List<RayCmd.EInputMode> inputModes = new List<RayCmd.EInputMode>();

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="mouseInput"></param>
            public void Record(MouseInput mouseInput)
            {
                this.mouseInput = mouseInput;

                inputModes.AddRange(mouseInput._cmds._cmds.Cast(m => m._inputMode));
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                for (int i = 0; i < inputModes.Count && i < mouseInput._cmds._cmds.Count; i++)
                {
                    mouseInput._cmds._cmds[i]._inputMode = inputModes[i];
                }
            }

        }
    }
}
