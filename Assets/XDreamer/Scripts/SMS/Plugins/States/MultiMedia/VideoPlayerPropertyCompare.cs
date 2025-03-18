using UnityEngine.Video;
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
    /// 视频播放器属性比较:比较视频进度
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VideoPlayerPropertyCompare))]
    [Tip("比较视频是否播放，循环、进度和完成播放", "Compare whether the video is played, cycle, progress and completion")]
    public class VideoPlayerPropertyCompare : BasePropertyCompare<VideoPlayerPropertyCompare>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "视频播放器属性比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(VideoPlayerPropertyCompare))]
        [Tip("比较视频进度", "Compare video progress")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 视频播放器
        /// </summary>
        [Name("视频播放器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public VideoPlayer _videoPlayer;

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
            IsPlaying,

            /// <summary>
            /// 循环
            /// </summary>
            [Name("循环")]
            Loop,

            /// <summary>
            /// 进度
            /// </summary>
            [Name("进度")]
            [Tip("此处无法准确判断视频是否播放完成，因为可播放帧可能小于总帧数，进度=当前可播放帧/总帧数", "It is impossible to accurately judge whether the video has been played, because the playable frames may be less than the total frames, and the progress = current playable frames / total frames")]
            Progress,

            /// <summary>
            /// 播放完成
            /// </summary>
            [Name("播放完成")]
            PlayFinished,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.Progress;

        /// <summary>
        /// 播放
        /// </summary>
        [Name("播放")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.IsPlaying)]
        [OnlyMemberElements]
        public EBoolPropertyValue _isPlaying = new EBoolPropertyValue();

        /// <summary>
        /// 循环
        /// </summary>
        [Name("循环")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Loop)]
        [OnlyMemberElements]
        public EBoolPropertyValue _loop = new EBoolPropertyValue();

        /// <summary>
        /// 进度
        /// </summary>
        [Name("进度")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Progress)]
        public FloatValueTrigger _progress = new FloatValueTrigger();

        /// <summary>
        /// 完成一次
        /// </summary>
        [Name("完成一次")]
        [Tip("勾选时：条件成立立即完成；不勾选时：成立条件随着播放进度值不断变化", "勾选时：条件成立立即完成；不勾选时：成立条件随着播放进度值不断变化")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Progress)]
        public bool finishOnce = true;

        private bool orgIsPlaying = false;
        private bool orgLoop = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            if (_videoPlayer)
            {
                _videoPlayer.loopPointReached += OnPlayFinished;

                orgIsPlaying = _videoPlayer.isPlaying;
                orgLoop = _videoPlayer.isLooping;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            if (_videoPlayer)
            {
                _videoPlayer.loopPointReached -= OnPlayFinished;
            }

            base.OnExit(stateData);
        }

        /// <summary>
        /// 播放完成
        /// </summary>
        /// <param name="videoPlayer"></param>
        private void OnPlayFinished(VideoPlayer videoPlayer)
        {
            if (_propertyName == EPropertyName.PlayFinished)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (!DataValidity())
            {
                return;
            }

            switch (_propertyName)
            {
                case EPropertyName.IsPlaying:
                    {
                        switch (_isPlaying.propertyValue)
                        {
                            case XCSJ.Scripts.EBool.Yes:
                                {
                                    finished = _videoPlayer.isPlaying;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.No:
                                {
                                    finished = !_videoPlayer.isPlaying;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.Switch:
                                {
                                    finished = _videoPlayer.isPlaying != orgIsPlaying;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.None:
                                {
                                    finished = true;
                                    break;
                                }
                        }
                        break;
                    }
                case EPropertyName.Loop:
                    {
                        switch (_loop.propertyValue)
                        {
                            case XCSJ.Scripts.EBool.Yes:
                                {
                                    finished = _videoPlayer.isLooping;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.No:
                                {
                                    finished = !_videoPlayer.isLooping;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.Switch:
                                {
                                    finished = _videoPlayer.isLooping != orgLoop;
                                    break;
                                }
                            case XCSJ.Scripts.EBool.None:
                                {
                                    finished = true;
                                    break;
                                }
                        }
                        break;
                    }
                case EPropertyName.Progress:
                    {
                        if (finishOnce && finished)
                        {
                            return;
                        }
                        finished = _progress.IsTrigger((float)(_videoPlayer.frame * 1D / _videoPlayer.frameCount));
                        break;
                    }
            }
        }
        
        public override bool DataValidity()
        {
            return _videoPlayer;
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