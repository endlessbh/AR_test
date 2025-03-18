using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [DisallowMultipleComponent]
    [Name("网络聊天")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public sealed class NetChat : NetMB, IAwake
    {
        public const int Max = 256;

        [Name("退出房间时清空聊天记录")]
        public bool clearOnQuitRoom = true;

        [Name("聊天信息列表")]
        public List<ChatInfo> chatInfos = new List<ChatInfo>();

        [Name("最大数目")]
        [Tip("-1表示不限制聊天记录的数目；否者超过对应数目后，最早的聊天记录会被移除；", "-1 means unlimited number of chat records; If not, the earliest chat record will be removed after exceeding the corresponding number;")]
        [Range(-1, NetChat.Max)]
        public int maxCount = NetChat.Max;

        [Name("自动清理")]
        [Tip("标识是否自动清理离线用户的聊天记录", "Identify whether to automatically clean up chat records of offline users")]
        public bool autoClear = true;

        [Name("输出系统信息")]
        [Tip("输出在MMO网络环境中发生的各种事件信息，包括玩家的进入、退出等", "Output various event information in MMO network environment, including player entry, exit, etc")]
        public bool outputSystemInfo = true;

        #region 聊天的各种回调事件

        /// <summary>
        /// 当收到聊天信息时回调
        /// </summary>
        public static event Action<NetChat, List<ChatInfo>> onReceived;

        /// <summary>
        /// 当移除聊天信息时回调
        /// </summary>
        public static event Action<NetChat, ChatInfo> onRemoved;

        /// <summary>
        /// 当所有聊天信息清空后回调
        /// </summary>
        public static event Action<NetChat> onCleaned;

        #endregion

        #region 基类函数覆盖

        private List<ChatInfo> waitSend = new List<ChatInfo>();

        private Dictionary<int, List<ChatInfo>> waitReceive = new Dictionary<int, List<ChatInfo>>();

        protected override bool OnTimedCheckChange()
        {
            foreach (var kv in waitReceive)
            {
                if (version > kv.Key)
                {
                    //Log.DebugFormat("重发{0}数据{1}", kv.Key, waitReceive.Count);
                    waitSend.AddRange(kv.Value);
                    waitReceive.Remove(kv.Key);
                    break;
                }
            }
            return waitSend.Count > 0;
        }

        protected override string OnSerializeData()
        {
            try
            {
                return JsonHelper.ToJson(waitSend);
            }
            finally
            {
                SetWaitReceive(waitSend);
                waitSend.Clear();
            }
        }

        private void SetWaitReceive(List<ChatInfo> infos)
        {
            if (!waitReceive.TryGetValue(version, out List<ChatInfo> list))
            {
                waitReceive[version] = list = new List<ChatInfo>();
            }
            //Log.DebugFormat("等待{0}数据", version);
            list.AddRange(infos);
        }

        protected override void OnDeserializeData(string data, Data dataObj)
        {
            if (CanOptimizable(dataObj))
            {
                waitReceive.Remove(version);
                //Log.DebugFormat("收到{0}数据", version);
            }

            if (JsonHelper.ToObject<List<ChatInfo>>(data) is List<ChatInfo> list)
            {
                //Debug.Log("OnDeserializeData: " + list.Count);
                RmoveInvalidChatInfo(list, i =>
                {
                    if (autoClear) return !LocalCache.TryGetPlayer(i.userGuid, out var player);
                    i.netChat = this;
                    return false;
                });

                AddChatInfo(list);
                LimitChatInfos();

                if (voiceChat) voiceChat.OnReceiveChatInfo(list);
            }
        }

        private void AddChatInfo(List<ChatInfo> chatInfos)
        {
            this.chatInfos.AddRange(chatInfos);

            //回调
            onReceived?.Invoke(this, chatInfos);
        }

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();
            Clear();
            if (voiceChat) voiceChat.OnSyncDisable();
        }

        public override void OnStop()
        {
            base.OnStop();
            Clear();
        }

        /// <summary>
        /// 移除无效聊天记录
        /// </summary>
        /// <param name="chatInfos"></param>
        /// <param name="invalidFunc"></param>
        private void RmoveInvalidChatInfo(List<ChatInfo> chatInfos, Func<ChatInfo, bool> invalidFunc)
        {
            for (int i = chatInfos.Count - 1; i >= 0; i--)
            {
                var info = chatInfos[i];
                if (invalidFunc(info))
                {
                    //执行回调
                    onRemoved?.Invoke(this, info);

                    //执行删除
                    chatInfos.RemoveAt(i);
                }
            }
        }

        public override void OnPlayerEnter(string guid)
        {
            base.OnPlayerEnter(guid);
            if (outputSystemInfo)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                OutputSystemInfo(string.Format("Player[{0}](GUID:{1}) enter room !", LocalCache.GetPlayer(guid)?.name, guid));
#else
                OutputSystemInfo(string.Format("玩家[{0}](编号:{1})进入房间！", LocalCache.GetPlayer(guid)?.name, guid));
#endif
            }
        }

        public override void OnPlayerExit(string guid)
        {
            base.OnPlayerExit(guid);
            if (outputSystemInfo)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                OutputSystemInfo(string.Format("Player[{0}](GUID:{1}) exit room !", LocalCache.GetPlayer(guid)?.name, guid));
#else
                OutputSystemInfo(string.Format("玩家[{0}](编号:{1})退出房间！", LocalCache.GetPlayer(guid)?.name, guid));
#endif
            }
            if (autoClear) RmoveInvalidChatInfo(chatInfos, i => i.userGuid == guid);
        }

        private void OutputSystemInfo(string text)
        {
            var info = new SystemInfo()
            {
                netChat = this,
                text = text,
            };
            AddChatInfo(new List<ChatInfo>() { info });
        }

        #endregion

        private void Send(ChatInfo chatInfo)
        {
            waitSend.Add(chatInfo);
            LimitChatInfos();
            MarkDirty();
        }

        public bool Send(string text, EContentType contentType = EContentType.Text)
        {
            if (!netActive || !canSync || string.IsNullOrEmpty(text)) return false;
            Send(new ChatInfo()
            {
                userGuid = LocalCache.userGuid,
                contentType = contentType,
                text = text,
            });
            return true;
        }

        public bool Send(byte[] textBytes, EContentType contentType = EContentType.Voice)
        {
            if (!netActive || !canSync || textBytes == null) return false;
            Send(new ChatInfo()
            {
                userGuid = LocalCache.userGuid,
                contentType = contentType,
                textBytes = textBytes,
            });
            return true;
        }

        public bool Send(AudioSource audioSource) => Send(audioSource.clip);

        public bool Send(AudioClip audioClip)
        {
            if (!netActive || !canSync || !audioClip) return false;
            Send(new ChatInfo()
            {
                userGuid = LocalCache.userGuid,
                contentType = EContentType.Voice,
                audioClip = audioClip,
            });
            return true;
        }

        private void ClearChatInfos()
        {
            chatInfos.Clear();
            onCleaned?.Invoke(this);
        }

        public void Clear()
        {
            waitSend.Clear();
            waitReceive.Clear();

            ClearChatInfos();
        }

        private void LimitChatInfos()
        {
            if (maxCount == -1) return;
            var deleteCount = chatInfos.Count - maxCount;
            if (deleteCount <= 0) return;
            if (deleteCount >= chatInfos.Count)
            {
                ClearChatInfos();
                return;
            }
            //执行回调
            for (int i = 0; i < deleteCount; i++)
            {
                onRemoved?.Invoke(this, chatInfos[i]);
            }
            //执行删除
            chatInfos.RemoveRange(0, deleteCount);
        }

        public void Awake()
        {
            if (!voiceChat)
            {
                voiceChat = GetComponent<VoiceChat>();
            }
        }

        #region 语音聊天

        public VoiceChat voiceChat { get; private set; }

        public bool supportVoice => voiceChat && voiceChat.support;

        public void StartVoice()
        {
            if (supportVoice)
            {
                voiceChat.StartVoice();
            }
        }

        public void StopVoice()
        {
            if (supportVoice)
            {
                voiceChat.StopVoice();
            }
        }

        public bool IsVoiceRecording()
        {
            return supportVoice && voiceChat.IsVoiceRecording();
        }

        public void SendVoice()
        {
            if (supportVoice)
            {
                voiceChat.SendVoice();
            }
        }

        public void PlayVoice(ChatInfo chatInfo)
        {
            if (voiceChat)
            {
                voiceChat.PlayVoice(chatInfo);
            }
        }

        #endregion
    }

    /// <summary>
    /// 聊天信息
    /// </summary>
    [Import]
    [Serializable]
    [Name("聊天信息")]
    public class ChatInfo : JsonObject<ChatInfo>
    {
        internal NetChat netChat { get; set; }

        public string userGuid = "";
        public EContentType contentType = EContentType.Text;
        public string text = "";

        internal byte[] _textBytes = null;
        [Json(false)]
        public virtual byte[] textBytes
        {
            get
            {
                if (_textBytes == null)
                {
                    _textBytes = Convert.FromBase64String(text);
                }
                return _textBytes;
            }
            set
            {
                _textBytes = value;
                if (_textBytes != null)
                {
                    text = Convert.ToBase64String(_textBytes);
                }
            }
        }

        internal AudioClip _audioClip;
        [Json(false)]
        public AudioClip audioClip
        {
            get
            {
                if (!_audioClip)
                {
                    _audioClip = AudioClipHelper.LoadFromMemory(textBytes);
                }
                return _audioClip;
            }
            set
            {
                _audioClip = value;
                if (_audioClip)
                {
                    textBytes = _audioClip.EncodeToWAV();
                }
            }
        }

        [Json(false)]
        public float audioLength
        {
            get
            {
                return audioClip ? audioClip.length : 0;
            }
        }

        internal string _userName = "";
        [Json(false)]
        public virtual string userName
        {
            get
            {
                if (LocalCache.TryGetPlayer(userGuid, out var user))
                {
                    return _userName = user.nickName ?? user.name;
                }
                return InvalidUserName + _userName;
            }
        }

        [Json(false)]
#if UNITY_WEBGL && !UNITY_EDITOR
        public const string InvalidUserName = "<color=#FF0000FF>[Offline User]</color>";
#else
        public const string InvalidUserName = "<color=#FF0000FF>[离线用户]</color>";
#endif

        internal NetPlayer _player;
        [Json(false)]
        public NetPlayer player
        {
            get
            {
                if (!_player)
                {
                    _player = LocalCache.GetPlayer(userGuid)?.netPlayer as NetPlayer;
                }
                return _player;
            }
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        public void PlayVoice()
        {
            if (contentType == EContentType.Voice && netChat)
            {
                netChat.PlayVoice(this);
            }
        }
    }

    /// <summary>
    /// 系统信息
    /// </summary>
    [Import]
    [Serializable]
    [Name("系统信息")]
    public class SystemInfo : ChatInfo
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        public override string userName => "<color=#00FF00FF>[System]</color>";
#else
        public override string userName => "<color=#00FF00FF>[系统]</color>";
#endif
    }

    /// <summary>
    /// 内容类型
    /// </summary>
    [Name("内容类型")]
    public enum EContentType
    {
        Text = 0,
        Voice,
    }
}
