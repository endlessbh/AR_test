using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class CategoryEditPanel : BasePanel
    {
        #region 变量

        /// <summary>
        /// 资源信息
        /// </summary>
        private Category category;

        /// <summary>
        /// 存储原始数据
        /// </summary>
        private Category tempCategory;

        /// <summary>
        /// 仅更新修改脏数据
        /// </summary>
        private bool onlyDiry = true;

        /// <summary>
        /// 是否包含子对象
        /// </summary>
        private bool includeChildren = false;

        #endregion 变量

        #region 构造函数和基础方法

        public CategoryEditPanel()
        {

        }

        /// <summary>
        /// 设置目录数据
        /// </summary>
        /// <param name="category">待设置的目录数据</param>
        private void SetData(Category category)
        {
            this.category = category;
            this.tempCategory = new Category(category.name, category.jsonPath, category.parent as Category);
            CopyItem(tempCategory, category);
        }

        /// <summary>
        /// 拷贝基础信息
        /// </summary>
        /// <param name="itemSource"></param>
        /// <param name="itemDest"></param>
        void CopyItem(Category itemSource, Category itemDest)
        {
            itemSource.name = itemDest.name;
            itemSource.tip = itemDest.tip;
            itemSource.description = itemDest.description;
            itemSource.assetType = itemDest.assetType;
            itemSource.visable = itemDest.visable;
            itemSource.index = itemDest.index;
            itemSource.imagePath = itemDest.imagePath;
            itemSource.expand = itemDest.expand;
            itemSource.MarkDirty(itemDest.isDirty);
        }

        #endregion 构造函数和基础方法

        #region 面板绘制

        public override void Render()
        {
            DrawEditPanel();
        }

        public override void OnEditModel(Model model)
        {
            if(model is Category)SetData(model as Category);
        }

        void DrawEditPanel()
        {
            
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();

            XEditorGUI.DrawObject(category, (o, fi) =>
            {
                if (fi == null) return false;
                switch (fi.Name)
                {
                    case nameof(Model.parent): return true;
                    case nameof(Model.guid): return true;
                    case nameof(Model.jsonPath): return true;
                    case nameof(Model.visiableInPanel): return true;
                    case nameof(Category.categories): return true;
                    case nameof(Category.items): return true;
                    case nameof(Category.itemTotalCount): return true;
                    case nameof(Category.visibleItemTotalCount): return true;
                    case nameof(Category.scrollPosition): return true;
                    case nameof(Category.categoryTotalCount): return true;
                    //case nameof(Category.expandCategoryTotalCount): return true;
                    //case nameof(Category.expandVisibleCategoryTotalCount): return true;
                }
                return false;
            });

            if (EditorGUI.EndChangeCheck()) category.MarkDirty(true);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.SelectableLabel("仅脏数据", GUILayout.Width(160), GUILayout.Height(15));
            onlyDiry = EditorGUILayout.Toggle(onlyDiry);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.SelectableLabel("子对象", GUILayout.Width(160), GUILayout.Height(15));
            includeChildren = EditorGUILayout.Toggle(includeChildren);
            EditorGUILayout.EndHorizontal();



            AssetEditPanel.DrawSubmit(()=> {
                CommonFun.FocusControl(); AssetLibWindow.instance.CallCancelEdit();},
                ()=> { CommonFun.FocusControl(); category.Output(onlyDiry, includeChildren); AssetDatabase.Refresh(); },
                ()=> {
                    CommonFun.FocusControl();
                    if (category.isDirty) CopyItem(this.category, this.tempCategory);
                    AssetLibWindow.instance.CallCancelEdit();
                });

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        #endregion 面板绘制
    }
}
