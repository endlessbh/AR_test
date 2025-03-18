using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginEmbeddedBrowser;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorEmbeddedBrowser.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        /// <summary>
        /// 浏览器窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(EmbeddedBrowserHelper.Title, nameof(EmbeddedBrowserManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name("浏览器窗口")]
        [XCSJ.Attributes.Icon(EIcon.Net)]
        [RequireManager(typeof(EmbeddedBrowserManager), typeof(XGUIManager))]
        [Manual(typeof(EmbeddedBrowserManager))]
        public static void BrowserWindow(ToolContext toolContext)
        {
#if XDREAMER_EMBEDDED_BROWSER
            var windowGameObject = XCSJ.EditorXGUI.ToolsMenu.LoadWindowPrefab("浏览器窗口", out var window, out var titleText, out var contentText);
            if (contentText)
            {
                contentText.gameObject.XSetActive(false);
            }
            if (window.content)
            {
                var browser = EditorToolsHelperExtension.ClonePrefab("Assets/ZFBrowser/Prefabs/Browser (GUI).prefab");
                if (browser)
                {
                    browser.transform.XSetTransformParent(window.content);
                    var rectTransform = browser.transform as RectTransform;
                    if (rectTransform)
                    {
                        rectTransform.SetFullScreen(5, 5, 5, 5);
                    }
                }
            }
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, windowGameObject);
#else
            Selection.activeObject = EmbeddedBrowserManager.instance;
#endif
        }
    }
}
