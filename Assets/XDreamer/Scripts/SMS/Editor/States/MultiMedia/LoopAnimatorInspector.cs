using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 循环动画检查器
    /// </summary>
    [Name("循环动画检查器")]
    [CustomEditor(typeof(LoopAnimator))]
    public class LoopAnimatorInspector : UnityAnimatorInspector<LoopAnimator>
    {
    }
}
