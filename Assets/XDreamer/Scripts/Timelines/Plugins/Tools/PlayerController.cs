using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 播放器控制器:主要关联界面的主播放器，可播放内容不是自播放均使用当前对象播放
    /// </summary>
    [Name("播放器控制器")]
    [RequireManager(typeof(TimelineManager))]
    public sealed class PlayerController : PlayableContentHostPlayer
    {
        /// <summary>
        /// 预制播放内容
        /// </summary>
        [Name("预制播放内容")]
        public PlayableContent _playableContent;

        /// <summary>
        /// 使用播放内容时长
        /// </summary>
        [Name("使用播放内容时长")]
        public bool _usePlayableContentTimeLength = true;

        /// <summary>
        /// 当前播放内容
        /// </summary>
        [Name("当前播放内容")]
        [Readonly]
        public PlayableContent currentPlayableContent;

        /// <summary>
        /// 已加载内容对象
        /// </summary>
        public override IPlayableContent loadedContent 
        {
            get => base.loadedContent;
            protected set
            {
                base.loadedContent = value;
                currentPlayableContent = value as PlayableContent;
            }
        } 

        /// <summary>
        /// 是否循环
        /// </summary>
        public override bool loop
        {
            get => hostPlayer != null ? hostPlayer.loop : base.loop;
            set
            {
                if (hostPlayer != null)
                {
                    hostPlayer.loop = value;
                }
                else
                {
                    base.loop = value;
                }
            }
        }

        /// <summary>
        /// 当前速度
        /// </summary>
        public override double speed
        {
            get => hostPlayer != null ? hostPlayer.speed : base.speed;
            set
            {
                if (hostPlayer != null)
                {
                    hostPlayer.speed = value;
                }
                else
                {
                    base.speed = value;
                }
            }
        }

        /// <summary>
        /// 百分比
        /// </summary>
        public override double percent
        {
            get => hostPlayer != null ? hostPlayer.percent : base.percent;
            set
            {
                // 设定值与当前值相同
                if (MathX.Approximately(value, percent)) return;

                if (hostPlayer != null)
                {
                    hostPlayer.percent = value;
                }
                else
                {
                    base.percent = value;
                }
            }
        }

        /// <summary>
        /// 当前时长
        /// </summary>
        public override double time
        {
            get
            {
                return hostPlayer != null ? hostPlayer.time : base.time;
            }
            set
            {
                if (hostPlayer != null)
                {
                    hostPlayer.time = value;
                }
                else
                {
                    base.time = value;
                }
            }
        }

        /// <summary>
        /// 总播放时间长
        /// </summary>
        public override double timeLength
        {
            get
            {
                if (hostPlayer != null)
                {
                    return hostPlayer.timeLength;
                }
                if (_usePlayableContentTimeLength && currentPlayableContent)
                {
                    return currentPlayableContent.timeLength;
                }
                return base.timeLength;
            }
            set
            {
                if (hostPlayer != null)
                {
                    hostPlayer.timeLength = value;
                }
                else
                {
                    base.timeLength = value;
                }
            }
        }

        /// <summary>
        /// 开启受控
        /// </summary>
        /// <param name="playableContentHost"></param>
        public void BeginControlled(IPlayableContentHost playableContentHost)
        {
            if (host == null)
            {
                host = playableContentHost;
            }
        }

        /// <summary>
        /// 结束受控
        /// </summary>
        /// <param name="playableContentHost"></param>
        public void EndControlled(IPlayableContentHost playableContentHost)
        {
            if (host == playableContentHost)
            {
                host = null;
            }
        }

        /// <summary>
        /// 缺省加载内容
        /// </summary>
        protected override IPlayableContent loadContentOnEnable => _playableContent;

        /// <summary>
        /// 播放内容：如果被宿主控制，当前对象被播放器界面调用也反向控制宿主播放内容
        /// </summary>
        /// <returns></returns>
        public override bool Play() => hostPlayer != null ? hostPlayer.Play() : base.Play();

        /// <summary>
        /// 暂停播放：如果被宿主控制，当前对象被播放器界面调用也反向控制宿主暂停播放
        /// </summary>
        /// <returns></returns>
        public override bool Pause() => hostPlayer != null ? hostPlayer.Pause() : base.Pause();

        /// <summary>
        /// 继续播放：如果被宿主控制，当前对象被播放器界面调用也反向控制宿主继续播放
        /// </summary>
        /// <returns></returns>
        public override bool Resume() => hostPlayer != null ? hostPlayer.Resume() : base.Resume();

        /// <summary>
        /// 停止播放：如果被宿主控制，当前对象被播放器界面调用也反向控制宿主停止播放
        /// </summary>
        /// <returns></returns>
        public override bool Stop() => hostPlayer != null ? hostPlayer.Stop() : base.Stop();

        /// <summary>
        /// 暂停或播放：如果被宿主控制，当前对象被播放器界面调用也反向控制宿主暂停或播放
        /// </summary>
        public override bool ResumeOrPlay()
        {
            if (hostPlayer != null)
            {
                return hostPlayer.playerState == EPlayerState.Pause ? hostPlayer.Resume() : hostPlayer.Play();
            }
            else
            {
                return base.ResumeOrPlay();
            }
        }

        /// <summary>
        /// 播放内容
        /// </summary>
        /// <param name="playableContent"></param>
        public void PlayContent(IPlayableContent playableContent)
        {
            if (playableContent == null) return;

            if (isLoaded)
            {
                if (loadedContent != playableContent)
                {
                    // 停止旧内容
                    Stop();

                    // 卸载旧内容，加载并播放新内容
                    Unload(loadedContent, host, isUnload => LoadAndPlay(playableContent, host));
                }
                else
                {
                    ResumeOrPlay();
                }
            }
            else
            {
                LoadAndPlay(playableContent, host);
            }
        }

        #region 界面

        /// <summary>
        /// 播放时间格式
        /// </summary>
        [Name("播放时间格式")]
        [EnumPopup]
        public EPlayerTimeFormat _playerTimeFormat = EPlayerTimeFormat.hh__mm__ss;

        /// <summary>
        /// 是否播放
        /// </summary>
        public bool isPlaying
        {
            get => IsPlaying();
            set
            {
                if (value)
                {
                    ResumeOrPlay();
                }
                else
                {
                    Pause();
                }
            }
        }

        /// <summary>
        /// 当前时间（格式）
        /// </summary>
        public string currentFormatTime => TimelineHelper.ConvertPlayerTimeFormat(time, _playerTimeFormat);

        /// <summary>
        /// 总时间（格式）
        /// </summary>
        public string totalFormatTime => TimelineHelper.ConvertPlayerTimeFormat(timeLength, _playerTimeFormat);

        #endregion
    }
}
