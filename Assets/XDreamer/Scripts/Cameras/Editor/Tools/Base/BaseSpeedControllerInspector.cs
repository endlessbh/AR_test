using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginsCameras.Tools.Base;

namespace XCSJ.EditorCameras.Tools.Base
{
    /// <summary>
    /// 基础速度控制器检查器
    /// </summary>
    [Name("基础速度控制器检查器")]
    [CustomEditor(typeof(BaseSpeedController), true)]
    public class BaseSpeedControllerInspector : BaseSpeedControllerInspector<BaseSpeedController>
    {
    }

    /// <summary>
    /// 基础速度控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSpeedControllerInspector<T> : BaseCameraToolControllerInspector<T>
       where T : BaseSpeedController
    {
    }
}

