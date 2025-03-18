using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(EditorGUI))]
    public class EditorGUI_LinkType : LinkType<EditorGUI_LinkType>
    {
        #region SetCurveEditorWindowCurve

        public static XMethodInfo SetCurveEditorWindowCurve_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetCurveEditorWindowCurve), TypeHelper.StaticNotPublic);

        public static void SetCurveEditorWindowCurve(AnimationCurve value, SerializedProperty property, Color color)
        {
            SetCurveEditorWindowCurve_MethodInfo.Invoke(null, new object[] { value, property, color });
        }

        #endregion

        #region ShowCurvePopup

        public static XMethodInfo ShowCurvePopup_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowCurvePopup), TypeHelper.StaticNotPublic);

        public static void ShowCurvePopup(Rect ranges)
        {
            ShowCurvePopup_MethodInfo.Invoke(null, new object[] { ranges });
        }

        #endregion
    }
}
