using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.PluginTimelines;
using XCSJ.PluginTimelines.UI;

namespace XCSJ.EditorTimelines.UI
{

    /// <summary>
    /// 可播放内容列表检查器
    /// </summary>
    [CustomEditor(typeof(PlayableContentList), true)]
    public class PlayableContentListInspector : InteractorInspector<PlayableContentList>
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
