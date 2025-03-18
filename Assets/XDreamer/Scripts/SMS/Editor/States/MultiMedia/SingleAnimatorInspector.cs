using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 单一动画检查器
    /// </summary>
    [Name("单一动画检查器")]
    [CustomEditor(typeof(SingleAnimator))]
    public class SingleAnimatorInspector : UnityAnimatorInspector<SingleAnimator>
    {
    }
}
