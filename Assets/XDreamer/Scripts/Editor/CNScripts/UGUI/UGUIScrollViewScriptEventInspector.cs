using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI滚动视图脚本事件检查器
    /// </summary>
    [Name("UGUI滚动视图脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIScrollViewScriptEvent))]
    public class UGUIScrollViewScriptEventInspector : BaseScriptEventInspector<UGUIScrollViewScriptEvent, EUGUIScrollViewScriptEventType, UGUIScrollViewScriptEventFunction, UGUIScrollViewScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIScrollViewScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIScrollViewScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<ScrollRect>();
        }
    }
}
