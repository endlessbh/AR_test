using System;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginMechanicalMotion.CNScripts
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值
        /// </summary>
        public const int Begin = (int)EExtensionID._0x31;

        /// <summary>
        /// 结束值
        /// </summary>
        public const int End = ((int)EExtensionID._0x32 - 1);
    }

    /// <summary>
    /// 循环类型
    /// </summary>
    [ScriptParamType(MechanismPathType)]
    public class MechanismPathType_ScriptParam : ScriptParam
    {
        public const int MechanismPathType = IDRange.Begin;

        public override Type paramType => typeof(Mechanism);

        public override string ParamObjectToString(object paramObject)
        {
            return MechanicalMotionHelper.MechanismToString(paramObject as Mechanism);
        }

        public override object StringToParamObject(string paramString)
        {
            return MechanicalMotionHelper.StringToMechanism(paramString);
        }
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 机械运动-目录
        /// <summary>
        /// 机械运动
        /// </summary>
        [ScriptName(MechanicalMotionManager.Title, nameof(MechanicalMotion), EGrammarType.Category)]
        [ScriptDescription("机械运动的相关脚本目录；")]
        #endregion
        MechanicalMotion,

        #region 获取平面机构运动速度
        /// <summary>
        /// 获取平面机构运动速度
        /// </summary>
        [ScriptName("获取平面机构运动速度", nameof(GetPlaneMechanismVelocity))]
        [ScriptDescription("获取平面机构运动速度", "Get Plane Mechanism Velocity")]
        [ScriptReturn("成功返回 速度 ; 失败返回 #False ;", "Successfully return motion velocity; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        #endregion
        GetPlaneMechanismVelocity,

        #region 设置平面机构运动速度
        /// <summary>
        /// 设置平面机构运动速度
        /// </summary>
        [ScriptName("设置平面机构运动速度", nameof(SetPlaneMechanismVelocity))]
        [ScriptDescription("设置平面机构运动速度", "Set Plane Mechanism Motion Velocity")]
        [ScriptReturn("成功返回 #True; 失败返回 #False;", "Successful return #True; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        [ScriptParams(2, EParamType.Double, "运动速度")]
        #endregion
        SetPlaneMechanismVelocity,

        #region 获取平面机构当前值

        /// <summary>
        /// 获取平面机构当前值
        /// </summary>
        [ScriptName("获取平面机构当前值", nameof(GetPlaneMechanismValue))]
        [ScriptDescription("获取平面机构当前值", "Get Plane Mechanism Value")]
        [ScriptReturn("成功返回 速度 ; 失败返回 #False ;", "Successfully return current value; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平移机构")]
        #endregion
        GetPlaneMechanismValue,

        #region 设置平面机构目标值
        /// <summary>
        /// 设置平面机构目标值
        /// </summary>
        [ScriptName("设置平面机构目标值", nameof(SetPlaneMechanismTargetValue))]
        [ScriptDescription("设置平面机构期望达到的目标值, 机构将按照设定的参数运动至目标值处", "Set Plane Mechanism Value")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;", "Successful return #True; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        [ScriptParams(2, EParamType.Double, "目标值")]
        [ScriptParams(3, EParamType.Combo, "参数类型:", "时间", "速度")]
        [ScriptParams(4, EParamType.Double, "时间")]
        [ScriptParams(5, EParamType.Double, "速度")]
        #endregion
        SetPlaneMechanismTargetValue,

        #region 设置平面机构至最小值
        /// <summary>
        /// 设置平面机构至最小值
        /// </summary>
        [ScriptName("设置平面机构至最小值", nameof(SetPlaneMechanismCurrentValueToMinValue))]
        [ScriptDescription("设置平面机构至最小值", "Set Plane Mechanism CurrentValue To MinValue")]
        [ScriptReturn("成功返回 #True; 失败返回 #False;", "Successful return #True; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        #endregion
        SetPlaneMechanismCurrentValueToMinValue,

        #region 设置平面机构至最大值
        /// <summary>
        /// 设置平面机构至最大值
        /// </summary>
        [ScriptName("设置平面机构至最大值", nameof(SetPlaneMechanismCurrentValueToMaxValue))]
        [ScriptDescription("设置平面机构至最大值", "Set Plane Mechanism CurrentValue To MaxValue")]
        [ScriptReturn("成功返回 #True; 失败返回 #False;", "Successful return #True; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        #endregion
        SetPlaneMechanismCurrentValueToMaxValue,

        #region 设置平面机构至初始值
        /// <summary>
        /// 设置平面机构至初始值
        /// </summary>
        [ScriptName("设置平面机构至初始值", nameof(SetPlaneMechanismCurrentValueToInitValue))]
        [ScriptDescription("设置平面机构至初始值", "Set Plane Mechanism CurrentValue To InitValue")]
        [ScriptReturn("成功返回 #True; 失败返回 #False;", "Successful return #True; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平面机构")]
        #endregion
        SetPlaneMechanismCurrentValueToInitValue,

        #region 获取平面机构最小值

        /// <summary>
        /// 获取平面机构最小值
        /// </summary>
        [ScriptName("获取平面机构最小值", nameof(GetPlaneMechanismMinValue))]
        [ScriptDescription("获取平面机构最小值", "Get Plane Mechanism MinValue")]
        [ScriptReturn("成功返回 最小值 ; 失败返回 #False ;", "Successfully return min value; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平移机构")]
        #endregion
        GetPlaneMechanismMinValue,

        #region 获取平面机构最大值

        /// <summary>
        /// 获取平面机构最大值
        /// </summary>
        [ScriptName("获取平面机构最大值", nameof(GetPlaneMechanismMaxValue))]
        [ScriptDescription("获取平面机构最大值", "Get Plane Mechanism MaxValue")]
        [ScriptReturn("成功返回 最大值 ; 失败返回 #False ;", "Successfully return max value; Failure returns #False;")]
        [ScriptParams(1, MechanismPathType_ScriptParam.MechanismPathType, "平移机构")]
        #endregion
        GetPlaneMechanismMaxValue,
    }
}

