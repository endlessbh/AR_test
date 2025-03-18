using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginTools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginHTCVive.Tools
{
    /// <summary>
    /// 基于Vive手柄的连续移动提供者
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Continuous Move Provider (Vive)")]
    [Name("基于Vive手柄的连续移动提供者")]
    [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
    [Tip("在当前游戏对象上创建一个[基于HTCVive手柄的连续移动提供者]组件对象", "Create a [ContinuousMoveProviderBase] component object on the current game object")]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [RequireManager(typeof(HTCViveManager), typeof(ToolsManager))]
    [Owner(typeof(HTCViveManager))]
    public class ViveContinuousMoveProvider
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : ContinuousMoveProviderBase
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// Vive手柄2D轴
        /// </summary>
        [Name("Vive手柄2D轴")]
        public ViveControllerAxis2D _viveControllerAxis2D = new ViveControllerAxis2D();

#if XDREAMER_XR_INTERACTION_TOOLKIT
        /// <summary>
        /// 读取输入
        /// </summary>
        /// <returns></returns>
        protected override Vector2 ReadInput()
        {
            _viveControllerAxis2D.TryGetAxis2DValue(out var value);
            return value;
        }
#endif
    }
}
