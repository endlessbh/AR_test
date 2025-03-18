using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.Utils;
using XCSJ.EditorExtension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Components;
using XCSJ.EditorSMS.States.Base;

namespace XCSJ.EditorSMS.States.Components
{
    /// <summary>
    /// 组件集合检查器
    /// </summary>
    [Name("组件集合检查器")]
    [CustomEditor(typeof(ComponentSet))]
    public class ComponentSetInspector : ObjectSetInspector<ComponentSet>
    {
        private static GameObject curAddGameObject = null;
        public SerializedProperty objectsSP;

        protected override void OnEnable()
        {
            if (!target) return;

            base.OnEnable();
            objectsSP = serializedObject.FindProperty(ObjectsString);
        }

        private void AddComponent(SerializedProperty memberProperty) 
        {            
            Component[] components = curAddGameObject.GetComponents<Component>();

            MenuHelper.DrawMenu(curAddGameObject.name, m =>
            {
                for (int i=0; i< components.Length; ++i)
                {
                    var component = components[i];

                    m.AddMenuItem((i+1).ToString() + "." + component.GetType().Name, (c) =>
                    {
                        var obj = c as UnityEngine.Object;
                        for (int j = 0; j < memberProperty.arraySize; ++j)
                        {
                            var sp = memberProperty.GetArrayElementAtIndex(j);
                            if (sp.objectReferenceValue == obj)
                            {
                                return;
                            }
                        }
                        memberProperty.arraySize++;
                        memberProperty.GetArrayElementAtIndex(memberProperty.arraySize - 1).objectReferenceValue = obj;
                        memberProperty.serializedObject.ApplyModifiedProperties();
                    }, component);
                }
            });

        }

        [Name("无效对象")]
        [Tip("移除无效对象", "Remove invalid object")]
        [XCSJ.Attributes.Icon(EIcon.Delete)]
        public static XGUIContent deleteButton { get; } = new XGUIContent(typeof(ComponentSetInspector), nameof(deleteButton));

        [Name("去除重复")]
        [Tip("去除重复对象", "Remove duplicate objects")]
        [XCSJ.Attributes.Icon(EIcon.Delete)]
        public static XGUIContent distinctButton { get; } = new XGUIContent(typeof(ComponentSetInspector), nameof(distinctButton));

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ComponentSet.includeSelf):
                    {
                        return;
                    }
                case ObjectsString:
                    {
                        GUILayout.BeginHorizontal();
                        {
                            curAddGameObject = EditorGUILayout.ObjectField("游戏对象", curAddGameObject, typeof(GameObject), true) as GameObject;

                            EditorGUI.BeginDisabledGroup(!curAddGameObject);
                            if (GUILayout.Button(new GUIContent("添加", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(18)))
                            {
                                AddComponent(objectsSP);
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        GUILayout.EndHorizontal();

                        if (stateComponent.DataValidity()) break;

                        var orgColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        base.OnDrawMember(serializedProperty, propertyData);
                        GUI.backgroundColor = orgColor;
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
