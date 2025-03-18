using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System;
using System.Linq;
using XCSJ.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using XCSJ.PluginTools.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginTools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 模拟控制器
    /// </summary>
    [Name("模拟控制器")]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(ToolsManager))]
    public class AnalogController
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : XRBaseController
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// 姿态IO
        /// </summary>
        [Header("输入输出IO")]
        [Name("姿态IO")]
        [Tip("提供姿态IO", "Provide attitude IO")]
        [ComponentPopup(typeof(IPoseIO), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public BaseAnalogControllerIO _poseIO;

        /// <summary>
        /// 交互IO
        /// </summary>
        [Name("交互IO")]
        [Tip("提供交互IO", "Provide interactive IO")]
        [ComponentPopup(typeof(IInteractIO), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public List<BaseAnalogControllerIO> _interactIOs = new List<BaseAnalogControllerIO>();

        /// <summary>
        /// 触觉脉冲IO
        /// </summary>
        [Name("触觉脉冲IO")]
        [Tip("提供触觉脉冲IO", "Provide tactile pulse IO")]
        [ComponentPopup(typeof(IHapticImpulseIO), searchFlags = ESearchFlags.Default, overrideLabel = true)]
        public List<BaseAnalogControllerIO> _hapticImpulseIOs = new List<BaseAnalogControllerIO>();

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        [Name("模拟鼠标输入")]
        [ComponentPopup]
        public AnalogMouseInput _analogMouseInput;

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        public AnalogMouseInput analogMouseInput => this.XGetComponent(ref _analogMouseInput);

        /// <summary>
        /// 确保模拟输入组件的有效性
        /// </summary>
        private void EnsureAnalogMouseInputValie()
        {
            if (!analogMouseInput) _analogMouseInput = this.XGetOrAddComponent<AnalogMouseInput>();
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected void OnDestroy()
        {
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            EnsureValidIO();
            EnsureAnalogMouseInputValie();
        }

#endif

        private void OnValidate()
        {
            EnsureValidIO();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            EnsureAnalogMouseInputValie();

            var ios = GetComponents<BaseAnalogControllerIO>();
            this.XModifyProperty(() =>
            {
                foreach (var io in ios) RegistIO(io);
            });
            EnsureValidIO();
        }

        /// <summary>
        /// 注册IO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="io"></param>
        public void RegistIO<T>(T io) where T : BaseAnalogControllerIO
        {
            if (!io) return;
            if (io is IPoseIO)
            {
                if (_poseIO == io) { }
                else if (!_poseIO) _poseIO = io;
                else
                {
                    Debug.LogWarningFormat("游戏对象[{0}]上模拟控制器组件，已注册有效姿态IO[{1}]组件,无法再注册[{2}]组件！",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.GameObjectComponentToString(_poseIO),
                        CommonFun.GameObjectComponentToString(io));
                }
            }
            if (io is IInteractIO)
            {
                _interactIOs.AddWithDistinct(io);
            }
            if (io is IHapticImpulseIO)
            {
                _hapticImpulseIOs.AddWithDistinct(io);
            }
        }

        /// <summary>
        /// 取消注册IO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="io"></param>
        public void UnregistIO<T>(T io) where T : BaseAnalogControllerIO
        {
            if (!io) return;
            if (io is IPoseIO)
            {
                if (io == _poseIO) _poseIO = default;
            }
            if (io is IInteractIO)
            {
                _interactIOs.Remove(io);
            }
            if (io is IHapticImpulseIO)
            {
                _hapticImpulseIOs.Remove(io);
            }
        }

        private void EnsureValidIO()
        {
            this.XModifyProperty(() =>
            {
                GetValid<IPoseIO>(ref _poseIO);
                GetValid<IInteractIO>(_interactIOs);
                GetValid<IHapticImpulseIO>(_hapticImpulseIOs);
            });
        }

        private void GetValid<T>(List<BaseAnalogControllerIO> IOs) where T : IAnalogControllerIO
        {
            IOs.RemoveAll(io => !io || !(io is T));
        }

        private void GetValid<T>(ref BaseAnalogControllerIO io) where T : IAnalogControllerIO
        {
            if (io && !(io is T))
            {
                io = default;
            }
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 更新跟踪输入：更新当前模拟控制器的姿态，即位置与旋转
        /// </summary>
        /// <param name="controllerState"></param>
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            if (_poseIO is IPoseIO validIO)
            {
                controllerState.inputTrackingState = validIO.TryGetPose(this, out controllerState.position, out controllerState.rotation);
            }
            else
            {
                controllerState.inputTrackingState = InputTrackingState.None;
            }
#else
            if (_poseIO is IPoseIO validIO)
            {
                controllerState.poseDataFlags = validIO.TryGetPose(this, out controllerState.position, out controllerState.rotation);
            }
            else
            {
                controllerState.poseDataFlags = PoseDataFlags.NoData;
            }
#endif
        }

        /// <summary>
        /// 更新输入：更新交互输入，包括选择，激活，UI交互
        /// </summary>
        /// <param name="controllerState"></param>
        protected override void UpdateInput(XRControllerState controllerState)
        {
            var active = false;
            var select = false;
            var ui = false;
            foreach (var p in _interactIOs)
            {
                if (p is IInteractIO provider)
                {
                    active = active || provider.IsPressedOfActivate(this);
                    select = select || provider.IsPressedOfSelect(this);
                    ui = ui || provider.IsPressedOfUI(this);
                }
            }

            controllerState.ResetFrameDependentStates();
            controllerState.activateInteractionState.SetFrameState(active);
            controllerState.selectInteractionState.SetFrameState(select);
            controllerState.uiPressInteractionState.SetFrameState(ui);

            if (_analogMouseInput) _analogMouseInput.Analog(select, active);
        }

        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="amplitude">播放脉冲的振幅（从0.0到1.0）</param>
        /// <param name="duration">播放触觉脉冲的持续时间（秒）</param>
        /// <returns></returns>
        public override bool SendHapticImpulse(float amplitude, float duration)
        {
            var send = false;
            foreach (var io in _hapticImpulseIOs)
            {
                if (io is IHapticImpulseIO validIO)
                {
                    send = validIO.SendHapticImpulse(this, amplitude, duration) || send;
                }
            }
            return send;
        }

#endif
    }
}
