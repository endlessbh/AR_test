using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI.Base
{
    /// <summary>
    /// 视图检查器：视图组件检查器
    /// </summary>
    [Name("视图检查器")]
    [CustomEditor(typeof(View), true)]
    public class ViewInspector : ViewInspector<View> { }

    /// <summary>
    /// 视图检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewInspector<T> : InteractProviderInspector<T> where T : View
    {
    }


    /// <summary>
    /// 视图交互器检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewInteractorInspector<T> : InteractorInspector<T> where T : ViewInteractor
    {
    }

    /// <summary>
    /// 可拖拽视图交互器兼差
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DraggableViewInteractorInspector<T> : ViewInteractorInspector<T> where T : DraggableViewInteractor
    {
    }
}
