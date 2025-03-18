using System;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibraryEditor<CurvePresetLibrary>类关联
    /// </summary>
    [LinkType(nameof(StaticMethod_GetLinkType), ELinkTypeMode.StaticMethod_GetLinkType)]
    public class PresetLibraryEditor_CurvePresetLibrary : PresetLibraryEditor<PresetLibraryEditor_CurvePresetLibrary, CurvePresetLibrary>
    {
        public PresetLibraryEditor_CurvePresetLibrary(object obj) : base(obj) { }

        public static Type StaticMethod_GetLinkType() => CurvePresetsContentsForPopupWindow.m_CurveLibraryEditor_FieldInfo.obj.FieldType;

        public override CurvePresetLibrary New(object obj) => new CurvePresetLibrary(obj);

        #region m_SaveLoadHelper

        public static XFieldInfo m_SaveLoadHelper_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_SaveLoadHelper), TypeHelper.InstanceNotPublicHierarchy);

        public ScriptableObjectSaveLoadHelper_CurvePresetLibrary m_SaveLoadHelper
        {
            get
            {
                return new ScriptableObjectSaveLoadHelper_CurvePresetLibrary(m_SaveLoadHelper_FieldInfo.GetValue(obj));
            }
        }

        #endregion
    }
}
