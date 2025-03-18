using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginTools;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 服务器模拟输入处理器:用于服务器处理模拟输入问题数据包
    /// </summary>
    [Name("服务器模拟输入处理器")]
    [Tip("用于服务器处理模拟输入问题数据包", "Packets used by the server to process analog input problems")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Server), rootType = typeof(Server), groupRule = EToolGroupRule.None)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class ServerAnalogInputProcessor : ServerProcessor
    {
        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="netQuestion"></param>
        protected override void OnReceived(Server server, NetQuestion netQuestion)
        {
            if (!(netQuestion is NetAnalogInputQuestion question)) return;
            if (!ValidServer(server)) return;
            question.netAnalogInput.Handle();
        }
    }
}
