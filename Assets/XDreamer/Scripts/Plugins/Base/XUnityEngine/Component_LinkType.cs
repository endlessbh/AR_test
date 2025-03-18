using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine
{
    public class Component_LinkType<T> : UnityEngine_Object<T>
        where T : Component_LinkType<T>
    {
        public Component_LinkType(Component obj) : base(obj) { }
        public Component_LinkType(object obj) : base(obj) { }
        protected Component_LinkType() { }
    }

    [LinkType(typeof(Component))]
    public class Component_LinkType : Component_LinkType<Component_LinkType>
    {
        public Component_LinkType(Component obj) : base(obj) { }
        public Component_LinkType(object obj) : base(obj) { }
        protected Component_LinkType() { }
    }
}
