using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation
{
    /// <summary>
    /// AR Foundation:用于对接Unity中AR Foundation的管理器组件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentOption(EComponentOption.Optional)]
    [ComponentKit(EKit.Advanced)]
    [Name(XARFoundationHelper.Title)]
    [Tip("用于对接Unity中AR Foundation的管理器组件", "Manager component for docking AR Foundation in unity")]
    [Guid("B50A9080-165D-4C50-8A3B-D6C2ACFBDB47")]
    [Version("23.428")]
    [Index(index = IndexAttribute.DefaultIndex + 10)]
    public sealed class XARFoundationManager : BaseManager<XARFoundationManager, EScriptID>
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


#if XDREAMER_AR_FOUNDATION

        public class D1
        {
            public ARFaceManager faceManager;
        }

#endif
    }
}
