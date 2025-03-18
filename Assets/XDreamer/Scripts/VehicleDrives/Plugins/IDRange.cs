using XCSJ.Extension;
using XCSJ.PluginVehicleDrive.DriveAssists;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.Scripts;
using XCSJ.Attributes;

namespace XCSJ.PluginVehicleDrive
{
    /// <summary>
    /// 数据流ID
    /// </summary>
    class IDRange
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public const int Begin = (int)EExtensionID._0x20;

        /// <summary>
        /// 结束值
        /// </summary>
        public const int End = ((int)EExtensionID._0x21 - 1);
    }

    /// <summary>
    /// 车辆驾驶脚本ID
    /// </summary>
    [Name("车辆驾驶脚本ID")]
    public enum EVehicleDriveScriptID
    {
        /// <summary>
        /// 起始ID
        /// </summary>
        _Begin = IDRange.Begin,

        #region 车辆驾驶-目录
        /// <summary>
        /// XBox 360
        /// </summary>
        [ScriptName("车辆驾驶", nameof(VehicleDriveCategory), EGrammarType.Category)]
        [ScriptDescription("车辆驾驶的相关脚本目录；")]
        #endregion
        VehicleDriveCategory,

        #region 引擎控制
        [ScriptName("发动机控制", nameof(EngineControl))]
        [ScriptDescription("发动机控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车辆驾驶器:", typeof(VehicleDriver))]
        [ScriptParams(2, EParamType.Combo, "操作:", "启动", "停止", "切换")]
        #endregion
        EngineControl,

        #region 动力输入
        [ScriptName("动力输入控制", nameof(EnginePowerControl))]
        [ScriptDescription("动力输入控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车辆驾驶器:", typeof(VehicleDriver))]
        [ScriptParams(2, EParamType.Combo, "操作:", "动力", "刹车", "转向")]
        [ScriptParams(3, EParamType.Float, "数值:", -1, 1)]
        #endregion
        EnginePowerControl,

        #region 手刹控制
        [ScriptName("手刹控制", nameof(HandbrakeControl))]
        [ScriptDescription("手刹控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车辆驾驶器:", typeof(VehicleDriver))]
        [ScriptParams(2, EParamType.Combo, "操作:", "拉手刹", "松手刹", "切换")]
        #endregion
        HandbrakeControl,

        #region 变速箱控制
        [ScriptName("变速箱控制", nameof(GearControl))]
        [ScriptDescription("变速箱控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车辆驾驶器:", typeof(VehicleDriver))]
        [ScriptParams(2, EParamType.Combo, "操作:", "空挡", "非空挡", "空挡与非切换", "升档", "降档")]
        #endregion
        GearControl,

        #region NOS控制
        [ScriptName("NOS(氮氧增压系统)控制", nameof(NOSControl))]
        [ScriptDescription("NOS控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "氮氧增压系统:", typeof(NOS))]
        [ScriptParams(2, EParamType.Combo, "操作:", "启动", "停止")]
        #endregion
        NOSControl,

        #region 车灯控制
        [ScriptName("车灯控制", nameof(VehicleLightControl))]
        [ScriptDescription("车灯控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车灯控制器:", typeof(VehicleLightController))]
        [ScriptParams(2, EParamType.Combo, "车灯类型:", "近光", "远光", "雾灯", "左转", "右转", "双闪")]
        [ScriptParams(3, EParamType.Combo, "操作:", "开", "关", "切换")]
        #endregion
        VehicleLightControl,

        #region 获取车辆属性
        [ScriptName("获取车辆属性", nameof(GetVehicleControlProperty))]
        [ScriptDescription("获取车辆属性")]
        [ScriptReturn("成功返回 有效值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "车辆驾驶器:", typeof(VehicleDriver))]
        [ScriptParams(2, EParamType.Combo, "数据类型:", "方向", "速度", "转速", "油量", "车轮转角", "方向盘转角")]
        #endregion
        GetVehicleControlProperty,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}

