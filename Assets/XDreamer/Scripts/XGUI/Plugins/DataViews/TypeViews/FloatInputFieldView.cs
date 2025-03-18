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
    /// 单精度输入框数据视图
    /// </summary>
    [Name("单精度输入框数据视图")]
    [DataViewAttribute(typeof(float))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class FloatInputFieldView : BaseModelViewCache
    {
        [Name("输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _floatView;

        public override Type viewValueType => typeof(float);

        public override object viewValue 
        { 
            get
            {
                float.TryParse(_floatView.text, out var value);
                return value;
            }
            set 
            { 
                var fv = (float)value;
                _floatView.text = Mathf.Abs(fv) < 0.00001 ? fv.ToString("F") : fv.ToString(); 
            }  
        }

        public void Awake()
        {
            _floatView.contentType = InputField.ContentType.DecimalNumber;
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _floatView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _floatView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }
    }
}
