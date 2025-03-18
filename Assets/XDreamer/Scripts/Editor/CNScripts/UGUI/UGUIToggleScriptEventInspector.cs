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
    /// UGUI切换脚本事件检查器
    /// </summary>
    [Name("UGUI切换脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIToggleScriptEvent))]
    public class UGUIToggleScriptEventInspector : BaseScriptEventInspector<UGUIToggleScriptEvent, EUGUIToggleScriptEventType, UGUIToggleScriptEventFunction, UGUIToggleScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIToggleScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIToggleScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Toggle>();
        }
    }
}
