using System.Reflection;
using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Dataflows.Events;

namespace XCSJ.EditorSMS.States.Dataflows.Events
{
    /// <summary>
    /// Unity事件触发器检查器
    /// </summary>
    [Name("Unity事件触发器检查器")]
    [CustomEditor(typeof(UnityEventTrigger))]
    public class UnityEventTriggerInspector : TriggerInspector<UnityEventTrigger>
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
            if (stateComponent.unityEventListener.fieldInfo is FieldInfo fieldInfo)
            {
                info.AppendFormat("事件字段类型:\t{0}", fieldInfo.FieldType.ToString());
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>事件字段无效</color>");
            }
            return info;
        }
    }
}
