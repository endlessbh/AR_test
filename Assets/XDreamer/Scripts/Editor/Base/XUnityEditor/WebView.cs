using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public interface IWebView : IScriptableObject_LinkType
    {

    }

    public interface IWebWiewCallback
    {
        /// <summary>
        /// <p>WebView在执行LoadURL,LoadFile时底层上自动调用本函数</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        void OnInitScripting();

        /// <summary>
        /// 当WebView对象被销毁、被其它WditorWindow遮挡等情况下，被执行的回调函数
        /// </summary>
        void OnBecameInvisible();

        /// <summary>
        /// <p>WebView加载无效或错误页面时回调</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">发生错误的Url</param>
        void OnLoadError(string url);

        /// <summary>
        /// <p>WebView的定位路径发生变化时回调；可捕获框架内子页面的URL</p>
        /// <p>特别注意:子类中必须重载本函数,WebView才会自动调用本函数**</p>
        /// </summary>
        /// <param name="url">定位后的Url</param>
        void OnLocationChanged(string url);
    }

#if UNITY_2020_1_OR_NEWER
    [Obsolete("在Unity2020.1版本中资源商店被使用网页与包管理器替代，导致UnityEditor.WebView类被移除！", true)]
#endif
    [LinkType("UnityEditor.WebView")]
    public class WebView : ScriptableObject_LinkType<WebView>, IWebView
    {
        public WebView() { }
        public WebView(object obj) : base(obj) { }
        public WebView(ScriptableObject obj) : base(obj) { }

        public static implicit operator bool(WebView webView)
        {
            return webView != null && !webView.IntPtrIsNull();
        }

        public void OnDestroy()
        {
            this.DestroyWebView();
        }

        #region DestroyWebView

        private static XMethodInfo DestroyWebView_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyWebView), TypeHelper.InstanceNotPublic);

        private void DestroyWebView()
        {
            //Debug.Log("DestroyWebView");
            DestroyWebView_MethodInfo.Invoke(obj, Empty<object>.Array);
            obj = null;
        }

        #endregion

        #region InitWebView

        public static XMethodInfo InitWebView_MethodInfo { get; } = new XMethodInfo(Type, nameof(InitWebView), TypeHelper.DefaultLookup);

        public void InitWebView(GUIView host, int x, int y, int width, int height, bool showResizeHandle)
        {
            InitWebView_MethodInfo.Invoke(obj, new object[] { host.scriptableObject, x, y, width, height, showResizeHandle });
        }

        public void InitWebView(IGUIView host, int x, int y, int width, int height, bool showResizeHandle)
        {
            InitWebView_MethodInfo.Invoke(obj, new object[] { host.scriptableObject, x, y, width, height, showResizeHandle });
        }

        #endregion

        public void InvokeJSMethod(string objectName, string name, params object[] args)
        {
            if (!scriptableObject) return;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(objectName);
            stringBuilder.Append('.');
            stringBuilder.Append(name);
            stringBuilder.Append('(');
            bool flag = true;
            foreach (object obj in args)
            {
                if (!flag)
                {
                    stringBuilder.Append(',');
                }
                bool flag2 = obj is string;
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                stringBuilder.Append(obj);
                if (flag2)
                {
                    stringBuilder.Append('"');
                }
                flag = false;
            }
            stringBuilder.Append(");");
            ExecuteJavascript(stringBuilder.ToString());
        }

        #region ExecuteJavascript

        public static XMethodInfo ExecuteJavascript_MethodInfo { get; } = new XMethodInfo(Type, nameof(ExecuteJavascript), TypeHelper.DefaultLookup);

        public void ExecuteJavascript(string scriptCode)
        {
            ExecuteJavascript_MethodInfo.Invoke(obj, new object[] { scriptCode });
        }

        #endregion

        private string _uri = "";
        public string uri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    Load(_uri);
                }
            }
        }

        public static bool IsFileProtocol(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return false;
            if (uri.StartsWith("file", true, CultureInfo.CurrentCulture)) return true;
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri value))
            {
                try
                {
                    return value.IsFile;
                }
                catch { }
            }
            return false;
        }

        public static bool IsAnyProtocol(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return false;
            return uri.Contains(":/") || uri.Contains(":\\");
        }

        public static bool IsHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return url.StartsWith("http", true, CultureInfo.CurrentCulture);
        }

        public static string TryToHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";
            if (IsAnyProtocol(url)) return url;
            return "http://" + url;
        }

        private static string[] UrlString = new string[] { "www.", ".cn", ".com" };
        public static bool HasAnyUrlString(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return UrlString.Any(s => url.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        public void Load(string uri)
        {
            if (string.IsNullOrEmpty(uri) || !scriptableObject) return;
            if (IsFileProtocol(uri))
            {
                LoadFile(uri);
            }
            else if (IsHttpUrl(uri) || IsAnyProtocol(uri))
            {
                LoadURL(uri);
            }
            else if (HasAnyUrlString(uri))
            {
                LoadURL(TryToHttpUrl(uri));
            }
            else
            {
                string path = Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")), uri);
                LoadFile(path);
            }
        }

        #region LoadURL

        public static XMethodInfo LoadURL_MethodInfo { get; } = new XMethodInfo(Type, nameof(LoadURL), TypeHelper.DefaultLookup);

        public void LoadURL(string url)
        {
            //Debug.Log("LoadURL:" + url);
            LoadURL_MethodInfo.Invoke(obj, new object[] { this.uri = url });
        }

        #endregion

        #region LoadFile

        public static XMethodInfo LoadFile_MethodInfo { get; } = new XMethodInfo(Type, nameof(LoadFile), TypeHelper.DefaultLookup);

        public void LoadFile(string path)
        {
            //Debug.Log("LoadFile:" + url);
            LoadFile_MethodInfo.Invoke(obj, new object[] { this.uri = path });
        }

        #endregion

        #region DefineScriptObject

        public static XMethodInfo DefineScriptObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(DefineScriptObject), TypeHelper.DefaultLookup);

        public bool DefineScriptObject(string path, ScriptableObject obj)
        {
            return (bool)DefineScriptObject_MethodInfo.Invoke(this.obj, new object[] { path, obj });
        }

        #endregion

        #region SetDelegateObject

        public static XMethodInfo SetDelegateObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetDelegateObject), TypeHelper.DefaultLookup);

        public void SetDelegateObject(ScriptableObject value)
        {
            SetDelegateObject_MethodInfo.Invoke(obj, new object[] { value });
        }

        public void SetDelegateObject<T>(T value) where T : ScriptableObject, IWebWiewCallback
        {
            SetDelegateObject_MethodInfo.Invoke(obj, new object[] { value });
        }

        #endregion

        #region SetHostView

        public static XMethodInfo SetHostView_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetHostView), TypeHelper.DefaultLookup);

        public void SetHostView(GUIView view)
        {
            SetHostView_MethodInfo.Invoke(obj, new object[] { view ? view.scriptableObject : null });
        }

        public void SetHostView(IGUIView view)
        {
            SetHostView_MethodInfo.Invoke(obj, new object[] { view?.obj });
        }

        #endregion

        #region SetSizeAndPosition

        public static XMethodInfo SetSizeAndPosition_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetSizeAndPosition), TypeHelper.DefaultLookup);

        public void SetSizeAndPosition(int x, int y, int width, int height)
        {
            SetSizeAndPosition_MethodInfo.Invoke(obj, new object[] { x, y, width, height });
        }

        #endregion

        #region SetFocus

        public static XMethodInfo SetFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetFocus), TypeHelper.DefaultLookup);

        public void SetFocus(bool value)
        {
            SetFocus_MethodInfo.Invoke(obj, new object[] { value });
        }

        #endregion

        #region HasApplicationFocus

        public static XMethodInfo HasApplicationFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(HasApplicationFocus), TypeHelper.DefaultLookup);

        public bool HasApplicationFocus()
        {
            return (bool)HasApplicationFocus_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region SetApplicationFocus

        public static XMethodInfo SetApplicationFocus_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetApplicationFocus), TypeHelper.DefaultLookup);

        /// <summary>
        /// 设置应用程序焦点
        /// </summary>
        /// <param name="applicationFocus"></param>
        public void SetApplicationFocus(bool applicationFocus)
        {
            SetApplicationFocus_MethodInfo.Invoke(obj, new object[] { applicationFocus });
        }

        #endregion

        #region Show

        public static XMethodInfo Show_MethodInfo { get; } = new XMethodInfo(Type, nameof(Show), TypeHelper.DefaultLookup);

        public void Show()
        {
            Show_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Hide

        public static XMethodInfo Hide_MethodInfo { get; } = new XMethodInfo(Type, nameof(Hide), TypeHelper.DefaultLookup);

        public void Hide()
        {
            Hide_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Back

        public static XMethodInfo Back_MethodInfo { get; } = new XMethodInfo(Type, nameof(Back), TypeHelper.DefaultLookup);

        public void Back()
        {
            Back_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region Forward

        public static XMethodInfo Forward_MethodInfo { get; } = new XMethodInfo(Type, nameof(Forward), TypeHelper.DefaultLookup);

        public void Forward()
        {
            Forward_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region SendOnEvent

        public static XMethodInfo SendOnEvent_MethodInfo { get; } = new XMethodInfo(Type, nameof(SendOnEvent), TypeHelper.DefaultLookup);

        public void SendOnEvent(string jsonStr)
        {
            SendOnEvent_MethodInfo.Invoke(obj, new object[] { jsonStr });
        }

        #endregion

        #region Reload

        public static XMethodInfo Reload_MethodInfo { get; } = new XMethodInfo(Type, nameof(Reload), TypeHelper.DefaultLookup);

        public void Reload()
        {
            Reload_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region AllowRightClickMenu

        public static XMethodInfo AllowRightClickMenu_MethodInfo { get; } = new XMethodInfo(Type, nameof(AllowRightClickMenu), TypeHelper.DefaultLookup);

        public void AllowRightClickMenu(bool allowRightClickMenu)
        {
            AllowRightClickMenu_MethodInfo.Invoke(obj, new object[] { allowRightClickMenu });
        }

        #endregion

        #region ShowDevTools

        public static XMethodInfo ShowDevTools_MethodInfo { get; } = new XMethodInfo(Type, nameof(ShowDevTools), TypeHelper.DefaultLookup);

        public void ShowDevTools()
        {
            ShowDevTools_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region ToggleMaximize

        public static XMethodInfo ToggleMaximize_MethodInfo { get; } = new XMethodInfo(Type, nameof(ToggleMaximize), TypeHelper.DefaultLookup);

        public void ToggleMaximize()
        {
            ToggleMaximize_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region OnDomainReload

        public static XMethodInfo OnDomainReload_MethodInfo { get; } = new XMethodInfo(Type, nameof(OnDomainReload), TypeHelper.DefaultLookup);

        public void OnDomainReload()
        {
            OnDomainReload_MethodInfo.Invoke(obj, Empty<object>.Array);
        }

        #endregion

        #region IntPtrIsNull

        public static XMethodInfo IntPtrIsNull_MethodInfo { get; } = new XMethodInfo(Type, nameof(IntPtrIsNull), TypeHelper.InstanceNotPublic);

        private bool IntPtrIsNull()
        {
            var x = IntPtrIsNull_MethodInfo.Invoke(obj, Empty<object>.Array);
            //return (bool)IntPtrIsNull_MethodInfo.Invoke(obj, Empty<object>.Array);
            return (bool)x;
        }

        #endregion
    }
}
