using System;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 音频源属性设置:控制音频源的播放、暂停、循环、设置音频剪辑、静音、音量和高音等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(AudioSourcePropertySet))]
    [Tip("控制音频源的播放、暂停、循环、设置音频剪辑、静音、音量和高音等", "Control the playback, pause and cycle of audio source, set audio clip, mute, volume and treble, etc")]
    public class AudioSourcePropertySet : ComponentPropertySet<AudioSourcePropertySet, AudioSource>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "音频源属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(AudioSourcePropertySet))]
        [Tip("控制音频源的播放、暂停、循环、设置音频剪辑、静音、音量和高音等", "Control the playback, pause and cycle of audio source, set audio clip, mute, volume and treble, etc")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 播放
            /// </summary>
            [Name("播放")]
            Play,

            /// <summary>
            /// 停止
            /// </summary>
            [Name("停止")]
            Stop,

            /// <summary>
            /// 播放停止切换
            /// </summary>
            [Name("播放停止切换")]
            SwitchPlayOrStop,

            /// <summary>
            /// 循环
            /// </summary>
            [Name("循环")]
            Loop,

            /// <summary>
            /// 设置视频剪辑
            /// </summary>
            [Name("设置视频剪辑")]
            SetAudioClip,

            /// <summary>
            /// 静音
            /// </summary>
            [Name("静音")]
            Mute,

            /// <summary>
            /// 音量
            /// </summary>
            [Name("音量")]
            Volume,

            /// <summary>
            /// 高音
            /// </summary>
            [Name("高音")]
            Pitch,

            /// <summary>
            /// 暂停
            /// </summary>
            [Name("暂停")]
            Pause,

            /// <summary>
            /// 继续
            /// </summary>
            [Name("继续")]
            UnPause,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.Play;

        /// <summary>
        /// 循环
        /// </summary>
        [Name("循环")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Loop)]
        [OnlyMemberElements]
        public EBoolPropertyValue _loopPropertyValue = new EBoolPropertyValue();

        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.SetAudioClip)]
        [OnlyMemberElements]
        public AudioClipPropertyValue _audioClipPropertyValue = new AudioClipPropertyValue();

        /// <summary>
        /// 静音
        /// </summary>
        [Name("静音")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Mute)]
        [OnlyMemberElements]
        public EBoolPropertyValue _mutePropertyValue = new EBoolPropertyValue();

        /// <summary>
        /// 音量
        /// </summary>
        [Name("音量")]
        [Tip("音量大小(0,1)", "Volume size (0,1)")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Volume)]
        [OnlyMemberElements]
        public FloatPropertyValue _volumePropertyValue = new FloatPropertyValue();

        /// <summary>
        /// 高音
        /// </summary>
        [Name("高音")]
        [Tip("音量大小(-3,3)", "Volume (- 3,3)")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Pitch)]
        [OnlyMemberElements]
        public FloatPropertyValue _pitchPropertyValue = new FloatPropertyValue();

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="audioSource"></param>
        protected override void SetComponentProperty(AudioSource audioSource)
        {
            switch (_propertyName)
            {
                case EPropertyName.Play:
                    {
                        audioSource.Play();
                        break;
                    }
                case EPropertyName.Stop:
                    {
                        audioSource.Stop();
                        break;
                    }
                case EPropertyName.SwitchPlayOrStop:
                    {
                        if (audioSource.isPlaying)
                        {
                            audioSource.Play();
                        }
                        else
                        {
                            audioSource.Stop();
                        }
                        break;
                    }
                case EPropertyName.Loop:
                    {
                        audioSource.loop = _loopPropertyValue.GetValue(audioSource.loop);
                        break;
                    }
                case EPropertyName.SetAudioClip:
                    {
                        audioSource.clip = _audioClipPropertyValue.propertyValue;
                        break;
                    }
                case EPropertyName.Mute:
                    {
                        audioSource.mute = _mutePropertyValue.GetValue(audioSource.mute);
                        break;
                    }
                case EPropertyName.Volume:
                    {
                        audioSource.volume = _volumePropertyValue.propertyValue;
                        break;
                    }
                case EPropertyName.Pitch:
                    {
                        audioSource.pitch = _pitchPropertyValue.propertyValue;
                        break;
                    }
                case EPropertyName.Pause:
                    {
                        audioSource.Pause();
                        break;
                    }
                case EPropertyName.UnPause:
                    {
                        audioSource.UnPause();
                        break;
                    }
            }
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_propertyName);
        }
    }
}
