using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO
{
    [ExecuteInEditMode]
    [RequireManager(typeof(MMOManager))]
    [RequireComponent(typeof(MMOManager))]
    [Name("多人在线MMO-HUD", "MMO-HUD")]
    [DisallowMultipleComponent]
    public sealed class MMOManagerHUD : InteractProvider, IReset
    {
        public MMOManager manager { get; private set; }

        public BaseGUIWindow hudWindow => window;

        [Name("HUD窗口")]
        public HUDWindow window = new HUDWindow();

        private void Awake()
        {
            window.HUD = this;
            window.manager = manager = GetComponent<MMOManager>();
        }

        private void Start()
        {
            window.Start();
        }

        private void OnGUI()
        {
            window.OnGUI();
        }

        public void Reset()
        {
            window.visable = true;
            window.rect = new Rect(0, 0, 420, 160);
            window._alignMode = ERectAnchor.TopRight;
        }

        [Serializable]
        [Name("HUD窗口")]
        public class HUDWindow : BaseGUIWindow, IStart
        {
            internal MMOManagerHUD HUD;

            internal MMOManager manager;

            [Name("启动时对齐")]
            public bool alignOnStart = true;

            private string searchText = "";

            public void Start()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.English);
#else
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.Chinese);
#endif
                if (alignOnStart) SetWindowPositionInScreen(_alignMode);
            }

            public override bool autoLayout => true;

            protected override void OnDrawContentLayout()
            {
                if (!HUD || !manager) return;
                GUILayout.BeginHorizontal();
                switch (manager.netState)
                {
                    case ENetState.SyncRooming:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label(string.Format("State: {0} {3:P}, Ping: {1:0.00} ms, ACode: {2}", manager.netState, manager.ping, manager.aCode, manager.asyncProgress));
#else
                            GUILayout.Label(string.Format("状态: {0} {3:P}, Ping: {1:0.00} ms, 返回码: {2}", manager.netState, manager.ping, manager.aCode, manager.asyncProgress));
#endif
                            break;
                        }
                    default:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label(string.Format("State: {0}, Ping: {1:0.00} ms, ACode: {2}", manager.netState, manager.ping, manager.aCode));
#else
                            GUILayout.Label(string.Format("状态: {0}, Ping: {1:0.00} ms, 返回码: {2}", manager.netState, manager.ping, manager.aCode));
#endif
                            break;
                        }
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                if (GUILayout.Button("Init", GUILayout.Width(60)))
#else
                if (GUILayout.Button("初始化", GUILayout.Width(60)))
#endif
                {
                    manager.InitNet();
                }
                GUILayout.EndHorizontal();
                switch (manager.netState)
                {
                    case ENetState.Init:
                    case ENetState.ConnectFail:
                    case ENetState.Closed:
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(CommonFun.NameTip(manager, nameof(manager.serverIP)), GUILayout.Width(60));
                            manager.serverIP = GUILayout.TextField(manager.serverIP);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(CommonFun.NameTip(manager, nameof(manager.serverPort)), GUILayout.Width(60));
                            manager.serverPort = Converter.instance.ConvertTo<int>(GUILayout.TextField(manager.serverPort.ToString()), MMOManager.DefaultPort);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Connect Server" + manager.connectServerSetting.waitedTimeString))
#else
                            if (GUILayout.Button("连接服务器" + manager.connectServerSetting.waitedTimeString))
#endif
                            {
                                manager.Connect();
                            }
                            if (GUILayout.Button("X", GUILayout.Width(30)))
                            {
                                manager.connectServerSetting.SwitchRule();
                            }
                            GUILayout.EndHorizontal();
                            break;
                        }
                    case ENetState.Connected:
                    case ENetState.LoginFail:
                    case ENetState.Logouted:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Close Server Connection"))
#else
                            if (GUILayout.Button("关闭服务器连接"))
#endif
                            {
                                manager.Close();
                            }

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(CommonFun.NameTip(manager, nameof(manager.serverUserName)), GUILayout.Width(60));
                            manager.serverUserName = GUILayout.TextField(manager.serverUserName);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(CommonFun.NameTip(manager, nameof(manager.serverUserPwd)), GUILayout.Width(60));
                            manager.serverUserPwd = GUILayout.PasswordField(manager.serverUserPwd, '*');
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Account Login" + manager.loginAccountSetting.waitedTimeString))
#else
                            if (GUILayout.Button("帐号登录" + manager.loginAccountSetting.waitedTimeString))
#endif
                            {
                                manager.Login();
                            }
                            if (GUILayout.Button("X", GUILayout.Width(30)))
                            {
                                manager.loginAccountSetting.SwitchRule();
                            }
                            GUILayout.EndHorizontal();
                            break;
                        }
                    case ENetState.Logined:
                    case ENetState.JoinRoomFail:
                    case ENetState.QuitRoomed:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Account Logout"))
