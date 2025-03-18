using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 语音聊天
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [DisallowMultipleComponent]
    [Name("语音聊天")]
    [RequireComponent(typeof(NetChat), typeof(AudioSource))]
    [RequireManager(typeof(MMOManager))]
    public class VoiceChat : InteractProvider, IAwake, IStart, IUpdate
    {
        private NetChat netChat;
        private AudioSource audioSource;

        [NonSerialized]
        [Readonly]
        [Name("音频剪辑")]
        private AudioClip micRecord;

        [NonSerialized]
        [Readonly]
        [Name("设备名")]
        public string device = "";

        [Name("循环")]
        [Tip("标识在达到音频时长时是否应继续录制，并从音频剪辑的开头开始环绕和录制；", "Identify whether recording should continue when the audio duration is reached, and surround and record from the beginning of the audio clip;")]
        public bool loop = true;

        [Name("音频时长")]
        [Tip("期望录制的音频最大时长", "Maximum audio duration expected to be recorded")]
        public int lengthSec = 15;

        [Name("音频采样率")]
        public int frequency = 44100;

        [Name("自动播放")]
        [Tip("收到语音聊天时，是否自动播放语音", "Whether to play voice automatically when receiving voice chat")]
        public bool autoPlay = true;

        [Name("待播放聊天信息")]
        public List<ChatInfo> playChatInfos = new List<ChatInfo>();

        public void OnReceiveChatInfo(List<ChatInfo> chatInfos)
        {
            foreach(var info in chatInfos)
            {
                if(info.contentType == EContentType.Voice)
                {
                    playChatInfos.Add(info);
                }
            }
        }

        public void OnSyncDisable()
        {
            playChatInfos.Clear();
        }

        public bool support => !string.IsNullOrEmpty(device);

        public void Awake()
        {
            if (!netChat)
            {
                netChat = GetComponent<NetChat>();
            }
            if (!audioSource)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        public void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            //获取设备麦克风
            var devices = Microphone.devices;
            if (devices == null || devices.Length == 0)
            {
                Log.Warning("当前设备上未找到有效的麦克风！");
                device = "";
            }
            else
            {
                device = devices[0];
            }
#endif
        }

        public void Update()
        {
            if (autoPlay && playChatInfos.Count > 0 && audioSource && !audioSource.isPlaying)
            {
                PlayVoice(playChatInfos[0]);
                playChatInfos.RemoveAt(0);
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
#else
        /// <summary>
        /// 音频数据长度；单位：Float4字节；
        /// </summary>
        private int audioDataLength = 0;
#endif

        public void StartVoice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (support && !Microphone.IsRecording(device))
            {
                audioDataLength = 0;
                micRecord = Microphone.Start(device, loop, lengthSec, frequency);
            }
#endif
        }

        public void StopVoice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (IsVoiceRecording())
            {
                audioDataLength = Microphone.GetPosition(device);
                Microphone.End(device);
            }
#endif
        }

        public void SendVoice()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
#else
            if (support && micRecord && !Microphone.IsRecording(device) && audioDataLength > 0)
            {
                var datas = new float[audioDataLength];

                micRecord.GetData(datas, 0);

                micRecord = AudioClip.Create(nameof(VoiceChat), audioDataLength, micRecord.channels, micRecord.frequency, false);
                micRecord.SetData(datas, 0);
                
                netChat.Send(micRecord);

                var Bytes = audioDataLength * 4;
                Debug.LogFormat("SendVoice: {0}, {1}, {2} s", Bytes, ByteHelper.ToDescription0(Bytes), micRecord.length);

                micRecord = null;
            }
#endif
        }

        public bool IsVoiceRecording()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return false;
#else
            return support && micRecord && Microphone.IsRecording(device);
#endif
        }

        public void PlayVoice(ChatInfo chatInfo)
        {
            if (audioSource && chatInfo != null)
            {
                audioSource.clip = chatInfo.audioClip;
                audioSource.Play();
            }
        }
    }
}
