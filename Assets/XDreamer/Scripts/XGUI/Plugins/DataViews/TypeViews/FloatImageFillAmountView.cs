using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 单精度图像填充量数据视图：数值范围在0到1
    /// </summary>
    [Name("单精度图像填充量数据视图")]
    [Tip("数值范围在0到1", "Values range from 0 to 1")]
    [DataViewAttribute(typeof(float))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class FloatImageFillAmountView : BaseModelViewCache
    {
        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Image _progressView;

        /// <summary>
        /// 视图值类型
        /// </summary>
        public override Type viewValueType => typeof(float);

        /// <summary>
        /// 视图值
        /// </summary>
        public override object viewValue { get => _progressView.fillAmount; set => _progressView.fillAmount = (float)value; }
    }
}
