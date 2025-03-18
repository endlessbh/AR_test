using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.EditorExtension.Base.Tools;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// Toggle切换批量检查器
    /// </summary>
    [Name("Toggle切换批量检查器")]
    [CustomEditor(typeof(ToggleSwitchBatch))]
    public class ToggleSwitchBatchInspector : StateComponentInspector<ToggleSwitchBatch>
    {
        private SerializedProperty togglesSP = null;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                togglesSP = serializedObject.FindProperty(nameof(ToggleSwitchBatch.toggles));
            }
            catch { }
        }

        private const float btnHeight = 18;

        [Name("所有Toggle")]
        [Tip("添加场景中所有Toggle", "Add all toggles in the scene")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public static XGUIContent allButton { get; } = new XGUIContent(typeof(ToggleSwitchBatchInspector), nameof(allButton));

        /// <summary>
        /// Toggle控件列表操作
        /// </summary>
        [Name("Toggle控件列表操作")]
        public bool togglesOperation;

        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ToggleSwitchBatch.toggles):
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(togglesOperation)));

                        var isLock = EditorHandler.IsLockInspectorWindow();
                        if (GUILayout.Button(isLock ? CommonFun.NameTip(EIcon.Lock) : CommonFun.NameTip(EIcon.Unlock), EditorStyles.miniButtonLeft, /*GUILayout.Width(60), */GUILayout.Height(btnHeight)))
                        {
                            EditorHandler.LockInspectorWindow(!isLock);
                        }

                        EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                        if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonMid, /*GUILayout.Width(70),*/ GUILayout.Height(btnHeight)))
                        {
                            AddToggle(CommonFun.GetComponents<Toggle>(Selection.gameObjects));
                        }
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button(allButton, EditorStyles.miniButtonRight,/* GUILayout.Width(70), */GUILayout.Height(btnHeight)))
                        {
                            AddToggle(CommonFun.GetComponentsInChildren<Toggle>(true));
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 添加切换
        /// </summary>
        /// <param name="toggles"></param>
        protected void AddToggle(IEnumerable<Toggle> toggles)
        {
            if (togglesSP == null || toggles == null) return;

            foreach (var tg in toggles)
            {
                if (!tg) continue;
                if (stateComponent.toggles.Contains(tg)) continue;

                togglesSP.arraySize++;
                togglesSP.GetArrayElementAtIndex(togglesSP.arraySize - 1).objectReferenceValue = tg;
            }
        }
    }
}
