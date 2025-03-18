using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginMMO.Transitions;

namespace XCSJ.EditorMMO.Transitions
{
    /// <summary>
    /// 网络属性比较检查器
    /// </summary>
    [Name("网络属性比较检查器")]
    [CustomEditor(typeof(NetPropertyCompare), true)]
    public class NetPropertyCompareInspector : TransitionComponentInspector<NetPropertyCompare>
    {

    }
}
