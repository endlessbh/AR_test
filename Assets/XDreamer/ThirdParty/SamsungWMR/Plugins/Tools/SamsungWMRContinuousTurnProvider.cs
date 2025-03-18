using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSamsungWMR.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginSamsungWMR.Tools
{
    /// <summary>
    /// 基于三星玄龙手柄的连续转动提供者
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Continuous Turn Provider (三星玄龙手柄)")]
    [Name("基于三星玄龙手柄的连续转动提供者")]
    [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
    [Tip("在当前游戏对象上创建一个[基于三星玄龙手柄的连续转动提供者]组件对象", "Create a [continuous mobile provider based on Samsung Xuanlong handle] component object on the current game object")]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [RequireManager(typeof(SamsungWMRManager))]
    [Owner(typeof(SamsungWMRManager))]
    public class SamsungWMRContinuousTurnProvider
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : ContinuousTurnProviderBase
#else
        : MB
#endif
    {
        /// <summary>
        /// 手柄类型
        /// </summary>
        [Name("手柄类型")]
        [EnumPopup]
        public EHandRule _handleType = EHandRule.Left;

        /// <summary>
        /// 摇杆或触控板
        /// </summary>
        [Name("摇杆或触控板")]
        [EnumPopup]
        public ESamsungWMRAxis2D _axisAndButton = ESamsungWMRAxis2D.JoyStick;


#if XDREAMER_XR_INTERACTION_TOOLKIT
        /// <summary>
        /// 读取输入
        /// </summary>
        /// <returns></returns>
        protected override Vector2 ReadInput()
        {
            _axisAndButton.TryGetAxis2DValue(_handleType, out var value);
            return value;
        }
#endif
    }
}
