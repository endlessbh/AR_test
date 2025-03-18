using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginTools;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 客户端中文脚本处理器:用于客户端处理中文脚本命令或其返回值的处理
    /// </summary>
    [Name("客户端中文脚本处理器")]
    [Tip("用于客户端处理中文脚本命令或其返回值的处理", "It is used for the client to process Chinese script commands or their return values")]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), nameof(Client), rootType = typeof(Client), groupRule = EToolGroupRule.None)]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    public class ClientCNScriptProcessor : ClientProcessor
    {
        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netAnswer"></param>
        protected override void OnReceived(Client client, NetAnswer netAnswer)
        {
            if (!(netAnswer is NetCNScriptAnswer answer)) return;
            if (!ValidClient(client)) return;
            var package = answer.netCNScriptPackage.Handle();
            switch(package.packageType)
            {
                case ENetCNScriptPackageType.ReturnSuccess:
                case ENetCNScriptPackageType.ReturnFail:
                    {
                        client.Send(package);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 客户端信息
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("客户端信息")]
    public class ClientInfo
    {
        /// <summary>
        /// 客户端规则
        /// </summary>
        [Name("客户端规则")]
        public enum EClientRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            Any,

            /// <summary>
            /// 列表中
            /// </summary>
            [Name("列表中")]
            InList,

            /// <summary>
            /// 不在列表中
            /// </summary>
            [Name("不在列表中")]
            NotInList,
        }

        /// <summary>
        /// 客户端规则
        /// </summary>
        [Name("客户端规则")]
        [EnumPopup]
        public EClientRule _clientRule = EClientRule.Any;

        /// <summary>
        /// 客户端列表
        /// </summary>
        [Name("客户端列表")]
        public List<Client> _clients = new List<Client>();

        /// <summary>
        /// 有效客户端
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool ValidClient(Client client)
        {
            if (!client) return false;
            switch (_clientRule)
            {
                case EClientRule.Any: return true;
                case EClientRule.InList: return _clients.Contains(client);
                case EClientRule.NotInList: return !_clients.Contains(client);
                default: return false;
            }
        }
    }
}
