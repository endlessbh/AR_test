using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Effects;

namespace XCSJ.EditorTools.Effects
{
    /// <summary>
    /// 特效交互器检查器
    /// </summary>
    [CustomEditor(typeof(EffectController), true)]
    public class EffectInteractorInspector : InteractorInspector<EffectController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(BaseEffect)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
