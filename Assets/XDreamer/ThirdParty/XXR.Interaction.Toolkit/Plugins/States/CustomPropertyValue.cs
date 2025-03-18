using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.States
{
    /// <summary>
    /// XR交互管理器属性值
    /// </summary>
    [Name("XR交互管理器属性值")]
    [Serializable]
#if XDREAMER_XR_INTERACTION_TOOLKIT
    [PropertyType(typeof(XRInteractionManager))]
#endif
    public class XRInteractionManagerPropertyValue
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : BaseComponentPropertyValue<XRInteractionManager>
#else
        : BaseComponentPropertyValue<MonoBehaviour>
#endif
    {
    }
}
