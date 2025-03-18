using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// SettingsWindow关联类
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + "SettingsWindow")]
    public class SettingsWindow_LinkType : SettingsWindow_LinkType<SettingsWindow_LinkType>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="editorWindow"></param>
        public SettingsWindow_LinkType(EditorWindow editorWindow) : base(editorWindow) { }
    }

    /// <summary>
    /// SettingsWindow关联类泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SettingsWindow_LinkType<T> : EditorWindow_LinkType<T>
        where T: SettingsWindow_LinkType<T>
    {
        protected SettingsWindow_LinkType() { }
        public SettingsWindow_LinkType(EditorWindow obj) : base(obj) { }
        public SettingsWindow_LinkType(object obj) : base(obj) { }

        #region m_Providers

        public static XFieldInfo m_Providers_FieldInfo { get; } = GetXFieldInfo(nameof(m_Providers));

        public SettingsProvider[] m_Providers => m_Providers_FieldInfo.GetValue(obj) as SettingsProvider[];

        #endregion
    }
}
