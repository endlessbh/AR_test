using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// 动画脚本事件检查器
    /// </summary>
    [Name("动画脚本事件检查器")]
    [CustomEditor(typeof(AnimationScriptEvent))]
    public class AnimationScriptEventInspector : BaseScriptEventInspector<AnimationScriptEvent, EAnimationScriptEventType, AnimationScriptEventFunction, AnimationScriptEventFunctionCollection>
    {
        /// <summary>
        /// 创建脚本事件
        /// </summary>
        [MenuItem(XDreamerMenu.ScriptEvent + AnimationScriptEvent.Title, false)]
        public static void CreateScriptEvent() => CreateComponentInternal();

        /// <summary>
        /// 验证创建脚本事件
        /// </summary>
        /// <returns></returns>
        [MenuItem(XDreamerMenu.ScriptEvent + AnimationScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent() => ValidateCreateComponentInternal();
    }
}
