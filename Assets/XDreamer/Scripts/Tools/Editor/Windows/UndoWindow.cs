// UnityEditor.UndoWindow
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;

namespace XCSJ.EditorTools.Windows
{
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Undo)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class UndoWindow : XEditorWindowWithScrollView<UndoWindow>
    {
        public const string Title = "撤销重做";

        private List<string> undos = new List<string>();

        private List<string> redos = new List<string>();

        private List<string> newUndos = new List<string>();

        private List<string> newRedos = new List<string>();

        private Vector2 undosScroll = Vector2.zero;

        private Vector2 redosScroll = Vector2.zero;

        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        private void Update()
        {
            Undo_LinkType.GetRecords(this.newUndos, this.newRedos);
            if (!this.undos.SequenceEqual(this.newUndos) || !this.redos.SequenceEqual(this.newRedos))
            {
                this.undos = new List<string>(this.newUndos);
                this.redos = new List<string>(this.newRedos);
                base.Repaint();
            }
        }

        protected override void OnGUI()
        {
            // 清空缓存
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("供开发者使用", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("清空缓存", GUILayout.Width(60)))
                {
                    Undo.ClearAll();
                }
            }
            GUILayout.EndHorizontal();

            float minHeight = base.position.height - 60f;
            float minWidth = base.position.width * 0.5f - 5f;
            bool performUndo = false;
            bool performRedo = false;
            GUILayout.BeginHorizontal();
            {
                // 撤销列表
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(string.Format("撤销列表[{0}]", undos.Count));

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("撤销", GUILayout.Width(40)))
                        {
                            performUndo = true; 
                        }
                    }
                    GUILayout.EndHorizontal();

                    this.undosScroll = GUILayout.BeginScrollView(this.undosScroll, EditorStyles.helpBox, GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth));

                    int num = 0;
                    foreach (string undo in this.undos)
                    {
                        GUILayout.Label(string.Format("{0} {1}", num++, undo));
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                // 重做列表
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(string.Format("重做列表[{0}]", redos.Count));

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("重做", GUILayout.Width(40)))
                        {
                            performRedo = true;
                        }
                    }
                    GUILayout.EndHorizontal();

                    this.redosScroll = GUILayout.BeginScrollView(this.redosScroll, EditorStyles.helpBox, GUILayout.MinHeight(minHeight), GUILayout.MinWidth(minWidth));

                    int num = 0;
                    foreach (string redo in this.redos)
                    {
                        GUILayout.Label(string.Format("{0} {1}", num++, redo));
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            if (performUndo)
            {
                Undo.PerformUndo();
            }

            if (performRedo)
            {
                Undo.PerformRedo();
            }
        }

        public override void OnGUIWithScrollView() { }
    }
}
