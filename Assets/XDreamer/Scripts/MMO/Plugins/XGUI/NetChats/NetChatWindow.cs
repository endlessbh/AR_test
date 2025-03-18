using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    [DisallowMultipleComponent]
    [Name("网络聊天窗口", "Net Chat Window")]
    [RequireManager(typeof(MMOManager))]
    public class NetChatWindow : UGUIWindow
    {
        [Name("网络聊天")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetChat _netChat;

        public NetChat netChat { get => _netChat; private set => _netChat = value; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!netChat)
            {
                netChat = FindObjectOfType<NetChat>();
            }

            NetChat.onReceived += OnReceived;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            NetChat.onReceived -= OnReceived;
        }


        protected void OnReceived(NetChat nc, List<ChatInfo> chatInfos)
        {
            if (netChat == nc)
            {

            }
        }
    }
}
