using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络属性检查器
    /// </summary>
    [Name("网络属性检查器")]
    [CustomEditor(typeof(NetProperty), true)]
    public class NetPropertyInspector : NetPropertyInspector<NetProperty>
    {

    }

    public class NetPropertyInspector<T> : NetMBInspector<T> where T : NetProperty
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(NetProperty.propertys):
                    {
                        try
                        {
                            EditorGUI.BeginChangeCheck();
                            BeginSyncVar();

                            serializedProperty.isExpanded = UICommonFun.Foldout(serializedProperty.isExpanded, propertyData.trLabel, () =>
                            {
                                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    serializedProperty.arraySize++;
                                    CommonFun.FocusControl();
                                }
                                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, GUILayout.Width(20), UICommonOption.Height16) && serializedProperty.arraySize > 0)
                                {
                                    serializedProperty.arraySize--;
                                    CommonFun.FocusControl();
                                }
                            });
                            if (!serializedProperty.isExpanded) return;

                            CommonFun.BeginLayout();

                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            EditorGUILayout.LabelField("NO.", UICommonOption.Width20);
                            EditorGUILayout.LabelField("名称", UICommonOption.Width120);
                            EditorGUILayout.LabelField("值");
                            EditorGUILayout.LabelField("操作", GUILayout.Width(40));
                            EditorGUILayout.EndHorizontal();

                            for (int i = 0; i < serializedProperty.arraySize; i++)
                            {
                                var sp = serializedProperty.GetArrayElementAtIndex(i);

                                EditorGUILayout.BeginHorizontal();

                                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width20);

                                var nameSP = sp.FindPropertyRelative(nameof(Property.name));
                                nameSP.stringValue = EditorGUILayout.DelayedTextField(nameSP.stringValue, UICommonOption.Width120);

                                var valueSP = sp.FindPropertyRelative(nameof(Property.value));
                                valueSP.stringValue = EditorGUILayout.DelayedTextField(valueSP.stringValue);

                                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    serializedProperty.InsertArrayElementAtIndex(i);
                                }
                                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, GUILayout.Width(20), UICommonOption.Height16))
                                {
                                    CommonFun.FocusControl();
                                    serializedProperty.DeleteArrayElementAtIndex(i);
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                            CommonFun.EndLayout();
                        }
                        finally
                        {
                            EndSyncVar();
                            if (EditorGUI.EndChangeCheck())
                            {
                                mb.MarkDirty();
                            }
                        }

                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
