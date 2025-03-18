using System.Text.RegularExpressions;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.RichTexts
{
    public class RichText
    {
        public static XGUIStyle richText => CommonGUIStyle.richText;
        public static XGUIStyle richTextLowerLeft { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
        {
            s.alignment = TextAnchor.LowerLeft;
            s.richText = true;
            s.wordWrap = true;
        });
        public static XGUIStyle buttonRichText { get; } = new XGUIStyle(nameof(GUI.skin.button), s =>
        {
            s.richText = true;
        });

        public static string Add(string text, Color color, int size, bool bold = true, bool italic = true, bool replace = true)
        {
            if (replace) text = Clear(text);
            if (italic) text = AddItalic(text);
            if (bold) text = AddBold(text);
            text = AddSize(text, size);
            text = AddColor(text, color);
            return text;
        }
        public static string Add(string text, RichTextStyle option, bool replace = true)
        {
            if (replace) text = Clear(text);
            return AddDirect(text, option);
        }
        public static string AddDirect(string text, RichTextStyle option)
        {
            if (option.italic) text = AddItalic(text);
            if (option.bold) text = AddBold(text);
            if (option.sizeFlag) text = AddSize(text, option.size);
            if (option.colorFlag) text = AddColor(text, option.color);
            return text;
        }
        public static string Clear(string text)
        {
            text = ClearColor(text);
            text = ClearSize(text);
            text = ClearBold(text);
            text = ClearItalic(text);
            return text;
        }

        public const string BeginColorPatternLeft = "<color=#";
        public const string BeginColorPatternMid = @"[0123456789abcdefABCDEF]+";
        public const string BeginColorPatternRight = ">";
        public const string BeginColorPattern = BeginColorPatternLeft + BeginColorPatternMid + BeginColorPatternRight;
        public const string EndColorPattern = "</color>";
        public static string AddColor(string text, Color color)
        {
            return string.Format("{0}{1}{2}{3}{4}", BeginColorPatternLeft, ColorUtility.ToHtmlStringRGBA(color), BeginColorPatternRight, text, EndColorPattern);
        }
        public static string AddColorIfNeed(string text, Color color)
        {
            return HasColor(text) ? text : AddColor(text, color);
        }
        public static string ClearColor(string text)
        {
            text = Regex.Replace(text, BeginColorPattern, "");
            text = text.Replace(EndColorPattern, "");
            return text;
        }
        public static string ReplaceColor(string text, Color color) => AddColor(ClearColor(text), color);
        public static bool HasColor(string text)
        {
            return Regex.IsMatch(text, BeginColorPattern) && text.Contains(EndColorPattern);
        }
        public static bool TryGetColor(string text, out Color color)
        {
            var match = Regex.Match(text, BeginColorPattern);
            if (match.Success)
            {
                return ColorUtility.TryParseHtmlString("#" + match.Value.Replace(BeginColorPatternLeft, "").Replace(BeginColorPatternRight, ""), out color);
            }
            color = Color.black;
            return false;
        }

        public const string BeginSizePatternLeft = "<size=";
        public const string BeginSizePatternMid = @"\d+";
        public const string BeginSizePatternRight = ">";
        public const string BeginSizePattern = BeginSizePatternLeft + BeginSizePatternMid + BeginSizePatternRight;
        public const string EndSizePattern = "</size>";
        public static string AddSize(string text, int size)
        {
            if (size < 0) return text;
            return string.Format("{0}{1}{2}{3}{4}", BeginSizePatternLeft, size, BeginSizePatternRight, text, EndSizePattern);
        }
        public static string AddSizeIfNeed(string text, int size)
        {
            return HasSize(text) ? text : AddSize(text, size);
        }
        public static string ClearSize(string text)
        {
            text = Regex.Replace(text, BeginSizePattern, "");
            text = text.Replace(EndSizePattern, "");
            return text;
        }
        public static string ReplaceSize(string text, int size) => AddSize(ClearSize(text), size);
        public static bool HasSize(string text)
        {
            return Regex.IsMatch(text, BeginSizePattern) && text.Contains(EndSizePattern);
        }
        public static bool TryGetSize(string text, out int size)
        {
            var match = Regex.Match(text, BeginSizePattern);
            if (match.Success)
            {
                return int.TryParse(match.Value.Replace(BeginSizePatternLeft, "").Replace(BeginSizePatternRight, ""), out size);
            }
            size = 0;
            return false;
        }

        public const string BeginBoldPattern = "<b>";
        public const string EndBoldPattern = "</b>";
        public static string Bold(string text, bool bold)
        {
            return bold ? AddBoldIfNeed(text) : ClearBold(text);
        }
        public static string AddBold(string text)
        {
            return string.Format("{0}{1}{2}", BeginBoldPattern, text, EndBoldPattern);
        }
        public static string AddBoldIfNeed(string text)
        {
            return HasBold(text) ? text : AddBold(text);
        }
        public static string ClearBold(string text)
        {
            text = text.Replace(BeginBoldPattern, "");
            text = text.Replace(EndBoldPattern, "");
            return text;
        }
        public static bool HasBold(string text) => text.Contains(BeginBoldPattern) && text.Contains(EndBoldPattern);

        public const string BeginItalicPattern = "<i>";
        public const string EndItalicPattern = "</i>";
        public static string Italic(string text, bool italic)
        {
            return italic ? AddItalicIfNeed(text) : ClearItalic(text);
        }
        public static string AddItalic(string text)
        {
            return string.Format("{0}{1}{2}", BeginItalicPattern, text, EndItalicPattern);
        }
        public static string AddItalicIfNeed(string text)
        {
            return HasItalic(text) ? text : AddItalic(text);
        }
        public static string ClearItalic(string text)
        {
            text = text.Replace(BeginItalicPattern, "");
            text = text.Replace(EndItalicPattern, "");
            return text;
        }
        public static bool HasItalic(string text) => text.Contains(BeginItalicPattern) && text.Contains(EndItalicPattern);

        public static RichTextStyle GetOption(string text)
        {
            var option = new RichTextStyle();
            if (TryGetColor(text, out Color color))
            {
                option.colorFlag = true;
                option.color = color;
            }
            if (TryGetSize(text, out int size))
            {
                option.sizeFlag = true;
                option.size = size;
            }
            option.bold = HasBold(text);
            option.italic = HasItalic(text);
            return option;
        }
    }
}
