using System;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public interface IEditorWindow_LinkType : IScriptableObject_LinkType
    {
        EditorWindow editorWindow { get; }
        
        HostView m_Parent { get; }
    }

    public class EditorWindow_LinkType<T> : ScriptableObject_LinkType<T>, IEditorWindow_LinkType
        where T : EditorWindow_LinkType<T>
    {
        public EditorWindow editorWindow => obj as EditorWindow;

        protected EditorWindow_LinkType() { }
        public EditorWindow_LinkType(EditorWindow obj) : base(obj) { }
        public EditorWindow_LinkType(object obj) : base(obj) { }

        #region m_Parent

        public static XFieldInfo m_Parent_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Parent), TypeHelper.DefaultInstance);

        public HostView m_Parent
        {
            get
            {
                return new HostView(m_Parent_FieldInfo?.GetValue(obj));
            }
        }

        #endregion

        #region docked

        public static XPropertyInfo docked_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(docked), TypeHelper.InstanceNotPublic);

        public bool docked
        {
            get
            {
                return (bool)docked_PropertyInfo.GetValue(obj);
            }
        }

        #endregion

        #region hasFocus

        public static XPropertyInfo hasFocus_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(hasFocus), TypeHelper.InstanceNotPublic);

        public bool hasFocus
        {
            get
            {
                return (bool)hasFocus_PropertyInfo.GetValue(obj);
            }
        }

        #endregion

        #region RepaintImmediately

        public static XMethodInfo RepaintImmediately_MethodInfo { get; } = GetXMethodInfo(nameof(RepaintImmediately), TypeHelper.InstanceNotPublicHierarchy);

        public void RepaintImmediately()
        {
            RepaintImmediately_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion
    }

    [LinkType(typeof(EditorWindow))]
    public class EditorWindow_LinkType : EditorWindow_LinkType<EditorWindow_LinkType>
    {
        protected EditorWindow_LinkType() { }
        public EditorWindow_LinkType(EditorWindow obj) : base(obj) { }
        public EditorWindow_LinkType(object obj) : base(obj) { }
    }
}
