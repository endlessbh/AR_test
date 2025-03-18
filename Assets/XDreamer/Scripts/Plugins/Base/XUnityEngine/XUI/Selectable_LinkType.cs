using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine.XEventSystems;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    public class Selectable_LinkType<T> : UIBehaviour_LinkType<T>
        where T : Selectable_LinkType<T>
    {
        public Selectable_LinkType(Selectable obj) : base(obj) { }
        public Selectable_LinkType(object obj) : base(obj) { }
        protected Selectable_LinkType() { }
    }

    [LinkType(typeof(Selectable))]
    public class Selectable_LinkType : Selectable_LinkType<Selectable_LinkType>
    {
        public Selectable_LinkType(Selectable obj) : base(obj) { }
        public Selectable_LinkType(object obj) : base(obj) { }
        protected Selectable_LinkType() { }
    }
}
