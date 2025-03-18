using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract
{
    /// <summary>
    /// 网络交互：用于实现多个Unity打包程序的网络数据通信；可实现远程管理等功能；
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name(NetInteractHelper.Title)]
    [Tip("用于实现多个Unity打包程序的网络数据通信；可实现远程管理等功能；", "Network data communication for realizing multiple unity packaging programs; It can realize remote management and other functions;")]
    [Guid("0CBEB096-D79F-4B2A-B3B0-78E3D40B620A")]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOption.Optional)]
    [Version("23.428")]
    //[Index(index = IndexAttribute.DefaultIndex + 10)]
    public class NetInteractManager : BaseManager<NetInteractManager, EScriptsID>
    {
        /// <summary>
        /// 处于激活态的服务器列表
        /// </summary>
        public static List<Server> activeServers { get; } = new List<Server>();

        /// <summary>
        /// 基于激活态的客户端列表
        /// </summary>
        public static List<Client> activeClients { get; } = new List<Client>();

        /// <summary>
        /// 获取虚拟客户端
        /// </summary>
        /// <param name="clientGuid"></param>
        /// <returns></returns>
        public ClientVirtual GetClientVirtual(string clientGuid)
        {
            if (string.IsNullOrEmpty(clientGuid)) return null;
            foreach (var s in activeServers)
            {
                if (s._server.GetClient(clientGuid) is ClientVirtual clientVirtual)
                {
                    return clientVirtual;
                }
            }
            return null;
        }

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EScriptsID id, ScriptParamList param)
        {
            switch (id)
            {
                case EScriptsID.ClientPropertySet:
                    {
                        var client = param[1] as Client;
                        if (client)
                        {
                            switch (param[2])
                            {
                                case "IP":
                                    {
                                        client.serverAddress = param[3] as string;
                                        return ReturnValue.Yes;
                                    }
                                case "端口":
                                    {
                                        if (int.TryParse(param[4] as string, out var value))
                                        {
                                            client.serverPort = value;
                                            return ReturnValue.Yes;
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}
