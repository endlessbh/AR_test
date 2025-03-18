using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginART
{
    /// <summary>
    /// ART:用于通过网络对接ART官方软件的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(ARTHelper.Title)]
    [Tip("用于通过网络对接ART官方软件的管理器组件", "Manager component for connecting ART official software through network")]
    [Guid("791DDF36-29C5-4B1A-9211-F27BC5AD3F45")]
    [Version("23.428")]
    public sealed class ARTManager : BaseManager<ARTManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => default;

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param) => ReturnValue.No;
    }
}
