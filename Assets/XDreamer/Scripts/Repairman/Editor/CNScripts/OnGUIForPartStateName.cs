using UnityEditor;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorSMS;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginRepairman.CNScripts;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.Exam;
using XCSJ.PluginRepairman.States.Study;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.Scripts;

namespace XCSJ.EditorRepairman.CNScripts
{
    /// <summary>
    /// 零件状态脚本参数绘制器
    /// </summary>
    [ScriptParamType(PartState_ScriptParam.ScriptPartStateType)]
    public class PartState_ScriptParamDrawer : EnumScriptParamDrawer<EAssembleState> { }

    /// <summary>
    /// 设备脚本参数绘制器
    /// </summary>
    [ScriptParamType(Device_ScriptParam.Device)]
    public class Device_ScriptParamDrawer : LimitStateComponent_ScriptParamDrawer<Device> { }

    /// <summary>
    /// 拆装学习脚本参数绘制器
    /// </summary>
    [ScriptParamType(RepairStudy_ScriptParam.RepairStudy)]
    public class RepairStudy_ScriptParamDrawer : LimitStateComponent_ScriptParamDrawer<RepairStudy> { }

    /// <summary>
    /// 拆装考试脚本参数绘制器
    /// </summary>
    [ScriptParamType(RepairExam_ScriptParam.RepairExam)]
    public class ForRepairmanTeachingName : LimitStateComponent_ScriptParamDrawer<RepairExam> { }

    /// <summary>
    /// 限定状态组件脚本参数绘制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitStateComponent_ScriptParamDrawer<T> : ScriptParamDrawer where T : StateComponent
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            EditorGUI.indentLevel = 2;
            paramObject = PopupStateComponent(paramObject);
        }

        /// <summary>
        /// 弹出状态组件
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T PopupStateComponent(object obj)
        {
            var objects = RootStateMachine.instance.GetStateComponents(typeof(T)).ToList(o => (T)o);
            return EditorSMSHelper.Popup<T>(objects, (T)obj, true);
        }
    }
}
