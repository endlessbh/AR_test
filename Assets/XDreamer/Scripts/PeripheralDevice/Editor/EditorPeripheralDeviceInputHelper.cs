using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginPeripheralDevice;

namespace XCSJ.EditorPeripheralDevice
{
    /// <summary>
    /// 编辑器外部设备输入辅助类
    /// </summary>
    public static class EditorPeripheralDeviceInputHelper
    {
        /// <summary>
        /// 打开输入调试器
        /// </summary>
        public static void DrawOpenInputDubugger()
        {
#if XDREAMER_INPUT_SYSTEM

            if (GUILayout.Button("打开[输入调试器]"))
            {
                EditorApplication.ExecuteMenuItem("Window/Analysis/Input Debugger");
            }
#endif
        }
    }
}
