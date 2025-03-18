using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 图形颜色，单向接收颜色, 设置图像
    /// </summary>

    [Name("颜色图像")]
    public class ColorImage : ColorPickerObserver
    {
        [Name("图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Image _image;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }

        /// <summary>
        /// HSV变化回调函数
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            if (_image)
            {
                _image.color = color;
            }
        }
    }
}