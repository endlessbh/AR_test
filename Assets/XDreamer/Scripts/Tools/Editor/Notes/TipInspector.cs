using System;
using UnityEngine;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginTools.Notes.Tips;
using static XCSJ.PluginTools.Notes.Tips.Tip;
using XCSJ.PluginXGUI;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginTools.Draggers;

namespace XCSJ.EditorTools.Notes.Tips
{
    /// <summary>
    /// 提示检查器
    /// </summary>
    [Name("提示检查器")]
    [CustomEditor(typeof(Tip))]
    [CanEditMultipleObjects]
    public class TipInspector : MBInspector<Tip>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("RectTransform Size", "矩形变换尺寸")]
        [Languages.LanguageTuple("XGUI Common Assets are missing [Tip Popup]", "XGUI通用资产缺少[提示弹出框]")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Tip._tipPopupAsset):
                    {
                        // 通用资源中缺少提示弹出框
                        if (targetObject._tipPopupAsset._assetSource == EAssetSource.CommonAssets)
                        {
                            if (!targetObject.tipPopup)
                            {
                                base.OnDrawMember(serializedProperty, propertyData);
                                if (XGUIManager.instance)
                                {
                                    EditorGUILayout.HelpBox(Tr("XGUI Common Assets are missing [Tip Popup]"), MessageType.Error);
                                }
                                return;
                            }
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}