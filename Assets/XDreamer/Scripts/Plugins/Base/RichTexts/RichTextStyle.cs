using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Tools;

namespace XCSJ.Extension.Base.RichTexts
{
    /// <summary>
    /// 富文本样式
    /// </summary>
    [Name("富文本样式")]
    [Import]
    [Serializable]
    public class RichTextStyle : Option, IToFriendlyString
    {
        public string name = "";
        public string tip = "";

        public bool colorFlag = false;
        [Json(exportString = true)]
        public Color color = UnityEngine.Color.black;

        public bool sizeFlag = false;
        public int size = 11;

        public bool bold = false;
        public bool italic = false;

        [Json(false)]
        public bool hasFlag => colorFlag || sizeFlag || bold || italic;

        public RichTextStyle()
        {
            expand = true;
            active = true;
            enable = true;
        }
        public RichTextStyle(int size, bool bold) : this()
        {
            this.size = size;
            this.sizeFlag = true;
            this.bold = bold;
        }
        public RichTextStyle(Color color, int size, bool bold) : this(size, bold)
        {
            this.color = color;
            this.colorFlag = true;
        }
        public RichTextStyle(Color color, int size, bool bold, bool italic) : this(color, size, bold)
        {
            this.italic = italic;
        }
        public RichTextStyle(string text) : this()
        {
            Set(text);
        }
        public RichTextStyle(RichTextStyle style)
        {
            Set(style);
        }

        public bool IsSameStyle(RichTextStyle style)
        {
            if (style == null) return false;
            if (style == this) return true;
            if (colorFlag != style.colorFlag || sizeFlag != style.sizeFlag || bold != style.bold || italic != style.italic) return false;
            if (colorFlag && color != style.color) return false;
            if (sizeFlag && size != style.size) return false;
            return true;
        }

        public void Set(RichTextStyle style)
        {
            this.expand = style.expand;
            this.enable = style.enable;
            this.active = style.active;

            this.name = style.name;
            this.tip = style.tip;

            this.colorFlag = style.colorFlag;
            this.color = style.color;
            this.sizeFlag = style.sizeFlag;
            this.size = style.size;
            this.bold = style.bold;
            this.italic = style.italic;
        }
        public void Set(string text)
        {
            Set(RichText.GetOption(text));
        }
        public void Set(bool flag)
        {
            colorFlag = flag;
            sizeFlag = flag;
            bold = flag;
            italic = flag;
        }
        public string Get(string text, bool replace = true) => RichText.Add(text, this, replace);

        public string Add(string text)
        {
            Set(true);
            return Get(text);
        }
        public string Clear(string text)
        {
            Set(false);
            return Get(text);
        }
        public string Format(string text, bool flag)
        {
            return flag ? Add(text) : Clear(text);
        }

        public string Color(string text, bool colorFlag)
        {
            return colorFlag ? AddColorIfNeed(text) : ClearColor(text);
        }
        public string Color(string text, Color color)
        {
            return ReplaceColor(text, color, true);
        }
        public string Color(string text)
        {
            return colorFlag ? ReplaceColor(text) : ClearColor(text);
        }
        public string AddColor(string text)
        {
            colorFlag = true;
            return RichText.AddColor(text, color);
        }
        public string AddColorIfNeed(string text)
        {
            colorFlag = true;
            return RichText.AddColorIfNeed(text, color);
        }
        public string ClearColor(string text)
        {
            colorFlag = false;
            return RichText.ClearColor(text);
        }
        public string ReplaceColor(string text, Color color, bool colorFlag = true)
        {
            this.color = color;
            this.colorFlag = colorFlag;
            return Color(text);
        }
        public string ReplaceColor(string text) => AddColor(ClearColor(text));

        public string Size(string text, bool sizeFlag)
        {
            return sizeFlag ? AddSizeIfNeed(text) : ClearSize(text);
        }
        public string Size(string text, int size)
        {
            return ReplaceSize(text, size, true);
        }
        public string Size(string text)
        {
            return sizeFlag ? ReplaceSize(text) : ClearSize(text);
        }
        public string AddSize(string text)
        {
            sizeFlag = true;
            return RichText.AddSize(text, size);
        }
        public string AddSizeIfNeed(string text)
        {
            sizeFlag = true;
            return RichText.AddSizeIfNeed(text, size);
        }
        public string ClearSize(string text)
        {
            sizeFlag = false;
            return RichText.ClearSize(text);
        }
        public string ReplaceSize(string text, int size, bool sizeFlag = true)
        {
            this.size = size;
            this.sizeFlag = sizeFlag;
            return Size(text);
        }
        public string ReplaceSize(string text) => AddSize(ClearSize(text));

        public string Bold(string text, bool bold)
        {
            return bold ? AddBoldIfNeed(text) : ClearBold(text);
        }
        public string Bold(string text)
        {
            return Bold(text, bold);
        }
        public string AddBold(string text)
        {
            bold = true;
            return RichText.AddBold(text);
        }
        public string AddBoldIfNeed(string text)
        {
            bold = true;
            return RichText.AddBoldIfNeed(text);
        }
        public string ClearBold(string text)
        {
            bold = false;
            return RichText.ClearBold(text);
        }

        public string Italic(string text, bool italic)
        {
            return italic ? AddItalicIfNeed(text) : ClearItalic(text);
        }
        public string Italic(string text)
        {
            return Italic(text, italic);
        }
        public string AddItalic(string text)
        {
            italic = true;
            return RichText.AddItalic(text);
        }
        public string AddItalicIfNeed(string text)
        {
            italic = true;
            return RichText.AddItalicIfNeed(text);
        }
        public string ClearItalic(string text)
        {
            italic = false;
            return RichText.ClearItalic(text);
        }

        public string ToFriendlyString()
        {
            return string.Format("{0}: {1}\n{2},{3},{4},{5}", name, tip, colorFlag ? ColorUtility.ToHtmlStringRGBA(color) : "默认颜色", sizeFlag ? size.ToString() : "默认字号", bold ? "加粗" : "正常", italic ? "斜体" : "正常");
        }
    }
}
