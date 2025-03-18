using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Dataflows.PropertyBinds;

namespace XCSJ.EditorSMS.States.Dataflows
{
    /// <summary>
    /// 设置属性检查器
    /// </summary>
    [Name("设置属性检查器")]
    [CustomEditor(typeof(SetProperty))]
    public class SetPropertyInspector : StateComponentInspector<SetProperty>
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
            if (stateComponent.binder.memberInfo is MemberInfo member)
            {
                info.AppendFormat("属性类型:\t{0}", TypeHelper.GetMemberType(member).ToString());
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>成员信息无效</color>");
            }
            info.AppendFormat("\n属性参数类型:\t{0}", stateComponent.argument._argumentType.TrLabel().text);
            return info;
        }
    }
}
