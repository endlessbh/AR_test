using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机子控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraSubController), true)]
    [Name("基础相机子控制器检查器")]
    public class BaseCameraSubComtrollerInspector : BaseCameraSubComtrollerInspector<BaseCameraSubController>
    {
    }

    /// <summary>
    /// 基础相机子控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraSubComtrollerInspector<T> : BaseSubControllerInspector<T>
       where T : BaseCameraSubController
    {
    }
}
