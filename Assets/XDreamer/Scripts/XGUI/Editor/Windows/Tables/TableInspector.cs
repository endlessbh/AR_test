using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.EditorXGUI.Windows.Tables
{
    /// <summary>
    /// 表检查器
    /// </summary>
    [Name("表检查器")]
    [CustomEditor(typeof(Table), true)]
    public class TableInspector : InteractorInspector<Table>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(TableProcessor)));

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
