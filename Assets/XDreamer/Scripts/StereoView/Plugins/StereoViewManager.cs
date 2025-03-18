using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety;
using XCSJ.PluginStereoView.CNScripts;
using XCSJ.PluginStereoView.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginStereoView
{
    /// <summary>
    /// 立体显示：用于调度、维护、管理单通道或多通道立体显示的管理器
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Peripheral)]
    [Name(StereoViewHelper.Title)]
    [Tip("用于调度、维护、管理单通道或多通道立体显示的管理器", "Manager for scheduling, maintaining and managing single channel or multi-channel stereoscopic display")]
    [Guid("19E8A69E-ABCD-431A-BD65-FEBC0E6AA908")]
    [Version("23.428")]
    [RequireManager(typeof(StereoViewManager))]
    public class StereoViewManager : BaseManager<StereoViewManager>//, IXRAccess
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
        /// <returns>返回值</returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch((EScriptID)id)
            {
                case EScriptID.GetScreenAnchorLinkInfo:
                    {
                        var link = param[1] as ScreenAnchorLink;
                        if (!link) break;
                        switch(param[2]as string)
                        {
                            case "关联旋转": return ReturnValue.True(CommonFun.Vector3ToString( link._linkRotation));
                        }
                        break;
                    }
                case EScriptID.SetScreenAnchorLinkInfo:
                    {
                        var link = param[1] as ScreenAnchorLink;
                        if (!link) break;
                        switch (param[2] as string)
                        {
                            case "关联旋转":
                                {
                                    link.linkRotation= CommonFun.StringToVector3(param[3] as string);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
