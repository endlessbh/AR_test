using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// 帧纹理
    /// </summary>
    [Name("帧纹理")]
    [Serializable]
    public class FrameTexture
    {
        /// <summary>
        /// 2D纹理
        /// </summary>
        [Name("2D纹理")]
        [Tip("序列帧纹理中的单帧2D纹理对象；", "Single frame 2D texture object in sequential frame texture;")]
        public Texture2D _texture2D;

        /// <summary>
        /// 延时秒数
        /// </summary>
        [Name("延时秒数")]
        [Tip("延时播放下一帧纹理的时间，单位为秒；", "Delay the time of playing the next frame texture, in seconds;")]
        public double _delaySec;

        /// <summary>
        /// 开始时间
        /// </summary>
        internal double beginTime = 0;

        /// <summary>
        /// 开始百分比
        /// </summary>
        internal double beginPercent = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public FrameTexture() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="texture2d"></param>
        /// <param name="delaySec"></param>
        public FrameTexture(Texture2D texture2d, float delaySec)
        {
            this._texture2D = texture2d;
            this._delaySec = delaySec;
        }
    }
}
