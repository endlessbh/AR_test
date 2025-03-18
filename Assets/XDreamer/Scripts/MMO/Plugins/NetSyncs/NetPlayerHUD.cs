using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.RichTexts;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO.NetSyncs
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [Name("网络玩家HUD", "Net Player HUD")]
    [RequireComponent(typeof(NetPlayer))]
    public class NetPlayerHUD : NetPropertyHUD
    {
        public NetPlayer player { get; private set; }

        #region 玩家昵称

        [Group("玩家昵称")]
        [Name("偏移量")]
        [Tip("玩家昵称框的偏移量", "Offset of player nickname box")]
        public Vector3 offset = new Vector3(0, 1, 0);

        public XGUIStyle style { get; } = new XGUIStyle(nameof(GUI.skin.box), s => s.richText = true);

        [Name("本地玩家文字颜色")]
        public Color localPlayerTextColor = Color.green;

        [Name("玩家文字颜色")]
        public Color playerTextColor = Color.white;

        #endregion

        public override void Awake()
        {
            base.Awake();
            player = GetComponent<NetPlayer>();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            var cam = Camera.current;
            if (cam)
            {
                var pos = transform.position + offset;

                var screenPos = cam.WorldToScreenPoint(pos);
                var camTransform = cam.transform;
                var camPosition = camTransform.position;
                var dir = pos - camPosition;
                if (Vector3.Dot(camTransform.forward, dir) > 0)
                {
                    //非当前玩家时计算遮挡
                    if (!player.isLocalPlayer && Physics.Raycast(camPosition, dir, out RaycastHit hitInfo, dir.magnitude))
                    {

                    }
                    else
                    {
                        var nickName = RichText.AddColor(player.nickName, player.isLocalPlayer ? localPlayerTextColor : playerTextColor);
                        var content = CommonFun.TempContent(nickName);
                        var style = this.style.style;
                        var size = style.CalcSize(content);

                        GUI.Button(new Rect(screenPos.x - size.x / 2, Screen.height - screenPos.y - size.y / 2, size.x, size.y), content, style);
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            window.visable = false;
            //style = new GUIStyle(GUI.skin.box);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            NetChat.onReceived += OnReceived;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            NetChat.onReceived -= OnReceived;
        }

        #region 玩家聊天信息

        [Group("玩家聊天信息")]
        [Name("显示聊天信息")]
        public bool displayChatInfo = true;

        [Name("聊天信息偏移量")]
        [Tip("玩家聊天信息框的偏移量", "Offset of player chat message box")]
        public Vector3 chatInfoOffset = new Vector3(0, 1.5f, 0);

        [Name("聊天信息偏移量类型")]
        [EnumPopup]
        public ECoordinateType chatInfoOffsetType = ECoordinateType.World;

        [Name("聊天信息生命时间")]
        [Tip("聊天信息生命时间；单位：秒；", "Chat information life time; Unit: second;")]
        public float chatInfoLifeTime = 5;

        [Name("聊天信息文本锚点")]
        [Tip("聊天信息文本锚点", "Chat message text anchor")]
        public TextAnchor chatInfoTextAnchor = TextAnchor.MiddleCenter;

        private void OnReceived(NetChat netChat, List<ChatInfo> chatInfos)
        {
            if (!displayChatInfo || !netChat || chatInfos == null || chatInfos.Count == 0) return;
            foreach (var info in chatInfos)
            {
                if (info == null) continue;

                var textBuddle = GUIManager.CreateTextBuddle(info.contentType.ToString(), Vector3.zero, ECoordinateType.GUI, chatInfoLifeTime, chatInfoTextAnchor);
                if (textBuddle != null)
                {
                    textBuddle.tag = info;
                    textBuddle.onGUI += this.OnGUIOfTextBuddle;
                    textBuddle.onDead += this.OnDeadOfTextBuddle;
                }
            }
        }

        private bool OnGUIOfTextBuddle(TextBuddle textBuddle)
        {
            if (textBuddle.tag is ChatInfo chatInfo)
            {
                var player = chatInfo.player;
                if (player)
                {
                    var position = player.transform.position;
                    switch (chatInfoOffsetType)
                    {
                        case ECoordinateType.GUI:
                            {
                                position = CommonFun.ConvertToGUIPoint(position, ECoordinateType.World) + chatInfoOffset;
                                break;
                            }
                        case ECoordinateType.Screen:
                            {
                                position = CommonFun.ConvertToGUIPoint(position, ECoordinateType.World) + CommonFun.ConvertToGUIPoint(chatInfoOffset, ECoordinateType.Screen);
                                break;
                            }
                        case ECoordinateType.ViewPort:
                            {
                                position = CommonFun.ConvertToGUIPoint(position, ECoordinateType.World) + CommonFun.ConvertToGUIPoint(chatInfoOffset, ECoordinateType.ViewPort);
                                break;
                            }
                        case ECoordinateType.World:
                        default:
                            {
                                position = CommonFun.ConvertToGUIPoint(position + chatInfoOffset, ECoordinateType.World);
                                break;
                            }
                    }
                    switch (chatInfo.contentType)
                    {
                        case EContentType.Voice:
                            {
#if UNITY_WEBGL && !UNITY_EDITOR
                                var text = string.Format("[Voice {0:0.00}]", chatInfo.audioLength);
#else
                                var text = string.Format("[语音 {0:0.00}]", chatInfo.audioLength);
#endif
                                var style = GUI.skin.button;
                                Vector2 size = style.CalcScreenSize(style.CalcSize(new GUIContent(text)));
                                Rect rect = new Rect(position.x - size.x / 2, position.y - size.y, size.x, size.y);

                                if (textBuddle.lifeTime < 1)
                                {
                                    Color backgroundColor = GUI.backgroundColor;
                                    Color bc = backgroundColor;
                                    bc.a = textBuddle.lifeTime;
                                    GUI.backgroundColor = bc;
                                    if (GUI.Button(rect, text, style))
                                    {
                                        chatInfo.PlayVoice();
                                    }
                                    GUI.backgroundColor = backgroundColor;
                                }
                                else
                                {
                                    if (GUI.Button(rect, text, style))
                                    {
                                        chatInfo.PlayVoice();
                                    }
                                }

                                return true;
                            }
                        case EContentType.Text:
                        default:
                            {
                                textBuddle.position = position;
                                textBuddle.text = chatInfo.text;
                                break;
                            }
                    }
                }
            }
            return false;
        }

        private void OnDeadOfTextBuddle(TextBuddle textBuddle)
        {
            textBuddle.onGUI -= this.OnGUIOfTextBuddle;
            textBuddle.onDead -= this.OnDeadOfTextBuddle;
        }

        #endregion
    }
}
