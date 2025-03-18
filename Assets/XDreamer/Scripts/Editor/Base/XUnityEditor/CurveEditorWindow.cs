using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurveEditorWindow))]
    public class CurveEditorWindow : EditorWindow_LinkType<CurveEditorWindow>
    {
        private CurveEditorWindow(object obj) : base(obj) { }

        #region instance

        public static XPropertyInfo instance_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(instance), TypeHelper.StaticPublicHierarchy);

        public static CurveEditorWindow instance => new CurveEditorWindow(instance_PropertyInfo.GetValue(null));

        #endregion

        #region currentPresetLibrary

        public static XPropertyInfo currentPresetLibrary_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(currentPresetLibrary), BindingFlags.Instance | BindingFlags.Public);

        public string currentPresetLibrary => currentPresetLibrary_PropertyInfo.GetValue(obj) as string;

        #endregion

        #region m_CurvePresets

        public static XFieldInfo m_CurvePresets_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurvePresets), BindingFlags.Instance | BindingFlags.NonPublic);

        public CurvePresetsContentsForPopupWindow m_CurvePresets => new CurvePresetsContentsForPopupWindow(m_CurvePresets_FieldInfo.GetValue(obj));

        public CurvePresetsContentsForPopupWindow curvePresetsValid
        {
            get
            {
                InitCurvePresets();
                return m_CurvePresets;
            }
        }

        #endregion

        #region InitCurvePresets

        public static XMethodInfo InitCurvePresets_MethodInfo { get; } = new XMethodInfo(Type, nameof(InitCurvePresets), BindingFlags.Instance | BindingFlags.NonPublic);

        public void InitCurvePresets() => InitCurvePresets_MethodInfo.Invoke(obj, null);

        #endregion

        private void SetCurveLibraryType(CurveLibraryType curveLibraryType)
        {
            curvePresetsValid.m_CurveLibraryEditor.m_SaveLoadHelper.fileExtensionWithoutDot = CurvePresetsContentsForPopupWindow.GetExtension(curveLibraryType);
            //Debug.Log(curvePresetsValid.m_CurveLibraryEditor.m_SaveLoadHelper.fileExtensionWithoutDot);
        }

        public string CreateNewLibrary(string curvePresetLibraryName, PresetFileLocation presetFileLocation, CurveLibraryType curveLibraryType)
        {
            instance.SetCurveLibraryType(curveLibraryType);
            return curvePresetsValid.m_CurveLibraryEditor.CreateNewLibraryCallback(curvePresetLibraryName, presetFileLocation);
        }

        public static AnimationCurve ShowCurveEditorWindow(AnimationCurve value, SerializedProperty property, Rect ranges, Color color)
        {
            EditorGUI_LinkType.SetCurveEditorWindowCurve(value, property, color);
            EditorGUI_LinkType.ShowCurvePopup(ranges);

            Event.current.Use();
            GUIUtility.ExitGUI();

            return property != null ? property.animationCurveValue : value;
        }
    }
}
