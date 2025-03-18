using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 资源库窗口顶部面板
    /// </summary>
    public class TopPanel : BasePanel
    {
        #region 变量

        /// <summary>
        /// 搜索文字
        /// </summary>
        public string searchText = "";

        /// <summary>
        /// 搜索类型
        /// </summary>
        private ESearchType searchType = ESearchType.Fuzzy;

        public GUIStyle editStyle;

        #endregion 变量

        #region BasePanel

        public override void Initialize()
        {
            base.Initialize();
            isEdit = AssetLibWindow.instance.inEdit;
            editStyle = AssetLibWindowStyle.instance.editStyle;
        }

        public override void OnEditModeChange(bool edit)
        {
            if (edit) editStyle = AssetLibWindowStyle.instance.editHighlightStyle;
            else editStyle = AssetLibWindowStyle.instance.editStyle;
        }

        /// <summary>
        /// 顶部面板渲染
        /// </summary>
        public override void Render()
        {
            DrawTopPanel();
        }

        #endregion BasePanel

        private bool isEdit = false;

        /// <summary>
        /// 绘制顶部面板
        /// </summary>
        private void DrawTopPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUI.BeginChangeCheck();
                this.searchText = UICommonFun.SearchTextField(this.searchText, "", false, UICommonOption.ToolbarSeachTextField, UICommonOption.ToolbarSeachCancelButton);
                searchType = (ESearchType)UICommonFun.EnumPopup(searchType, EditorStyles.toolbarDropDown, UICommonOption.Width80);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetLibWindow.instance.CallSearchItems(searchText, searchType);
                }

#if XDREAMER_EDITION_XDREAMERDEVELOPER
                EditorGUI.BeginChangeCheck();
                isEdit = GUILayout.Toggle(isEdit, UICommonOption.Script, EditorStyles.toolbarButton, UICommonOption.WH24x16);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetLibWindow.instance.inEdit = isEdit;
                }

                if (GUILayout.Button(AssetLibWindow.saveGUIContent, EditorStyles.toolbarButton, UICommonOption.WH24x16))
                {
                    AssetLibWindow.instance.CallSaveAll();
                    AssetDatabase.Refresh();
                }
#endif
                if (GUILayout.Button("免责声明", EditorStyles.toolbarButton, UICommonOption.WH48x16))
                {
                    AssetLibWindow.instance.ShowDeclaration();
                }
                if (GUILayout.Button(UICommonOption.Help, EditorStyles.toolbarButton, UICommonOption.WH24x16))
                {
                    UICommonFun.OpenManual(AssetLibWindow.instance);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}