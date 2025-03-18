using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 相机变换通过ZVR检查器
    /// </summary>
    [Name("相机变换通过ZVR检查器")]
    [CustomEditor(typeof(CameraTransformByZVR))]
    public class CameraTransformByZVRInspector : BaseCameraToolControllerInspector<CameraTransformByZVR>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorZVRHelper.DrawSelectZVRManager();
        }
    }
}
