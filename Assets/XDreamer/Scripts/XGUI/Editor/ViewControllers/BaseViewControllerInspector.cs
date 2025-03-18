using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.DataViews.GroupViews;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 基础视图控制器检查器
    /// </summary>
    [Name("基础视图控制器检查器")]
    [CustomEditor(typeof(BaseViewController), true)]
    [CanEditMultipleObjects]
    public class BaseViewControllerInspector : BaseViewControllerInspector<BaseViewController>
    {
    }

    /// <summary>
    /// 基础视图控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseViewControllerInspector<T> : ViewInteractorInspector<T> where T : BaseViewController
    {
    }
}
