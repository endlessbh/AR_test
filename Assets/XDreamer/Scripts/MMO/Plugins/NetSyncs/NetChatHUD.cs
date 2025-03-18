using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络聊天HUD
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [Name("网络聊天HUD", "Net Chat HUD")]
    [RequireComponent(typeof(NetChat))]
    [RequireManager(typeof(MMOManager))]
    public sealed class NetChatHUD : InteractProvider, IReset
    {
        public NetChat chat { get; private set; }

        [Name("HUD窗口")]
        public HUDWindow window = new HUDWindow();

        private void Awake()
        {
            window.HUD = this;
            window.chat = chat = GetComponent<NetChat>();
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
            window.rect = new Rect(0, 0, 420, 270);
            window._alignMode = ERectAnchor.Left;
        }

        [Serializable]
        [Name("HUD窗口")]
        public class HUDWindow : BaseGUIWindow, IStart
        {
            internal NetChatHUD HUD;

            internal NetChat chat;

            [Name("启动时对齐")]
            public bool alignOnStart = true;

            [Name("显示用户列表")]
            public bool displayUserList = true;

            [Name("用户列表宽度")]
            public float userListWidth = 120;

            [Name("显示数目")]
            [Tip("-1表示显示所有聊天信息", "-1 indicates that all chat messages are displayed")]
            [Range(-1, NetChat.Max)]
            public int displayCount = NetChat.Max / 4;

            [Name("内容类型")]
            [EnumPopup]
            public EContentType contentType = EContentType.Text;

            private float startVoiceTime = 0;

            public override bool autoLayout => true;

            private string text = "";

            private GUIStyle m_BoxStyle = null;

            public GUIStyle boxStyle
            {
                get
                {
                    if (m_BoxStyle == null)
                    {
                        m_BoxStyle = new GUIStyle(GUI.skin.box);
                        m_BoxStyle.alignment = TextAnchor.UpperLeft;
                        m_BoxStyle.richText = true;
                    }
                    return m_BoxStyle;
                }
            }

            public void Start()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.English);
#else
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.Chinese);
#endif
                if (alignOnStart) SetWindowPositionInScreen(_alignMode);
                scrollPosition = new Vector2(1, 1);
            }

            public override Rect scrollRect
            {
                get
                {
                    var rect = base.scrollRect;
                    return new Rect(rect.x, rect.y, rect.width - (displayUserList ? userListWidth : 0), rect.height - SingleLineHeight);
                }
            }

            private Vector2 userListScrollPosition = new Vector2();

            protected override void OnDrawContent()
            {
                var contentRect = this.contentRect;

                #region 输入框与功能按钮

                {
                    var rect = new Rect(contentRect.x, contentRect.yMax - SingleLineHeight, contentRect.width, SingleLineHeight);
                    GUILayout.BeginArea(rect);
                    GUILayout.BeginHorizontal();

                    switch (contentType)
                    {
                        case EContentType.Text:
                            {
#if UNITY_WEBGL && !UNITY_EDITOR
                                if (chat && chat.supportVoice && GUILayout.Button("Voice", GUILayout.Width(50)))
#else
                                if (chat && chat.supportVoice && GUILayout.Button("语音", GUILayout.Width(50)))
#endif
                                {
                                    contentType = EContentType.Voice;
                                }

                                text = GUILayout.TextField(text);

#if UNITY_WEBGL && !UNITY_EDITOR
                                if (GUILayout.Button("Send", GUILayout.Width(50)) && chat)
#else
                                if (GUILayout.Button("发送", GUILayout.Width(50)) && chat)
#endif
                                {
                                    chat.Send(text);
                                }

                                break;
                            }
                        case EContentType.Voice:
                            {
#if UNITY_WEBGL && !UNITY_EDITOR
                                if (GUILayout.Button("Text", GUILayout.Width(50)) && chat)
#else
                                if (GUILayout.Button("文本", GUILayout.Width(50)) && chat)
#endif
                                {
                                    contentType = EContentType.Text;
                                }

                                if (chat && chat.supportVoice)
                                {
                                    if (chat.IsVoiceRecording())
                                    {
#if UNITY_WEBGL && !UNITY_EDITOR
                                        if (GUILayout.Button(string.Format("Send Voice {0:0.00}/{1}", Time.time - startVoiceTime, chat.voiceChat.lengthSec)) && chat)
#else
                                        if (GUILayout.Button(string.Format("发送语音 {0:0.00}/{1}", Time.time - startVoiceTime, chat.voiceChat.lengthSec)) && chat)
#endif
                                        {
                                            chat.StopVoice();
                                            chat.SendVoice();
                                        }

#if UNITY_WEBGL && !UNITY_EDITOR
                                        if (GUILayout.Button("Cancel", GUILayout.Width(50)) && chat)
#else
                                        if (GUILayout.Button("取消", GUILayout.Width(50)) && chat)
#endif
                                        {
                                            chat.StopVoice();
                                        }
                                    }
                                    else
                                    {
#if UNITY_WEBGL && !UNITY_EDITOR
                                        if (GUILayout.Button("Start Voice") && chat)
#else
                                        if (GUILayout.Button("开始语音") && chat)
#endif
                                        {
                                            chat.StartVoice();
                                            startVoiceTime = Time.time;
                                        }
                                    }
                                }
                                else
                                {
                                    contentType = EContentType.Text;
                                }

                                break;
                            }
                    }

#if UNITY_WEBGL && !UNITY_EDITOR
                    if (GUILayout.Button("Cls", GUILayout.Width(40)) && chat)
#else
                    if (GUILayout.Button("清空", GUILayout.Width(40)) && chat)
#endif
                    {
                        chat.Clear();
                    }

                    if (GUILayout.Button(displayUserList ? ">>" : "<<", GUILayout.Width(30)))
                    {
                        displayUserList = !displayUserList;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }

                #endregion

                #region 用户列表
                if (displayUserList && MMOManager.instance)
                {
                    var rect = new Rect(contentRect.xMax - userListWidth, contentRect.y, userListWidth, contentRect.height - SingleLineHeight);
                    try
                    {
                        GUILayout.BeginArea(rect);

#if UNITY_WEBGL && !UNITY_EDITOR
                        GUILayout.Box("User List", GUILayout.ExpandWidth(true), GUILayout.Height(SingleLineHeight));
#else
                        GUILayout.Box("用户列表", GUILayout.ExpandWidth(true), GUILayout.Height(SingleLineHeight));
#endif

                        userListScrollPosition = GUILayout.BeginScrollView(userListScrollPosition, GUI.skin.box, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

                        foreach (var kv in LocalCache.players)
                        {
                            var player = kv.Value;
                            if (player.guid == LocalCache.userGuid)
                            {
                                var bak = GUI.contentColor;
                                GUI.contentColor = Color.green;

                                GUILayout.Box(string.Format("{0}[{1}]", player.nickName ?? player.name, player.guid), boxStyle);

                                GUI.contentColor = bak;
                            }
                            else
                            {
                                GUILayout.Box(string.Format("{0}[{1}]", player.nickName ?? player.name, player.guid), boxStyle);
                            }
                        }
                    }
                    finally
                    {
                        GUILayout.EndScrollView();
                        GUILayout.EndArea();
                    }
                }

                #endregion

                #region 聊天框背景

                //绘制背景
                GUI.Box(scrollRect, "", boxStyle);

                #endregion

                base.OnDrawContent();
            }

            protected override void OnDrawContentLayout()
            {
                if (!HUD || !chat) return;
                switch (chat.state)
                {
                    case ENetState.SyncRoomed:
                        {
                            var total = chat.chatInfos.Count;
                            var count = displayCount == -1 ? total : Math.Min(displayCount, total);
                            var startIndex = total - count;

                            //for (int i = startIndex; i < total; i++)
                            for (int i = total - 1; i >= startIndex; i--)
                            {
                                var info = chat.chatInfos[i];

                                GUILayout.BeginHorizontal();

                                var content = CommonFun.TempContent(info.userName);
                                boxStyle.CalcMinMaxWidth(content, out float minWidth, out float maxWidth);
                                if (GUILayout.Button(content, boxStyle, GUILayout.Width(minWidth)))
                                {

                                }
                                switch (info.contentType)
                                {
                                    case EContentType.Text:
                                        {
                                            GUILayout.Button(info.text, boxStyle, GUILayout.ExpandWidth(true));
                                            break;
                                        }
                                    case EContentType.Voice:
                                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                                            if (GUILayout.Button(string.Format("[Voice {0:0.00}]", info.audioLength), GUILayout.ExpandWidth(true)))
#else
                                            if (GUILayout.Button(string.Format("[语音 {0:0.00}]", info.audioLength), GUILayout.ExpandWidth(true)))
#endif
                                            {
                                                info.PlayVoice();
                                            }
                                            break;
                                        }
                                }
                                GUILayout.EndHorizontal();
                            }
                            break;
                        }
                    default:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Can't chat without joining any valid rooms!", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
#else
                            GUILayout.Label("未加入任何有效房间,无法聊天！", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
#endif
                            break;
                        }
                }
            }
        }
    }
}
