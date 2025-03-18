using System.Collections.Generic;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryLocations))]
    public class PresetLibraryLocations : LinkType<PresetLibraryLocations>
    {
        #region GetAvailableFilesWithExtensionOnTheHDD

        public static XMethodInfo GetAvailableFilesWithExtensionOnTheHDD_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetAvailableFilesWithExtensionOnTheHDD));

        public static List<string> GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation fileLocation, string fileExtensionWithoutDot)
        {
            return GetAvailableFilesWithExtensionOnTheHDD_MethodInfo.Invoke(null, new object[] { (int)fileLocation, fileExtensionWithoutDot }) as List<string>;
        }

        #endregion
    }
}
