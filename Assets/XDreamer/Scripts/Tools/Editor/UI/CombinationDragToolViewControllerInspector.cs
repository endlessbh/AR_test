using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Draggers;
using XCSJ.EditorXGUI.DataViews.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.UI;
using static XCSJ.PluginTools.Draggers.CombinationDragToolController;

namespace XCSJ.EditorTools.SelectionUtils
{
    /// <summary>
    /// 一键拖拽工具视图控制器检查器
    /// </summary>
    [Name("一键拖拽工具视图控制器检查器")]
    [CustomEditor(typeof(CombinationDragToolViewController))]
    public class CombinationDragToolViewControllerInspector : BaseViewControllerInspector<CombinationDragToolViewController>
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            CombinationDragToolControllerInspector.onChanged += OnChanged;
        }
        
        private static void OnChanged(CombinationDragToolController combinationDragToolController, EToolType toolType)
        {

            var components = CommonFun.GetComponentsInChildren<CombinationDragToolViewController>(true);
            foreach (var item in components)
            {
                if (item.combinationDragToolController == combinationDragToolController)
                {
                    item.OnCombinationDragToolControllerChanged(toolType);
                }
            }
        }
    }
}
