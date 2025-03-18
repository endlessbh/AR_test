using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion.CNScripts;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginMechanicalMotion
{
    /// <summary>
    /// 机械运动：用于模拟机械连杆结构、齿轮等运动形态
    /// </summary>
    [Name(Title)]
    [Tip("用于模拟机械连杆结构、齿轮等运动形态", "It is used to simulate the motion form of mechanical connecting rod structure and gear")]
    [Guid("7A8ADFC4-C21A-4631-9F03-97E8A8C89841")]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOption.Optional)]
    [Version("23.428")]
    public sealed class MechanicalMotionManager : BaseManager<MechanicalMotionManager, EScriptID>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "机械运动";

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                case EScriptID.GetPlaneMechanismVelocity:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            return ReturnValue.True(planeMechanism.velocity);
                        }
                        break;
                    }
                case EScriptID.SetPlaneMechanismVelocity:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            planeMechanism.velocity = (double)param[2];
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.GetPlaneMechanismValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            return ReturnValue.True(planeMechanism.currentValue);
                        }
                        break;
                    }
                case EScriptID.SetPlaneMechanismTargetValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            var value = (double)param[2];
                            var dataType = param[3] as string;
                            switch (dataType)
                            {
                                case "时间":
                                    {
                                        var time = (double)param[4];
                                        planeMechanism.SetTargetValueByTime(value, time);

                                        return ReturnValue.Yes;
                                    }
                                case "速度":
                                    {
                                        var velocity = (double)param[5];
                                        planeMechanism.SetTargetValueByVelocity(value, velocity);

                                        return ReturnValue.Yes;
                                    }
                            }
                        }
                        break;
                    }
                case EScriptID.SetPlaneMechanismCurrentValueToMinValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            planeMechanism.SetMinValue();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.SetPlaneMechanismCurrentValueToMaxValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            planeMechanism.SetMaxValue();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.SetPlaneMechanismCurrentValueToInitValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            planeMechanism.SetInitValue();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EScriptID.GetPlaneMechanismMinValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            return ReturnValue.True(planeMechanism.minValue);
                        }
                        break;
                    }
                case EScriptID.GetPlaneMechanismMaxValue:
                    {
                        var planeMechanism = param[1] as PlaneMechanism;
                        if (planeMechanism)
                        {
                            return ReturnValue.True(planeMechanism.maxValue);
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
