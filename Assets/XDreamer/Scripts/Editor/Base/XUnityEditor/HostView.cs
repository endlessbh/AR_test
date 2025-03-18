using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public class HostView<T> : GUIView<T>
        where T : HostView<T>, new()
    {
        public HostView() { }
        public HostView(object obj) : base(obj) { }
        public HostView(ScriptableObject obj) : base(obj) { }
    }

    [LinkType("UnityEditor.HostView")]
    public class HostView : HostView<HostView>
    {
        public HostView() { }
        public HostView(object obj) : base(obj) { }
        public HostView(ScriptableObject obj) : base(obj) { }
    }
}
