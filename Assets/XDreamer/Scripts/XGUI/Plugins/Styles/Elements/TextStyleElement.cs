using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 文本样式
    /// </summary>
    [StyleLink(typeof(Text), typeof(EParamsSetting))]
    [Name("文本")]
    public class TextStyleElement : BaseStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
            [Name("字体")]
            [EnumFieldName("字体")]
            Font = 1 << 0,

            [Name("字体样式")]
            [EnumFieldName("字体样式")]
            FontStyle = 1 << 1,

            [Name("字体大小")]
            [EnumFieldName("字体大小")]
            FontSize = 1 << 2,

            [Name("行间距")]
            [EnumFieldName("行间距")]
            LineSpace = 1 << 3,

            [Name("文本对齐")]
            [EnumFieldName("文本对齐")]
            Alignment = 1 << 4,

            [Name("水平包裹模式")]
            [EnumFieldName("水平包裹模式")]
            HorzWrap = 1 << 5,

            [Name("垂直包裹模式")]
            [EnumFieldName("垂直包裹模式")]
            VertWrap = 1 << 6,

            [Name("颜色")]
            [EnumFieldName("颜色")]
            Color = 1 << 7,

            [Name("材质")]
            [EnumFieldName("材质")]
            Material = 1 << 8,
        }

        [Group("特征")]
        /// <summary>
        /// 字体
        /// </summary>
        [Name("字体")]
        public Font _font;

        /// <summary>
        /// 字体样式
        /// </summary>
        [Name("字体样式")]
        public FontStyle _fontStyle = FontStyle.Normal;

        /// <summary>
        /// 字体大小
        /// </summary>
        [Name("字体大小")]
        public int _fontSize = 14;

        /// <summary>
        /// 行间距
        /// </summary>
        [Name("行间距")]
        public float _lineSpacing = 1;

        [Group("段落")]
        /// <summary>
        /// 文本对齐
        /// </summary>
        [Name("文本对齐")]
        public TextAnchor _alignment = TextAnchor.UpperLeft;

        /// <summary>
        /// 水平包裹模式
        /// </summary>
        [Name("水平包裹模式")]
        public HorizontalWrapMode _horizontalWrapMode = HorizontalWrapMode.Overflow;

        /// <summary>
        /// 垂直包裹模式
        /// </summary>
        [Name("垂直包裹模式")]
        public VerticalWrapMode _verticalWrapMode = VerticalWrapMode.Truncate;

        [Group("颜色材质")]
        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public Color _color = new Color(0f, 0f, 0f, 1f);

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        public Material _material;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Text, typeof(EParamsSetting), paramsMask, (text, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Font: text.font = _font; break;
                    case EParamsSetting.FontStyle: text.fontStyle = _fontStyle; break;
                    case EParamsSetting.FontSize: text.fontSize = _fontSize; break;
                    case EParamsSetting.LineSpace: text.lineSpacing = _lineSpacing; break;
                    case EParamsSetting.Alignment: text.alignment = _alignment; break;
                    case EParamsSetting.HorzWrap: text.horizontalOverflow = _horizontalWrapMode; break;
                    case EParamsSetting.VertWrap: text.verticalOverflow = _verticalWrapMode; break;
                    case EParamsSetting.Color: text.color = _color; break;
                    case EParamsSetting.Material: text.material = _material; break;
                }
            });
        }

        /// <summary>
        /// 对象提取样式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Text, typeof(EParamsSetting), paramsMask, (text, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Font: _font = text.font; break;
                    case EParamsSetting.FontStyle: _fontStyle = text.fontStyle; break;
                    case EParamsSetting.FontSize: _fontSize = text.fontSize; break;
                    case EParamsSetting.LineSpace: _lineSpacing = text.lineSpacing; break;
                    case EParamsSetting.Alignment: _alignment = text.alignment; break;
                    case EParamsSetting.HorzWrap: _horizontalWrapMode = text.horizontalOverflow; break;
                    case EParamsSetting.VertWrap: _verticalWrapMode = text.verticalOverflow; break;
                    case EParamsSetting.Color: _color = text.color; break;
                    case EParamsSetting.Material: _material = text.material; break;
                }
            });
        }
    }

}
