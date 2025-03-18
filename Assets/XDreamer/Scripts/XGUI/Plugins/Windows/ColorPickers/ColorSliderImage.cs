using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 设置滑动条背景色的组件 ： 颜色会随着滑动条值变化而变化
    /// </summary>

    [Name("颜色滑竿背景色")]
    public class ColorSliderImage : ColorValueObserver
    {
        /// <summary>
        /// 滑动条背景色组件
        /// </summary>
        [Name("滑动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Slider slider;

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        [ComponentPopup]
        public RawImage _image;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (!_image) _image = GetComponent<RawImage>();
            if (!slider) slider = GetComponentInParent<Slider>();
        }

        /// <summary>
        /// 启动
        /// </summary>
        private void Start() => CreateTexture();

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            if (!_image && _image.texture)
            {
                DestroyImmediate(_image.texture);
            }

            base.OnDestroy();
        }

        /// <summary>
        /// 颜色变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnValueChanged(float value)
        {
            switch (colorValue)
            {
                case EColorValues.R:
                case EColorValues.G:
                case EColorValues.B: 
                case EColorValues.S:
                case EColorValues.V:
                    CreateTexture();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// 创建贴图
        /// </summary>
        private void CreateTexture()
        {
            if (!slider || !TryGetColor(out Color color) || !_image) return;
            
            if (_image.texture)
            {
                DestroyImmediate(_image.texture);
            }

            // 生成贴图
            var dir = slider.direction;
            _image.texture = CreateTexture(colorValue, color,
                dir == Slider.Direction.BottomToTop || dir == Slider.Direction.TopToBottom,
                dir == Slider.Direction.TopToBottom || dir == Slider.Direction.RightToLeft);

            // 设置UV
            switch (dir)
            {
                case Slider.Direction.BottomToTop:
                case Slider.Direction.TopToBottom:
                    _image.uvRect = new Rect(0, 0, 2, 1);
                    break;
                case Slider.Direction.LeftToRight:
                case Slider.Direction.RightToLeft:
                    _image.uvRect = new Rect(0, 0, 1, 2);
                    break;
                default:
                    break;
            }
        }
    }
}