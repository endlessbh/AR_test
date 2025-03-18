using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.EditorSMS.Transitions.Base
{
    /// <summary>
    /// 触发器检查器
    /// </summary>
    [CustomEditor(typeof(Trigger), true)]
    [Name("触发器检查器")]
    public class TriggerInspector : TriggerInspector<Trigger>
    {
    }

    /// <summary>
    /// 触发器泛型检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TriggerInspector<T> : TransitionComponentInspector where T : Trigger
    {
        /// <summary>
        /// 触发器
        /// </summary>
        public T trigger => target as T;
    }
}
