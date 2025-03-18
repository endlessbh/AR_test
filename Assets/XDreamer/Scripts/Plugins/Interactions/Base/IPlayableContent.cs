using System;
using System.Collections.Generic;
using UnityEngine.Events;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.Extension.Interactions.Base
{
    /// <summary>
    /// 可播放内容
    /// </summary>
    public interface IPlayableContent : ITL, IPercent
    {
        /// <summary>
        /// 可播放内容宿主：当内容受控时，本对象不为空
        /// </summary>
        IPlayableContentHost host { get; set; }

        /// <summary>
        /// 播放器：当内容被播放时，本对象不为空
        /// </summary>
        IContentPlayer player { get; }

        /// <summary>
        /// 当加载内容
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnLoad(PlayableData playableData);

        /// <summary>
        /// 当卸载内容
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnUnload(PlayableData playableData);

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnPlay(PlayableData playableData);

        /// <summary>
        /// 当暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnPause(PlayableData playableData);

        /// <summary>
        /// 当恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnResume(PlayableData playableData);

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        EInteractResult OnStop(PlayableData playableData);

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        void OnSetPercent(Percent percent, PlayableData playableData);
    }

    /// <summary>
    /// 播放内容宿主:控制另一个播放内容的对象，自身没有播放内容
    /// </summary>
    public interface IPlayableContentHost 
    {

    }

    /// <summary>
    /// 可播放数据
    /// </summary>
    public class PlayableData : InteractData<PlayableData>
    {
        /// <summary>
        /// 可播放内容
        /// </summary>
        public IPlayableContent playableContent { get; internal set; }

        /// <summary>
        /// 播放器
        /// </summary>
        public IContentPlayer player { get; private set; }

        /// <summary>
        /// 可播放内容宿主
        /// </summary>
        public IPlayableContentHost playableContentHost { get; private set; }

        /// <summary>
        /// 加载内容回调方法
        /// </summary>
        public Action<bool> onLoad { get; private set; }

        /// <summary>
        /// 卸载内容回调方法
        /// </summary>
        public Action<bool> onUnload { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PlayableData() { }

        /// <summary>
        /// 构造函数：用于传递宿主控制播放内容
        /// </summary>
        /// <param name="playableContentHost"></param>
        public PlayableData(IPlayableContentHost playableContentHost) : this(null, null, playableContentHost) { }

        /// <summary>
        /// 构造函数：用于非交互系统的播放控制
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="player"></param>
        /// <param name="playableContentHost"></param>
        public PlayableData(IPlayableContent playableContent, IContentPlayer player, IPlayableContentHost playableContentHost = null)
        {
            Set(playableContent, player, playableContentHost);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="playableContent"></param>
        /// <param name="player"></param>
        /// <param name="playableContentHost"></param>
        /// <param name="loadOrUnloadFun"></param>
        /// <param name="isLoadFun"></param>
        public PlayableData(string cmdName, InteractObject interactor, IPlayableContent playableContent, IContentPlayer player, IPlayableContentHost playableContentHost = null, Action<bool> loadOrUnloadFun = null, bool isLoadFun = false) : base(cmdName, interactor)
        {
            Set(playableContent, player, playableContentHost);

            if (isLoadFun)
            {
                onLoad = loadOrUnloadFun;
            }
            else
            {
                onUnload = loadOrUnloadFun;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is PlayableData data)
            {
                data.Set(playableContent, player, playableContentHost);
                data.onLoad = onLoad;
                data.onUnload= onUnload;
            }
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="playableContent"></param>
        /// <param name="player"></param>
        /// <param name="playableContentHost"></param>
        internal void Set(IPlayableContent playableContent, IContentPlayer player, IPlayableContentHost playableContentHost = null)
        {
            this.playableContent = playableContent;
            this.player = player;
            this.playableContentHost = playableContentHost;
        }
    }
}
