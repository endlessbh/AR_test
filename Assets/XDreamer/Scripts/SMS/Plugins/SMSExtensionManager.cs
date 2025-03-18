using System;
using System.Runtime.InteropServices;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS
{
    /// <summary>
    /// 状态机系统扩展:基于状态机系统的扩展功能管理器插件；包括：状态机时间轴化扩展等；
    /// </summary>
    [Name("状态机系统扩展")]
    [Tip("基于状态机系统的扩展功能管理器插件；包括：状态机时间轴化扩展等；", "Extended function manager plug-in based on state machine system; Including: time axis extension of state machine;")]
    [Guid("2DA89778-0424-440B-B3A7-82A6EC73783B")]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOption.Optional)]
    [Version("23.428")]
    [Index(index = 100)]
    public class SMSExtensionManager : BaseManager<SMSExtensionManager, ESMSExtensionScriptID>
    {
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(ESMSExtensionScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                case ESMSExtensionScriptID.GetWorkClipTimeLength:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        return ReturnValue.True(workClip.timeLength.ToString());
                    }
                case ESMSExtensionScriptID.SetWorkClipTimeLength:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        workClip.timeLength = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case ESMSExtensionScriptID.GetWorkClipOnceTimeLength:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        return ReturnValue.True(workClip.onceTimeLength.ToString());
                    }
                case ESMSExtensionScriptID.SetWorkClipOnceTimeLength:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        workClip.onceTimeLength = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case ESMSExtensionScriptID.GetWorkClipLoopType:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        return ReturnValue.True(workClip.loopType.ToString());
                    }
                case ESMSExtensionScriptID.SetWorkClipLoopType:
                    {
                        if (!(param[1] is WorkClip workClip)) break;
                        workClip.loopType = (ELoopType)param[2];
                        return ReturnValue.Yes;
                    }
            }
            return ReturnValue.No;
        }
    }
}
