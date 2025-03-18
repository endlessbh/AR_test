using System;
using XCSJ.Extension.Base.Dataflows.Base;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Base
{
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

    /// <summary>
    /// 交互图层遮罩属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(InteractionLayerMask))]
    public class InteractionLayerMaskPropertyValue : PropertyValue<InteractionLayerMask>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public InteractionLayerMaskPropertyValue() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public InteractionLayerMaskPropertyValue(InteractionLayerMask value) : base(value) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value"></param>
        public InteractionLayerMaskPropertyValue(int value) : base(value) { }
    }
#endif
}
