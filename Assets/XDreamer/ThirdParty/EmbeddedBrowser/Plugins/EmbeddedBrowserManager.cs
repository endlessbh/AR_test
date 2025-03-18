using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginEmbeddedBrowser.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginEmbeddedBrowser
{
    /// <summary>
    /// 嵌入式浏览器:用于通过网络对接ART官方软件的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(EmbeddedBrowserHelper.Title)]
    [Tip("用于对接嵌入式浏览器官方软件的管理器组件;", "Manager component for docking with official software of embedded browser")]
    [Guid("0D60980C-97DE-4A92-8897-EF08DA019142")]
    [Version("23.428")]
    public class EmbeddedBrowserManager : BaseManager<EmbeddedBrowserManager, EScriptID>
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EScriptID id, ScriptParamList param)
        {
            return ReturnValue.No;
        }
    }
}
