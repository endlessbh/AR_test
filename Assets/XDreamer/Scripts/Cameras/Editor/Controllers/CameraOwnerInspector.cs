using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机拥有者检查器
    /// </summary>
    [Name("相机拥有者检查器")]
    [CustomEditor(typeof(CameraOwner), true)]
    public class CameraOwnerInspector : MBInspector<CameraOwner>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorCameraHelperExtension.DrawCurrentCameraAsInitCamera(() => targetObject.GetComponentInChildren<BaseCameraMainController>(false));
            EditorCameraHelperExtension.DrawSelectCameraManager();
        }
    }
}
