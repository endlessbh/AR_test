using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public class WebViewV8CallbackCSharp : LinkType<WebViewV8CallbackCSharp>
    {
        public WebViewV8CallbackCSharp() { }
        public WebViewV8CallbackCSharp(object obj) : base(obj) { }

        #region Callback

        public static XMethodInfo Callback_MethodInfo { get; } = new XMethodInfo(Type, nameof(Callback));

        public void Callback(string result)
        {
            Callback_MethodInfo.Invoke(obj, null);
        }

        #endregion

        #region OnDestroy

        public static XMethodInfo OnDestroy_MethodInfo { get; } = new XMethodInfo(Type, nameof(OnDestroy));

        public void OnDestroy()
        {
            OnDestroy_MethodInfo.Invoke(obj, null);
        }

        #endregion

        #region DestroyCallBack

        public static XMethodInfo DestroyCallBack_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyCallBack));

        public void DestroyCallBack()
        {
            DestroyCallBack_MethodInfo.Invoke(obj, null);
        }

        #endregion
    }
}
