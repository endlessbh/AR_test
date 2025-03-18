using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorMechanicalMotion.Tools;
using XCSJ.PluginMechanicalMotion;

namespace XCSJ.EditorMechanicalMotion
{
    /// <summary>
    /// 机械运动管理器检查器
    /// </summary>
    [Name("机械运动管理器检查器")]
    [CustomEditor(typeof(MechanicalMotionManager))]
    public class MechanicalMotionManagerInspector : BaseManagerInspector<MechanicalMotionManager>
    {
        /// <summary>
        /// 绘制检查器UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MechanismInspector.DrawRootMechanism();
        }
    }
}