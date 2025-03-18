using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.RichTexts;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class RichTextEditorWindow : XEditorWindowWithScrollView<RichTextEditorWindow>
    {
        public const string Title = "富文本编辑器";

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        public string text = Product.Name;

        public Color color = Color.red;
        public int size = 58;

        protected override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(UICommonOption.Copy, UICommonOption.WH24x16))
            {
                CommonFun.CopyTextToClipboardForPC(text);
            }
            EditorGUILayout.EndHorizontal();
            text = EditorRichText.TextAreaHandle(text, ref color, ref size, GUILayout.ExpandWidth(true));

            base.OnGUI();
        }

        public override void OnGUIWithScrollView()
        {
            EditorRichText.SelectableLabel(text);
        }      
    }
}
