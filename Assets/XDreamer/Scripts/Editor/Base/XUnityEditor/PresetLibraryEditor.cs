using System.Reflection;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibraryEditor<T>泛型类关联
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPresetLibrary"></typeparam>
    public abstract class PresetLibraryEditor<T, TPresetLibrary> : LinkType<T>
        where T : PresetLibraryEditor<T, TPresetLibrary>
        where TPresetLibrary : PresetLibrary<TPresetLibrary>, new()
    {
        public PresetLibraryEditor(object obj) : base(obj) { }

        #region DeletePreset

        public static XMethodInfo DeletePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(DeletePreset));

        public void DeletePreset(int presetIndex) => DeletePreset_MethodInfo.Invoke(obj, new object[] { presetIndex });

        #endregion

        #region ReplacePreset

        public static XMethodInfo ReplacePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(ReplacePreset));

        public void ReplacePreset(int presetIndex, object presetObject) => ReplacePreset_MethodInfo.Invoke(obj, new object[] { presetIndex, presetObject });

        #endregion

        #region MovePreset

        public static XMethodInfo MovePreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(MovePreset));

        public void MovePreset(int presetIndex, int destPresetIndex, bool insertAfterDestIndex) => MovePreset_MethodInfo.Invoke(obj, new object[] { presetIndex, destPresetIndex, insertAfterDestIndex });

        #endregion

        #region CreateNewPreset

        public static XMethodInfo CreateNewPreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(CreateNewPreset));

        public void CreateNewPreset(object presetObject, string presetName) => CreateNewPreset_MethodInfo.Invoke(obj, new object[] { presetObject, presetName });

        #endregion

        #region GetCurrentLib

        public static XMethodInfo GetCurrentLib_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetCurrentLib));

        public TPresetLibrary GetCurrentLib() => New(GetCurrentLib_MethodInfo.Invoke(obj, null));

        #endregion

        public abstract TPresetLibrary New(object obj);

        #region LibraryModeChange

        public static XMethodInfo LibraryModeChange_MethodInfo { get; } = new XMethodInfo(Type, nameof(LibraryModeChange), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        public void LibraryModeChange(object userData) => LibraryModeChange_MethodInfo.Invoke(obj, new object[] { userData });

        public void LibraryModeChange(string presetLibraryFileFullPath) => LibraryModeChange_MethodInfo.Invoke(obj, new object[] { presetLibraryFileFullPath });

        #endregion

        #region CreateNewLibraryCallback

        public static XMethodInfo CreateNewLibraryCallback_MethodInfo { get; } = new XMethodInfo(Type, nameof(CreateNewLibraryCallback), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        public string CreateNewLibraryCallback(string libraryName, PresetFileLocation fileLocation) => CreateNewLibraryCallback_MethodInfo.Invoke(obj, new object[] { libraryName, (int)fileLocation }) as string;

        #endregion
    }
}
