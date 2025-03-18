using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// 外部设备输入:可用于管理外部设备(简称外设)输入；主要基于Unity新版输入系统与旧版输入管理器实现；
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Core)]
    [Name(PeripheralDeviceInputHelper.Title)]
    [Tip("可用于管理外部设备(简称外设)输入；主要基于Unity新版输入系统与旧版输入管理器实现；", "It can be used to manage the input of external devices (hereinafter referred to as peripherals); It is mainly based on the new version of unity input system and the old version of input manager;")]
    [Guid("7D480A49-79BE-42C7-81B7-425D5DFF44FE")]
    [Version("23.428")]
    public class PeripheralDeviceInputManager : BaseManager<PeripheralDeviceInputManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns>脚本列表</returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="param">脚本参数</param>
        /// <returns>ReturnValue</returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            return ReturnValue.No;
        }
    }
}
