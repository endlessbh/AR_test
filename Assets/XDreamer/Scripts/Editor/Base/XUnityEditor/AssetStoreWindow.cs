using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.AssetStoreWindow")]
    public class AssetStoreWindow : EditorWindow_LinkType<AssetStoreWindow>
    {
        public AssetStoreWindow(EditorWindow obj) : base(obj) { }
        public AssetStoreWindow(object obj) : base(obj) { }


#if UNITY_2020_1_OR_NEWER

#else//在Unity2020.1版本中资源商店被使用网页与包管理器替代，本类中原诸多反射方法被移除！

        #region Init

        public static XMethodInfo Init_MethodInfo { get; } = GetXMethodInfo(nameof(Init));

        public static AssetStoreWindow Init()
        {
            return new AssetStoreWindow(Init_MethodInfo.Invoke(null, null));
        }

        #endregion

        #region webView

        public static XFieldInfo webView_FieldInfo { get; } = GetXFieldInfo(nameof(webView));

        public WebView webView
        {

            get => new WebView(webView_FieldInfo.GetValue(obj));
            set
            {
                if (value)
                {
                    webView_FieldInfo.SetValue(obj, value.obj);
                }
            }
        }

        #endregion

#endif
    }
}
