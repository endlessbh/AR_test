using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// <see cref="SettingsService"/>关联类
    /// </summary>
    [LinkType(typeof(SettingsService))]
    public class SettingsService_LinkType:LinkType<SettingsService_LinkType>
    {
        #region FetchSettingsProviders

        public static XMethodInfo FetchSettingsProviders_MethodInfo { get; } = GetXMethodInfo(nameof(FetchSettingsProviders));

        public static SettingsProvider[] FetchSettingsProviders() => FetchSettingsProviders_MethodInfo.Invoke(null, null) as SettingsProvider[];

        #endregion
    }
}