#else
                            if (GUILayout.Button("帐号登出"))
#endif
                            {
                                manager.Logout();
                            }
                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Room List" + manager.refreshRoomListSetting.waitedTimeString))
#else
                            if (GUILayout.Button("房间列表" + manager.refreshRoomListSetting.waitedTimeString))
#endif
                            {
                                manager.RoomList();
                            }
                            if (GUILayout.Button("X", GUILayout.Width(30)))
                            {
                                manager.refreshRoomListSetting.SwitchRule();
                            }
                            searchText = GUILayout.TextField(searchText, GUILayout.Width(60));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Room Name");
                            GUILayout.Label("Pwd", GUILayout.Width(60));
                            GUILayout.Label("User Count", GUILayout.Width(60));
                            GUILayout.Label("Operation", GUILayout.Width(60));
#else
                            GUILayout.Label("房间名");
                            GUILayout.Label("密码", GUILayout.Width(60));
                            GUILayout.Label("人数", GUILayout.Width(60));
                            GUILayout.Label("操作", GUILayout.Width(60));
#endif
                            GUILayout.EndHorizontal();

                            foreach (var room in manager.rooms)
                            {
                                if (!StringHelper.SearchMatch(room.name, searchText)) continue;

                                GUILayout.BeginHorizontal();

                                GUILayout.Label(room.name);
#if UNITY_WEBGL && !UNITY_EDITOR
                                GUILayout.Label(room.pwd ? "Yes" : "No", GUILayout.Width(60));
#else
                                GUILayout.Label(room.pwd ? "有" : "无", GUILayout.Width(60));
#endif
                                GUILayout.Label(string.Format("{0}/{1}", room.userCount, room.limitCount), GUILayout.Width(60));
#if UNITY_WEBGL && !UNITY_EDITOR
                                if (GUILayout.Button("Join", GUILayout.Width(60))
                                    && room.userCount < room.limitCount)
#else
                                if (GUILayout.Button("加入", GUILayout.Width(60))
                                    && room.userCount < room.limitCount)
#endif
                                {
                                    manager.roomGuid = room.roomGuid;
                                    manager.roomName = room.name;
                                    manager.JoinRoom(EJoinRoomRule.Join);
                                }

                                GUILayout.EndHorizontal();
                            }
                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Room Name", GUILayout.Width(60));
#else
                            GUILayout.Label("房间名", GUILayout.Width(60));
#endif
                            manager.roomName = GUILayout.TextField(manager.roomName);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Room Pwd", GUILayout.Width(60));
#else
                            GUILayout.Label("房间密码", GUILayout.Width(60));
#endif
                            manager.roomPwd = GUILayout.PasswordField(manager.roomPwd, '*');
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Room User Count", GUILayout.Width(60));
#else
                            GUILayout.Label("房间人数", GUILayout.Width(60));
#endif
                            manager.roomUserLimitCount = Converter.instance.ConvertTo<int>(GUILayout.TextField(manager.roomUserLimitCount.ToString()), Define.RoomUserLimitCountDefault);
                            manager.roomUserLimitCount = Define.GetRoomUserLimitCount(manager.roomUserLimitCount, Define.RoomUserLimitCountMax);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Default Room" + manager.joinRoomSetting.waitedTimeString))
#else
                            if (GUILayout.Button("默认房间" + manager.joinRoomSetting.waitedTimeString))
#endif
                            {
                                manager.JoinRoom(EJoinRoomRule.Default);
                            }
                            if (GUILayout.Button("X", GUILayout.Width(30)))
                            {
                                manager.joinRoomSetting.SwitchRule();
                            }
#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("New Room"))
#else
                            if (GUILayout.Button("新建房间"))
#endif
                            {
                                manager.JoinRoom(EJoinRoomRule.New);
                            }
                            GUILayout.EndHorizontal();

                            break;
                        }
                    case ENetState.JoinRoomed:
                    case ENetState.QuitRoomFail:
                    case ENetState.SyncRooming:
                    case ENetState.SyncRoomed:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label(string.Format("Room Name: {0}, User Count: {1}/{2}", manager.roomName, manager.roomInfo.userCount, manager.roomInfo.limitCount));

                            if (GUILayout.Button("Quit Room"))
                            {
                                manager.QuitRoom();
                            }
#else
                            GUILayout.Label(string.Format("房间名: {0}, 人数: {1}/{2}", manager.roomName, manager.roomInfo.userCount, manager.roomInfo.limitCount));

                            if (GUILayout.Button("退出房间"))
                            {
                                manager.QuitRoom();
                            }
#endif

                            break;
                        }
                }
            }
        }
    }
}
