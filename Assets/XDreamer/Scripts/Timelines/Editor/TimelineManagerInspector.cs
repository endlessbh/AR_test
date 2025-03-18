using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginTimelines;

namespace XCSJ.EditorTimelines
{
    /// <summary>
    /// 时间轴检查器
    /// </summary>
    [Name("时间轴检查器")]
    [CustomEditor(typeof(TimelineManager), true)]
    public class TimelineManagerInspector : BaseManagerInspector<TimelineManager>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            InitCategoryList();
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CategoryListExtension.DrawVertical(categoryList);
        }

        private static CategoryList categoryList = null;

        private void InitCategoryList()
        {
            if (categoryList == null)
            {
                categoryList = EditorToolsHelper.GetWithPurposes(TimelineManager.Title, TimelineManager.ToolLib, TimelineManager.PlayableContent);
            }
        }
    }
}
