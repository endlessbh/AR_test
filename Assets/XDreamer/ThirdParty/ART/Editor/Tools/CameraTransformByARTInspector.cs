using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 相机变换通过ART检查器
    /// </summary>
    [CustomEditor(typeof(CameraTransformByART))]
    [Name("相机变换通过ART检查器 ")]
    public class CameraTransformByARTInspector : BaseCameraToolControllerInspector<CameraTransformByART>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorARTHelper.DrawSelectARTManager();
        }
    }
}
