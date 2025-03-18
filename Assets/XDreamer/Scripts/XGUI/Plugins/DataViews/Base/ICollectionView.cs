using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 集合型数据视图
    /// </summary>
    //[Name("集合型数据视图")]
    //[Tool(XGUICategory.BaseDataView, rootType = typeof(XGUIManager))]
    //[DataViewAttribute(typeof(ICollection))]
    public abstract class ICollectionView : BaseModelView
    {
        [Name("模版视图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform _templateView;

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

        }

    }
}
