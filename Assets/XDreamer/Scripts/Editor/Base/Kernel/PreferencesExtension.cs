using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using static XCSJ.EditorCommonUtils.Preferences;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 首选项扩展
    /// </summary>
    public class PreferencesExtension
    {
        /// <summary>
        /// 获取设置提供者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SettingsProvider GetSettingsProvider<T>(string name) where T : PreferenceAttribute
        {
            return GetSettingsProvider(typeof(T), name);
        }

        /// <summary>
        /// 获取设置提供者
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SettingsProvider GetSettingsProvider(Type type, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (!(Preferences.GetPreferencesInfo(type) is PreferencesInfo preferencesInfo)) return null;

            var settings = new SettingsProvider(nameof(Preferences) + "/" + name, SettingsScope.User, preferencesInfo.GetKeywords());
            settings.guiHandler = preferencesInfo.DrawOptions;
            settings.titleBarGuiHandler = preferencesInfo.DrawButtons;
            settings.activateHandler = (searchContext, visualElement) => preferencesInfo.OnActivate(searchContext);
            settings.deactivateHandler = preferencesInfo.OnDeactivate;
            return settings;
        }
    }
}
