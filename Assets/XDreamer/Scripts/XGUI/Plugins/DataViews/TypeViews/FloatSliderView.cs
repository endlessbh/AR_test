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
    /// 单精度滑动条数据视图
    /// </summary>
    [Name("单精度滑动条数据视图")]
    [DataViewAttribute(typeof(float))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class FloatSliderView : BaseModelViewCache
    {
        /// <summary>
        /// 滑动条
        /// </summary>
        [Name("滑动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Slider _floatLimitView;

        public override Type viewValueType => typeof(float);

        public override object viewValue { get => _floatLimitView.value; set => _floatLimitView.value = (float)value; }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _floatLimitView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _floatLimitView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }
    }
}
