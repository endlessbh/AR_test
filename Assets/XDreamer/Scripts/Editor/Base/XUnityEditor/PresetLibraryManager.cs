using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryManager))]
    public class PresetLibraryManager : UnityEngine_Object<PresetLibraryManager>
    {
        [LinkType(EditorHelper.UnityEditorPrefix + nameof(PresetLibraryManager) + "+" + nameof(LibraryCache))]
        public class LibraryCache : LinkType<LibraryCache>
        {
            #region loadedLibraries

            public static XPropertyInfo loadedLibraries_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(loadedLibraries));

            public List<ScriptableObject> loadedLibraries
            {
                get => (List<ScriptableObject>)loadedLibraries_PropertyInfo.GetValue(obj);
            }

            #endregion

            public LibraryCache(object obj) : base(obj) { }
        }

        private PresetLibraryManager(object obj) : base(obj) { }

        #region instance

        public static XPropertyInfo instance_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(instance), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        
        public static PresetLibraryManager instance => new PresetLibraryManager(instance_PropertyInfo.GetValue(null));

        #endregion

        #region m_LibraryCaches

        public static XFieldInfo m_LibraryCaches_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_LibraryCaches), BindingFlags.NonPublic | BindingFlags.Instance);

        public List<LibraryCache> m_LibraryCaches
        {
            get
            {
                List<LibraryCache> libraryCaches = new List<LibraryCache>();
                if (m_LibraryCaches_FieldInfo.GetValue(obj) is IList libraryCacheList)
                {
                    foreach (var curvePreset in libraryCacheList)
                    {
                        libraryCaches.Add(new LibraryCache(curvePreset));
                    }
                }
                return libraryCaches;
            }
        }

        #endregion

        #region GetPresetLibraryCache

        public static XMethodInfo GetPresetLibraryCache_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetPresetLibraryCache));

        public LibraryCache GetPresetLibraryCache(string identifier)
        {
            return new LibraryCache(GetPresetLibraryCache_MethodInfo.Invoke(obj, new object[] { identifier }));
        }

        #endregion
    }
}
