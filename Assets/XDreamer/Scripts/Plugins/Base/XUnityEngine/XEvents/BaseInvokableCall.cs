using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    public class BaseInvokableCall<T> : LinkType<T>
       where T : BaseInvokableCall<T>
    {
        public BaseInvokableCall(object obj) : base(obj) { }

        protected BaseInvokableCall() { }
    }

    [LinkType("UnityEngine.Events.BaseInvokableCall")]
    public class BaseInvokableCall : BaseInvokableCall<BaseInvokableCall>
    {
        public BaseInvokableCall(object obj) : base(obj) { }
        protected BaseInvokableCall() { }
    }
}
