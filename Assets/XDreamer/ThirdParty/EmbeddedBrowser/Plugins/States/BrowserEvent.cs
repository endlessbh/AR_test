using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginEmbeddedBrowser;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

#if XDREAMER_EMBEDDED_BROWSER

using ZenFulcrum.EmbeddedBrowser;
using static ZenFulcrum.EmbeddedBrowser.BrowserNative;

#endif

namespace XCSJ.PluginEmbeddedBrowser.States
{
    /// <summary>
    /// 浏览器事件: 浏览器事件
    /// </summary>
    [ComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
    [Name(Title, nameof(BrowserEvent))]
    [Tip("浏览器事件")]
    [XCSJ.Attributes.Icon(EIcon.Trigger)]
    [Owner(typeof(EmbeddedBrowserManager))]
    public class BrowserEvent : Trigger<BrowserEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "浏览器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(EmbeddedBrowserHelper.Title, typeof(EmbeddedBrowserManager))]
        [StateComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
        [Name(Title, nameof(BrowserEvent))]
        [Tip("浏览器事件")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        [Name("浏览器")]
        [Tip("浏览器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Browser _Browser;

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        public Browser Browser => this.XGetComponentInGlobal(ref _Browser, true);

#endif

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [Tip("事件类型")]
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当控制台消息
            /// </summary>
            [Name("当控制台消息")]
            OnConsoleMessage,

            /// <summary>
            /// 调整大小后
            /// </summary>
            [Name("调整大小后")]
            AfterResize,

            /// <summary>
            /// 当加载
            /// </summary>
            [Name("当加载")]
            OnLoad,

            /// <summary>
            /// 当获取错误
            /// </summary>
            [Name("当获取错误")]
            OnFetchError,

            /// <summary>
            /// 当证书错误
            /// </summary>
            [Name("当证书错误")]
            OnCertError,

            /// <summary>
            /// 当证书错误
            /// </summary>
            [Name("当证书错误")]
            OnSadTab,

            /// <summary>
            /// 当纹理已更新
            /// </summary>
            [Name("当纹理已更新")]
            OnTextureUpdated,

            /// <summary>
            /// 当导航状态变化
            /// </summary>
            [Name("当导航状态变化")]
            OnNavStateChange,

            /// <summary>
            /// 当下载已开始
            /// </summary>
            [Name("当下载已开始")]
            OnDownloadStarted,

            /// <summary>
            /// 当下载状态变化
            /// </summary>
            [Name("当下载状态变化")]
            OnDownloadStatus,

            /// <summary>
            /// 当节点聚焦
            /// </summary>
            [Name("当节点聚焦")]
            OnNodeFocus,

            /// <summary>
            /// 当浏览器聚焦
            /// </summary>
            [Name("当浏览器聚焦")]
            OnBrowserFocus,

            /// <summary>
            /// 当任意浏览器创建
            /// </summary>
            [Name("当任意浏览器创建")]
            OnAnyBrowserCreated,

            /// <summary>
            /// 当任意浏览器销毁
            /// </summary>
            [Name("当任意浏览器销毁")]
            OnAnyBrowserDestroyed,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [Tip("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnLoad;

#if XDREAMER_EMBEDDED_BROWSER

        private Browser _BrowserBak;

#endif

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
#if XDREAMER_EMBEDDED_BROWSER
            _BrowserBak = this._Browser;
            if (_BrowserBak)
            {
                _BrowserBak.onConsoleMessage += OnConsoleMessage;
                _BrowserBak.afterResize += AfterResize;
                _BrowserBak.onLoad += OnLoad;
                _BrowserBak.onFetchError += OnFetchError;
                _BrowserBak.onCertError += OnCertError;
                _BrowserBak.onSadTab += OnSadTab;
                _BrowserBak.onTextureUpdated += OnTextureUpdated;
                _BrowserBak.onNavStateChange += OnNavStateChange;
                _BrowserBak.onDownloadStarted += OnDownloadStarted;
                _BrowserBak.onDownloadStatus += OnDownloadStatus;
                _BrowserBak.onNodeFocus += OnNodeFocus;
                _BrowserBak.onBrowserFocus += OnBrowserFocus;
            }
            Browser.onAnyBrowserCreated += OnAnyBrowserCreated;
            Browser.onAnyBrowserDestroyed += OnAnyBrowserDestroyed;

#endif
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
#if XDREAMER_EMBEDDED_BROWSER
            if (_BrowserBak)
            {
                _BrowserBak.onConsoleMessage -= OnConsoleMessage;
                _BrowserBak.afterResize -= AfterResize;
                _BrowserBak.onLoad -= OnLoad;
                _BrowserBak.onFetchError -= OnFetchError;
                _BrowserBak.onCertError -= OnCertError;
                _BrowserBak.onSadTab -= OnSadTab;
                _BrowserBak.onTextureUpdated -= OnTextureUpdated;
                _BrowserBak.onNavStateChange -= OnNavStateChange;
                _BrowserBak.onDownloadStarted -= OnDownloadStarted;
                _BrowserBak.onDownloadStatus -= OnDownloadStatus;
                _BrowserBak.onNodeFocus -= OnNodeFocus;
                _BrowserBak.onBrowserFocus -= OnBrowserFocus;
            }
            Browser.onAnyBrowserCreated -= OnAnyBrowserCreated;
            Browser.onAnyBrowserDestroyed -= OnAnyBrowserDestroyed;
#endif
        }

        /// <summary>
        /// 消息变量字符串
        /// </summary>
        [Name("消息变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnConsoleMessage)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _messageVarString;

        /// <summary>
        /// 源变量字符串
        /// </summary>
        [Name("源变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnConsoleMessage)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _sourceVarString;

        private void OnConsoleMessage(string message, string source)
        {
            if (finished) return;
            if (_eventType != EEventType.OnConsoleMessage) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_messageVarString, message);
                scriptManager.TrySetOrAddSetHierarchyVarValue(_sourceVarString, source);
            }
        }

        private void AfterResize(Texture2D image)
        {
            if (finished) return;
            if (_eventType != EEventType.AfterResize) return;

            finished = true;
        }

        /// <summary>
        /// 加载变量字符串
        /// </summary>
        [Name("加载变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnLoad)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _loadVarString;

        #if XDREAMER_EMBEDDED_BROWSER
        private void OnLoad(JSONNode node)
        {
            if (finished) return;
            if (_eventType != EEventType.OnLoad) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_loadVarString, node.AsJSON);
            }
        }
#endif

        /// <summary>
        /// 获取错误变量字符串
        /// </summary>
        [Name("获取错误变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnFetchError)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _fetchErrorVarString;

        #if XDREAMER_EMBEDDED_BROWSER

        private void OnFetchError(JSONNode node)
        {
            if (finished) return;
            if (_eventType != EEventType.OnFetchError) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_fetchErrorVarString, node.AsJSON);
            }
        }

#endif

        /// <summary>
        /// 证书错误变量字符串
        /// </summary>
        [Name("证书错误变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnCertError)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _certErrorVarString;

        #if XDREAMER_EMBEDDED_BROWSER

        private void OnCertError(JSONNode node)
        {
            if (finished) return;
            if (_eventType != EEventType.OnCertError) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_certErrorVarString, node.AsJSON);
            }
        }

#endif

        private void OnSadTab()
        {
            if (finished) return;
            if (_eventType != EEventType.OnSadTab) return;

            finished = true;
        }

        private void OnTextureUpdated()
        {
            if (finished) return;
            if (_eventType != EEventType.OnTextureUpdated) return;

            finished = true;
        }

        private void OnNavStateChange()
        {
            if (finished) return;
            if (_eventType != EEventType.OnNavStateChange) return;

            finished = true;
        }

        /// <summary>
        /// 下载已开始ID变量字符串
        /// </summary>
        [Name("下载已开始ID变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnDownloadStarted)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _downloadStartedIdVarString;

        /// <summary>
        /// 下载已开始节点变量字符串
        /// </summary>
        [Name("下载已开始节点变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnDownloadStarted)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _downloadStartedNodeVarString;

#if XDREAMER_EMBEDDED_BROWSER

        private void OnDownloadStarted(int id, JSONNode node)
        {
            if (finished) return;
            if (_eventType != EEventType.OnDownloadStarted) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_certErrorVarString, id.ToString());
                scriptManager.TrySetOrAddSetHierarchyVarValue(_downloadStartedNodeVarString, node.AsJSON);
            }
        }

#endif

        /// <summary>
        /// 下载已开始ID变量字符串
        /// </summary>
        [Name("下载已开始ID变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnDownloadStatus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _downloadStatusIdVarString;

        /// <summary>
        /// 下载已开始节点变量字符串
        /// </summary>
        [Name("下载已开始节点变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnDownloadStatus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _downloadStatusNodeVarString;

#if XDREAMER_EMBEDDED_BROWSER

        private void OnDownloadStatus(int id, JSONNode node)
        {
            if (finished) return;
            if (_eventType != EEventType.OnDownloadStatus) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_downloadStatusIdVarString, id.ToString());
                scriptManager.TrySetOrAddSetHierarchyVarValue(_downloadStatusNodeVarString, node.AsJSON);
            }
        }

#endif

        /// <summary>
        /// 标签名变量字符串
        /// </summary>
        [Name("标签名变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnNodeFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _tagNameVarString;

        /// <summary>
        /// 可编辑变量字符串
        /// </summary>
        [Name("可编辑变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnNodeFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _editableVarString;

        /// <summary>
        /// 值变量字符串
        /// </summary>
        [Name("值变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnNodeFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _valueVarString;

        private void OnNodeFocus(string tagName, bool editable, string value)
        {
            if (finished) return;
            if (_eventType != EEventType.OnNodeFocus) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_tagNameVarString, tagName);
                scriptManager.TrySetOrAddSetHierarchyVarValue(_editableVarString, editable.ToString());
                scriptManager.TrySetOrAddSetHierarchyVarValue(_valueVarString, value);
            }
        }

        /// <summary>
        /// 鼠标聚焦变量字符串
        /// </summary>
        [Name("鼠标聚焦变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnBrowserFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _mouseFocusedVarString;

        /// <summary>
        /// 键盘聚焦变量字符串
        /// </summary>
        [Name("键盘聚焦变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnBrowserFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _keyboardFocusedVarString;

        private void OnBrowserFocus(bool mouseFocused, bool keyboardFocused)
        {
            if (finished) return;
            if (_eventType != EEventType.OnBrowserFocus) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_mouseFocusedVarString, mouseFocused.ToString());
                scriptManager.TrySetOrAddSetHierarchyVarValue(_keyboardFocusedVarString, keyboardFocused.ToString());
            }
        }

        /// <summary>
        /// 已创建浏览器变量字符串
        /// </summary>
        [Name("已创建浏览器变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnBrowserFocus)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _browserCreatedVarString;

#if XDREAMER_EMBEDDED_BROWSER

        private void OnAnyBrowserCreated(Browser browser)
        {
            if (finished) return;
            if (_eventType != EEventType.OnAnyBrowserCreated) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_browserCreatedVarString, CommonFun.GameObjectComponentToString(browser));
            }
        }

#endif

        /// <summary>
        /// 已销毁浏览器变量字符串
        /// </summary>
        [Name("已销毁浏览器变量字符串")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnAnyBrowserDestroyed)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _browserDestroyedVarString;

#if XDREAMER_EMBEDDED_BROWSER

        private void OnAnyBrowserDestroyed(Browser browser)
        {
            if (finished) return;
            if (_eventType != EEventType.OnAnyBrowserDestroyed) return;

            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_browserDestroyedVarString, CommonFun.GameObjectComponentToString(browser));
            }
        }
#endif

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_eventType); //base.ToFriendlyString();
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
#if XDREAMER_EMBEDDED_BROWSER
            if (Browser) { }
#endif
        }
    }
}
