using System;
using UnityEngine;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ColorPickers;

namespace XCSJ.EditorXGUI.Widgets
{
    /// <summary>
    /// 日期时间文本检查器
    /// </summary>
    [Name("日期时间文本检查器")]
    [CustomEditor(typeof(XGUICommonAssets))]
    public class XGUICommonAssetsInspector : MBInspector<XGUICommonAssets>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(XGUICommonAssets._dialogBox):
                    {
                        DrawAddButton<DialogBox>(serializedProperty, propertyData, "对话框");
                        return;
                    }
                case nameof(XGUICommonAssets._logViewController):
                    {
                        DrawAddButton<LogViewController>(serializedProperty, propertyData, "日志窗口");
                        return;
                    }
                case nameof(XGUICommonAssets._tipPopup):
                    {
                        DrawAddButton<TipPopup>(serializedProperty, propertyData, XCSJ.EditorXGUI.ToolsMenu.TipPopupName);
                        return;
                    }
                case nameof(XGUICommonAssets._colorPicker):
                    {
                        DrawAddButton<ColorPicker>(serializedProperty, propertyData, "调色板");
                        return;
                    }
                case nameof(XGUICommonAssets._menuGenerator):
                    {
                        DrawAddButton<ColorPicker>(serializedProperty, propertyData, "弹出菜单");
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 添加UI小装置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        /// <param name="UIObjectName"></param>
        private void DrawAddButton<T>(SerializedProperty serializedProperty, PropertyData propertyData, string UIObjectName) where T : MB
        {
            EditorGUILayout.BeginHorizontal();
            {
                base.OnDrawMember(serializedProperty, propertyData);

                EditorGUI.BeginDisabledGroup(serializedProperty.objectReferenceValue);
                {
                    if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        var rt = XCSJ.EditorXGUI.ToolsMenu.CreateCanvasAndLoadPrefabByXGUIPath(UIObjectName);
                        if (rt)
                        {
                            serializedProperty.objectReferenceValue = rt.GetComponentInChildren<T>();
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
