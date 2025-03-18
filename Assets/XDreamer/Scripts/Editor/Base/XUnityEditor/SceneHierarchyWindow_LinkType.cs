using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;
using static UnityEditor.SearchableEditorWindow;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(SceneHierarchyWindow))]
    public class SceneHierarchyWindow_LinkType : SearchableEditorWindow_LinkType<SceneHierarchyWindow_LinkType>
    {
        public SceneHierarchyWindow_LinkType(object obj) : base(obj) { }

        #region SetSearchFilter

        public static XMethodInfo SetSearchFilter_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetSearchFilter), BindingFlags.Instance | BindingFlags.NonPublic);

        public void SetSearchFilter(string searchFilter, SearchMode searchMode, bool setAll= false, bool delayed = false) => SetSearchFilter_MethodInfo.Invoke(obj, new object[] { searchFilter, (int)searchMode, setAll, delayed });

        #endregion
    }


    public static class SceneHierarchyWindow_Extension
    {
        /// <summary>
        /// 设置搜索过滤器
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="searchMode"></param>
        /// <param name="setAll"></param>
        /// <param name="delayed"></param>
        public static void SetSearchFilter(string filter, SearchMode searchMode, bool setAll = false, bool delayed = false)
        {
            SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));

            foreach (SearchableEditorWindow window in windows)
            {
                if (window.GetType().Name == nameof(SceneHierarchyWindow))
                {
                    new SceneHierarchyWindow_LinkType(window).SetSearchFilter(filter, searchMode, setAll, delayed);
                }
            }
        }
    }
}
