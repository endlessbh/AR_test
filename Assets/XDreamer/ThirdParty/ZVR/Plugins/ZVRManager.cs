using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginZVR
{
    /// <summary>
    /// ZVR:用于对接ZVR官方插件包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(ZVRHelper.Title)]
    [Tip("用于对接ZVR官方插件包的管理器组件", "Manager component for docking ZVR official plug-in package")]
    [Guid("A9459A51-D7D4-477F-B139-A26DBDD82CFE")]
    [Version("23.428")]
    public sealed class ZVRManager : BaseManager<ZVRManager>
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
