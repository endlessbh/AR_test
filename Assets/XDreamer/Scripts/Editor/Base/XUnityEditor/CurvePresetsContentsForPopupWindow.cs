using System.Reflection;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetsContentsForPopupWindow))]
    public class CurvePresetsContentsForPopupWindow : LinkType<CurvePresetsContentsForPopupWindow>
    {
        public CurvePresetsContentsForPopupWindow(object obj) : base(obj) { }

        #region m_CurveLibraryEditor

        public static XFieldInfo m_CurveLibraryEditor_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurveLibraryEditor), BindingFlags.Instance | BindingFlags.NonPublic);

        public PresetLibraryEditor_CurvePresetLibrary m_CurveLibraryEditor => new PresetLibraryEditor_CurvePresetLibrary(m_CurveLibraryEditor_FieldInfo.GetValue(obj));

        #endregion

        #region m_CurveLibraryType

        public static XFieldInfo m_CurveLibraryType_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_CurveLibraryType), BindingFlags.Instance | BindingFlags.NonPublic);

        public CurveLibraryType m_CurveLibraryType
        {
            get => (CurveLibraryType)m_CurveLibraryType_FieldInfo.GetValue(obj);
            set => m_CurveLibraryType_FieldInfo.obj?.SetValue(obj, (int)value);
        }

        #endregion

        #region GetExtension

        public static XMethodInfo GetExtension_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetExtension), BindingFlags.Static| BindingFlags.NonPublic);

        public static string GetExtension(CurveLibraryType curveLibraryType) => GetExtension_MethodInfo.Invoke(null, new object[] { (int)curveLibraryType }) as string;

        #endregion
    }
}
