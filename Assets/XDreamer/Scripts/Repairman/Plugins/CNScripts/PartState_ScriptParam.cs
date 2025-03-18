using System;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginRepairman.States.Study;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginRepairman.CNScripts
{
    /// <summary>
    /// 零件状态
    /// </summary>
    [ScriptParamType(ScriptPartStateType)]
    public class PartState_ScriptParam : EnumScriptParam<EAssembleState>
    {
        /// <summary>
        /// 脚本零件状态类型
        /// </summary>
        public const int ScriptPartStateType = IDRange.Begin;
    }

    /// <summary>
    /// 限定状态组件脚本参数
    /// </summary>
    public abstract class LimitStateComponent_ScriptParam : Virtual_ScriptParam
    {
        /// <summary>
        /// 脚本字符串转参数对象
        /// </summary>
        /// <param name="paramString"></param>
        /// <returns></returns>
        public override object StringToParamObject(string paramString) => SMSHelper.StringToStateComponent(paramString);
    }

    /// <summary>
    /// 设备
    /// </summary>
    [ScriptParamType(Device)]
    public class Device_ScriptParam : LimitStateComponent_ScriptParam
    {
        public const int Device = IDRange.Begin + 10;

        /// <summary>
        /// 参数类型
        /// </summary>
        public override Type paramType => typeof(Device);
    }

    /// <summary>
    /// 拆装学习
    /// </summary>
    [ScriptParamType(RepairStudy)]
    public class RepairStudy_ScriptParam : LimitStateComponent_ScriptParam
    {
        public const int RepairStudy = IDRange.Begin + 100;

        /// <summary>
        /// 参数类型
        /// </summary>
        public override Type paramType => typeof(RepairStudy);
    }

    /// <summary>
    /// 拆装考试
    /// </summary>
    [ScriptParamType(RepairExam)]
    public class RepairExam_ScriptParam : LimitStateComponent_ScriptParam
    {
        public const int RepairExam = IDRange.Begin + 101;

        /// <summary>
        /// 参数类型
        /// </summary>
        public override Type paramType => typeof(RepairExam);
    }
}
