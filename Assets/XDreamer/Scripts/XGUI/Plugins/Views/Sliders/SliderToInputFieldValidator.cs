using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Views.InputFields;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑竿与输入框关联器
    /// </summary>
    [Name("滑竿与输入框关联器")]
    [RequireComponent(typeof(InputFieldValidator))]
    public class SliderToInputFieldValidator : View
    {
        /// <summary>
        /// 滑竿
        /// </summary>
        [Name("滑竿")]
        [ComponentPopup]
        public Slider _slider = null;

        /// <summary>
        /// 滑竿值显示精度
        /// </summary>
        [Name("滑竿值显示精度")]
        [Range(0,10)]
        public int precision = 3;

        private InputFieldValidator _fieldValidator;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            _fieldValidator = GetComponent<InputFieldValidator>();

            if (!_slider) _slider = GetComponent<Slider>();
            if (_slider)
            {
                // 将Slider输入范围设置给输入域检查器
                _fieldValidator._numberRange = new Vector2(_slider.minValue, _slider.maxValue);
                SliderToInputField();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_slider)
            {
                _fieldValidator.onValueChanged += OnInputFieldValueChanged;
                _slider.onValueChanged.AddListener(OnSliderValueChanged);

                SliderToInputField(); // 启用时将滑动条的值设置到输入文本框中
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_slider)
            {
                _fieldValidator.onValueChanged -= OnInputFieldValueChanged;
                _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }

        /// <summary>
        /// 滑动条值发生变化
        /// </summary>
        /// <param name="value"></param>
        protected void OnSliderValueChanged(float value) => SliderToInputField();

        /// <summary>
        /// 输入检查器值发生变化
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="inputValue"></param>
        protected void OnInputFieldValueChanged(bool valid, string inputValue)
        {
            if (inChanging) return;

            if (valid)
            {
                if (float.TryParse(inputValue, out float f))
                {
                    _slider.value = f;
                }
            }
            else
            {
                SliderToInputField();
            }
        }

        private bool inChanging = false;

        private void SliderToInputField()
        {
            inChanging = true;
            _fieldValidator.inputFieldValue = GetSliderValueText(_slider.value);
            inChanging = false;
        }

        private string GetSliderValueText(float value) => value.ToString("F" + precision);
    }
}
