using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginSamsungWMR
{
    /// <summary>
    /// 三星玄龙:用于对接三星玄龙头盔和手柄的管理器插件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Peripheral)]
    [Name(Title)]
    [Tip("用于对接三星玄龙头盔和手柄的管理器插件", "Manager plug-in for docking Samsung Xuanlong helmet and handle")]
    [Guid("DF6188E8-2EB9-4E45-A218-5D6DEB59DDBF")]
    [Version("23.428")]
    public sealed class SamsungWMRManager : BaseManager<SamsungWMRManager, EScriptID>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "三星玄龙";

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EScriptID id, ScriptParamList param)
        {
            return ReturnValue.Yes;
        }
    }
}
