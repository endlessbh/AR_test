using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.PluginTools.Draggers;
using static XCSJ.PluginTools.Draggers.CombinationDragToolController;

namespace XCSJ.EditorTools.Draggers
{
    /// <summary>
    /// 一键拖拽工具控制器检查器
    /// </summary>
    [Name("一键拖拽工具控制器检查器")]
    [CustomEditor(typeof(CombinationDragToolController), true)]
    [CanEditMultipleObjects]
    public class CombinationDragToolControllerInspector : InteractorInspector<CombinationDragToolController>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDragToolEnable(targetObject);
        }

        private static bool expand = true;

        private static EnumTypeData enumTypeData = null;

        public static event Action<CombinationDragToolController, EToolType> onChanged;

        [Languages.LanguageTuple("Enable Tools", "启用工具")]
        private static void DrawDragToolEnable(CombinationDragToolController combinationDragToolController)
        {
            if (enumTypeData == null)
            {
                enumTypeData = EnumTypeData.GetEnumTypeData(typeof(EToolType));
            }

            expand = UICommonFun.Foldout(expand, "Enable Tools".Tr(typeof(CombinationDragToolControllerInspector)));
            if (!expand) return;

            CommonFun.BeginLayout();
            {
                for (int i = 0; i < enumTypeData.displayNames.Length; i++)
                {
                    var tooltype = (EToolType)enumTypeData.GetEnumByIndex(i);
                    EditorGUI.BeginChangeCheck();
                    var active = EditorGUILayout.Toggle(enumTypeData.displayNames[i], combinationDragToolController.GetToolActive(tooltype));
                    if (EditorGUI.EndChangeCheck())
                    {
                        combinationDragToolController.SetToolActive(tooltype, active);
                        onChanged?.Invoke(combinationDragToolController, tooltype);
                    }
                }
            }
            CommonFun.EndLayout();
        }
    }
}
