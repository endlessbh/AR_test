using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor.Web
{
    [LinkType("UnityEditor.Web.JSProxyMgr")]
    public class JSProxyMgr : LinkType<JSProxyMgr>
    {
        public JSProxyMgr(object obj) : base(obj) { }

        #region GetInstance

        public static XMethodInfo GetInstance_MethodInfo { get; } = GetXMethodInfo(nameof(GetInstance), TypeHelper.StaticPublic);

        public static JSProxyMgr GetInstance()
        {
            return new JSProxyMgr(GetInstance_MethodInfo.Invoke(null, null));
        }

        #endregion

        #region AddGlobalObject

        public static XMethodInfo AddGlobalObject_MethodInfo { get; } = GetXMethodInfo(nameof(AddGlobalObject), TypeHelper.InstancePublic);

        public void AddGlobalObject(string referenceName, object obj)
        {
            AddGlobalObject_MethodInfo.Invoke(this.obj, new object[] { referenceName, obj });
        }

        #endregion

        #region RemoveGlobalObject

        public static XMethodInfo RemoveGlobalObject_MethodInfo { get; } = GetXMethodInfo(nameof(RemoveGlobalObject), TypeHelper.InstancePublic);

        public void RemoveGlobalObject(string referenceName)
        {
            RemoveGlobalObject_MethodInfo.Invoke(this.obj, new object[] { referenceName });
        }

        #endregion
    }
}
