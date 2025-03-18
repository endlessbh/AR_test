using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Views.Inputs;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 基础发送器
    /// </summary>
    [Name("基础发送器")]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    public abstract class BaseSender : InteractProvider
    {
        /// <summary>
        /// 网络端
        /// </summary>
        [Name("网络端")]
        public NetEnd _netEnd = new NetEnd();

        /// <summary>
        /// 仅用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _netEnd.FindNetEnds();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _netEnd.FindNetEnds();
        }
    }

    /// <summary>
    /// 网络端
    /// </summary>
    [Serializable]
    public class NetEnd
    {
        /// <summary>
        /// 网络端规则
        /// </summary>
        [Name("网络端规则")]
        [EnumPopup]
        public ENetEndType _netEndType = ENetEndType.Client;

        /// <summary>
        /// 客户端列表
        /// </summary>
        [Name("客户端列表")]
        public List<Client> _clients = new List<Client>();

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        public List<Server> _servers = new List<Server>();

        /// <summary>
        /// 查找网络端
        /// </summary>
        public void FindNetEnds()
        {
            FindNetEnds(_netEndType);
        }

        private void FindNetEnds(ENetEndType netEndRule)
        {
            switch (netEndRule)
            {
                case ENetEndType.Client:
                    {
                        if (_clients.Count == 0 || !_clients.Exists(c => c))
                        {
                            var clientArray = UnityEngine.Object.FindObjectsOfType(typeof(Client));
                            if (clientArray.Length > 0)
                            {
                                _clients.AddRange(clientArray.Cast<Client>());
                            }
                        }
                        break;
                    }
                case ENetEndType.Server:
                    {
                        if (_servers.Count == 0 || !_servers.Exists(s => s))
                        {
                            var serverArray = UnityEngine.Object.FindObjectsOfType(typeof(Server));
                            if (serverArray.Length > 0)
                            {
                                _servers.AddRange(serverArray.Cast<Server>());
                            }
                        }
                        break;
                    }
                case ENetEndType.Both:
                    {
                        FindNetEnds(ENetEndType.Client);
                        FindNetEnds(ENetEndType.Server);
                        break;
                    }
            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="syncData"></param>
        public void Send(ISyncData syncData)
        {
            switch (_netEndType)
            {
                case ENetEndType.Client:
                    {
                        ClientSend(syncData?.ToNetQuestion());
                        break;
                    }
                case ENetEndType.Server:
                    {
                        ServertSend(syncData?.ToNetAnswer());
                        break;
                    }
                case ENetEndType.Both:
                    {
                        ClientSend(syncData?.ToNetQuestion());
                        ServertSend(syncData?.ToNetAnswer());
                        break;
                    }
            }
        }

        private void ClientSend(NetQuestion netQuestion)
        {
            if (netQuestion == null) return;
            foreach (var c in _clients)
            {
                if (c) c.Send(netQuestion);
            }
        }

        private void ServertSend(NetAnswer netAnswer)
        {
            if (netAnswer == null) return;
            foreach (var s in _servers)
            {
                if (s) s.Broadcast(netAnswer);
            }
        }
    }

    /// <summary>
    /// 同步数据接口
    /// </summary>
    public interface ISyncData
    {
        /// <summary>
        /// 转网络问题
        /// </summary>
        /// <returns></returns>
        NetQuestion ToNetQuestion();

        /// <summary>
        /// 转网络答案
        /// </summary>
        /// <returns></returns>
        NetAnswer ToNetAnswer();
    }

    /// <summary>
    /// 网络端类型
    /// </summary>
    [Name("网络端类型")]
    public enum ENetEndType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 客户端
        /// </summary>
        [Name("客户端")]
        Client,

        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        Server,

        /// <summary>
        /// 两者
        /// </summary>
        [Name("两者")]
        Both,
    }
}
