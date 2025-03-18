using UnityEngine;
using System.Collections;
using UnityEditor;
using XCSJ.PluginEasyAR;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
#if XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.EditorEasyAR
{
    /// <summary>
    /// 脚本图片目标事件检查器
    /// </summary>
    [Name("脚本图片目标事件检查器")]
    [CustomEditor(typeof(ScriptImageTargetEvent))]
    public class ScriptImageTargetEventInspector : BaseScriptEventInspector<ScriptImageTargetEvent, EImageTargetScriptEventType, ImageTargetScriptEventFunction, ImageTargetScriptEventFunctionCollection>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + "EasyAR/脚本图片目标事件", false)]
        public static void CreateScriptEvent()
        {
#if XDREAMER_EASYAR_2_3_0
            CreateComponentWithRequireInternal<ImageTargetBaseBehaviour>();
#endif
        }

        [MenuItem(XDreamerMenu.ScriptEvent + "EasyAR/脚本图片目标事件", true)]
        public static bool ValidateCreateScriptEvent()
        {
#if XDREAMER_EASYAR_2_3_0
            return ValidateCreateComponentWithRequireInternal<ImageTargetBaseBehaviour>() && EasyARManager.instance;
#else
            return false;
#endif
        }
    }
}
