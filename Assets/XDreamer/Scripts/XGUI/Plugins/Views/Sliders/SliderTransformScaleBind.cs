using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条控制缩放
    /// </summary>
    [Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [Name("滑动条控制缩放")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class SliderTransformScaleBind : SliderTransformBind
    {
        private Vector3 recordScale;

        /// <summary>
        /// 初始化滑竿值
        /// </summary>
        protected override void InitSliderValue()
        {
            if (_targetTransform)
            {
                xyzSlider.SetValue(_targetTransform.localScale);
            }
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        protected override void RecordData()
        {
            if (_targetTransform)
            {
                recordScale = transform.localScale;
            }
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        protected override void RecoverData()
        {
            if (_targetTransform)
            {
                transform.localScale = recordScale;
            }
        }

        /// <summary>
        /// X滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderXValueChanged(float value)
        {
            if (_targetTransform)
            {
                var ls = _targetTransform.localScale;
                _targetTransform.localScale = new Vector3(value, ls.y, ls.z);
            }
        }

        /// <summary>
        /// Y滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderYValueChanged(float value)
        {
            if (_targetTransform)
            {
                var ls = _targetTransform.localScale;
                _targetTransform.localScale = new Vector3(ls.x, value, ls.z);
            }
        }

        /// <summary>
        /// Z滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderZValueChanged(float value)
        {
            if (_targetTransform)
            {
                var ls = _targetTransform.localScale;
                _targetTransform.localScale = new Vector3(ls.x, ls.y, value);
            }
        }
    }

}
