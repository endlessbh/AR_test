using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 当前色块
    /// </summary>
    [Name("当前色块")]
    public class ColorPresets : ColorPickerObserver
    {
        /// <summary>
        /// 预制颜色列表
        /// </summary>
        [Name("预制颜色列表")]
        public List<Color> _colorList = new List<Color>();

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色克隆对象")] 
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ColorButton _colorButton;

        /// <summary>
        /// 创建颜色按钮
        /// </summary>
        [Name("创建预制颜色")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ColorButtonCreater _createButton;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (_colorButton)
            {
                _colorButton.gameObject.SetActive(false);
                _colorList.ForEach(c => CreateColorButton(c));
            }

            if (_createButton)
            {
                _createButton.button.onClick.AddListener(OnCreateButtonClick);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_createButton) _createButton.button.onClick.RemoveListener(OnCreateButtonClick);
        }
        
        private void OnCreateButtonClick() => CreateColorButton(_createButton.color);

        private ColorButton CreateColorButton(Color color)
        {
            if (!_colorButton) return null;

            try
            {
                var btn = GameObject.Instantiate(_colorButton, _colorButton.transform.parent);
                btn.gameObject.SetActive(true);
                btn.color = color;
                return btn;
            }
            catch
            {
                return null;
            }
            finally
            {
                // 将创建按钮设置为最后一个对象
                _createButton.transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// 颜色变更事件回调
        /// </summary>
        /// <param name="colorPicker"></param>
        /// <param name="color"></param>
        protected override void OnRGBAChanged(ColorPicker colorPicker, Color color)
        {
            if (_colorPicker != colorPicker) return;

            if (_createButton) _createButton.color = color;
        }
    }
}