using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools.Base;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 对象关联信息检查器
    /// </summary>
    [Name("对象关联信息检查器")]
    [CustomEditor(typeof(ObjectLinkInfo), true)]
    public class ObjectLinkInfoInspector: TriggerEventMBInspector<ObjectLinkInfo>
    {
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 显示运行时帮助信息
        /// </summary>
        protected override bool displayRuntimeHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var sb = base.GetHelpInfo();
            sb.AppendFormat("SQL语句:\t{0}", targetObject.ToFriendlyString());
            return sb;
        }

        /// <summary>
        /// 获取运行时辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetRuntimeHelpInfo()
        {
            var sb = base.GetRuntimeHelpInfo();
            sb.AppendFormat("SQL语句:\t{0}", targetObject.GetSql());
            return sb;
        }
    }
}
