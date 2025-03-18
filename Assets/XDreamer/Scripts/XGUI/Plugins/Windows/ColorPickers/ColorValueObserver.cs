using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色值
    /// </summary>
    [Name("颜色值")]
    public enum EColorValues
    {
        /// <summary>
        /// 红
        /// </summary>
        [Name("红")]
        R,

        /// <summary>
        /// 绿
        /// </summary>
        [Name("绿")]
        G,

        /// <summary>
        /// 蓝
        /// </summary>
        [Name("蓝")]
        B,

        /// <summary>
        /// 透明度
        /// </summary>
        [Name("透明度")]
        A,

        /// <summary>
        /// 色调
        /// </summary>
        [Name("色调")]
        H,

        /// <summary>
        /// 饱和度
        /// </summary>
        [Name("饱和度")]
        S,

        /// <summary>
        /// 明度
        /// </summary>
        [Name("明度")]
        V
    }

    /// <summary>
    /// 颜色值交互器
    /// </summary>
    public abstract class ColorValueObserver : ColorPickerObserver
    {
        /// <summary>
        /// 颜色类型
        /// </summary>
        [Name("颜色类型")]
        [EnumPopup]
        public EColorValues colorValue = EColorValues.R;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (TryGetColorValue(out float value))
            {
                OnValueChanged(value);
            }
        }

        /// <summary>
        /// RGBA值变化回调
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            switch (colorValue)
            {
                case EColorValues.R: OnValueChanged(color.r); break;
                case EColorValues.G: OnValueChanged(color.g); break;
                case EColorValues.B: OnValueChanged(color.b); break;
                case EColorValues.A: OnValueChanged(color.a); break;
                default: break;
            }
        }

        /// <summary>
        /// HSV值变化回调
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="hsv"></param>
        protected override void OnHSVChanged(ColorPicker colorPicker, Vector3 hsv)
        {
            if (_colorPicker != colorPicker) return;

            switch (colorValue)
            {
                case EColorValues.H: OnValueChanged(hsv.x); break;
                case EColorValues.S: OnValueChanged(hsv.y); break;
                case EColorValues.V: OnValueChanged(hsv.z); break;
                default: break;
            }
        }

        /// <summary>
        /// 值变化回调
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnValueChanged(float value);

        /// <summary>
        /// 根据颜色类型获取颜色值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool TryGetColorValue(out float value)
        {
            if (_colorPicker)
            {
                switch (colorValue)
                {
                    case EColorValues.R: value = _colorPicker.normalizedR; return true;
                    case EColorValues.G: value = _colorPicker.normalizedG; return true;
                    case EColorValues.B: value = _colorPicker.normalizedB; return true;
                    case EColorValues.A: value = _colorPicker.normalizedA; return true;
                    case EColorValues.H: value = _colorPicker.normalizedH; return true;
                    case EColorValues.S: value = _colorPicker.normalizedS; return true;
                    case EColorValues.V: value = _colorPicker.normalizedV; return true;
                }
            }
            value = 0;
            return false;
        }

        /// <summary>
        /// 设置颜色值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        protected bool SetColorValue(float value)
        {
            if (!_colorPicker) return false;

            switch (colorValue)
            {
                case EColorValues.R: _colorPicker.normalizedR = value; return true;
                case EColorValues.G: _colorPicker.normalizedG = value; return true;
                case EColorValues.B: _colorPicker.normalizedB = value; return true;
                case EColorValues.A: _colorPicker.normalizedA = value; return true;
                case EColorValues.H: _colorPicker.normalizedH = value; return true;
                case EColorValues.S: _colorPicker.normalizedS = value; return true;
                case EColorValues.V: _colorPicker.normalizedV = value; return true;
                default: return false;
            }
        }

        /// <summary>
        /// 创建条形贴图
        /// </summary>
        /// <param name="type">颜色类型</param>
        /// <param name="color"></param>
        /// <param name="vertical">水平或垂直</param>
        /// <param name="inverted">反向</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(EColorValues type, Color color, bool vertical, bool inverted)
        {
            // 分配大小
            int size;
            switch (type)
            {
                case EColorValues.R:
                case EColorValues.G:
                case EColorValues.B:
                case EColorValues.A: size = 255; break;
                case EColorValues.H: size = 360; break;
                case EColorValues.S:
                case EColorValues.V: size = 100; break;
                default: throw new System.NotImplementedException("");
            }

            Texture2D texture = vertical ? new Texture2D(1, size) : new Texture2D(size, 1);

            texture.hideFlags = HideFlags.DontSave;
            Color32[] colors = new Color32[size];

            // 上色
            Color32 baseColor = color;
            switch (type)
            {
                case EColorValues.R:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, baseColor.g, baseColor.b, 255);
                    }
                    break;
                case EColorValues.G:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, i, baseColor.b, 255);
                    }
                    break;
                case EColorValues.B:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, baseColor.g, i, 255);
                    }
                    break;
                case EColorValues.A:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, i, i, 255);
                    }
                    break;
                case EColorValues.H:
                    for (int i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = Color.HSVToRGB((float)i / size, 1, 1);
                    }
                    break;
                case EColorValues.S:
                    {
                        Color.RGBToHSV(color, out float h, out float s, out float v);
                        for (int i = 0; i < size; i++)
                        {
                            colors[inverted ? size - 1 - i : i] = Color.HSVToRGB(h, (float)i / size, v);
                        }
                    }
                    break;
                case EColorValues.V:
                    {
                        Color.RGBToHSV(color, out float h, out float s, out float v);
                        for (int i = 0; i < size; i++)
                        {
                            colors[inverted ? size - 1 - i : i] = Color.HSVToRGB(h, s, (float)i / size);
                        }
                    }
                    break;
                default: throw new System.NotImplementedException("");
            }
            texture.SetPixels32(colors);
            texture.Apply();

            return texture;
        }
    }
}
