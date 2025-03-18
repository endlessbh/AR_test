using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor.Web
{
#if UNITY_2020_1_OR_NEWER
    [Obsolete("在Unity2020.1版本中资源商店被使用网页与包管理器替代，导致UnityEditor.Web.WebScriptObject类被移除！", true)]
#endif
    [LinkType("UnityEditor.Web.WebScriptObject")]
    public class WebScriptObject : ScriptableObject_LinkType<WebScriptObject>
    {
        public WebScriptObject() { }
        public WebScriptObject(object obj) : base(obj) { }
        public WebScriptObject(ScriptableObject obj) : base(obj) { }

        #region webView

        public static XPropertyInfo webView_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(webView), TypeHelper.DefaultLookup);

        public WebView webView
        {
            get
            {
                return new WebView(webView_PropertyInfo.GetValue(obj));
            }
            set
            {
                webView_PropertyInfo.SetValue(obj, value ? value.unityEngineObject : null);
            }
        }

        #endregion

        #region ProcessMessage

        public static XMethodInfo ProcessMessage_MethodInfo { get; } = new XMethodInfo(Type, nameof(ProcessMessage), TypeHelper.DefaultLookup);

        public bool ProcessMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            return (bool)ProcessMessage_MethodInfo.Invoke(obj, new object[] { jsonRequest, callback?.obj });
        }

        #endregion

        #region processMessage

        public static XMethodInfo processMessage_MethodInfo { get; } = new XMethodInfo(Type, nameof(processMessage), TypeHelper.DefaultLookup);

        public bool processMessage(string jsonRequest, WebViewV8CallbackCSharp callback)
        {
            return (bool)processMessage_MethodInfo.Invoke(obj, new object[] { jsonRequest, callback?.obj });
        }

        #endregion
    }
}
