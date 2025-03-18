using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine
{
    public class Behaviour_LinkType<T> : Component_LinkType<T>
        where T : Behaviour_LinkType<T>
    {
        public Behaviour_LinkType(Behaviour obj) : base(obj) { }
        public Behaviour_LinkType(object obj) : base(obj) { }
        protected Behaviour_LinkType() { }
    }

    [LinkType(typeof(Behaviour))]
    public class Behaviour_LinkType : Behaviour_LinkType<Behaviour_LinkType>
    {
        public Behaviour_LinkType(Behaviour obj) : base(obj) { }
        public Behaviour_LinkType(object obj) : base(obj) { }
        protected Behaviour_LinkType() { }
    }
}
