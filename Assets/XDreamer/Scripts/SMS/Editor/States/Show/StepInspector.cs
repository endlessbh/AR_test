using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.EditorSMS.States.Show
{
    /// <summary>
    /// 步骤检查器
    [Name("步骤检查器")]
    /// </summary>
    [CustomEditor(typeof(Step), true)]
    public class StepInspector : StateComponentInspector
    {
        private Step step => target as Step;

        public static List<StepClip> FindStepClips(Step step)
        {
            var steps = new List<StepClip>();
            var stateCollection = step.parent.stateCollection;
            if (step.parent && stateCollection)
            {
                steps = stateCollection.GetComponentsInChildren<StepClip>().ToList().Where(clip=>clip.step==step).ToList();
            }
            return steps;
        }

        public bool expanded = false;

        private static bool synDescriptionStateName = true;

        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Step.description):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (synDescriptionStateName = UICommonFun.ButtonToggle(new GUIContent("同步状态名", "如果对象为空,自动查找场景中相同名称的游戏对象"), synDescriptionStateName, EditorStyles.miniButtonRight, GUILayout.Width(60)))
                        {
                            var stp = step;
                            if (stp.description != stp.parent.name)
                            {
                                serializedProperty.stringValue = step.parent.name;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            expanded = UICommonFun.Foldout(expanded, new GUIContent("步骤片段列表"), DrawAddClipButton);

            if (!expanded) return;

            try
            {
                CommonFun.BeginLayout();
                var clips = FindStepClips(step);
                for (int i = 0; i < clips.Count; i++)
                {
                    var c = clips[i];
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField((i+1)+"."+c.parent.name, c, c.GetType(), true);
                    if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        c.Delete(true);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            finally
            {
                CommonFun.EndLayout();
            }
        }

        private void DrawAddClipButton()
        {
            var image = EditorIconHelper.GetIconInLib(EIcon.Add);
            if (GUILayout.Button(new GUIContent("后续节点", image), EditorStyles.miniButtonMid, GUILayout.Width(80), UICommonOption.Height16))
            {
                foreach (var outTransition in step.parent.outTransitions)
                {
                    var clip = outTransition.outState.GetComponent<StepClip>();
                    if (!clip)
                    {
                        // 不许把有Step组件的状态加入到状态片段中
                        if (!outTransition.outState.GetComponent<Step>())
                        {
                            clip = outTransition.outState.AddComponent<StepClip>();
                        }
                    }
                    if (clip)
                    {
                        clip.step = step;
                    }
                }
            }
            if (GUILayout.Button(new GUIContent("选择", image), EditorStyles.miniButtonRight, GUILayout.Width(60), UICommonOption.Height16))
            {
                // 列举本状态机内所有 非步骤且（无步骤剪辑或有步骤剪辑，但指向不是当前步骤的剪辑）
                var addStates = step.parent.stateCollection.GetStatesOfAllowInAndOut().Where(s=> 
                ((!s.GetComponent<StepClip>() || s.GetComponent<StepClip>().step!=step)
                 && !s.GetComponent<Step>())).ToList();
                if(addStates.Count>0)
                {
                    MenuHelper.DrawMenu(typeof(Step).Name, m =>
                    {
                        foreach (var state in addStates)
                        {
                            m.AddMenuItem(state.name, (s) =>
                            {
                                var clip = ((State)s).GetComponent<StepClip>();
                                if (!clip)
                                {
                                    clip = ((State)s).AddComponent<StepClip>();
                                }
                                clip.step = step;
                            }, state);
                        }
                    });
                }
                else
                {
                    Debug.LogWarning("当前状态机中，无可作为步骤剪辑的状态！");
                }
            }
        }

        /// <summary>
        /// 成员属性列表
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <param name="value"></param>
        public static void SetMemberPropertyList(SerializedProperty memberProperty, UnityEngine.Object value)
        {
            for (int i = 0; i < memberProperty.arraySize; ++i)
            {
                var sp = memberProperty.GetArrayElementAtIndex(i);
                if (sp.objectReferenceValue == value)
                {
                    return;
                }
            }

            memberProperty.arraySize++;
            memberProperty.GetArrayElementAtIndex(memberProperty.arraySize - 1).objectReferenceValue = value;
            memberProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
