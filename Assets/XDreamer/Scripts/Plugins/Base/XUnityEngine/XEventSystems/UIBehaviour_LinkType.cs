using UnityEngine.EventSystems;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEventSystems
{
    public class UIBehaviour_LinkType<T> : MonoBehaviour_LinkType<T>
        where T : UIBehaviour_LinkType<T>
    {
        public UIBehaviour_LinkType(UIBehaviour obj) : base(obj) { }
        public UIBehaviour_LinkType(object obj) : base(obj) { }
        protected UIBehaviour_LinkType() { }
    }

    [LinkType(typeof(UIBehaviour))]
    public class UIBehaviour_LinkType : UIBehaviour_LinkType<UIBehaviour_LinkType>
    {
        public UIBehaviour_LinkType(UIBehaviour obj) : base(obj) { }
        public UIBehaviour_LinkType(object obj) : base(obj) { }
        protected UIBehaviour_LinkType() { }
    }
}
