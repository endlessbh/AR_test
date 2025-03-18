using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS
{
    /// <summary>
    /// 扩展ID区间
    /// </summary>
    public static class SMSExtensionIDRange
    {
        public const int Begin = (int)EExtensionID._0x6;
        public const int End = (int)EExtensionID._0x13 - 1;

        public const int Fragment = 0x20;//32

        public const int Common = Begin + Fragment * 0;
        public const int MonoBehaviour = Begin + Fragment * 1;
        public const int StateLib = Begin + Fragment * 2; //状态库占 32*10
        public const int Tools = Begin + Fragment * 12;
        public const int Editor = Begin + Fragment * 13;
    }

    /// <summary>
    /// 循环类型
    /// </summary>
    [ScriptParamType(LoopType)]
    public class LoopType_ScriptParam : EnumScriptParam<ELoopType>
    {
        public const int LoopType = SMSExtensionIDRange.Begin;
    }

    [Name("SMS扩展脚本ID")]
    public enum ESMSExtensionScriptID
    {
        _Begin = SMSExtensionIDRange.Begin,

        [ScriptName("SMS-状态机系统扩展", EGrammarType.Category)]
        SMSExtension,

        [ScriptName("获取工作剪辑时长", nameof(GetWorkClipTimeLength))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        GetWorkClipTimeLength,

        [ScriptName("设置工作剪辑时长", nameof(SetWorkClipTimeLength))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        [ScriptParams(2, EParamType.FloatSlider, "时长:", 0f, 300f, defaultObject = 3f)]
        SetWorkClipTimeLength,

        [ScriptName("获取工作剪辑单次时长", nameof(GetWorkClipOnceTimeLength))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        GetWorkClipOnceTimeLength,

        [ScriptName("设置工作剪辑单次时长", nameof(SetWorkClipOnceTimeLength))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        [ScriptParams(2, EParamType.FloatSlider, "单次时长:", 0f, 300f, defaultObject = 3f)]
        SetWorkClipOnceTimeLength,

        [ScriptName("获取工作剪辑循环类型", nameof(GetWorkClipLoopType))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        GetWorkClipLoopType,

        [ScriptName("设置工作剪辑循环类型", nameof(SetWorkClipLoopType))]
        [ScriptParams(1, StateComponent_ScriptParam.StateComponent, "工作剪辑:", typeof(WorkClip))]
        [ScriptParams(2, LoopType_ScriptParam.LoopType, "循环类型:")]
        SetWorkClipLoopType,
    }

    /// <summary>
    /// 状态库索引
    /// </summary>
    public static class StateLibIndex
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public const int Begin = 0;

        /// <summary>
        /// 片段
        /// </summary>
        public const int Fragment = 32;

        /// <summary>
        /// 常用
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        ///// <summary>
        ///// Mono
        ///// </summary>
        //public const int Mono = Begin + Fragment * 2;

        ///// <summary>
        ///// UGUI
        ///// </summary>
        //public const int UGUI = Begin + Fragment * 3;

        ///// <summary>
        ///// 中文脚本
        ///// </summary>
        //public const int Script = Begin + Fragment * 4;

        ///// <summary>
        ///// 动作
        ///// </summary>
        //public const int Motion = Begin + Fragment * 5;

        ///// <summary>
        ///// 多媒体
        ///// </summary>
        //public const int Media = Begin + Fragment * 6;

        ///// <summary>
        ///// 状态操作
        ///// </summary>
        //public const int StateOperation = Begin + Fragment * 7;

        ///// <summary>
        ///// 展示
        ///// </summary>
        //public const int Show = Begin + Fragment * 8;

        ///// <summary>
        ///// 时间轴
        ///// </summary>
        //public const int Timeline = Begin + Fragment * 9;

        ///// <summary>
        ///// 数据
        ///// </summary>
        //public const int Data = Begin + Fragment * 10;

        ///// <summary>
        ///// 数据流-数据
        ///// </summary>
        //public const int DataFlow = Begin + Fragment * 11;

        ///// <summary>
        ///// 选择集
        ///// </summary>
        //public const int Selection = Begin + Fragment * 12;

        ///// <summary>
        ///// 拆装修理-模型
        ///// </summary>
        //public const int RepairmanModel = Begin + Fragment * 13;

        ///// <summary>
        ///// 拆装修理-流程
        ///// </summary>
        //public const int RepairmanFlow = Begin + Fragment * 14;

        /// <summary>
        /// 其它
        /// </summary>
        public const int Other = Begin + Fragment * 15;
    }
}

