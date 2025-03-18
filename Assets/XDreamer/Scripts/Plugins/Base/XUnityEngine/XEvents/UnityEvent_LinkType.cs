using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine.XEvents
{
    public class UnityEvent_LinkType<T> : UnityEventBase_LinkType<T>
       where T : UnityEvent_LinkType<T>
    {
        public UnityEvent_LinkType(UnityEvent obj) : base(obj) { }

        public UnityEvent_LinkType(object obj) : base(obj) { }

        protected UnityEvent_LinkType() { }

        #region GetDelegate

        public static XMethodInfo GetDelegate_MethodInfo { get; } = GetXMethodInfo(nameof(GetDelegate), TypeHelper.DefaultStatic);

        public static BaseInvokableCall GetDelegate(UnityAction action)
        {
            return new BaseInvokableCall(GetDelegate_MethodInfo?.Invoke(null, new object[] { action }));
        }

        #endregion
    }

    [LinkType(typeof(UnityEvent))]
    public class UnityEvent_LinkType : UnityEvent_LinkType<UnityEvent_LinkType>
    {
        public UnityEvent_LinkType(UnityEvent obj) : base(obj) { }
        public UnityEvent_LinkType(object obj) : base(obj) { }
        protected UnityEvent_LinkType() { }
    }
}
