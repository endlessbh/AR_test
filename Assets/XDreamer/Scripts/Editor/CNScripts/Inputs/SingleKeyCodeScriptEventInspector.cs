using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.CNScripts.Inputs;
using XCSJ.EditorCommonUtils;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.CNScripts.Inputs
{
    /// <summary>
    /// 单一按键脚本事件检查器
    /// </summary>
    [Name("单一按键脚本事件检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(SingleKeyCodeScriptEvent))]
    public class SingleKeyCodeScriptEventInspector : BaseScriptEventInspector<SingleKeyCodeScriptEvent, ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction, SingleKeyCodeScriptEventFunctionCollection>
    {
        [MenuItem(EditorScriptHelper.InputMenu + SingleKeyCodeScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(EditorScriptHelper.InputMenu + SingleKeyCodeScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("特别注意:LeftCommand与LeftWindows对应相同的回调事件！仅回调 LeftCommand 事件；", "在执行时仅回调 LeftCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(new GUIContent("特别注意:RightCommand与RightApple对应相同的回调事件！仅回调 RightCommand 事件；", "在执行时仅回调 RightCommand  事件"), UICommonOption.labelYellowBG, GUILayout.ExpandWidth(true));

            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
