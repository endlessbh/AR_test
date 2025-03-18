using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.Attributes;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机目标控制器检查器
    /// </summary>
    [Name("相机目标控制器检查器")]
    [CustomEditor(typeof(CameraTargetController), true)]
    public class CameraTargetControllerInspector : BaseCameraTargetControllerInspector<CameraTargetController>
    {
    }
}


