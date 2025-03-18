using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Others;

namespace XCSJ.EditorSMS.States.Others
{
    /// <summary>
    /// 放置检查器
    /// </summary>
    [Name("放置检查器")]
    [CustomEditor(typeof(Put))]
    public class PutInspector : StateComponentInspector<Put>
    {

    }
}
