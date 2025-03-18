using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 文本视图
    /// </summary>
    [Name("文本视图")]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [Tool(XGUICategory.Data, rootType = typeof(XGUIManager), index = 1)]
    public class TextView : BaseUIView<Text>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            [Name("文本")]
            Text,

            [Name("字体")]
            Font,

            [Name("字体样式")]
            FontStyle,

            [Name("字体大小")]
            FontSize,

            [Name("行间距")]
            LineSpace,

            [Name("文本对齐")]
            Alignment,

            [Name("水平包裹模式")]
            HorzWrap,

            [Name("垂直包裹模式")]
            VertWrap,

            [Name("颜色")]
            Color,

            [Name("材质")]
            Material,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Text;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Text: return typeof(string);
                    case EViewMember.Font: return typeof(Font);
                    case EViewMember.FontStyle: return typeof(FontStyle);
                    case EViewMember.FontSize: return typeof(int);
                    case EViewMember.LineSpace: return typeof(float);
                    case EViewMember.Alignment: return typeof(TextAnchor);
                    case EViewMember.HorzWrap: return typeof(HorizontalWrapMode);
                    case EViewMember.VertWrap: return typeof(VerticalWrapMode);
                    case EViewMember.Color: return typeof(Color);
                    case EViewMember.Material: return typeof(Material);
                    default: return null;
                }
            }
        }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public override object viewValue 
        { 
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Text: return view.text;
                    case EViewMember.Font: return view.font;
                    case EViewMember.FontStyle: return view.fontStyle;
                    case EViewMember.FontSize: return view.fontSize;
                    case EViewMember.LineSpace: return view.lineSpacing;
                    case EViewMember.Alignment: return view.alignment;
                    case EViewMember.HorzWrap: return view.horizontalOverflow;
                    case EViewMember.VertWrap: return view.verticalOverflow;
                    case EViewMember.Color: return view.color;
                    case EViewMember.Material: return view.material;
                    default: return null;
                }
            }
            set
            {
                {
                    switch (_viewMember)
                    {
                        case EViewMember.Text: view.text = value.ToString(); break;
                        case EViewMember.Font: view.font = value as Font; break;
                        case EViewMember.FontStyle: view.fontStyle = (FontStyle)value; break;
                        case EViewMember.FontSize: view.fontSize = (int)value; break;
                        case EViewMember.LineSpace: view.lineSpacing = (float)value; break;
                        case EViewMember.Alignment: view.alignment = (TextAnchor)value; break;
                        case EViewMember.HorzWrap: view.horizontalOverflow = (HorizontalWrapMode)value; break;
                        case EViewMember.VertWrap: view.verticalOverflow = (VerticalWrapMode)value; break;
                        case EViewMember.Color: view.color = (Color)value; break;
                        case EViewMember.Material: view.material = value as Material; break;
                    }
                }
            }
        }
    }
}