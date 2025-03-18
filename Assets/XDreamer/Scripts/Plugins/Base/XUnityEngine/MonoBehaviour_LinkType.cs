using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.XUnityEngine
{
    public class MonoBehaviour_LinkType<T> : Behaviour_LinkType<T>
        where T : MonoBehaviour_LinkType<T>
    {
        public MonoBehaviour_LinkType(MonoBehaviour obj) : base(obj) { }
        public MonoBehaviour_LinkType(object obj) : base(obj) { }
        protected MonoBehaviour_LinkType() { }
    }

    [LinkType(typeof(MonoBehaviour))]
    public class MonoBehaviour_LinkType : MonoBehaviour_LinkType<MonoBehaviour_LinkType>
    {
        public MonoBehaviour_LinkType(MonoBehaviour obj) : base(obj) { }
        public MonoBehaviour_LinkType(object obj) : base(obj) { }
        protected MonoBehaviour_LinkType() { }
    }
}
