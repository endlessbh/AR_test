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
    /// UGUI按钮脚本事件检查器
    /// </summary>
    [Name("UGUI按钮脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIButtonScriptEvent))]
    public class UGUIButtonScriptEventInspector : BaseScriptEventInspector<UGUIButtonScriptEvent, EUGUIButtonScriptEventType, UGUIButtonScriptEventFunction, UGUIButtonScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIButtonScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIButtonScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Button>();
        }
    }
}
