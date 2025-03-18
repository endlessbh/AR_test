using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorRepairman.Inspectors;
using XCSJ.PluginRepairman.States;

namespace XCSJ.EditorRepairman.States
{
    /// <summary>
    /// 零件检查器
    /// </summary>
    [Name("零件检查器")]
    [CustomEditor(typeof(Part), true)]
    public class PartInspector : ItemInspector
    {
        private XCSJ.PluginRepairman.Tools.Part linkPart => stateComponent is Part p ? p.interactPart : null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (linkPart) { }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // 绘制关联零件组件的分类名称
            Tools.PartInspector.DrawPartCategoryName(linkPart);
        }
    }
}
