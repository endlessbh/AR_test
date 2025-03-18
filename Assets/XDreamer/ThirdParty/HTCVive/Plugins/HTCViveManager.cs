using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginHTCVive.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginHTCVive
{
    /// <summary>
    /// HTC Vive:实现HTC Vive的按钮事件监听，相机控制，交互输入等操作；
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Peripheral)]
    [Name(Title)]
    [Tip("实现HTC Vive的按钮事件监听，相机控制，交互输入等操作；", "Realize button event monitoring, camera control, interactive input and other operations of HTC vive;")]
    [Guid("579FC5B2-4EFD-4ADC-ADFA-8804876906B2")]
    [Version("23.428")]
    public class HTCViveManager : BaseManager<HTCViveManager, EHTCViveScriptID>
    {
        public const string Title = "HTC Vive";

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="param">脚本参数</param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EHTCViveScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                case EHTCViveScriptID.ActiveSteamVRInputActionSet:
                    {
                        var actionSetName = param[1] as string;
                        if (!string.IsNullOrEmpty(actionSetName))
                        {
                            SteamVRHelper.ActiveActionSet(actionSetName, (EBool)param[2]);
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
