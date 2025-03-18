using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色按钮
    /// </summary>
    [Name("颜色按钮")]
    [RequireComponent(typeof(Button))]
    public class ColorButton : ColorPickerObserver
    {
        /// <summary>
        /// 按钮设置
        /// </summary>
        public Button button 
        { 
            get
            {
                if (!_button)
                {
                    _button = GetComponent<Button>();
                }
                return _button;
            }
            private set { }
        }
        private Button _button = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            button.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            button.onClick.RemoveListener(OnClick);
        }

        /// <summary>
        /// 点击响应：设置颜色拾取器的值
        /// </summary>
        protected virtual void OnClick()
        {
            if (_colorPicker)
            {
                _colorPicker.color = color;
            }
        }

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color
        {
            get
            {
                if (button.image)
                {
                    return button.image.color;
                }
                return Color.clear;
            }
            set
            {
                if (button.image)
                {
                    button.image.color = value;
                }
            }
        }
    }
}
