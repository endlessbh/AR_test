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
    /// UGUI操纵杆按钮脚本事件检查器
    /// </summary>
    [Name("UGUI操纵杆按钮脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(UGUIJoystickButtonScriptEvent))]
    public class UGUIJoystickButtonScriptEventInspector : BaseScriptEventInspector<UGUIJoystickButtonScriptEvent, EUGUIJoystickButtonScriptEventType, UGUIJoystickButtonScriptEventFunction, UGUIJoystickButtonScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIJoystickButtonScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.UGUIMenu + UGUIJoystickButtonScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentWithRequireInternal<Button>();
        }
    }
}
