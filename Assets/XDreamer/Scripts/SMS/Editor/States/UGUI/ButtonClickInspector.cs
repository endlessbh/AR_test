using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.Attributes;
using XCSJ.EditorXGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    /// <summary>
    /// 按钮点击检查器
    /// </summary>
    [Name("按钮点击检查器")]
    [CustomEditor(typeof(ButtonClick))]
    public class ButtonClickInspector : StateComponentInspector<ButtonClick>
    {
        private SerializedProperty buttonsSP = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                buttonsSP = serializedObject.FindProperty(nameof(ButtonClick.buttons));
            }
            catch { }
        }

        private const float btnHeight = 18;

        [Name("选择对象")]
        [Tip("添加场景中选择的游戏对象\n(需锁定检查器窗口)", "Add game objects selected in the scene \n (need to lock the inspector window)")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent selectGameObjectGUIContent { get; } = new XGUIContent(typeof(ButtonClickInspector), nameof(selectGameObjectGUIContent));

        [Name("所有按钮")]
        [Tip("添加场景中所有按钮", "Add all buttons in the scene")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent allButton { get; } = new XGUIContent(typeof(ButtonClickInspector), nameof(allButton));

        /// <summary>
        /// 按钮列表操作
        /// </summary>
        [Name("按钮列表操作")]
        public bool buttonsOperation;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch(serializedProperty.name)
            {
                case nameof(stateComponent.button):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.button, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var btn = ToolsMenu.CreateUIWithStyle<Button>();
                                serializedProperty.objectReferenceValue = btn;
                                return btn.gameObject;
                            });
                        });
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(stateComponent.buttons):
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(buttonsOperation)));

                        var isLock = EditorHandler.IsLockInspectorWindow();
                        if (GUILayout.Button(isLock ? CommonFun.NameTip(EIcon.Lock) : CommonFun.NameTip(EIcon.Unlock), EditorStyles.miniButtonLeft, /*GUILayout.Width(60),*/ GUILayout.Height(btnHeight)))
                        {
                            EditorHandler.LockInspectorWindow(!isLock);
                        }

                        EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                        if (GUILayout.Button(selectGameObjectGUIContent, EditorStyles.miniButtonMid, /*GUILayout.Width(60), */GUILayout.Height(btnHeight)))
                        {
                            AddButton(CommonFun.GetComponents<Button>(Selection.gameObjects));
                        }
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button(allButton, EditorStyles.miniButtonRight,/* GUILayout.Width(60),*/ GUILayout.Height(btnHeight)))
                        {
                            AddButton(CommonFun.GetComponentsInChildren<Button>(true));
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="buttons"></param>
        protected void AddButton(IEnumerable<Button> buttons)
        {
            if (buttonsSP == null || buttons == null) return;

            foreach (var btn in buttons)
            {
                if (!btn) continue;
                if (stateComponent.buttons.Contains(btn)) continue;

                buttonsSP.arraySize++;
                buttonsSP.GetArrayElementAtIndex(buttonsSP.arraySize - 1).objectReferenceValue = btn;
            }
        }
    }
}
