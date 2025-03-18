using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// GIF纹理更新器
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public abstract class GifTextureUpdater : PlayableContent
    {
        /// <summary>
        /// Gif纹理
        /// </summary>
        [Name("Gif纹理")]
        [Tip("Gif纹理资源的存储对象", "Storage object of GIF texture resource")]
        public GifTexture _gifTexture = new GifTexture();

        private EInteractResult loadResult = EInteractResult.None;

        /// <summary>
        /// 当加载gif时
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnLoad(PlayableData playableData)
        {
            if (loadResult == EInteractResult.None)
            {
                loadResult = EInteractResult.Wait;
                _gifTexture.Load(this, gif => loadResult = gif != null ? EInteractResult.Finished : EInteractResult.Aborted);
            }
            return loadResult;
        }

        /// <summary>
        /// 当卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnUnload(PlayableData playableData)
        {
            loadResult = EInteractResult.None;
            return base.OnUnload(playableData);
        }

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData) => OnUpdateFrameTexture(_gifTexture.GetFrameTexture(percent.percent01OfWorkCurve)?._texture2D);

        /// <summary>
        /// 当更新帧纹理时
        /// </summary>
        /// <param name="texture2D"></param>
        protected abstract void OnUpdateFrameTexture(Texture2D texture2D);
    }
}
