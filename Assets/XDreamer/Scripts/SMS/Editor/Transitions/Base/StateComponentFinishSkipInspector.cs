using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.EditorSMS.Transitions.Base
{
    /// <summary>
    /// 状态组件完成跳过检查器
    /// </summary>
    [Name("状态组件完成跳过检查器")]
    [CustomEditor(typeof(StateComponentFinishSkip))]
    public class StateComponentFinishSkipInspector : TransitionComponentInspector
    {
        public SerializedProperty objectsSP;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            if (!target) return;
            base.OnEnable();
            objectsSP = serializedObject.FindProperty(nameof(StateComponentFinishSkip.componentsInState));
        }

        /// <summary>
        /// 入状态组件操作
        /// </summary>
        [Name("入状态组件操作")]
        public bool componentsInStateOperation;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(StateComponentFinishSkip.componentsInState):
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(componentsInStateOperation)));
                        if (GUILayout.Button(new GUIContent(" 添加", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonRight, GUILayout.Width(60), GUILayout.Height(18)))
                        {
                            AddStateComponent(objectsSP);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void AddStateComponent(SerializedProperty memberProperty)
        {
            try
            {
                State inState = (target as TransitionComponent).parent.inState;

                MenuHelper.DrawMenu(memberProperty.name, m =>
                {
                    for (int i = 0; i < inState.components.Length; ++i)
                    {
                        var component = inState.components[i];
                        m.AddMenuItem((i + 1).ToString() + "." + component.GetType(), (c) =>
                        {
                            var obj = c as UnityEngine.Object;

                            // 已存在
                            for (int j = 0; j < memberProperty.arraySize; ++j)
                            {
                                var sp = memberProperty.GetArrayElementAtIndex(j);
                                if (sp != null && sp.objectReferenceValue == obj)
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
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

