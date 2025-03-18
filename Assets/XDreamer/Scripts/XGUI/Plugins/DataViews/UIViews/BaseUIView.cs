using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 数据视图：将底层数据和上层视图对象进行绑定
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public abstract class BaseUIView<TView> : BaseModelViewCache where TView:Component
    {
        [Name("视图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public TView _view;

        public TView view => this.XGetComponent(ref _view);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (view) { }
        }
    }
}
