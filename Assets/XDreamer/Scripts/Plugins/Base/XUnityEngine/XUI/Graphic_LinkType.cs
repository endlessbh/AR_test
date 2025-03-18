using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine.XEventSystems;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    public class Graphic_LinkType<T>: UIBehaviour_LinkType<T>
         where T : Graphic_LinkType<T>
    {
        public Graphic_LinkType(Graphic obj) : base(obj) { }
        public Graphic_LinkType(object obj) : base(obj) { }
        protected Graphic_LinkType() { }
    }

    [LinkType(typeof(Graphic))]
    public class Graphic_LinkType : Graphic_LinkType<Graphic_LinkType>
    {
        public Graphic_LinkType(Graphic obj) : base(obj) { }
        public Graphic_LinkType(object obj) : base(obj) { }
        protected Graphic_LinkType() { }
    }
}
