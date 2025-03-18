using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 资源编辑面板
    /// </summary>
    public class AssetEditPanel : BasePanel
    {
        #region 变量

        /// <summary>
        /// 资源信息
        /// </summary>
        private Item asset;

        /// <summary>
        /// 缓存资源信息
        /// </summary>
        private Item tempItem;

        private AssetPanel assetPanel;

        #endregion 变量

        #region 构造函数和基础方法

        public AssetEditPanel()
        {

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void SetData(Item item)
        {
            this.asset = item;
            assetPanel = new AssetPanel(item);
            this.tempItem = new Item(item.name, item.jsonPath, item.parent as Category, item.asset);
            CopyItem(tempItem, item);
        }

        void CopyItem(Item itemSource, Item itemDest)
        {
            itemSource.name = itemDest.name;
            itemSource.tip = itemDest.tip;
            itemSource.description = itemDest.description;
            itemSource.assetType = itemDest.assetType;
            itemSource.index = itemDest.index;
            itemSource.visable = itemDest.visable;
            itemSource.imagePath = itemDest.imagePath;
            itemSource.tags.Clear();
            itemSource.tags.AddRange(itemDest.tags);
            itemSource.source = itemDest.source;
            itemSource.MarkDirty(itemDest.isDirty);
        }

        #endregion 构造函数和基础方法

        #region 面板渲染

        public override void Render()
        {
            DrawEditPanel();
        }

        public override void OnEditModel(Model model)
        {
            if (model is Item) SetData(model as Item);
        }

        void DrawEditPanel()
        {
            EditorGUILayout.BeginHorizontal();

            //绘制资源显示
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            if (assetPanel != null) assetPanel.Render(new Rect(AssetLibWindowStyle.SeparatorWidth + AssetLibWindow.instance.categoryVector.x + 6, AssetLibWindowStyle.TopHeight + AssetLibWindowStyle.NavBarHeight + 5, 300, 300), false);
            EditorGUILayout.EndVertical();

            DrawAssetEditForm();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAssetEditForm()
        {
            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();

            XEditorGUI.DrawObject(asset, (o, fi) =>
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
                    case nameof(Category.scrollPosition): return true;
                    case nameof(Item.tags):
                        {
                            XEditorGUI.DrawList(asset.tags, () =>
                            {
                                if (GUILayout.Button("标签", EditorStyles.label))
                                {
                                    asset.tags.Sort((x, y) => string.Compare(x, y));
                                }
                            }, (t, i) =>
                            {
                                asset.tags[i] = EditorGUILayout.TextField(asset.tags[i]);
                            }, 20, UICommonOption._24 * 5, UICommonOption.WH24x16);

                            if (GUILayout.Button("添加标签", GUILayout.Width(60)))
                            {
                                asset.tags.Add("");
                            }
                            return true;
                        }
                }
                return false;
            });

            if (EditorGUI.EndChangeCheck())
            {
                asset.MarkDirty(true);
                assetPanel.ResetInfo();
            }

            DrawSubmit(()=> { CommonFun.FocusControl(); AssetLibWindow.instance.CallSubmitEdit(asset); }, 
                ()=> { CommonFun.FocusControl(); if (asset.isDirty) { File<Item>.SaveFile(asset.jsonPath, asset); AssetDatabase.Refresh(); } }, 
                ()=> {
                    CommonFun.FocusControl();
                    if (asset.isDirty) CopyItem(this.asset, this.tempItem);
                    AssetLibWindow.instance.CallCancelEdit();
            });

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        public static void DrawSubmit(Action submit, Action save, Action cancel)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("修改"))
            {
                submit();
            }
            GUILayout.Space(80);
            //if (GUILayout.Button("保存"))
            //{
            //    save();
            //}
            //GUILayout.Space(20);
            if (GUILayout.Button("取消"))
            {
                cancel();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        #endregion 面板渲染
    }
}
