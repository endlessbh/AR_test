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
    /// 布尔切换数据视图
    /// </summary>
    [Name("布尔切换数据视图")]
    [DataViewAttribute(typeof(bool))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class BoolToggleView : BaseModelViewCache
    {
        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _boolView;

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _boolView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _boolView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }

        public override Type viewValueType => typeof(bool);

        public override object viewValue { get => _boolView.isOn; set => _boolView.isOn = (bool)value; }
    }
}
