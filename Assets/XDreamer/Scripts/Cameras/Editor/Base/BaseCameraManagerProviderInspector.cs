using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera.Cameras;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机管理器提供者检查器
    /// </summary>
    [CustomEditor(typeof(BaseCameraManagerProvider), true)]
    [Name("基础相机管理器提供者检查器")]
    public class BaseCameraManagerProviderInspector : BaseCameraManagerProviderInspector<BaseCameraManagerProvider>
    {
    }

    /// <summary>
    /// 基础相机管理器提供者检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraManagerProviderInspector<T> : MBInspector<T>
       where T : BaseCameraManagerProvider
    {
    }
}
