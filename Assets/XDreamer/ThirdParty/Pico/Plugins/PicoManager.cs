using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginPico.CNScripts;

namespace XCSJ.PluginPico
{
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Peripheral)]
    [Name(PicoHelper.Title)]
    [Tip("用于对接PICO官方插件包的管理器组件", "Manager component for docking PICO official plug-in package")]
    [Guid("14789DB5-1818-4743-92ED-D700C153D586")]
    [Version("23.428")]
    public sealed class PicoManager : BaseManager<PicoManager, EScriptID>
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
