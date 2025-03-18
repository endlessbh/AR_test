using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginXGUI.Views.ScrollViews
{
    public abstract class UIItem<T> : View where T : Component
    {
        public UIContainer<T> container { get; set; }
    }
}
