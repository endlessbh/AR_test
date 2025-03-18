using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTimelines.Tools;
using XCSJ.Collections;
using System.Linq;

namespace XCSJ.EditorTimelines.Tools
{
    /// <summary>
    /// 路径动画检查器
    /// </summary>
    [CustomEditor(typeof(PathAnimation), true)]
    public class PathAnimationInspector : PlayableContentInspector<PathAnimation>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(PathAnimation._movePath):
                case nameof(PathAnimation._viewPath):
                case nameof(PathAnimation._moveTransforms):
                    {
                        EditorGUILayout.BeginHorizontal();

                        var batchGameObjectSP = serializedObject.FindProperty(nameof(PathAnimation._batchGameObject));
                        var includeSP = serializedObject.FindProperty(nameof(PathAnimation._include));
                        var chilerenSP = serializedObject.FindProperty(nameof(PathAnimation._chileren));

                        targetObject._batchGameObject = (Transform)EditorGUILayout.ObjectField(TrLabelByTarget(nameof(targetObject._batchGameObject)), targetObject._batchGameObject, typeof(Transform), true);
                        includeSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(PathAnimation._include)), includeSP.boolValue, EditorStyles.miniButtonMid, GUILayout.Width(35));
                        chilerenSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(PathAnimation._chileren)), chilerenSP.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(35));

                        if (targetObject._batchGameObject)
                        {
                            if (includeSP.boolValue)
                            {
                                AddObjects(serializedProperty, targetObject._batchGameObject);
                            }
                            if (chilerenSP.boolValue)
                            {
                                AddObjects(serializedProperty, CommonFun.GetChildGameObjects(targetObject._batchGameObject).Cast(go => go.transform).ToArray());
                            }
                            targetObject._batchGameObject = null;
                        }

                        EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                        if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonLeft, UICommonOption.Width80, UICommonOption.Height18))
                        {
                            AddObjects(serializedProperty, Selection.gameObjects.ToList(go => go.transform).ToArray());
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.EndHorizontal();

                        EditorSerializedObjectHelper.DrawArrayHandleRule(serializedProperty);

                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void AddObjects(SerializedProperty objectsSP, params Transform[] gameObjects)
        {
            if (objectsSP == null || gameObjects == null) return;

            for (int i = gameObjects.Length - 1; i >= 0; --i)
            {
                var gameObject = gameObjects[i];
                if (!gameObject) continue;

                objectsSP.arraySize++;
                objectsSP.GetArrayElementAtIndex(objectsSP.arraySize - 1).objectReferenceValue = gameObject;
            }
        }
    }
}
