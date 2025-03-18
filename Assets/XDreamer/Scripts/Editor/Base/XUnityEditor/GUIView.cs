using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public interface IGUIView: IView
    {
        void Repaint();
    }

    public class GUIView<T> : View<T>, IGUIView
        where T : GUIView<T>, new()
    {
        public GUIView() { }
        public GUIView(object obj) : base(obj) { }
        public GUIView(ScriptableObject obj) : base(obj) { }

        #region hasFocus

        public static XPropertyInfo hasFocus_PropertyInfo { get; } = GetXPropertyInfo(nameof(hasFocus));

        public bool hasFocus
        {
            get
            {
                return (bool)hasFocus_PropertyInfo.GetValue(obj);
            }
        }

        #endregion

        #region Repaint

        public static XMethodInfo Repaint_MethodInfo { get; } = new XMethodInfo(Type, nameof(Repaint));

        public void Repaint()
        {
            Repaint_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion
    }

    [LinkType("UnityEditor.GUIView")]
    public class GUIView : GUIView<GUIView>
    {
        public GUIView() { }
        public GUIView(object obj) : base(obj) { }
        public GUIView(ScriptableObject obj) : base(obj) { }
    }
}
