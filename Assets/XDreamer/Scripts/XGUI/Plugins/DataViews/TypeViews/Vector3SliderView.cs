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
    /// 三维向量滑动条数据视图
    /// </summary>
    [Name("三维向量滑动条数据视图")]
    [DataViewAttribute(typeof(Vector3))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class Vector3SliderView : BaseModelViewCache
    {
        /// <summary>
        /// X值
        /// </summary>
        [Name("X值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Slider _x;

        /// <summary>
        /// Y值
        /// </summary>
        [Name("Y值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Slider _y;

        /// <summary>
        /// Z值
        /// </summary>
        [Name("Z值")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Slider _z;

        public override Type viewValueType => typeof(Vector3);

        public override object viewValue
        {
            get => lastValue;
            set
            {
                lastValue = (Vector3)value;

                if (_x) _x.value = lastValue.x;
                if (_y) _y.value = lastValue.y;
                if (_z) _z.value = lastValue.z;
            }
        }
        private Vector3 lastValue;

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

        private void OnXValueChanged(float value)
        {
            if (inModelToView) return;

            lastValue.x = value;
            ViewToModelIfCanAndTrigger();
        }

        private void OnYValueChanged(float value)
        {
            if (inModelToView) return;

            lastValue.y = value;
            ViewToModelIfCanAndTrigger();
        }

        private void OnZValueChanged(float value)
        {
            if (inModelToView) return;

            lastValue.z = value;
            ViewToModelIfCanAndTrigger();
        }
    }
}
