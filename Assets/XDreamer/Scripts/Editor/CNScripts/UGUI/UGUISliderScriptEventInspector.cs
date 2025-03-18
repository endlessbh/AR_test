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
    /// UGUI滑动条脚本事件检查器
    /// </summary>
    [Name("UGUI滑动条脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUISliderScriptEvent))]
    public class UGUISliderScriptEventInspector : BaseScriptEventInspector<UGUISliderScriptEvent, EUGUISliderScriptEventType, UGUISliderScriptEventFunction, UGUISliderScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUISliderScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUISliderScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Slider>();
        }
    }
}
