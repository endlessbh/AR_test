using System;
using XCSJ.Extension.Base.Dataflows.Base;

#if XDREAMER_VOXELTRACKER
using VoxelStationUtil;
#endif

namespace XCSJ.PluginVoxelTracker.States
{
#if XDREAMER_VOXELTRACKER

    /// <summary>
    /// 按钮ID属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(ButtonID))]
    public class ButtonIDPropertyValue : EnumPropertyValue<ButtonID>
    {
    }

    /// <summary>
    /// Drag Policy属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(DragPolicy))]
    public class DragPolicyPropertyValue : EnumPropertyValue<DragPolicy>
    {
    }

    /// <summary>
    /// 射线状态属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(RayState))]
    public class RayStatePropertyValue : EnumPropertyValue<RayState>
    {
    }

    /// <summary>
    /// 交互笔功能属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(StylusFeature))]
    public class StylusFeaturePropertyValue : EnumPropertyValue<StylusFeature>
    {
    }

#endif
}
