using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Inputs;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.EditorTools.Inputs
{
    /// <summary>
    /// 表检查器
    /// </summary>
    [Name("表检查器")]
    [CustomEditor(typeof(AnalogMouseInput), true)]
    public class AnalogMouseInputInspector : InteractorInspector<AnalogMouseInput>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(AnalogMouseInput)));

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
