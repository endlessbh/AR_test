using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Sliders;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色区域
    /// </summary>

    [Name("颜色区域")]
    public class SVBoxSlider : ColorPickerObserver
    {
        /// <summary>
        /// 贴图宽高
        /// </summary>
        [Name("贴图宽高")]
        public Vector2Int _textureSize = new Vector2Int(128, 128);

        private void OnValidate()
        {
            _textureSize.x = Mathf.Clamp(_textureSize.x, 8, 1024);
            _textureSize.y = Mathf.Clamp(_textureSize.y, 8, 1024);
        }

        /// <summary>
        /// 盒型滑动条
        /// </summary>
        [Name("盒型滑动条")]
        [ComponentPopup]
        public BoxSlider _slider;

        /// <summary>
        /// 贴图
        /// </summary>
        [Name("贴图")]
        [ComponentPopup]
        public RawImage _image;

        public bool dataValid => _slider && _image;

        private float lastH = -1;

        protected override void Awake()
        {
            base.Awake();

            if (!_slider) _slider = GetComponent<BoxSlider>();
            if (!_image) _image = GetComponent<RawImage>();

            if (dataValid)
            {
                _slider.onValueChanged.AddListener(SliderChanged);

                CreateSVTexture(_colorPicker != null ? _colorPicker.normalizedH : 0);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_image && _image.texture)
            {
                DestroyImmediate(_image.texture);
            }

            if (_slider)
            {
                _slider.onValueChanged.RemoveListener(SliderChanged);
            }
        }

        /// <summary>
        /// HSV变化回调函数
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="hsv"></param>
        protected override void OnHSVChanged(ColorPicker colorPicker, Vector3 hsv)
        {
            if (_colorPicker != colorPicker) return;

            if (lastH != hsv.x)
            {
                lastH = hsv.x;
                CreateSVTexture(hsv.x);
            }

            if (hsv.y != _slider.normalizedX)
            {
                _slider.normalizedX = hsv.y;
            }

            if (hsv.z != _slider.normalizedY)
            {
                _slider.normalizedY = hsv.z;
            }
        }

        private void SliderChanged(float s, float v)
        {
            if (_colorPicker)
            {
                _colorPicker.normalizedS = s;
                _colorPicker.normalizedV = v;
            }
        }

        private void CreateSVTexture(float h)
        {
            if (_image.texture != null) DestroyImmediate(_image.texture);

            _image.texture = CreateSVTexture(h, _textureSize.x, _textureSize.y);
        }

        /// <summary>
        /// 依据H生成SV图（色区）
        /// </summary>
        /// <returns></returns>
        public static Texture2D CreateSVTexture(float h, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.hideFlags = HideFlags.DontSave;

            for (int s = 0; s < width; s++)
            {
                Color[] colors = new Color[height];
                for (int v = 0; v < height; v++)
                {
                    colors[v] = Color.HSVToRGB(h, s * 1.0f / width, v * 1.0f / height);
                }
                texture.SetPixels(s, 0, 1, height, colors);
            }
            texture.Apply();

            return texture;
        }
    }
}