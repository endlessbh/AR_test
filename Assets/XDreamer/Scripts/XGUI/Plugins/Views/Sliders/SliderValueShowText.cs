using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条值显示文本
    /// </summary>
    //[Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [Name("滑动条值显示文本")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    [ExecuteInEditMode]
    public class SliderValueShowText : View
    {
        /// <summary>
        /// 滑动条
        /// </summary>
        [Name("滑动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Slider slider;

        /// <summary>
        /// 当前值文本
        /// </summary>
        [Name("当前值文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text text;

        /// <summary>
        /// 显示文本小数位数
        /// </summary>
        [Name("显示文本小数位数")]
        [Range(0, 5)]
        [HideInSuperInspector(nameof(text), EValidityCheckType.Null)]
        public int showTextDecimalDigitNumber = 1;

        /// <summary>
        /// 最小值文本
        /// </summary>
        [Name("最小值文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _minValueText;

        /// <summary>
        /// 最大值文本
        /// </summary>
        [Name("最大值文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _maxValueText;

        /// <summary>
        /// 滑竿值输入框
        /// </summary>
        [Name("滑竿值输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _valueInputField;

        protected void Awake()
        {
            if (!slider) slider = GetComponent<Slider>();
            if (!text) text = GetComponent<Text>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (slider)
            {
                slider.onValueChanged.AddListener(OnSliderValueChanged);

                OnSliderValue();
            }

            if (_valueInputField)
            {
                _valueInputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                _valueInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (slider)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }

            if (_valueInputField)
            {
                _valueInputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
                _valueInputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                OnSliderValue();
            }
        }
#endif

        protected virtual void OnSliderValueChanged(float value)
        {
            OnSliderValue();
        }

        private bool setInputField = false;

        private void OnSliderValue()
        {
            if (!slider) return;

            // 设置当前值文本
            if (text)
            {
                text.text = slider.value.ToString("F" + showTextDecimalDigitNumber.ToString());
            }

            // 设置最小值文本
            if (_minValueText)
            {
                _minValueText.text = slider.minValue.ToString();
            }

            // 设置最大值文本
            if (_maxValueText)
            {
                _maxValueText.text = slider.maxValue.ToString();
            }

            // 设置输入框值
            if (_valueInputField)
            {
                setInputField = true;
                _valueInputField.text = slider.value.ToString();
                setInputField = false;
            }
        }

        private void OnInputFieldValueChanged(string value)
        {
            if (setInputField) return;

            if (slider && float.TryParse(value, out var velocity))
            {
                if (velocity >= slider.minValue && velocity <= slider.maxValue)
                {
                    return;
                }

                slider.value = velocity;
            }
        }

        private void OnInputFieldEndEdit(string value)
        {
            if (setInputField) return;

            if (slider && float.TryParse(value, out var velocity))
            {
                slider.value = velocity;
            }
        }
    }
}
