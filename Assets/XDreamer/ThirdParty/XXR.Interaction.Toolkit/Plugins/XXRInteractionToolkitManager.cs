using System;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginXXR.Interaction.Toolkit
{
    /// <summary>
    /// XR交互工具包:用于对接Unity中XR交互工具包的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Peripheral)]
    [Name(XRITHelper.Title, XRITHelper.TitleEN)]
    [Tip("用于对接Unity中XR交互工具包的管理器组件", "Manager component for docking XR interaction toolkit in Unity")]
    [Guid("2A906A6B-F567-4A69-BAF2-788548E14A06")]
    [Version("23.428")]
    [Index(index = 100)]
    public sealed class XXRInteractionToolkitManager : BaseManager<XXRInteractionToolkitManager, EScriptID>
    {
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
