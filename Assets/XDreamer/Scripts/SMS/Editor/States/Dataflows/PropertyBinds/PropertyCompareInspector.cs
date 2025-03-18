using System.Reflection;
using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.Helper;
using XCSJ.PluginSMS.States.Dataflows.PropertyBinds;

namespace XCSJ.EditorSMS.States.Dataflows
{
    /// <summary>
    /// 属性比较检查器
    /// </summary>
    [Name("属性比较检查器")]
    [CustomEditor(typeof(PropertyCompare))]
    public class PropertyCompareInspector : TriggerInspector<PropertyCompare>
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
            if (stateComponent._propertyDetection._binder.memberInfo is MemberInfo member)
            {
                info.AppendFormat("属性类型:\t{0}", TypeHelper.GetMemberType(member).ToString());
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>成员信息无效</color>");
            }
            return info;
        }
    }
}
