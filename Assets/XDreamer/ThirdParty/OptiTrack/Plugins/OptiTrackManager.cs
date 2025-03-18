using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginOptiTrack.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.PluginOptiTrack
{
    /// <summary>
    /// OptiTrack:用于对接OptiTrack官方插件包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(OptiTrackHelper.Title)]
    [Tip("用于对接OptiTrack官方插件包的管理器组件", "Manager component for docking with OptiTrack official plug-in package")]
    [Guid("3CCCFED9-B0F9-4C42-B453-919EDEB861AE")]
    [Version("23.428")]
    public sealed class OptiTrackManager : BaseManager<OptiTrackManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            //switch((EScriptID)id)
            //{

            //}
            return ReturnValue.No;
        }
    }
}
