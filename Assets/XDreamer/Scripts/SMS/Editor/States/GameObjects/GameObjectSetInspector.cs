using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States.Base;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.Scripts;
using XCSJ.Caches;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.EditorSMS.States.Base;
using static XCSJ.PluginSMS.States.GameObjects.GameObjectSet;

namespace XCSJ.EditorSMS.States.GameObjects
{
    /// <summary>
    /// 游戏对象集合检查器
    /// </summary>
    [Name("游戏对象集合检查器")]
    [CustomEditor(typeof(GameObjectSet), true)]
    public class GameObjectSetInspector : ObjectSetInspector<GameObjectSet>
    {
        private SerializedProperty objectsSP;        

        private Color orgColor;

        private void AddObjects(params GameObject[] gameObjects)
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

        private void RemoveObjects(params GameObject[] gameObjects)
        {
            if (objectsSP == null || gameObjects == null) return;

            for (int i = gameObjects.Length - 1; i >= 0; --i)
            {
                var gameObject = gameObjects[i];
                if (!gameObject) continue;
                if (!stateComponent.Contains(gameObject)) continue;

                for (int j = 0; j < objectsSP.arraySize; ++j)
                {
                    if (objectsSP.GetArrayElementAtIndex(j).objectReferenceValue == gameObject)
                    {
                        UICommonFun.DeleteArrayElementAtIndex(objectsSP, j);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                objectsSP = serializedObject.FindProperty(ObjectsString);
            }
            catch { }
        }

        /// <summary>
        /// 对象集操作
        /// </summary>
        [Name("对象集操作")]
        public bool objectSetOperation;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case ObjectsString:
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(TrLabel(nameof(objectSetOperation)));
                        EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                        if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonLeft, UICommonOption.Width80, UICommonOption.Height18))
                        {
                            AddObjects(Selection.gameObjects);
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        if (!stateComponent.DataValidity())
                        {
                            orgColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;
                            base.OnDrawMember(serializedProperty, propertyData);
                            GUI.backgroundColor = orgColor;
                            return;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);

            switch (serializedProperty.name)
            {
                case nameof(GameObjectSet.includeSelf):
                    {
                        var serializedObject = this.serializedObject;

                        var findObjectInRuntimeSP = serializedObject.FindProperty(nameof(GameObjectSet.findObjectInRuntime));
                        var findObjectInRuntimeData = targetPropertyCache.GetPropertyData(findObjectInRuntimeSP);
                        EditorGUILayout.PropertyField(findObjectInRuntimeSP, findObjectInRuntimeData.trLabel);

                        var selectionTypeSP = serializedObject.FindProperty(nameof(GameObjectSet.selectionType));
                        var selectionTypeData = targetPropertyCache.GetPropertyData(selectionTypeSP);
                        EditorGUILayout.PropertyField(selectionTypeSP, selectionTypeData.trLabel);

                        switch (stateComponent.selectionType)
                        {
                            case ESelectionType.GameObject:
                            case ESelectionType.GameObjectChildren:
                            case ESelectionType.GameObjectAllChildren:
                            case ESelectionType.GameObjectAndAllParents:
                                {
                                    EditorGUILayout.BeginHorizontal();

                                    var includeSP = serializedObject.FindProperty(nameof(GameObjectSet.include));
                                    var includeData = targetPropertyCache.GetPropertyData(includeSP);

                                    var childrenSP = serializedObject.FindProperty(nameof(GameObjectSet.children));
                                    var childrenData = targetPropertyCache.GetPropertyData(childrenSP);

                                    stateComponent.batchGameObject = (GameObject)EditorGUILayout.ObjectField(CommonFun.NameTooltip(target, nameof(stateComponent.batchGameObject)), stateComponent.batchGameObject, typeof(GameObject), true);
                                    includeSP.boolValue = UICommonFun.ButtonToggle(includeData.trLabel, includeSP.boolValue, EditorStyles.miniButtonMid, GUILayout.Width(35));
                                    childrenSP.boolValue = UICommonFun.ButtonToggle(childrenData.trLabel, childrenSP.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(35));
                                    if (stateComponent.includeSelf && !stateComponent.batchGameObject)
                                    {
                                        // 在添加自身的情况下，不重复的添加自己
                                        var _selfGO = SMSHelper.FindSMControllerInLoadedScene(stateComponent.parent).gameObject;
                                        if (!stateComponent.Contains(_selfGO))
                                        {
                                            stateComponent.batchGameObject = _selfGO;
                                        }
                                    }
                                    if (stateComponent.batchGameObject)
                                    {
                                        if (includeSP.boolValue)
                                        {
                                            AddObjects(stateComponent.batchGameObject);
                                        }
                                        if (childrenSP.boolValue)
                                        {
                                            AddObjects(CommonFun.GetChildGameObjects(stateComponent.batchGameObject.transform).ToArray());
                                        }

                                        stateComponent.batchGameObject = null;
                                    }

                                    EditorGUILayout.EndHorizontal();

                                    break;
                                }
                            case ESelectionType.Tag:
                                {
                                    DrawTagLayerName(null, DrawTag, () => { return GameObject.FindGameObjectsWithTag(stateComponent.selectedTag); }
                                        );
                                    break;
                                }
                            case ESelectionType.Layer:
                                {
                                    DrawTagLayerName(null, DrawLayer, () => { return FindGameObjectsWithLayer(stateComponent.selectedLayer); }
                                        );
                                    break;
                                }
                            case ESelectionType.Name:
                                {
                                    DrawTagLayerName(null, DrawName, () => { return FindGameObjectsWithName(stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);

                                    break;
                                }
                            case ESelectionType.TagAndLayer:
                                {
                                    DrawTagLayerName(DrawTag, DrawLayer, () => { return FindGameObjectsWithTagAndLayer(stateComponent.selectedTag, stateComponent.selectedLayer); });
                                    break;
                                }
                            case ESelectionType.TagAndName:
                                {
                                    DrawTagLayerName(DrawTag, DrawName, () => { return FindGameObjectsWithTagAndName(stateComponent.selectedTag, stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);
                                    break;
                                }
                            case ESelectionType.LayerAndName:
                                {
                                    DrawTagLayerName(DrawLayer, DrawName, () => { return FindGameObjectsWithLayerAndName(stateComponent.selectedLayer, stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);
                                    break;
                                }
                            case ESelectionType.Variable:
                                {
                                    DrawVariable();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        private void DrawTagLayerName(Action beforeVerticalDrawFun, Action beforeAddButtonDrawFun, Func<GameObject[]> findObjects, Action afterVerticalDrawFun = null)
        {
            beforeVerticalDrawFun?.Invoke();

            EditorGUILayout.BeginHorizontal();
            {
                beforeAddButtonDrawFun?.Invoke();

                if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    AddObjects(findObjects.Invoke());
                    GUI.FocusControl("");
                }
                else if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Delete)), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    RemoveObjects(findObjects.Invoke());
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();

            afterVerticalDrawFun?.Invoke();
        }

        private void DrawTag() => stateComponent.selectedTag = EditorGUILayout.TagField(CommonFun.NameTooltip(ESelectionType.Tag), stateComponent.selectedTag);
       
        private void DrawLayer() => stateComponent.selectedLayer = EditorGUILayout.LayerField(CommonFun.NameTooltip(ESelectionType.Layer), stateComponent.selectedLayer);

        private void DrawName()
        {
            var guiContent = CommonFun.NameTooltip(target, nameof(stateComponent.findName));

            if (stateComponent.nameIsVariable)
            {
                stateComponent.findName = DrawVariableValue(stateComponent.findName, guiContent);
            }
            else
            {
                stateComponent.findName = EditorGUILayout.TextField(guiContent, stateComponent.findName);
            }

            stateComponent.compareNameRule = (ECompareNameRule)UICommonFun.EnumPopup(stateComponent.compareNameRule, UICommonOption.Width60);
        }       

        private void DrawNameIsVariable()
        {
            stateComponent.nameIsVariable = EditorGUILayout.Toggle(CommonFun.NameTip(target, nameof(GameObjectSet.nameIsVariable)), stateComponent.nameIsVariable);
        }

        private void DrawVariable() => stateComponent.variable = DrawVariableValue(stateComponent.variable, CommonFun.NameTooltip(target, nameof(stateComponent.variable)));
        
        private static string DrawVariableValue(string varName, GUIContent guiContent = null)
        {
            try
            {
                EditorGUILayout.BeginHorizontal();

                varName = EditorGUILayout.TextField(guiContent, varName);

                var varNames = ScriptManager.instance.varCollection.GetVarNames();
                int varIndex = varNames.IndexOf(varName);
                int newIndex = EditorGUILayout.Popup(varIndex, varNames, UICommonOption.Width100);
                if (newIndex != varIndex && newIndex >= 0 && newIndex < varNames.Length)
                {
                    varName = varNames[newIndex];
                }

                return VariableHelper.Format(varName);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
