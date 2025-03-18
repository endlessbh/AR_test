using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.EditorSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据记录器检查器
    /// </summary>
    [Name("数据记录器检查器")]
    [CustomEditor(typeof(DataRecorder), true)]
    public class DataRecorderInspector : StateComponentInspector<DataRecorder>
    {
    }
}
