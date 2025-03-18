using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginPeripheralDevice;

namespace XCSJ.EditorPeripheralDevice
{
    /// <summary>
    /// 外部设备输入管理器检查器
    [Name("外部设备输入管理器检查器")]
    [CustomEditor(typeof(PeripheralDeviceInputManager))]
    /// </summary>
    public class PeripheralDeviceInputManagerInspector: BaseManagerInspector<PeripheralDeviceInputManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            EditorPeripheralDeviceInputHelper.DrawOpenInputDubugger();
        }
    }
}
