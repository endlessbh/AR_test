using System;
using UnityEditor;
using UnityEngine;
using XCSJ;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 编辑器风格查看器 窗口类
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class EditorStyleViewerWindow : XEditorWindowWithScrollView<EditorStyleViewerWindow>
    {
        public const string Title = "编辑器风格查看器";

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 搜索文本
        /// </summary>
        private string search = string.Empty;

        /// <summary>
        /// 界面渲染函数
        /// </summary>
        protected override void OnGUI()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal("HelpBox");
            GUILayout.Label("单击示例将复制其名到剪贴板", "label");
            GUILayout.FlexibleSpace();
            GUILayout.Label("查找:");
            search = UICommonFun.SearchTextField(search);
            EditorGUILayout.EndHorizontal();

            base.OnGUI();
        }

        public override void OnGUIWithScrollView()
        {
            foreach (GUIStyle style in GUI.skin)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    EditorGUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                    GUILayout.Space(7);
                    if (GUILayout.Button(style.name, style))
                    {
                        EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(11);
                }
            }
        }
    }
}
