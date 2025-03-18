using XCSJ.Extension.Base.Components;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.GroupViews;

namespace XCSJ.PluginXGUI.ViewControllers
{
    /// <summary>
    /// 视图控制器基类
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    public abstract class BaseViewController : ViewInteractor, IController
    {
        public virtual void Reset() { }
    }
}
