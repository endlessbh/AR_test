using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
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
    /// 响应JS: 响应JS
    /// </summary>
    [ComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
    [Name(Title, nameof(ResponseJS))]
    [Tip("针对指定的嵌入式浏览器页面做代码级交互，可直接响应对应页面的JS代码；即实现JavaScript到Unity的数据通信；")]
    [XCSJ.Attributes.Icon(EIcon.Script)]
    [Owner(typeof(EmbeddedBrowserManager))]
    public class ResponseJS : Trigger<ResponseJS>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "响应JS";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(EmbeddedBrowserHelper.Title, typeof(EmbeddedBrowserManager))]
        [StateComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
        [Name(Title, nameof(ResponseJS))]
        [Tip("针对指定的嵌入式浏览器页面做代码级交互，可直接响应对应页面的JS代码；即实现JavaScript到Unity的数据通信；")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        [Name("浏览器")]
        [Tip("浏览器", "Browser")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Browser _Browser;

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        public Browser Browser => this.XGetComponentInGlobal(ref _Browser, true);

#endif

        /// <summary>
        /// 响应类型
        /// </summary>
        [Name("响应类型")]
        public enum EResponeType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 注册函数
            /// </summary>
            [Name("注册函数")]
            RegisterFunction,

            /// <summary>
            /// 通过Id添加事件监听器
            /// </summary>
            [Name("通过Id添加事件监听器")]
            AddEventListenerById,
        }

        /// <summary>
        /// 响应类型
        /// </summary>
        [Name("响应类型")]
        [EnumPopup]
        public EResponeType _responeType = EResponeType.RegisterFunction;

        /// <summary>
        /// JS函数名
        /// </summary>
        [Name("JS函数名")]
        [HideInSuperInspector(nameof(_responeType), EValidityCheckType.NotEqual, EResponeType.RegisterFunction)]
        public StringPropertyValue _jsFunctionName = new StringPropertyValue();

        /// <summary>
        /// 元素ID
        /// </summary>
        [Name("元素ID")]
        [HideInSuperInspector(nameof(_responeType), EValidityCheckType.NotEqual, EResponeType.AddEventListenerById)]
        public StringPropertyValue _jsElementId = new StringPropertyValue();

        /// <summary>
        /// JS事件名
        /// </summary>
        [Name("JS事件名")]
        [HideInSuperInspector(nameof(_responeType), EValidityCheckType.NotEqual, EResponeType.AddEventListenerById)]
        public StringPropertyValue _jsEventName = new StringPropertyValue();

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_EMBEDDED_BROWSER
            browserBak = this._Browser;
            if (browserBak)
            {
                RegisterJSCallback();
                browserBak.onLoad += OnLoad;
            }
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
            if (browserBak)
            {
                browserBak.onLoad -= OnLoad;
                browserBak = null;
            }

#endif
            callbackFuncNameBak = "";
        }

#if XDREAMER_EMBEDDED_BROWSER

        Browser browserBak;

        private void OnLoad(JSONNode args)
        {
            if (browserBak && args["status"] == 200)
            {
                callbackFuncNameBak = "";
                RegisterJSCallback();
            }
        }

#endif

        string callbackFuncNameBak;

        private void RegisterJSCallback(string callbackFuncName)
        {
            //防止重复定义
            if (callbackFuncName == callbackFuncNameBak) return;

#if XDREAMER_EMBEDDED_BROWSER
            browserBak.RegisterFunction(callbackFuncName, OnJSCallback);
#endif
            callbackFuncNameBak = callbackFuncName;
        }

#if XDREAMER_EMBEDDED_BROWSER

        private void RegisterJSCallback()
        {
            if (!browserBak) return;
            switch (_responeType)
            {
                case EResponeType.RegisterFunction:
                    {
                        RegisterJSCallback(_jsFunctionName.GetValue());
                        break;
                    }
                case EResponeType.AddEventListenerById:
                    {
                        var elementId = _jsElementId.GetValue();
                        var eventName = _jsEventName.GetValue();

                        //定义回调函数
                        var callbackFuncName = string.Format("_{0}_{1}_{2}", elementId, eventName, GetHashCode().ToString().Replace("-", "_"));
                        RegisterJSCallback(callbackFuncName);

                        //事件函数
                        var eventFuncName = string.Format("{0}_event", callbackFuncName);
                        //执行函数
                        var invokeFuncName = string.Format("{0}_invoke", callbackFuncName);

                        var jsFormat = @"if(typeof {2} === 'undefined') {2} = function() {{ {3}('{0}', '{1}', Array.prototype.slice.call(arguments)); }};
{4} = function()
{{
    var element = document.getElementById('{0}');
    if(!element) return;
    element.removeEventListener('{1}', {2});
    element.addEventListener('{1}', {2});
}}
{4}();";
                        var js = string.Format(jsFormat, elementId, eventName, eventFuncName, callbackFuncName, invokeFuncName);
                        browserBak.EvalJS(js);
                        break;
                    }
            }
        }

        private void OnJSCallback(JSONNode args)
        {
            finished = true;

            var scriptManager = ScriptManager.instance;
            if (scriptManager)
            {
                var callbackParams = args.AsJSON;
                scriptManager.TrySetOrAddSetHierarchyVarValue(_callbackParamVarString, callbackParams);
                scriptManager.ExecuteFunction(_callbackFunction, callbackParams);
            }
        }

#endif

        /// <summary>
        /// 回调参数变量字符串
        /// </summary>
        [Name("回调参数变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _callbackParamVarString;

        /// <summary>
        /// 回调函数
        /// </summary>
        [Name("回调函数")]
        public CustomFunction _callbackFunction = new CustomFunction();

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
