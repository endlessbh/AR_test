using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// <see cref="XDreamer"/>首选项扩展
    /// </summary>
    public class XDreamerPreferencesExtension
    {
        /// <summary>
        /// 首选项GUI:返回设置提供者
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            return PreferencesExtension.GetSettingsProvider<XDreamerPreferencesAttribute>(Product.Name);
        }
    }
}
