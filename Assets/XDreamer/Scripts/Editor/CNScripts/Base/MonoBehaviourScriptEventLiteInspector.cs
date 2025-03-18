using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.CNScripts.Inputs;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件简版检查器
    /// </summary>
    [Name("MonoBehaviour脚本事件简版检查器")]
    [CanEditMultipleObjects, CustomEditor(typeof(MonoBehaviourScriptEventLite))]
    public class MonoBehaviourScriptEventLiteInspector : BaseScriptEventInspector<MonoBehaviourScriptEventLite, EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteFunction, MonoBehaviourScriptEventLiteFunctionCollection>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + MonoBehaviourScriptEventLite.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
