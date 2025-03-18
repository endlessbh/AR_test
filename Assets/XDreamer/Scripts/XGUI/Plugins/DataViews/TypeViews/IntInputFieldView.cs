using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 整数输入框数据视图
    /// </summary>
    [Name("整数输入框数据视图")]
    [DataViewAttribute(typeof(int))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class IntInputFieldView : BaseModelViewCache
    {
        /// <summary>
        /// 输入框
        /// </summary>
        [Name("输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _intView;

        public override Type viewValueType => typeof(int);

        public override object viewValue 
        { 
            get
            {
                int.TryParse(_intView.text, out var intValue);
                return intValue;
            }
            set
            {
                _intView.text = value.ToString();
            }
        }

        public void Awake()
        {
            _intView.contentType = InputField.ContentType.IntegerNumber;
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _intView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _intView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }
    }
}
