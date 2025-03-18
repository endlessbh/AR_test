using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Logs.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorExtension.Logs.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 状态统计窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("状态统计窗口")]
        [Tip("状态统计窗口", "Status statistics window")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        //[Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(StateStatistics))]
        public static void CreateVerticalWindow(ToolContext toolContext)
        {
            var windowGameObject = XCSJ.EditorXGUI.ToolsMenu.LoadWindowPrefab("状态统计", out var window, out var titleText, out var contentText);
            if (window)
            {
                var stateStatistics = window.XGetOrAddComponent<StateStatistics>();
                stateStatistics.text = contentText;
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, windowGameObject);
        }
    }
}
