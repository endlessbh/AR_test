using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Components;

namespace XCSJ.EditorExtension.Base.Controllers
{
    /// <summary>
    /// 基础控制器检查器
    /// </summary>
    [Name("基础控制器检查器")]
    [CustomEditor(typeof(BaseController), true)]
    public class BaseControllerInspector : BaseControllerInspector<BaseController>
    {
    }

    /// <summary>
    /// 基础控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseControllerInspector<T> : MBInspector<T>
        where T : BaseController
    {
    }
}
