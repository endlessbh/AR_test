using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.EditorWindows
{
    /// <summary>
    /// 代码预览
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    public class TextPreviewWindow : XEditorWindowWithScrollView<TextPreviewWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "文本预览";

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="text"></param>
        public static void Open(string text = "")
        {
            OpenAndFocus();
            instance.UpdateCodeText(text);
        }

        /// <summary>
        /// 以JSON方式打开
        /// </summary>
        /// <param name="o"></param>
        public static void OpenAsJson(object o) => Open(JsonHelper.ToJson(o, true));

        /// <summary>
        /// 文本
        /// </summary>
        public string text = "";

        private void UpdateCodeText(string text)
        {
            this.text = text;

            texts = text.Split('\n').ToList();

            var sb = new StringBuilder();
            var count = texts.Count;
            for (int i = 0; i < count; i++)
            {
                sb.AppendLine((i + 1).ToString());
            }
            numText = sb.ToString();
            numWidth = 42 + (Math.Max(count.ToString().Length, 3) - 3) * 14;
        }

        public string numText = "";

        public float numWidth = 42;

        private List<string> texts = new List<string>();

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(numText, GUILayout.ExpandHeight(true), GUILayout.Width(numWidth));
            EditorGUILayout.TextArea(text, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }
    }
}
