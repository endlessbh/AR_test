using System;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 自动连接网络；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Net)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(AutoConnectNet))]
    [Tip("连接网络，断线后会自动重新尝试连接网络", "After the network is disconnected, it will automatically try to reconnect")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class AutoConnectNet : Trigger<AutoConnectNet>
    {
        public const string Title = "自动连接网络";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(AutoConnectNet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("连接网络，断线后会自动重新尝试连接网络", "After the network is disconnected, it will automatically try to reconnect")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 重连间隔时间
        /// </summary>
        [Name("重连间隔时间")]
        [Tip("网络连接失败时，自动重连的间隔时间", "The interval between automatic reconnection when the network connection fails")]
        [Range(3, 1000)]
        public float _delayTime = 5;

        /// <summary>
        /// 设置
        /// </summary>
        [Name("设置")]
        [Serializable]
        [LanguageFileOutput]
        public class Setting
        {
            [Name("规则")]
            [Tip("自动连接或询问", "Automatic connection or inquiry")]
            [EnumPopup]
            public TimeSetting.ERule rule = TimeSetting.ERule.Auto;
            //public int tryCount = -1;

            public bool auto => rule == TimeSetting.ERule.Auto;
        }

        [Name("连接配置")]
        [Tip("自动或询问连接", "Automatic or inquiry connection")]
        public Setting _connect = new Setting();

        public bool onlyConnect => _connect.auto && !_login.auto && !_joinRoom.auto;

        [Name("登录配置")]
        [Tip("自动或询问登录", "Automatic or inquiry login")]
        public Setting _login = new Setting();

        public bool onlyLogin => !_connect.auto && _login.auto && !_joinRoom.auto;

        [Name("加入配置")]
        [Tip("自动或询问配置", "Automatic or query configuration")]
        public Setting _joinRoom = new Setting();

        [Name("加入房间规则")]
        [EnumPopup]
        public EJoinRoomRule _joinRoomRule = EJoinRoomRule.Default;

        public bool onlyJoinRoom => !_connect.auto && !_login.auto && _joinRoom.auto;

        private float timeCounter = 0;

        private MMOManager mmo => MMOManager.instance;

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            //注册状态变更事件
            MMOManager.onNetStateChanged += OnNetStateChanged;

            // 设置时间上线，先连接一次
            timeCounter = _delayTime;
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            //取消注册状态变更事件
            MMOManager.onNetStateChanged -= OnNetStateChanged;
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            timeCounter += Time.deltaTime;
            if (timeCounter > _delayTime)
            {
                timeCounter = 0;

                if (forceInit)
                {
                    forceInit = false;
                    mmo.ForceInit();
                }

                AutoConnect();
            }
        }

        private void OnNetStateChanged(ENetState oldState, ENetState state)
        {
            AutoConnect();
        }

        private bool forceInit = false;

        private void AutoConnect()
        {
            switch (mmo.netState)
            {
                case ENetState.Unknow:
                case ENetState.ConnectFail:
                case ENetState.NetError:
                    {
                        forceInit = true;
                        break;
                    }
                case ENetState.Init:
                    {
                        if (_connect.auto) mmo.Connect();
                        break;
                    }
                case ENetState.Closed:break;
                case ENetState.Connecting: break;
                case ENetState.Connected:
                    {
                        if (onlyConnect)
                        {
                            finished = true;
                            break;
                        }
                        if (_login.auto)
                        {
                            mmo.Login();
                        }
                        break;
                    }
                case ENetState.WillClose: break;
                case ENetState.Closing: break;
                case ENetState.Logining: break;
                case ENetState.Logined:
                    {
                        if (onlyLogin)
                        {
                            finished = true;
                            break;
                        }
                        if (_joinRoom.auto)
                        {
                            mmo.JoinRoom(_joinRoomRule);
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                    }
                case ENetState.LoginFail:
                    {
                        if (_login.auto) mmo.Login();
                        break;
                    }
                case ENetState.Logouting: break;
                case ENetState.Logouted: break;
                case ENetState.LogoutFail: break;
                case ENetState.JoinRooming: break;
                case ENetState.JoinRoomed: break;
                case ENetState.JoinRoomFail:
                    {
                        if (_joinRoom.auto) mmo.JoinRoom(_joinRoomRule);
                        break;
                    }
                case ENetState.SyncRooming: break;
                case ENetState.SyncRoomed:
                    {
                        if (_joinRoom.auto)
                        {
                            finished = true;
                        }
                        break;
                    }
                case ENetState.QuitRooming: break;
                case ENetState.QuitRoomed: break;
                case ENetState.QuitRoomFail: break;
                default: break;
            }
        }

        public override string ToFriendlyString()
        {
            return _delayTime + "秒";
        }
    }
}