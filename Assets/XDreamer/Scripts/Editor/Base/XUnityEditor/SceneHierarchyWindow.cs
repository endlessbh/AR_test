using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(SceneHierarchyWindow))]
    public class SceneHierarchyWindow: SearchableEditorWindow_LinkType<SceneHierarchyWindow>
    {
        protected SceneHierarchyWindow() { }
        public SceneHierarchyWindow(SearchableEditorWindow obj) : base(obj) { }
        public SceneHierarchyWindow(object obj) : base(obj) { }

        #region s_LastInteractedHierarchy

        public static XFieldInfo s_LastInteractedHierarchy_FieldInfo { get; } = new XFieldInfo(Type, nameof(s_LastInteractedHierarchy));

        public static SceneHierarchyWindow s_LastInteractedHierarchy
        {
            get
            {
                return new SceneHierarchyWindow(s_LastInteractedHierarchy_FieldInfo?.GetValue(null));
            }
        }

        #endregion

        #region SetExpandedRecursive

        public static XMethodInfo SetExpandedRecursive_MethodInfo { get; } = GetXMethodInfo(nameof(SetExpandedRecursive));

        public void SetExpandedRecursive(int id, bool expand)
        {
            SetExpandedRecursive_MethodInfo.Invoke(obj, new object[] { id, expand });
        }

        #endregion
    }
}
