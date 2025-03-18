using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorSMS.States.Show
{
    /// <summary>
    /// 步骤索引界面
    /// </summary>
    [XCSJ.Languages.LanguageFileOutput]
    public static class UIPlanTreeViewEditor
    {
        /// <summary>
        /// 创建步骤索引界面
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.Create)]
        [Name("步骤索引界面")]
        [XCSJ.Attributes.Icon(EIcon.Step)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager), typeof(SMSManager))]
        public static void CreateIndexStepUI(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("状态机系统/步骤索引界面.prefab"));
        }
    }
}
