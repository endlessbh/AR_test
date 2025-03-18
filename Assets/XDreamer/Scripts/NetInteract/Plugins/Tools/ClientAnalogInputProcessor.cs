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
    /// 客户端模拟输入处理器:用于客户端处理模拟输入答案数据包
    /// </summary>
    [Name("客户端模拟输入处理器")]
    [Tip("用于客户端处理模拟输入答案数据包", "Used for the client to process the analog input answer packet")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Client), rootType = typeof(Client), groupRule = EToolGroupRule.None)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class ClientAnalogInputProcessor : ClientProcessor
    {
        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netAnswer"></param>
        protected override void OnReceived(Client client, NetAnswer netAnswer)
        {
            if (!(netAnswer is NetAnalogInputAnswer answer)) return;
            if (!ValidClient(client)) return;
            answer.netAnalogInput.Handle();
        }
    }
}
