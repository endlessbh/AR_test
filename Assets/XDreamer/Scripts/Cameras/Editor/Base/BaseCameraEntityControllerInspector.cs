using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机实体控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraEntityController), true)]
    [Name("基础相机实体控制器检查器")]
    public class BaseCameraEntityControllerInspector : BaseCameraEntityControllerInspector<BaseCameraEntityController>
    {
    }

    /// <summary>
    /// 基础相机实体控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraEntityControllerInspector<T> : BaseCameraCoreControllerInspector<T>
       where T : BaseCameraEntityController
    {
    }
}
