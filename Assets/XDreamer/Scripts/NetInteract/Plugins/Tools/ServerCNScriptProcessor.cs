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
    /// 服务器中文脚本处理器:用于服务器处理中文脚本命令或其返回值的处理
    /// </summary>
    [Name("服务器中文脚本处理器")]
    [Tip("用于服务器处理中文脚本命令或其返回值的处理", "It is used for the server to process Chinese script commands or their return values")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Server), rootType = typeof(Server), groupRule = EToolGroupRule.None)]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    public class ServerCNScriptProcessor : ServerProcessor
    {
        protected override void OnReceived(Server server, NetQuestion netQuestion)
        {
            if (!(netQuestion is NetCNScriptQuestion question)) return;
            if (!ValidServer(server)) return;
            var package = question.scriptPackage.Handle();
            switch (package.packageType)
            {
                case ENetCNScriptPackageType.ReturnSuccess:
                case ENetCNScriptPackageType.ReturnFail:
                    {
                        netQuestion.client?.Send(package);
                        break;
                    }
            }
        }
    }
}
