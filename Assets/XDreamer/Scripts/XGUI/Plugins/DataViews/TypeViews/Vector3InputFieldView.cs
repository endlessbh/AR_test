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
    /// 三维向量输入框数据视图
    /// </summary>
    [Name("三维向量输入框数据视图")]
    [DataViewAttribute(typeof(Vector3))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class Vector3InputFieldView : BaseModelViewCache
    {
        /// <summary>
        /// X值
        /// </summary>
        [Name("X值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _x;

        /// <summary>
        /// Y值
        /// </summary>
        [Name("Y值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _y;

        /// <summary>
        /// Z值
        /// </summary>
        [Name("Z值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _z;

        public override Type viewValueType => typeof(Vector3);

        public override object viewValue
        {
            get => lastValue;
            set
            {
                lastValue = (Vector3)value;

                if (_x)
                {
                    _x.text = Mathf.Abs(lastValue.x) < 0.00001 ? lastValue.x.ToString("F") : lastValue.x.ToString();
                }

                if (_y)
                {
                    _y.text = Mathf.Abs(lastValue.y) < 0.00001 ? lastValue.y.ToString("F") : lastValue.y.ToString();
                }

                if (_z)
                {
                    _z.text = Mathf.Abs(lastValue.z) < 0.00001 ? lastValue.z.ToString("F") : lastValue.z.ToString();
                }
            }
        }
        private Vector3 lastValue;

        public void Awake()
        {
            if (_x) _x.contentType = InputField.ContentType.DecimalNumber;
            if (_y) _y.contentType = InputField.ContentType.DecimalNumber;
            if (_z) _z.contentType = InputField.ContentType.DecimalNumber;
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_x) _x.onValueChanged.AddListener(OnXValueChanged);
            if (_y) _y.onValueChanged.AddListener(OnYValueChanged);
            if (_z) _z.onValueChanged.AddListener(OnZValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (_x) _x.onValueChanged.RemoveListener(OnXValueChanged);
            if (_y) _y.onValueChanged.RemoveListener(OnYValueChanged);
            if (_z) _z.onValueChanged.RemoveListener(OnZValueChanged);
        }

        private void OnXValueChanged(string value)
        {
            if (inModelToView) return;

            if (float.TryParse(value, out var v))
            {
                lastValue.x = v;
                ViewToModelIfCanAndTrigger();
            }
        }

        private void OnYValueChanged(string value)
        {
            if (inModelToView) return;

            if (float.TryParse(value, out var v))
            {
                lastValue.y = v;
                ViewToModelIfCanAndTrigger();
            }
        }

        private void OnZValueChanged(string value)
        {
            if (inModelToView) return;

            if (float.TryParse(value, out var v))
            {
                lastValue.z = v;
                ViewToModelIfCanAndTrigger();
            }
        }
    }
}
