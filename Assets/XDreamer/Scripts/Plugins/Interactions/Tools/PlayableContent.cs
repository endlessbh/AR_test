using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 播放命令枚举
    /// </summary>
    public enum EPlayableCmd
    {
        [Name("播放")]
        Play,

        [Name("停止")]
        Stop,

        [Name("暂停")]
        Pause,

        [Name("继续")]
        Resume,

        [Name("加载内容")]
        LoadContent,

        [Name("卸载内容")]
        UnloadContent,
    }

    /// <summary>
    /// 播放命令
    /// </summary>
    [Serializable]
    public class PlayableCmd : Cmd<EPlayableCmd> { }

    /// <summary>
    /// 播放命令列表
    /// </summary>
    [Serializable]
    public class PlayableCmds : Cmds<EPlayableCmd, PlayableCmd> { }

    /// <summary>
    /// 可播放内容
    /// 1、定义动画数据、工作曲线、循环特征等数据，并计算和设置百分比
    /// 2、可作为播放内容进行被加载、被卸载，被播放、被停止、被暂停和被恢复
    /// 3、可执行加载内容、卸载内容，播放、停止、继续和恢复命令
    /// </summary>
    public abstract class PlayableContent : WorkClipInteractor, IPlayableContent, IContentPlayer
    {
        /// <summary>
        /// 播放命令列表
        /// </summary>
        [Name("播放命令列表")]
        [OnlyMemberElements]
        public PlayableCmds _playableCmds = new PlayableCmds();

        /// <summary>
        /// 启用时播放
        /// </summary>
        [Name("启用时播放")]
        public bool _playOnEnable = true;

        /// <summary>
        /// 命令
        /// </summary>
        public override List<string> cmds => _playableCmds.cmdNames;

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public virtual void Reset() => _playableCmds.Reset();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            ResetPlayData();
            Load(loadContentOnEnable, this as IPlayableContentHost, isLoad =>
            {
                if (_playOnEnable) Play();
            });
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Unload(loadedContent, this as IPlayableContentHost);
        }

        /// <summary>
        /// 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
        /// </summary>
        protected virtual void Update()
        {
            if (isControlled) return;

            switch (playerState)
            {
                case EPlayerState.Play:
                case EPlayerState.Resume:
                    {
                        playerState = EPlayerState.Playing;
                        break;
                    }
                case EPlayerState.Playing:
                    {
                        _timeCounter += Time.deltaTime * speed;
                        percent = _timeCounter / timeLength;
                        break;
                    }
            }
        }

        #region 可播放内容功能  

        /// <summary>
        /// 当内容加载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnLoad(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当内容卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnUnload(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPlay(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPause(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnResume(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnStop(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 播放进度
        /// </summary>
        public virtual double percent
        {
            get => percentData.percent01;
            set
            {
                SetPercentInternel(value);

                if (!isControlled)// 自主播放时外部UI会当前对象时间
                {
                    _timeCounter = timeLength * value;
                }
            }
        }

        private void SetPercentInternel(double inPercent)
        {
            OnSetPercent(percentData.Update(inPercent), new PlayableData(this, this));

            if (isControlled) return;

            if (inPercent >= 0)
            {
                switch (loopType)
                {
                    case ELoopType.None:
                        {
                            if (percentData.percent >= 1 || MathX.Approximately(percentData.percent01, 1) || MathX.ApproximatelyZero(timeLength))
                            {
                                playerState = EPlayerState.Finished;
                            }
                            break;
                        }
                    case ELoopType.Loop:
                    case ELoopType.PingPong:
                        {
                            if (MathX.ApproximatelyZero(timeLength))
                            {
                                playerState = EPlayerState.Finished;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 设置内容百分比回调
        /// </summary>
        /// <param name="percent"></param>
        public virtual void OnSetPercent(Percent percent, PlayableData playableData) 
        {
            // 当播放内容不等于自身时更新
            if (loadedContent!=null && loadedContent != (IPlayableContent)this)
            {
                loadedContent.percent = percent.percent01OfWorkCurve;
            }
        }

        /// <summary>
        /// 被播放器调用，处理越界发生时的百分比
        /// </summary>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="playableData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        public virtual void OnOutOfBounds(EOutOfBoundsMode outOfBoundsMode, double percent, PlayableData playableData, double lastPercent)
        {
            switch (outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        SetPercentInternel(0);
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        SetPercentInternel(1);
                        break;
                    }
            }
        }

        #endregion

        #region 播放器功能

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (interactData is PlayableData playableData && playableData.playableContent != null && _playableCmds.TryGetECmd(playableData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EPlayableCmd.LoadContent: return !isLoaded;
                    case EPlayableCmd.UnloadContent: return isLoaded;
                    case EPlayableCmd.Play:
                        {
                            return !MathX.ApproximatelyZero(timeLength) && !isControlled && (playerState == EPlayerState.None || playerState == EPlayerState.LoadContent || playerState == EPlayerState.LoadedContent || playerState == EPlayerState.Free || playerState == EPlayerState.Stop || playerState == EPlayerState.Finished);
                        }
                    case EPlayableCmd.Stop: return isLoaded && !isControlled;
                    case EPlayableCmd.Pause: return isLoaded && !isControlled && playerState == EPlayerState.Playing;
                    case EPlayableCmd.Resume: return isLoaded && !isControlled && playerState == EPlayerState.Pause;
                }
            }

            return false;
        }

        /// <summary>
        /// 执行交互
        /// </summary>
        /// <param name="interactData"></param>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (interactData is PlayableData playableData && _playableCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EPlayableCmd.LoadContent:
                        {
                            playerState = EPlayerState.LoadContent;

                            var rs = playableData.playableContent.OnLoad(playableData);
                            switch (rs)
                            {
                                case EInteractResult.Finished:
                                    {
                                        loadedContent = playableData.playableContent;
                                        playableData.playableContent.host = playableData.playableContentHost;
                                        playerState = EPlayerState.LoadedContent;
                                        playableData.onLoad?.Invoke(true);
                                        break;
                                    }
                                case EInteractResult.Aborted:
                                    {
                                        playableData.onLoad?.Invoke(false);
                                        break;
                                    }
                            }
                            return rs;
                        }
                    case EPlayableCmd.UnloadContent:
                        {
                            playerState = EPlayerState.UnloadContent;

                            var rs = playableData.playableContent.OnUnload(playableData);
                            switch (rs)
                            {
                                case EInteractResult.Finished:
                                    {
                                        loadedContent = null;
                                        playableData.playableContent.host = null;
                                        playerState = EPlayerState.UnloadedContent;
                                        playableData.onUnload?.Invoke(true);
                                        break;
                                    }
                                case EInteractResult.Aborted:
                                    {
                                        playableData.onUnload?.Invoke(false);
                                        break;
                                    }
                            }
                            return rs;
                        }
                    case EPlayableCmd.Play:
                        {
                            if (isLoaded)
                            {
                                playerState = EPlayerState.Play;
                                ResetPlayData();
                                return playableData.playableContent.OnPlay(playableData);
                            }
                            else
                            {
                                if (playerState != EPlayerState.LoadContent)
                                {
                                    Load(playableData.playableContent); 
                                }
                                return EInteractResult.Wait;
                            }
                        }
                    case EPlayableCmd.Stop:
                        {
                            playerState = EPlayerState.Stop;

                            var rs = playableData.playableContent.OnStop(playableData);
                            if (rs == EInteractResult.Finished)
                            {
                                playerState = EPlayerState.Free;
                            }
                            return rs;
                        }
                    case EPlayableCmd.Pause:
                        {
                            playerState = EPlayerState.Pause;

                            return playableData.playableContent.OnPause(playableData);
                        }
                    case EPlayableCmd.Resume:
                        {
                            playerState = EPlayerState.Resume;

                            return playableData.playableContent.OnResume(playableData);
                        }
                }
            }

            return EInteractResult.Aborted;
        }

        /// <summary>
        /// 创建交互数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override InteractData CreateInteractData(string cmdName, params InteractObject[] interactables)
        {
            return new PlayableData(cmdName, this, this, this);
        }

        /// <summary>
        /// 播放宿主：自播放时，对象为空
        /// </summary>
        public IPlayableContentHost host 
        {
            get => _host;
            set
            {
                // 不允许自己作为自己宿主
                if (this is IPlayableContentHost h && h == value) return;

                _host = value;
            }
        }
        private IPlayableContentHost _host;

        /// <summary>
        /// 内容播放器
        /// </summary>
        public IContentPlayer player => this;

        /// <summary>
        /// 宿主播放器
        /// </summary>
        public IContentPlayer hostPlayer => host as IContentPlayer;

        /// <summary>
        /// 外部控制播放
        /// </summary>
        public bool isControlled => host != null;

        /// <summary>
        /// 播放状态
        /// </summary>
        public virtual EPlayerState playerState
        {
            get => isControlled ? EPlayerState.BeganControlled : _playerState;
            protected set => _playerState = value;
        }
        private EPlayerState _playerState = EPlayerState.None;

        /// <summary>
        /// 是否已加载内容
        /// </summary>
        public bool isLoaded => loadedContent != null;

        /// <summary>
        /// 已加载内容对象
        /// </summary>
        public virtual IPlayableContent loadedContent { get; protected set; } = null;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected void ResetPlayData()
        {
            _timeCounter = 0;
            percent = 0;
        }

        private double _timeCounter = 0;

        /// <summary>
        /// 当前时间
        /// </summary>
        public virtual double time { get => _timeCounter; set => _timeCounter = value; }

        /// <summary>
        /// 播放速度
        /// </summary>
        public virtual double speed { get; set; } = 1;

        /// <summary>
        /// 缺省加载播放内容
        /// </summary>
        protected virtual IPlayableContent loadContentOnEnable => this;

        /// <summary>
        /// 加载可播放内容
        /// </summary>
        /// <param name="host"></param>
        public void Load(IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> onLoad = null)
        {
            TryInteract(EPlayableCmd.LoadContent, playableContent, host, onLoad, true);
        }

        /// <summary>
        /// 卸载可播放内容
        /// </summary>
        /// <param name="onUnload"></param>
        public void Unload(IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> onUnload = null)
        {
            TryInteract(EPlayableCmd.UnloadContent, playableContent, host, onUnload, false);
        }

        /// <summary>
        /// 是否播放中
        /// </summary>
        /// <returns></returns>
        public virtual bool IsPlaying() => hostPlayer != null ? hostPlayer.IsPlaying() : playerState == EPlayerState.Playing;

        /// <summary>
        /// 恢复或播放
        /// </summary>
        public virtual bool ResumeOrPlay() => playerState == EPlayerState.Pause ? Resume() : Play();

        /// <summary>
        /// 加载并播放
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        public void LoadAndPlay(IPlayableContent playableContent, IPlayableContentHost host = null)
        {
            Load(playableContent, host, isLoad => { if (isLoad) Play(); });
        }

        /// <summary>
        /// 停止并卸载
        /// </summary>
        public void StopAndULoad(IPlayableContentHost host = null)
        {
            Stop();

            Unload(loadedContent, host);
        }

        /// <summary>
        /// 播放
        /// </summary>
        public virtual bool Play() => TryInteract(EPlayableCmd.Play, loadedContent != null ? loadedContent : loadContentOnEnable);

        /// <summary>
        /// 暂停
        /// </summary>
        public virtual bool Pause() => TryInteract(EPlayableCmd.Pause, loadedContent);

        /// <summary>
        /// 恢复
        /// </summary>
        public virtual bool Resume() => TryInteract(EPlayableCmd.Resume, loadedContent);

        /// <summary>
        /// 停止
        /// </summary>
        public virtual bool Stop() => TryInteract(EPlayableCmd.Stop, loadedContent);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="playableCmd"></param>
        /// <param name="playableContent"></param>
        /// <param name="host"></param>
        /// <param name="loadOrUnloadFun"></param>
        /// <param name="isLoad"></param>
        /// <returns></returns>
        protected bool TryInteract(EPlayableCmd playableCmd, IPlayableContent playableContent, IPlayableContentHost host = null, Action<bool> loadOrUnloadFun = null, bool isLoad = true)
        {
            if (playableContent == null) return false;

            return TryInteract(new PlayableData(_playableCmds.GetCmdName(playableCmd), this, playableContent, this, host, loadOrUnloadFun, isLoad), out var result) && result == EInteractResult.Finished;
        }

        #endregion
    }
}
