using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.RichTexts;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    public static class EditorRichText
    {
        private static RichTextStyle DefaultStyle = new RichTextStyle();
        public static Vector2Int SizeClamp { get; } = new Vector2Int(0, 100);

        public static XGUIStyle miniButtonRichText { get; } = new XGUIStyle(nameof(EditorStyles.miniButton), s =>
        {
            s.richText = true;
        });
        public static XGUIStyle miniButtonLeftRichText { get; } = new XGUIStyle(nameof(EditorStyles.miniButtonLeft), s =>
        {
            s.richText = true;
        });
        public static XGUIStyle miniButtonMidRichText { get; } = new XGUIStyle(nameof(EditorStyles.miniButtonMid), s =>
        {
            s.richText = true;
        });
        public static XGUIStyle miniButtonRightRichText { get; } = new XGUIStyle(nameof(EditorStyles.miniButtonRight), s =>
        {
            s.richText = true;
        });

        public static GUIStyle GetMiniButtonRichTextStyle(int count, int index)
        {
            switch (UICommonFun.GetStyleIndex(count, index))
            {
                case 1: return miniButtonLeftRichText;
                case 2: return miniButtonMidRichText;
                case 3: return miniButtonRightRichText;
                default: return miniButtonRichText;
            }
        }

        public static string TextAreaHandle(string text, ref Color color, ref int size, params GUILayoutOption[] options)
        {
            return TextAreaHandle(null, text, ref color, ref size, options);
        }
        public static string TextAreaHandle(GUIContent label, string text, ref Color color, ref int size, params GUILayoutOption[] options)
        {
            try
            {
                DefaultStyle.Set(text);
                DefaultStyle.color = color;
                DefaultStyle.size = size;
                return TextAreaHandle(label, text, DefaultStyle, options);
            }
            finally
            {
                color = DefaultStyle.color;
                size = DefaultStyle.size;
            }
        }
        public static string TextAreaHandle(string text, RichTextStyle richTextStyle, params GUILayoutOption[] options)
        {
            return TextAreaHandle(null, text, richTextStyle, options);
        }
        public static string TextAreaHandle(GUIContent label, string text, RichTextStyle richTextStyle, params GUILayoutOption[] options)
        {
            return Handle(label, text, richTextStyle, t => EditorGUILayout.TextArea(t, options));
        }

        public static string Handle(string text, ref Color color, ref int size, Func<string, string> onAfterVertical)
        {
            return Handle(null, text, ref color, ref size, onAfterVertical);
        }
        public static string Handle(string text, ref Color color, ref int size)
        {
            return Handle(null, text, ref color, ref size);
        }
        public static string Handle(GUIContent label, string text, ref Color color, ref int size, Func<string, string> onAfterVertical = null)
        {
            try
            {
                DefaultStyle.Set(text);
                DefaultStyle.color = color;
                DefaultStyle.size = size;
                return Handle(label, text, DefaultStyle, onAfterVertical);
            }
            finally
            {
                color = DefaultStyle.color;
                size = DefaultStyle.size;
            }
        }
        public static string Handle(GUIContent label, string text, RichTextStyle richTextStyle, Func<string, string> onAfterVertical = null)
        {
            bool hasPrefixLabel = label != null;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (hasPrefixLabel)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                EditorGUILayout.BeginVertical();
            }

            text = InternalHandle(text, richTextStyle);

            if (onAfterVertical != null)
            {
                text = onAfterVertical(text);
            }

            if (hasPrefixLabel)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            return text;
        }

        private static string InternalHandle(string text, RichTextStyle richTextStyle)
        {
            text = InternalBaseHandle(text, richTextStyle);
            text = InternalExtensionHandle(text, richTextStyle);
            return text;
        }
        private static string InternalBaseHandle(string text, RichTextStyle richTextStyle)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                var flag = UICommonFun.ButtonToggle(CommonFun.TempContent("R", "点击为文本添加或清除样式;\nR:RichText(富文本)首字母简写"), richTextStyle.hasFlag, miniButtonRichText, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.Format(text, flag);
                }

                //颜色
                EditorGUI.BeginChangeCheck();
                richTextStyle.colorFlag = UICommonFun.ButtonToggle(CommonFun.TempContent(RichText.AddColor("A", richTextStyle.color), "点击更改文本颜色"), richTextStyle.colorFlag, miniButtonLeftRichText, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.Color(text);
                }
                EditorGUI.BeginChangeCheck();
                richTextStyle.color = EditorGUILayout.ColorField(richTextStyle.color, GUILayout.Width(60));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.ReplaceColor(text);
                }

                //加粗
                EditorGUI.BeginChangeCheck();
                richTextStyle.bold = UICommonFun.ButtonToggle(CommonFun.TempContent(RichText.AddBold("B"), "点击将文本加粗"), richTextStyle.bold, miniButtonLeftRichText, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.Bold(text);
                }

                //斜体
                EditorGUI.BeginChangeCheck();
                richTextStyle.italic = UICommonFun.ButtonToggle(CommonFun.TempContent(RichText.AddItalic("I"), "点击将文本设置为斜体"), richTextStyle.italic, miniButtonRightRichText, GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.Italic(text);
                }

                //字号(字体尺寸)
                EditorGUI.BeginChangeCheck();
                richTextStyle.sizeFlag = UICommonFun.ButtonToggle(CommonFun.TempContent("A " + richTextStyle.size.ToString(), "点击更改字号(字体尺寸)"), richTextStyle.sizeFlag, miniButtonLeftRichText, GUILayout.Width(40));
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.Size(text);
                }
                if (GUILayout.Button(CommonFun.TempContent("A+", "增大字号(字体尺寸)"), EditorStyles.miniButtonMid, GUILayout.Width(30)))
                {
                    text = richTextStyle.ReplaceSize(text, Mathf.Clamp(richTextStyle.size + 1, SizeClamp.x, SizeClamp.y));
                }
                if (GUILayout.Button(CommonFun.TempContent("A-", "减小字号(字体尺寸)"), EditorStyles.miniButtonMid, GUILayout.Width(30)))
                {
                    text = richTextStyle.ReplaceSize(text, Mathf.Clamp(richTextStyle.size - 1, SizeClamp.x, SizeClamp.y));
                }
                EditorGUI.BeginChangeCheck();
                richTextStyle.size = EditorGUILayout.IntSlider(richTextStyle.size, SizeClamp.x, SizeClamp.y);
                if (EditorGUI.EndChangeCheck())
                {
                    text = richTextStyle.ReplaceSize(text);
                }

                //配置
                if (GUILayout.Button(CommonFun.TempContent(EditorIconHelper.GetIconInLib(EIcon.Config), CommonFun.Tooltip(typeof(RichTextOption))), EditorStyles.miniButtonRight, GUILayout.Width(18), GUILayout.Height(14)))
                {
                    XDreamerPreferences.OpenWindow(true, typeof(RichTextOption));
                }
            }           
            EditorGUILayout.EndHorizontal();

            return text;
        }
        private static string InternalExtensionHandle(string text, RichTextStyle richTextStyle)
        {
            var rteOption = RichTextOption.weakInstance;
            if (rteOption == null || !rteOption.displayStyles) return text;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("样式:", GUILayout.Width(30));

            var styles = rteOption.richTextStyles.Where(o => o.enable).ToList();
            var count = styles.Count;
            var col = rteOption.countPerRow;
            var row = count / col + (count % col == 0 ? 0 : 1);

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < row; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < col; j++)
                {
                    var index = i * col + j;
                    if (index >= count) break;

                    var style = styles[index];
                    var guiStyle = GetMiniButtonRichTextStyle(Mathf.Min(col, count - i * col), j);

                    EditorGUI.BeginChangeCheck();
                    var click = UICommonFun.ButtonToggle(CommonFun.TempContent(style.Get(style.name), style.ToFriendlyString()), style.IsSameStyle(richTextStyle), guiStyle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if(click)
                        {
                            richTextStyle.Set(style);
                            if (rteOption.hideAfterSelectStyle) richTextStyle.active = false;
                        }
                        else
                        {
                            richTextStyle.Set(false);
                            if (rteOption.hideAfterDeselectStyle) richTextStyle.active = false;
                        }
                        text = richTextStyle.Get(text);

                        
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();            

            EditorGUILayout.EndHorizontal();

            return text;
        }

        public static void SelectableLabel(string text, GUIStyle style = null)
        {
            style = GetLabelStyle(style);
            var size = style.CalcSize(CommonFun.TempContent(text));
            EditorGUILayout.SelectableLabel(text, style, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }

        public static void Label(string text, GUIStyle style = null)
        {
            Label(CommonFun.TempContent(text), style);
        }
        public static void Label(GUIContent content, GUIStyle style = null)
        {
            style = GetLabelStyle(style);
            var size = style.CalcSize(content);
            EditorGUILayout.LabelField(content, style, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }
        public static GUIStyle GetLabelStyle(GUIStyle style) => style ?? UICommonOption.richText;

        public static bool Button(string text, GUIStyle style = null)
        {
            return Button(CommonFun.TempContent(text), style);
        }
        public static bool Button(GUIContent content, GUIStyle style = null)
        {
            style = GetButtonStyle(style);
            var size = style.CalcSize(content);
            return GUILayout.Button(content, style, GUILayout.Width(size.x), GUILayout.Height(size.y));
        }
        public static GUIStyle GetButtonStyle(GUIStyle style) => style ?? RichText.buttonRichText;
    }
}
