using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 显示某个颜色值的Slider
    /// </summary>
    [Name("颜色滑竿")]
    [RequireComponent(typeof(Slider))]
    public class ColorSlider : ColorValueObserver
    {
        /// <summary>
        /// 滑动条
        /// </summary>
        [Name("滑动条")]
        [ComponentPopup]
        public Slider _slider;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (_slider)
            {
                _slider = GetComponent<Slider>();
                _slider.onValueChanged.AddListener(OnSliderChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDestroy()
        {
            if (_slider)
            {
                _slider.onValueChanged.RemoveListener(OnSliderChanged);
            }

            base.OnDestroy();
        }

        /// <summary>
        /// 使用滑动条归一化的值
        /// </summary>
        /// <param name="value"></param>
        private void OnSliderChanged(float value) => SetColorValue(_slider.normalizedValue);

        /// <summary>
        /// 颜色单值变更回调事件
        /// </summary>
        /// <param name="value"></param>
        protected override void OnValueChanged(float value) => _slider.normalizedValue = value;
    }
}