using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// ColorPicker 观察者
    /// </summary>
    public abstract class ColorPickerObserver : View
    {
        /// <summary>
        /// 颜色拾取器
        /// </summary>
        [Name("颜色拾取器")]
        [ComponentPopup]
        public ColorPicker _colorPicker;

        protected void Reset()
        {
            if (!_colorPicker)
            {
                _colorPicker = FindObjectOfType<ColorPicker>();
            }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (!_colorPicker) _colorPicker = GetComponentInParent<ColorPicker>();
            if (_colorPicker)
            {
                ColorPicker.onColorChanged += OnRGBAChanged;
                ColorPicker.onHSVChanged += OnHSVChanged;
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_colorPicker)
            {
                ColorPicker.onColorChanged -= OnRGBAChanged;
                ColorPicker.onHSVChanged -= OnHSVChanged;
            }
        }

        /// <summary>
        /// RGBA颜色变化回调函数;空函数；
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected virtual void OnRGBAChanged(ColorPicker colorPicker, Color color) { }

        /// <summary>
        /// HSV变化回调函数 ：空函数
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="hsv"></param>
        protected virtual void OnHSVChanged(ColorPicker colorPicker, Vector3 hsv) { }

        /// <summary>
        /// 获取当前颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool TryGetColor(out Color color)
        {
            if (_colorPicker)
            {
                color = _colorPicker.color;
                return true;
            }
            else
            {
                color = Color.clear;
                return false;
            }
        }
    }
}
