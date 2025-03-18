using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 基础模拟控制器IO检查器
    /// </summary>
    [CustomEditor(typeof(BaseAnalogControllerIO), true)]
    public class BaseAnalogControllerIOInspector : BaseAnalogControllerIOInspector<BaseAnalogControllerIO>
    {
    }

    /// <summary>
    /// 基础模拟控制器IO检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseAnalogControllerIOInspector<T> : MBInspector<T>
        where T : BaseAnalogControllerIO
    {
    }
}
