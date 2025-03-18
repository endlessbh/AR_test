using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Inputs
{
    /// <summary>
    /// 触摸脚本事件检查器
    /// </summary>
    [Name("触摸脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(TouchScriptEvent))]
    public class TouchScriptEventInspector : BaseScriptEventInspector<TouchScriptEvent, ETouchScriptEventType, TouchScriptEventFunction, TouchScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.InputMenu + TouchScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.InputMenu + TouchScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }
    }
}
