using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.PluginCommonUtils;
using static XCSJ.PluginMMO.LocalCache;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 用户信息列表
    /// </summary>
    [Name("用户信息列表")]
    [RequireManager(typeof(MMOManager))]
    public class UserInfoList : ComponentPool<UserInfoItem>
    {
        /// <summary>
        /// 主动刷新
        /// </summary>
        [Name("主动刷新")]
        public bool _activeUpdate = false;

        /// <summary>
        /// 刷新间格时间
        /// </summary>
        [Name("刷新间格时间")]
        [Range(1, 60)]
        [HideInSuperInspector(nameof(_activeUpdate), EValidityCheckType.Equal, false)]
        public float _updateTime = 3;

        [Name("自动滚动")]
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
        /// 用户项增加事件回调函数
        /// </summary>
        public static event Action<UserInfoList, UserInfoItem> onAddUserInfoItem = null;

        /// <summary>
        /// 用户项增加事件回调函数
        /// </summary>
        public static event Action<UserInfoList, UserInfoItem> onUpdateUserNameItem = null;

        /// <summary>
        /// 用户项移除事件回调函数
        /// </summary>
        public static event Action<UserInfoList, UserInfoItem> onRemoveUserInfoItem = null;

        protected override void DefaultReset(UserInfoItem component)
        {
            onRemoveUserInfoItem?.Invoke(this, component);
            component.ResetData();
            base.DefaultReset(component);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_template)
            {
                MMOManager.onNetStateChanged += OnNetStateChanged;

                NetIdentity.onPlayerEnter += OnPlayerEnter;
                NetIdentity.onPlayerExit += OnPlayerExit;

                OnJoinRoom();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_template)
            {
                MMOManager.onNetStateChanged -= OnNetStateChanged;

                NetIdentity.onPlayerEnter -= OnPlayerEnter;
                NetIdentity.onPlayerExit -= OnPlayerExit;
            }
        }

        private float timeCounter = 0;

        protected void Update()
        {
            // 主动刷新信息
            if (_activeUpdate)
            {
                timeCounter += Time.deltaTime;
                if (timeCounter> _updateTime)
                {
                    timeCounter = 0;
                    foreach (var item in LocalCache.players.Values)
                    {
                        if (!string.IsNullOrEmpty(item.nickName))
                        {
                            var ui = componentPool.Find(obj => obj.playerInfo.guid == item.guid);
                            if (ui)
                            {
                                ui.userName = item.nickName;
                                onUpdateUserNameItem?.Invoke(this, ui);
                            }
                        }
                    }
                }
            }
        }

        private void OnNetStateChanged(ENetState oldNetState, ENetState newNetState)
        {
            switch (newNetState)
            {
                case ENetState.Unknow: break;
                case ENetState.Init: break;
                case ENetState.Connecting: break;
                case ENetState.Connected: break;
                case ENetState.ConnectFail: break;
                case ENetState.WillClose: break;
                case ENetState.Closing: break;
                case ENetState.Closed: break;
                case ENetState.Logining: break;
                case ENetState.Logined: break;
                case ENetState.LoginFail: break;
                case ENetState.Logouting: break;
                case ENetState.Logouted: break;
                case ENetState.LogoutFail: break;
                case ENetState.JoinRooming: break;
                case ENetState.JoinRoomed: OnJoinRoom(); break;
                case ENetState.JoinRoomFail: break;
                case ENetState.SyncRooming: break;
                case ENetState.SyncRoomed: break;
                case ENetState.QuitRooming: break;
                case ENetState.QuitRoomed: OnQuitRoom(); break;
                case ENetState.QuitRoomFail: break;
                case ENetState.NetError: break;
                default: break;
            }
        }

        private void OnJoinRoom()
        {
            OnQuitRoom();
            foreach (var item in LocalCache.players)
            {
                CreateUserInfoItem(item.Key, item.Value);
            }
        }

        private void OnQuitRoom() => componentPool.Clear();

        private void OnPlayerEnter(NetIdentity netID, string useID)
        {
            if (LocalCache.players.ContainsKey(useID))
            {
                CreateUserInfoItem(useID);
            }
        }

        private void OnPlayerExit(NetIdentity netID, string useID)
        {
            componentPool.Free(item => item.userID == useID);
        } 
        
        private void CreateUserInfoItem(string useID)
        {
            if (LocalCache.players.TryGetValue(useID, out PlayerInfo playerInfo))
            {
                CreateUserInfoItem(useID, playerInfo);
            }
        }

        private void CreateUserInfoItem(string useID, PlayerInfo playerInfo)
        {
            var item = componentPool.Find(obj => obj.playerInfo.guid == useID);
            if (!item)
            {
                item = componentPool.Alloc();
                item.playerInfo = playerInfo;
                onAddUserInfoItem?.Invoke(this, item);

                CommonFun.DelayCall(()=>
                {
                    if (_autoScrollBar && _scrollbar)
                    {
                        _scrollbar.value = scrollValue;
                    }
                }, 0.3f);
            }
        }
    }
}
