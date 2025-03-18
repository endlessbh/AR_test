using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// ART流客户端检查器
    /// </summary>
    [Name("ART流客户端检查器 ")]
    [CustomEditor(typeof(ARTStreamClient), true)]
    public class ARTStreamClientInspector : MBInspector<ARTStreamClient>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Connection Status", "连接状态")]
        [LanguageTuple("Connected", "已连接")]
        [LanguageTuple("Not Connected", "未连接")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Connection Status".Tr(), targetObject.IsConnected() ? "Connected".Tr() : "Not Connected".Tr());
        }
    }
}
