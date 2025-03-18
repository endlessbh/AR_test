using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色按钮
    /// </summary>

    [Name("颜色按钮")]
    public class ColorButtonCreater : ColorButton
    {
        /// <summary>
        /// 点击
        /// </summary>
        protected override void OnClick() { }

        /// <summary>
        /// RGB回调函数
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            base.OnRGBAChanged(colorPicker, color);

            this.color = color;
        }
    }
}
