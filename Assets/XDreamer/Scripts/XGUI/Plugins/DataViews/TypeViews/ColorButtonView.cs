using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows.ColorPickers;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 颜色按钮数据视图：点击按钮调出调色板窗口，通过调色板窗口修改绑定的颜色属性值
    /// </summary>
    [Name("颜色按钮数据视图")]
    [Tip("点击按钮调出调色板窗口，通过调色板窗口修改绑定的颜色属性值", "Click the button to call up the palette window, and modify the bound color attribute value through the palette window")]
    [DataViewAttribute(typeof(Color))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class ColorButtonView : BaseModelViewCache
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _colorView;

        [Name("调色板资产")]
        public ColorPickerAsset _colorPickerAsset;

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _colorView.onClick.AddListener(OnClick);

            ColorPicker.onDisable += OnColorPickerDisable;
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _colorView.onClick.RemoveListener(OnClick);

            ColorPicker.onDisable -= OnColorPickerDisable;
        }

        [Languages.LanguageTuple("No palette objects exist in the scene", "场景中不存在调色板对象")]
        private void OnClick()
        {
            colorPicker = _colorPickerAsset.view;
            if (colorPicker)
            {
                // 先隐藏解除之前其他颜色按钮与其的关系
                colorPicker.Hide();

                colorPicker.Display((Color)viewValue);
                connect = true;

                ColorPicker.onColorChanged -= OnColorPickerColorChanged;
                ColorPicker.onColorChanged += OnColorPickerColorChanged;

            }
            else
            {
                Debug.LogError(LanguageHelper.Tr("No palette objects exist in the scene", this));
            }
        }

        private ColorPicker colorPicker;
        private bool connect = false;

        /// <summary>
        /// 视图值类型
        /// </summary>
        public override Type viewValueType => typeof(Color);

        /// <summary>
        /// 视图值
        /// </summary>
        public override object viewValue 
        { 
            get => _colorView.colors.normalColor; 
            set 
            {
                XGUIHelper.SetColor(_colorView, (Color)value);
            } 
        }

        private void OnColorPickerColorChanged(ColorPicker colorPicker, Color color)
        {
            if (this.colorPicker == colorPicker)
            {
                viewValue = color;

                ViewToModelIfCanAndTrigger();
            }
        }

        private void OnColorPickerDisable(MB colorPicker)
        {
            if (this.colorPicker == colorPicker && connect)
            {
                ColorPicker.onColorChanged -= OnColorPickerColorChanged;
                connect = false;
            }
        }
    }
}
