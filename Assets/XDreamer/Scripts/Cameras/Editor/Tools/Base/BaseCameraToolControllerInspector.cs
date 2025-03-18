using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.PluginsCameras.Tools.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机工具控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraToolController), true)]
    [Name("基础相机工具控制器检查器")]
    public class BaseCameraToolControllerInspector : BaseCameraToolControllerInspector<BaseCameraToolController>
    {
    }

    /// <summary>
    /// 基础相机工具控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraToolControllerInspector<T> : BaseSubControllerInspector<T>
       where T : BaseCameraToolController
    {
    }
}
