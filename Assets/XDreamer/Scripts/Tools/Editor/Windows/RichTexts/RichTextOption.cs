using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.RichTexts;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    [XDreamerPreferences]
    [Name("富文本")]
    [Tip("富文本样式编辑工具配置", "Rich text style editing tool configuration")]
    [Import]
    public class RichTextOption : XDreamerOption<RichTextOption>
    {
        [Name("显示样式")]
        public bool displayStyles = true;

        [Name("每行数目")]
        [Tip("富文本编辑工具显示时,内置富文本样式每行显示的数目", "When the rich text editing tool is displayed, the number of built-in rich text styles displayed per line")]
        public int countPerRow = 16;

        [Name("选择样式后隐藏")]
        public bool hideAfterSelectStyle = true;

        [Name("取消选择样式后隐藏")]
        public bool hideAfterDeselectStyle = false;

        public bool richTextStylesExpanded = true;

        [Name("富文本样式列表")]
        public List<RichTextStyle> richTextStyles = new List<RichTextStyle>();

        public RichTextStyle richTextStyle = new RichTextStyle();

        public void Add(RichTextStyle richTextStyle)
        {
            if (richTextStyle != null &&
                !string.IsNullOrEmpty(richTextStyle.name)
                && !richTextStyles.Exists(s => s.name == richTextStyle.name))
            {
                richTextStyles.Add(new RichTextStyle(richTextStyle));
            }
        }

        public override void OnModified()
        {
            base.OnModified();
            InitStylesIfNeed();
        }

        protected override void OnInit()
        {
            base.OnInit();
            InitStylesIfNeed();
        }

        protected void InitStylesIfNeed()
        {
            if (richTextStyles.Count > 0) return;
            richTextStyles.Add(new RichTextStyle(42, true) { enable = false, name = "N0", tip = "普通,初号" });
            richTextStyles.Add(new RichTextStyle(36, true) { enable = false, name = "N0-", tip = "普通,小初" });
            richTextStyles.Add(new RichTextStyle(26, true) { enable = true, name = "N1", tip = "普通,一号" });
            richTextStyles.Add(new RichTextStyle(24, true) { enable = true, name = "N1-", tip = "普通,小一" });
            richTextStyles.Add(new RichTextStyle(22, true) { enable = true, name = "N2", tip = "普通,二号" });
            richTextStyles.Add(new RichTextStyle(18, true) { enable = true, name = "N2-", tip = "普通,小二" });
            richTextStyles.Add(new RichTextStyle(16, true) { enable = true, name = "N3", tip = "普通,三号" });
            richTextStyles.Add(new RichTextStyle(15, true) { enable = true, name = "N3-", tip = "普通,小三" });
            richTextStyles.Add(new RichTextStyle(14, true) { enable = true, name = "N4", tip = "普通,四号" });
            richTextStyles.Add(new RichTextStyle(12, true) { enable = true, name = "N4-", tip = "普通,小四" });

            var color = new Color(0, 128 / 255f, 196 / 255f, 1);
            richTextStyles.Add(new RichTextStyle(color, 42, true) { enable = false, name = "H0", tip = "高亮,初号" });
            richTextStyles.Add(new RichTextStyle(color, 36, true) { enable = false, name = "H0-", tip = "高亮,小初" });
            richTextStyles.Add(new RichTextStyle(color, 26, true) { enable = true, name = "H1", tip = "高亮,一号" });
            richTextStyles.Add(new RichTextStyle(color, 24, true) { enable = true, name = "H1-", tip = "高亮,小一" });
            richTextStyles.Add(new RichTextStyle(color, 22, true) { enable = true, name = "H2", tip = "高亮,二号" });
            richTextStyles.Add(new RichTextStyle(color, 18, true) { enable = true, name = "H2-", tip = "高亮,小二" });
            richTextStyles.Add(new RichTextStyle(color, 16, true) { enable = true, name = "H3", tip = "高亮,三号" });
            richTextStyles.Add(new RichTextStyle(color, 15, true) { enable = true, name = "H3-", tip = "高亮,小三" });
            richTextStyles.Add(new RichTextStyle(color, 14, true) { enable = true, name = "H4", tip = "高亮,四号" });
            richTextStyles.Add(new RichTextStyle(color, 12, true) { enable = true, name = "H4-", tip = "高亮,小四" });

            color = Color.red;
            richTextStyles.Add(new RichTextStyle(color, 42, true) { enable = false, name = "E0", tip = "错误,初号" });
            richTextStyles.Add(new RichTextStyle(color, 36, true) { enable = false, name = "E0-", tip = "错误,小初" });
            richTextStyles.Add(new RichTextStyle(color, 26, true) { enable = true, name = "E1", tip = "错误,一号" });
            richTextStyles.Add(new RichTextStyle(color, 24, true) { enable = true, name = "E1-", tip = "错误,小一" });
            richTextStyles.Add(new RichTextStyle(color, 22, true) { enable = true, name = "E2", tip = "错误,二号" });
            richTextStyles.Add(new RichTextStyle(color, 18, true) { enable = true, name = "E2-", tip = "错误,小二" });
            richTextStyles.Add(new RichTextStyle(color, 16, true) { enable = true, name = "E3", tip = "错误,三号" });
            richTextStyles.Add(new RichTextStyle(color, 15, true) { enable = true, name = "E3-", tip = "错误,小三" });
            richTextStyles.Add(new RichTextStyle(color, 14, true) { enable = true, name = "E4", tip = "错误,四号" });
            richTextStyles.Add(new RichTextStyle(color, 12, true) { enable = true, name = "E4-", tip = "错误,小四" });

            color = Color.yellow;
            richTextStyles.Add(new RichTextStyle(color, 42, true) { enable = false, name = "W0", tip = "警告,初号" });
            richTextStyles.Add(new RichTextStyle(color, 36, true) { enable = false, name = "W0-", tip = "警告,小初" });
            richTextStyles.Add(new RichTextStyle(color, 26, true) { enable = true, name = "W1", tip = "警告,一号" });
            richTextStyles.Add(new RichTextStyle(color, 24, true) { enable = true, name = "W1-", tip = "警告,小一" });
            richTextStyles.Add(new RichTextStyle(color, 22, true) { enable = true, name = "W2", tip = "警告,二号" });
            richTextStyles.Add(new RichTextStyle(color, 18, true) { enable = true, name = "W2-", tip = "警告,小二" });
            richTextStyles.Add(new RichTextStyle(color, 16, true) { enable = true, name = "W3", tip = "警告,三号" });
            richTextStyles.Add(new RichTextStyle(color, 15, true) { enable = true, name = "W3-", tip = "警告,小三" });
            richTextStyles.Add(new RichTextStyle(color, 14, true) { enable = true, name = "W4", tip = "警告,四号" });
            richTextStyles.Add(new RichTextStyle(color, 12, true) { enable = true, name = "W4-", tip = "警告,小四" });
        }

        public static RichTextStyle Get(string name)
        {
            return weakInstance.richTextStyles.FirstOrDefault(style => style.name == name);
        }
    }

    [CommonEditor(typeof(RichTextOption))]
    public class RichTextOptionEditor : XDreamerOptionEditor<RichTextOption>
    {
        private const int IndexWidth = XDreamerInspector.IndexWidth;
        private const int EnableWidth = 30;
        private const int NameWidth = 60;
        private const int TipWidth = 80;
        private const int ColorEnableWidth = 50;
        private const int ColorWidth = 50;
        private const int BoldWidth = 50;
        private const int ItalicWidth = 50;
        private const int SizeEnableWidth = 50;
        private const int OperationElementWidth = 20;
        private const int OperationWidth = OperationElementWidth * 5;

        public override bool OnGUI(object obj, FieldInfo fieldInfo)
        {
            var preference = this.preference;
            switch (fieldInfo.Name)
            {
                case nameof(preference.countPerRow):
                    {
                        preference.countPerRow = EditorGUILayout.IntSlider(CommonFun.NameTooltip(preference, nameof(preference.countPerRow)), preference.countPerRow, 1, 32);
                        return true;
                    }
                case nameof(preference.richTextStylesExpanded):
                case nameof(preference.richTextStyle):
                    {
                        return true;
                    }
                case nameof(preference.richTextStyles):
                    {
                        if (!(preference.richTextStylesExpanded = UICommonFun.Foldout(preference.richTextStylesExpanded, CommonFun.NameTip(preference, nameof(preference.richTextStyles))))) return true;

                        CommonFun.BeginLayout();

                        #region List内容绘制

                        XEditorGUI.DrawList(preference.richTextStyles, () =>
                        {
                            GUILayout.Label("启用", GUILayout.Width(EnableWidth));
                            GUILayout.Label("名称", GUILayout.Width(NameWidth));
                            GUILayout.Label("提示", GUILayout.Width(TipWidth));
                            GUILayout.Label("颜色启用", GUILayout.Width(ColorEnableWidth));
                            GUILayout.Label("颜色", GUILayout.Width(ColorWidth));
                            GUILayout.Label("粗体", GUILayout.Width(BoldWidth));
                            GUILayout.Label("斜体", GUILayout.Width(ItalicWidth));
                            GUILayout.Label("尺寸启用", GUILayout.Width(SizeEnableWidth));
                            GUILayout.Label("尺寸");
                        }, (style, i) =>
                        {
                            style.enable = EditorGUILayout.Toggle(style.enable, GUILayout.Width(EnableWidth));

                            EditorGUI.BeginChangeCheck();
                            var name = EditorGUILayout.DelayedTextField(style.name, GUILayout.Width(NameWidth));
                            if (EditorGUI.EndChangeCheck()
                            && !string.IsNullOrEmpty(name)
                            && !preference.richTextStyles.Exists(s => s.name == name && s != style))
                            {
                                style.name = name;
                            }
                            style.tip = EditorGUILayout.TextField(style.tip, GUILayout.Width(TipWidth));

                            style.colorFlag = EditorGUILayout.Toggle(style.colorFlag, GUILayout.Width(ColorEnableWidth));
                            style.color = EditorGUILayout.ColorField(style.color, GUILayout.Width(ColorWidth));

                            style.bold = EditorGUILayout.Toggle(style.bold, GUILayout.Width(BoldWidth));
                            style.italic = EditorGUILayout.Toggle(style.italic, GUILayout.Width(ItalicWidth));

                            style.sizeFlag = EditorGUILayout.Toggle(style.sizeFlag, GUILayout.Width(SizeEnableWidth));
                            style.size = EditorGUILayout.IntSlider(style.size, 0, 100);
                        },
                        IndexWidth,
                        UICommonOption._24 * 5, UICommonOption.WH24x16);

                        #endregion

                        #region 信息添加

                        UICommonFun.BeginHorizontal(false);
                        GUILayout.Label("信息", GUILayout.Width(IndexWidth));
                        var richTextStyle = preference.richTextStyle;

                        richTextStyle.enable = EditorGUILayout.Toggle(richTextStyle.enable, GUILayout.Width(EnableWidth));

                        richTextStyle.name = EditorGUILayout.TextField(richTextStyle.name, GUILayout.Width(NameWidth));
                        richTextStyle.tip = EditorGUILayout.TextField(richTextStyle.tip, GUILayout.Width(TipWidth));

                        richTextStyle.colorFlag = EditorGUILayout.Toggle(richTextStyle.colorFlag, GUILayout.Width(ColorWidth));
                        richTextStyle.color = EditorGUILayout.ColorField(richTextStyle.color, GUILayout.Width(ColorWidth));

                        richTextStyle.bold = EditorGUILayout.Toggle(richTextStyle.bold, GUILayout.Width(BoldWidth));
                        richTextStyle.italic = EditorGUILayout.Toggle(richTextStyle.italic, GUILayout.Width(ItalicWidth));

                        richTextStyle.sizeFlag = EditorGUILayout.Toggle(richTextStyle.sizeFlag, GUILayout.Width(SizeEnableWidth));
                        richTextStyle.size = EditorGUILayout.IntSlider(richTextStyle.size, 0, 100);

                        if (GUILayout.Button("添加", EditorStyles.miniButtonRight, GUILayout.Width(OperationWidth)))
                        {
                            preference.Add(richTextStyle);
                        }
                        UICommonFun.EndHorizontal();

                        #endregion

                        CommonFun.EndLayout();

                        return true;
                    }
            }

            return base.OnGUI(obj, fieldInfo);
        }
    }
}
