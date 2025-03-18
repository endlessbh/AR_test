using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public interface IView: IScriptableObject_LinkType
    {

    }

    public class View<T> : ScriptableObject_LinkType<T>, IView
        where T : View<T>, new()
    {
        public View() { }
        public View(object obj) : base(obj) { }
        public View(ScriptableObject obj) : base(obj) { }
    }

    [LinkType("UnityEditor.View")]
    public class View:View<View>
    {
        public View() { }
        public View(object obj) : base(obj) { }
        public View(ScriptableObject obj) : base(obj) { }
    }
}
