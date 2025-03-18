using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
#if UNITY_2018_3_OR_NEWER
    [Obsolete("因UnityEditor.PreferencesWindow被移除,本类不可再被使用", true)]
#endif
    [LinkType(TypeFullName)]
    public class PreferencesWindow_LinkType : SettingsWindow_LinkType<PreferencesWindow_LinkType>
    {
        private const string TypeFullName = EditorHelper.UnityEditorPrefix + "PreferencesWindow";

        [LinkType(TypeFullName + "+" + nameof(Section))]
        public class Section : LinkType<Section>
        {
            public Section(object obj) : base(obj) { }

            #region content

            public static XFieldInfo content_FieldInfo { get; } = new XFieldInfo(Type, nameof(content), BindingFlags.Instance | BindingFlags.Public);

            public GUIContent content
            {
                get => content_FieldInfo.GetValue(obj) as GUIContent;
            }

            #endregion
        }

        public PreferencesWindow_LinkType(EditorWindow editorWindow) : base(editorWindow) { }

        #region m_Sections

        public static XFieldInfo m_Sections_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Sections), BindingFlags.Instance | BindingFlags.NonPublic);

        public List<Section> m_Sections
        {
            get
            {
                var list = new List<Section>();
                IList sections = m_Sections_FieldInfo.GetValue(obj) as IList;
                for (int i = 0; i < sections.Count; i++)
                {
                    list.Add(new Section(sections[i]));
                }
                return list;
            }
        }

        #endregion

        #region m_SelectedSectionIndex

        public static XFieldInfo m_SelectedSectionIndex_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_SelectedSectionIndex), BindingFlags.Instance | BindingFlags.NonPublic);

        public int m_SelectedSectionIndex
        {
            get => (int)m_SelectedSectionIndex_FieldInfo.GetValue(obj);
            set => m_SelectedSectionIndex_FieldInfo.SetValue(obj, value);
        }

        #endregion

        #region selectedSectionIndex

        public static XPropertyInfo selectedSectionIndex_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(selectedSectionIndex), BindingFlags.Instance | BindingFlags.NonPublic);

        public int selectedSectionIndex
        {
            get => (int)selectedSectionIndex_PropertyInfo.GetValue(obj);
            set => selectedSectionIndex_PropertyInfo.SetValue(obj, value);
        }

        #endregion

        #region ShowPreferencesWindow

        public static XMethodInfo ShowPreferencesWindow_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowPreferencesWindow), BindingFlags.NonPublic | BindingFlags.Static);

        public static void ShowPreferencesWindow()
        {
            ShowPreferencesWindow_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
}
