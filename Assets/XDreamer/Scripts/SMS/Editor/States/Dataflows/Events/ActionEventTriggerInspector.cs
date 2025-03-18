using System.Reflection;
using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Dataflows.Events;

namespace XCSJ.EditorSMS.States.Dataflows.Events
{
    /// <summary>
    /// Action事件触发器检查器
    /// </summary>
    [Name("Action事件触发器检查器")]
    [CustomEditor(typeof(ActionEventTrigger))]
    public class ActionEventTriggerInspector : TriggerInspector<ActionEventTrigger>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (stateComponent.actionEventListener.eventMethodInfo is MethodInfo method)
            {
                var parameters = method.GetParameters();
                info.AppendFormat("事件参数数目:\t{0}", parameters.Length.ToString());
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    info.AppendFormat("\n事件参数[{0}]类型:\t{1}", i.ToString(), p.ParameterType.FullName);
                }
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>事件字段无效</color>");
            }
            return info;
        }
    }
}
