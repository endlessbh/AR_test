using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.States.Base;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.EditorSMS.Toolkit;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 路径检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathInspector<T> : MotionInspector<T> where T : Path<T>
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
                case nameof(stateComponent._movePath):
                case nameof(stateComponent._viewPath):
                    {
                        EditorGUILayout.BeginHorizontal();

                        var batchGameObjectSP = serializedObject.FindProperty(nameof(stateComponent.batchGameObject));
                        var includeSP = serializedObject.FindProperty(nameof(stateComponent.include));
                        var chilerenSP = serializedObject.FindProperty(nameof(stateComponent.chileren));

                        stateComponent.batchGameObject = (Transform)EditorGUILayout.ObjectField(TrLabelByTarget(nameof(stateComponent.batchGameObject)), stateComponent.batchGameObject, typeof(Transform), true);
                        includeSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(stateComponent.include)), includeSP.boolValue, EditorStyles.miniButtonMid, GUILayout.Width(35));
                        chilerenSP.boolValue = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(stateComponent.chileren)), chilerenSP.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(35));

                        if (stateComponent.batchGameObject)
                        {
                            if (includeSP.boolValue)
                            {
                                AddObjects(serializedProperty, stateComponent.batchGameObject);
                            }
                            if (chilerenSP.boolValue)
                            {
                                AddObjects(serializedProperty, CommonFun.GetChildGameObjects(stateComponent.batchGameObject).Cast(go => go.transform).ToArray());
                            }
                            stateComponent.batchGameObject = null;
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

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (UICommonFun.ButtonOfMiddlerCenter(CommonFun.TempContent("路径编辑器"), GUILayout.Width(230), GUILayout.Height(24)))
            {
                PathWindow.OpenAndFocus();
            }
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        [LanguageTuple("\nMoving path length :\t{0}\nFull movement path length :\t{1}\nStandard moving speed :\t{2}\nMoving speed :\t\t{3}", "\n移动路径长度:\t{0}\n完整移动路径长度:\t{1}\n标准移动速度:\t{2}\n移动速度:\t\t{3}")]
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            try
            {
                var pathLength = MathU.PathLength(workClip.GetMovePath());
                var realFullPathLength = MathU.PathLength(workClip.GetFullMovePath());
                var standardMoveSpeed = realFullPathLength / workClip.timeLength;
                var moveSpeed = standardMoveSpeed * workClip.parent.speed;
                return info.AppendFormat(Tr("\nMoving path length :\t{0}\nFull movement path length :\t{1}\nStandard moving speed :\t{2}\nMoving speed :\t\t{3}"), pathLength, realFullPathLength, standardMoveSpeed, moveSpeed);
            }
            catch
            {
                return info;
            }
        }

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        /// <param name="path"></param>
        public static void OnDrawGizmos(T path)
        {
            var option = PathWindowOption.weakInstance;
            if (option.overrideDrawGizmos && PathWindow.valid) return;

            if (!path.enable) return;
            var colorBK = Gizmos.color;
            var keyPointColor = option.pathKeyPointBoxColor;
            var pathColor = option.pathLineColor;
            var r = option.keyPointSizeValue;
            try
            {
                Gizmos.color = keyPointColor;
                foreach (var p in path.GetMovePath())
                {
                    Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                }

                XGizmos.DrawPath(path.movePathType, path.GetMovePath(), pathColor);

                switch (path.viewRule)
                {
                    case EViewRule.ViewPath:
                        {
                            Gizmos.color = keyPointColor;
                            foreach (var p in path.GetViewPath())
                            {
                                Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                            }

                            XGizmos.DrawPath(path.viewPathType, path.GetViewPath(), pathColor);
                            break;
                        }
                }

            }
            catch { }
            finally
            {
                Gizmos.color = colorBK;
            }
        }
    }
}
