using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 聊天信息列表
    /// </summary>
    [Name("聊天信息列表")]
    [RequireManager(typeof(MMOManager))]
    public class ChatInfoItemList :  ComponentPool<ChatInfoItem>
    {
        [Name("聊天窗口")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public NetChatWindow _netChatWindow = null;

        [Name("自动滚动")]
        [Readonly(EEditorMode.Runtime)]
        public bool _autoScrollBar = true;

        [Name("滚动条")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        [HideInSuperInspector(nameof(_autoScrollBar), EValidityCheckType.Equal, false)]
        public Scrollbar _scrollbar = null;

        [Name("滚动值")]
        [Readonly(EEditorMode.Runtime)]
        [HideInSuperInspector(nameof(_autoScrollBar), EValidityCheckType.Equal, false)]
        public float scrollValue = 0;

        /// <summary>
        /// 添加聊天项回调事件
        /// </summary>
        public static event Action<ChatInfoItemList, ChatInfoItem> onAddChatInfoItem = null;

        /// <summary>
        /// 移除聊天项回调事件
        /// </summary>
        public static event Action<ChatInfoItemList, ChatInfoItem> onRemoveChatInfoItem = null;

        /// <summary>
        /// 缺省重置
        /// </summary>
        /// <param name="component"></param>
        protected override void DefaultReset(ChatInfoItem component)
        {
            component.ResetData();
            base.DefaultReset(component);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_netChatWindow)
            {
                _netChatWindow = GetComponentInParent<NetChatWindow>();
            }

            if (_template && _netChatWindow.netChat)
            {
                NetChat.onReceived += OnReceived;
                NetChat.onRemoved += OnRemoved;
                NetChat.onCleaned += OnCleaned;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_template && _netChatWindow.netChat)
            {
                NetChat.onReceived -= OnReceived;
                NetChat.onRemoved -= OnRemoved;
                NetChat.onCleaned -= OnCleaned;
            }
        }

        private void OnReceived(NetChat netChat, List<ChatInfo> chatInfos)
        {
            if (_netChatWindow.netChat == netChat)
            {
                CreateChatInfoItem(chatInfos.Last());
            }
        }

        private void OnRemoved(NetChat netChat, ChatInfo chatInfo)
        {
            if (_netChatWindow.netChat == netChat)
            {
                var item = componentPool.Find(c => c.chatInfo == chatInfo);
                if (item!=null)
                {
                    componentPool.Free(item);
                    onRemoveChatInfoItem?.Invoke(this, item);
                }
            }
        }

        private void OnCleaned(NetChat netChat)
        {
            if (_netChatWindow.netChat == netChat)
            {
                componentPool.Clear();
            }
        }

        private void CreateChatInfoItem(ChatInfo chatInfo)
        {
            if (chatInfo == null) return;

            var item = componentPool.Find(obj => obj.chatInfo == chatInfo);
            if (item)
            {
                item.chatInfo = chatInfo;
            }
            else
            {
                item = componentPool.Alloc();
                if (item)
                {
                    item.chatInfo = chatInfo;
                    onAddChatInfoItem?.Invoke(this, item);
                }
            }

            CommonFun.DelayCall(() =>
            {
                if (_autoScrollBar && _scrollbar)
                {
                    _scrollbar.value = scrollValue;
                }
            }, 0.3f);
        }
    }
}
