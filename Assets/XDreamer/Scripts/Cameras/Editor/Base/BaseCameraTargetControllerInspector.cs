using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机目标控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraTargetController), true)]
    [Name("基础相机目标控制器检查器")]
    public class BaseCameraTargetControllerInspector : BaseCameraTargetControllerInspector<BaseCameraTargetController>
    {
    }

    /// <summary>
    /// 基础相机目标控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraTargetControllerInspector<T> : BaseCameraCoreControllerInspector<T>
       where T : BaseCameraTargetController
    {
    }
}
