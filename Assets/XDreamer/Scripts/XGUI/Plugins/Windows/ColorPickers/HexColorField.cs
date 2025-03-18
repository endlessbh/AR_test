using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 十六进制的颜色输入
    /// </summary>
    [Name("十六进制颜色输入框")]
    [RequireComponent(typeof(InputField))]
    public class HexColorField : ColorPickerObserver
    {
        /// <summary>
        /// 显示透明度值
        /// </summary>
        [Name("显示透明度值")]
        public bool displayAlpha;

        private InputField _hexInputField;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _hexInputField = GetComponent<InputField>();

            _hexInputField.onEndEdit.AddListener(SetColor);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _hexInputField.onEndEdit.RemoveListener(SetColor);
        }

        private void SetColor(string newHex)
        {
            Color color;
            if (!newHex.StartsWith("#"))
            {
                newHex = "#" + newHex;
            }
               
            if (ColorUtility.TryParseHtmlString(newHex, out color))
            {
                _colorPicker.color = color;
            }
            else
            {
                Debug.Log("hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
            }
        }

        private string ColorToHex(Color32 color)
        {
            return displayAlpha
                ? string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a)
                : string.Format("#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
        }

        /// <summary>
        /// RGBA颜色变化回调函数
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            _hexInputField.text = ColorToHex(color);
        }
    }
}