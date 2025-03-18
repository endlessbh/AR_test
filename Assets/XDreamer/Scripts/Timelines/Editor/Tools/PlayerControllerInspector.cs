using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.PluginTimelines;
using XCSJ.PluginTimelines.Tools;

namespace XCSJ.EditorTimelines.Tools
{
    /// <summary>
    /// 播放器控制器检查器
    /// </summary>
    [CustomEditor(typeof(PlayerController), true)]
    public class PlayerControllerInspector : InteractorInspector<PlayerController>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(TimelineManager.PlayableContent));

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
