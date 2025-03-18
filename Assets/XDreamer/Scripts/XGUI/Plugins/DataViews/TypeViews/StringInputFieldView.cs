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
    /// 字符串输入框数据视图
    /// </summary>
    [Name("字符串输入框数据视图")]
    [DataViewAttribute(typeof(string))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class StringInputFieldView : BaseModelViewCache
    {
        /// <summary>
        /// 输入框
        /// </summary>
        [Name("输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _stringView;

        public override Type viewValueType => typeof(string);

        public override object viewValue { get => _stringView.text; set => _stringView.text = (string)value; }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _stringView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _stringView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }
    }
}
