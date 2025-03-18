using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using UnityEngine.XR;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.Extension.Interactions.Tools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 基础模拟控制器IO：模拟控制器的输入输出的基类
    /// </summary>
    [Name("基础模拟控制器IO")]
    [Tip("模拟控制器的输入输出的基类", "Base class for analog controller input and output")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    public abstract class BaseAnalogControllerIO : Interactor, IAnalogControllerIO
    {
        /// <summary>
        /// 模拟控制器
        /// </summary>
        [Name("模拟控制器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AnalogController _analogController;

        /// <summary>
        /// 模拟控制器
        /// </summary>
        public AnalogController analogController => this.XGetComponentInParent(ref _analogController);

        /// <summary>
        /// 注册IO
        /// </summary>
        public void RegistIO()
        {
            if (analogController)
            {
                analogController.RegistIO(this);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var analogController = this.analogController;
            if (analogController)
            {
                analogController.RegistIO(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            var analogController = this.analogController;
            if (analogController)
            {
                analogController.UnregistIO(this);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            RegistIO();
        }
    }

    /// <summary>
    /// 基础模拟控制器IO接口
    /// </summary>
    public interface IAnalogControllerIO { }

    /// <summary>
    /// 姿态IO接口
    /// </summary>
    public interface IPoseIO : IAnalogControllerIO
    {
#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation);

#else
        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        PoseDataFlags TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation);

#endif

#endif
    }

    /// <summary>
    /// 交互IO接口
    /// </summary>
    public interface IInteractIO : IAnalogControllerIO
    {
        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfActivate(AnalogController analogController);

        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfSelect(AnalogController analogController);

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        bool IsPressedOfUI(AnalogController analogController);
    }

    /// <summary>
    /// 触觉脉冲IO接口
    /// </summary>
    public interface IHapticImpulseIO : IAnalogControllerIO
    {
        /// <summary>
        /// 发送触觉脉冲
        /// </summary>
        /// <param name="amplitude">播放脉冲的振幅（从0.0到1.0）</param>
        /// <param name="duration">播放触觉脉冲的持续时间（秒）</param>
        /// <returns></returns>
        bool SendHapticImpulse(AnalogController analogController, float amplitude, float duration);
    }
}
