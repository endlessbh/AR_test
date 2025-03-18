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
    /// 执行JS: 执行JS
    /// </summary>
    [ComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
    [Name(Title, nameof(ExecuteJS))]
    [Tip("针对指定的嵌入式浏览器页面做代码级交互，可直接执行对应页面的JS代码并获取对应执行的结果；即实现Unity到JavaScript的数据通信；")]
    [XCSJ.Attributes.Icon(EIcon.Script)]
    [Owner(typeof(EmbeddedBrowserManager))]
    public class ExecuteJS : Trigger<ExecuteJS>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "执行JS";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(EmbeddedBrowserHelper.Title, typeof(EmbeddedBrowserManager))]
        [StateComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
        [Name(Title, nameof(ExecuteJS))]
        [Tip("针对指定的嵌入式浏览器页面做代码级交互，可直接执行对应页面的JS代码并获取对应执行的结果；即实现Unity到JavaScript的数据通信；")]
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
        /// 执行类型
        /// </summary>
        public enum EExecuteType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 调用函数
            /// </summary>
            [Name("调用函数")]
            CallFunction,

            /// <summary>
            /// 评估JS
            /// </summary>
            [Name("评估JS")]
            EvalJS,

            /// <summary>
            /// 评估JSCSP
            /// </summary>
            [Name("评估JSCSP")]
            EvalJSCSP,
        }

        /// <summary>
        /// 执行类型
        /// </summary>
        [Name("执行类型")]
        [EnumPopup]
        public EExecuteType _executeType = EExecuteType.CallFunction;

        /// <summary>
        /// JS函数名
        /// </summary>
        [Name("JS函数名")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.CallFunction)]
        public StringPropertyValue _jsFunctionName = new StringPropertyValue();

        /// <summary>
        /// JS函数参数
        /// </summary>
        [Name("JS函数参数")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.CallFunction)]
        public List<StringPropertyValue> _jsFunctionParams = new List<StringPropertyValue>();

        /// <summary>
        /// JS脚本
        /// </summary>
        [Name("JS脚本")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.EvalJS)]
        public StringPropertyValue_TextArea _jsScript = new StringPropertyValue_TextArea();

        /// <summary>
        /// JS脚本URL
        /// </summary>
        [Name("JS脚本URL")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.EvalJS)]
        public StringPropertyValue _jsScriptURL = new StringPropertyValue();

        /// <summary>
        /// JS脚本CSP
        /// </summary>
        [Name("JS脚本CSP")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.EvalJSCSP)]
        public StringPropertyValue_TextArea _jsScriptCSP = new StringPropertyValue_TextArea();

        /// <summary>
        /// JS脚本URLCSP
        /// </summary>
        [Name("JS脚本URLCSP")]
        [HideInSuperInspector(nameof(_executeType), EValidityCheckType.NotEqual, EExecuteType.EvalJSCSP)]
        public StringPropertyValue _jsScriptURLCSP = new StringPropertyValue();

        /// <summary>
        /// 执行结果
        /// </summary>
        public enum EExecuteResult
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("不等待执行结果", "Do not wait for execution results")]
            None,

            /// <summary>
            /// 完成
            /// </summary>
            [Name("完成")]
            [Tip("等待执行完成，不论执行是否成功或失败", "Wait for the execution to complete, whether the execution succeeds or fails")]
            Done,

            /// <summary>
            /// 当被解决
            /// </summary>
            [Name("当被解决")]
            [Tip("等待成功执行", "Wait for successful execution")]
            OnResolved,

            /// <summary>
            /// 当被拒绝
            /// </summary>
            [Name("当被拒绝")]
            [Tip("等待执行失败，即发生异常", "An exception occurs when waiting for execution fails")]
            OnRejected,
        }

        /// <summary>
        /// 执行结果
        /// </summary>
        [Name("执行结果")]
        [EnumPopup]
        public EExecuteResult _executeResult = EExecuteResult.Done;

        /// <summary>
        /// 结果变量字符串
        /// </summary>
        [Name("结果变量字符串")]
        [Tip("将成功执行的结果信息存储在结果变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to the result variable string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _resultVarString;

        /// <summary>
        /// 结果变量字符串列表
        /// </summary>
        [Name("结果变量字符串列表")]
        [Tip("将成功执行的结果信息存储在结果变量字符串列表内每个变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to each variable string in the result variable string list")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public List<string> _resultVarStrings = new List<string>();

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_EMBEDDED_BROWSER
            var Browser = this._Browser;
            if (Browser)
            {
                IPromise<JSONNode> promise = default;
                switch (_executeType)
                {
                    case EExecuteType.EvalJS:
                        {
                            promise = Browser.EvalJS(_jsScript.GetValue(), _jsScriptURL.GetValue());
                            break;
                        }
                    case EExecuteType.EvalJSCSP:
                        {
                            promise = Browser.EvalJSCSP(_jsScript.GetValue(), _jsScriptURL.GetValue());
                            break;
                        }
                    case EExecuteType.CallFunction:
                        {
                            promise = Browser.CallFunction(_jsFunctionName.GetValue(), _jsFunctionParams.Cast(pv => JSONNode.Parse(pv.GetValue())).ToArray());
                            break;
                        }
                    default: return;
                }

                //执行结果的处理
                if (_executeResult != EExecuteResult.None)
                {
                    promise.Done(OnResolved, OnRejected);
                }
            }
#endif
        }

#if XDREAMER_EMBEDDED_BROWSER

        private void OnResolved(JSONNode result)
        {
            switch (_executeResult)
            {
                case EExecuteResult.Done:
                case EExecuteResult.OnResolved:
                    {
                        finished = true;
                        var scriptManager = ScriptManager.instance;
                        if (scriptManager)
                        {
                            var json = result.AsJSON;
                            scriptManager.TrySetOrAddSetHierarchyVarValue(_resultVarString, json);
                            foreach (var vs in _resultVarStrings)
                            {
                                scriptManager.TrySetOrAddSetHierarchyVarValue(vs, json);
                            }
                        }
                        break;
                    }
            }
        }

#endif

        private void OnRejected(Exception exception)
        {
            switch (_executeResult)
            {
                case EExecuteResult.Done:
                case EExecuteResult.OnRejected:
                    {
                        finished = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_executeType);//base.ToFriendlyString();
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
