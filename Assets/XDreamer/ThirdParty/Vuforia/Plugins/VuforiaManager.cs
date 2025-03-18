using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginVuforia.CNScripts;

namespace XCSJ.PluginVuforia
{
    /// <summary>
    /// Vuforia
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Advanced)]
    [Name(VuforiaHelper.Title)]
    [Tip("用于对接Vuforia官方插件包的管理器组件", "Manager component for docking Vuforia official plug-in package")]
    [Guid("34EEC7F6-DE83-4A63-9E63-8725A50B0B04")]
    [Version("23.428")]
    [Index(index = IndexAttribute.DefaultIndex + 25)]
    public sealed class VuforiaManager : BaseManager<VuforiaManager, EScriptID>
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
