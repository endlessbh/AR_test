using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 视频播放器属性设置:控制视频播放器的播放、暂停、循环和设置视频剪辑等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VideoPlayerPropertySet))]
    [Tip("控制视频播放器的播放、暂停、循环和设置视频剪辑等", "Control the playback, pause, cycle and set video clips of the video player")]
    public class VideoPlayerPropertySet : ComponentPropertySet<VideoPlayerPropertySet, VideoPlayer>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "视频播放器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(VideoPlayerPropertySet))]
        [Tip("控制视频播放器的播放、暂停、循环和设置视频剪辑等", "Control the playback, pause, cycle and set video clips of the video player")]
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
            /// 循环
            /// </summary>
            [Name("循环")]
            Loop,

            /// <summary>
            /// 设置视频剪辑
            /// </summary>
            [Name("设置视频剪辑")]
            SetVedioClip,

            /// <summary>
            /// 速度
            /// </summary>
            [Name("速度")]
            Speed,

            /// <summary>
            /// 进度
            /// </summary>
            [Name("进度")]
            Progress,

            /// <summary>
            /// 播放
            /// </summary>
            [Name("播放")]
            Play = 1000,

            /// <summary>
            /// 暂停
            /// </summary>
            [Name("暂停")]
            Pause,

            /// <summary>
            /// 继续
            /// </summary>
            [Name("继续")]
            Resume,

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
            /// 播放停止切换
            /// </summary>
            [Name("播放停止切换")]
            SwitchPauseOrResume,

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
        public EBoolPropertyValue _loop = new EBoolPropertyValue();

        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.SetVedioClip)]
        [OnlyMemberElements]
        public VideoClipPropertyValue _vedioClip = new VideoClipPropertyValue();

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度")]
        [Tip("速度值为大于等于0的数", "The speed value is a number greater than or equal to 0")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Speed)]
        [OnlyMemberElements]
        public FloatPropertyValue _speed = new FloatPropertyValue();

        /// <summary>
        /// 进度
        /// </summary>
        [Name("进度")]
        [Tip("进度值为0到1之间的数", "Number of progress values between 0 and 1")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Progress)]
        [OnlyMemberElements]
        public FloatPropertyValue _progress = new FloatPropertyValue();

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="videoPlayer"></param>
        protected override void SetComponentProperty(VideoPlayer videoPlayer)
        {
            switch (_propertyName)
            {
                case EPropertyName.Loop:
                    {
                        videoPlayer.isLooping = _loop.GetValue(videoPlayer.isLooping);
                        break;
                    }
                case EPropertyName.SetVedioClip:
                    {
                        videoPlayer.clip = _vedioClip.propertyValue;
                        break;
                    }
                case EPropertyName.Speed:
                    {
                        videoPlayer.playbackSpeed = _speed.propertyValue;
                        break;
                    }
                case EPropertyName.Progress:
                    {
                        videoPlayer.frame = (int)(Mathf.Clamp01(_progress.propertyValue) * videoPlayer.frameCount);
                        break;
                    }
                case EPropertyName.Play:
                    {
                        videoPlayer.Play(); 
                        break;
                    }
                case EPropertyName.Pause:
                    {
                        videoPlayer.Pause();
                        break;
                    }
                case EPropertyName.Resume:
                    {
                        if(videoPlayer.isPaused) videoPlayer.Play();
                        break;
                    }
                case EPropertyName.Stop:
                    {
                        videoPlayer.Stop(); 
                        break;
                    }
                case EPropertyName.SwitchPlayOrStop:
                    {
                        if (videoPlayer.isPlaying)
                        {
                            videoPlayer.Play();
                        }
                        else
                        {
                            videoPlayer.Stop();
                        }
                        break;
                    }
                case EPropertyName.SwitchPauseOrResume:
                    {
                        if (videoPlayer.isPaused)
                        {
                            videoPlayer.Play();
                        }
                        else
                        {
                            videoPlayer.Pause();
                        }
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
