using System.Reflection;
using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Helper;
using XCSJ.PluginSMS.States.Dataflows.PropertyBinds;

namespace XCSJ.EditorSMS.States.Dataflows
{
    /// <summary>
    /// 获取属性检查器
    /// </summary>
    [Name("获取属性检查器")]
    [CustomEditor(typeof(GetProperty))]
    public class GetPropertyInspector : StateComponentInspector<GetProperty>
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
            if (stateComponent._binder.memberInfo is MemberInfo member)
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
