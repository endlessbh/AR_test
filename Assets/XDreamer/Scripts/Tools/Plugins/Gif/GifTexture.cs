using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// Gif纹理
    /// </summary>
    [Serializable]
    [Name("Gif纹理")]
    [LanguageFileOutput]
    public class GifTexture
    {
        /// <summary>
        /// Gif源
        /// </summary>
        [Name("Gif源")]
        [EnumPopup]
        public EGifSource _gifSource = EGifSource.TextAsset;

        /// <summary>
        /// Gif文本资产
        /// </summary>
        [Name("Gif文本资产")]
        [HideInSuperInspector(nameof(_gifSource), EValidityCheckType.NotEqual, EGifSource.TextAsset)]
        public TextAsset _gifTextAsset;

        /// <summary>
        /// Gif序列帧
        /// </summary>
        [Name("Gif序列帧")]
        [HideInSuperInspector(nameof(_gifSource), EValidityCheckType.NotEqual, EGifSource.SequenceFrames)]
        public SequenceFrames _gifSequenceFrames = new SequenceFrames();

        ///// <summary>
        ///// Gif纹理2D
        ///// </summary>
        //[Name("Gif纹理2D")]
        //[HideInSuperInspector(nameof(_gifSource), EValidityCheckType.NotEqual, EGifSource.Texture2D)]
        //public Texture2D _gifTexture2D;

        /// <summary>
        /// 序列帧缓存
        /// </summary>
        [Name("序列帧缓存")]
        [HideInSuperInspector(nameof(_gifSource), EValidityCheckType.Equal, EGifSource.SequenceFrames)]
        [Readonly]
        public SequenceFrames _sequenceFramesCache = new SequenceFrames();

        /// <summary>
        /// 序列帧
        /// </summary>
        public SequenceFrames sequenceFrames { get; private set; }

        Action<GifTexture> onLoaded = null;

        /// <summary>
        /// 在加载中
        /// </summary>
        public bool inLoading { get; private set; } = false;

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="onLoaded"></param>
        /// <returns></returns>
        public bool Load(MB mb = null, Action<GifTexture> onLoaded = null)
        {
            if (inLoading) return false;

            switch (_gifSource)
            {
                //case EGifSource.Texture2D:
                //    {
                //        if (!_gifTexture2D) return false;

                //        inLoading = true;
                //        this.onLoaded = onLoaded;

                //        if (_useCoroutine && mb)
                //        {
                //            mb.StartCoroutine(UniGif.GetTextureListCoroutine(mb, _gifTexture2D.GetRawTextureData(), OnLoad, _filterMode, _wrapMode, _outputDebugLog));
                //        }
                //        else
                //        {
                //            var gifTextures = UniGif.GetTextureList(_gifTexture2D.GetRawTextureData(), out int loopCount, out int width, out int height, _filterMode, _wrapMode, _outputDebugLog);
                //            OnLoad(gifTextures, loopCount, width, height);
                //        }

                //        return true;
                //    }
                case EGifSource.TextAsset:
                    {
                        if (!_gifTextAsset) return false;

                        inLoading = true;
                        this.onLoaded = onLoaded;

                        if (_useCoroutine && mb)
                        {
                            mb.StartCoroutine(UniGif.GetTextureListCoroutine(mb, _gifTextAsset.bytes, OnLoad, _filterMode, _wrapMode, _outputDebugLog));
                        }
                        else
                        {
                            var gifTextures = UniGif.GetTextureList(_gifTextAsset.bytes, out int loopCount, out int width, out int height, _filterMode, _wrapMode, _outputDebugLog);
                            OnLoad(gifTextures, loopCount, width, height);
                        }

                        return true;
                    }
                case EGifSource.SequenceFrames:
                    {
                        inLoading = true;

                        sequenceFrames = _gifSequenceFrames;

                        inLoading = false;
                        onLoaded?.Invoke(this);
                        return true;
                    }
            }

            return false;
        }

        private void OnLoad(List<FrameTexture> gifTextures, int loopCount, int width, int height)
        {
            this.loopCount = loopCount;
            this.width = width;
            this.height = height;

            sequenceFrames = _sequenceFramesCache;

            _sequenceFramesCache.Clear();
            if (gifTextures != null)
            {
                _sequenceFramesCache._frameTextures = gifTextures;
                _sequenceFramesCache.MarkDirty();
            }
            //else if (_gifSource == EGifSource.Texture2D && _gifTexture2D)
            //{
            //    _sequenceFramesCache._frameTextures.Add(new FrameTexture(_gifTexture2D, 0));
            //    _sequenceFramesCache.MarkDirty();
            //}

            inLoading = false;
            onLoaded?.Invoke(this);
            onLoaded = null;
        }

        /// <summary>
        /// 获取帧纹理
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public FrameTexture GetFrameTexture(double percent)
        {
            if (inLoading) return default;
            return sequenceFrames?.GetFrameTexture(percent);
        }

        /// <summary>
        /// 使用协程
        /// </summary>
        [Name("使用协程")]
        [Tip("使用协程处理GIF数据", "Processing GIF data using a coroutine")]
        public bool _useCoroutine = false;

        /// <summary>
        /// 过滤模式
        /// </summary>
        [Name("过滤模式")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，每帧纹理的过滤模式；", "When transforming GIF texture resource data stream into sequential frame texture, the filtering mode of each frame texture;")]
        public FilterMode _filterMode = FilterMode.Point;

        /// <summary>
        /// 纹理循环模式
        /// </summary>
        [Name("纹理循环模式")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，每帧纹理的纹理循环模式；", "When GIF texture resource data stream is transformed into sequential frame texture, the texture cycle mode of each frame texture;")]
        public TextureWrapMode _wrapMode = TextureWrapMode.Clamp;

        /// <summary>
        /// 输出调试信息
        /// </summary>
        [Name("输出调试信息")]
        [Tip("Gif纹理资源数据流转化为序列帧纹理时，是否输出转化过程中的一些调试信息；错误信息直接输出，不受此项限制；", "Whether to output some debugging information in the process of transforming GIF texture resource data stream into sequence frame texture; Direct output of error information is not subject to this restriction;")]
        public bool _outputDebugLog = false;


        /// <summary>
        /// 宽
        /// </summary>
        public int width { get; private set; } = -1;

        /// <summary>
        /// 高
        /// </summary>
        public int height { get; private set; } = -1;

        /// <summary>
        /// 循环次数
        /// </summary>
        public int loopCount { get; private set; } = -1;
    }

    /// <summary>
    /// 序列帧
    /// </summary>
    [Serializable]
    [Name("序列帧")]
    [LanguageFileOutput]
    public class SequenceFrames : IMarkDirty
    {
        /// <summary>
        /// 帧纹理列表
        /// </summary>
        [Name("帧纹理列表")]
        public List<FrameTexture> _frameTextures = new List<FrameTexture>();

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            _frameTextures?.Clear();
            MarkDirty();
        }

        /// <summary>
        /// 上次索引
        /// </summary>
        [Name("上次索引")]
        [Readonly]
        public int lastIndex = -1;

        private FrameTexture lastFrameTexture;

        /// <summary>
        /// 获取帧纹理
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public FrameTexture GetFrameTexture(double percent)
        {
            var index = GetIndex(percent);
            if (index == lastIndex) return lastFrameTexture;

            if (index < 0 || index >= _frameTextures.Count) return default;

            lastIndex = index;
            lastFrameTexture = _frameTextures[index];

            return lastFrameTexture;
        }

        /// <summary>
        /// 获取总时长
        /// </summary>
        /// <returns></returns>
        public double GetTotalTimeLength()
        {
            InitIfNeed();
            return totalTimeLength;
        }

        private bool needInit = true;

        private void InitIfNeed()
        {
            if (needInit)//需要初始化
            {
                needInit = false;

                totalTimeLength = 0d;
                foreach (var frameTexture in _frameTextures)
                {
                    frameTexture.beginTime = totalTimeLength;
                    totalTimeLength += frameTexture._delaySec;
                }

                if (MathX.ApproximatelyZero(totalTimeLength))
                {
                    validList = false;
                    invalidIndex = -1;
                }
                else
                {
                    validList = true;
                    foreach (var frameTexture in _frameTextures)
                    {
                        frameTexture.beginPercent = frameTexture.beginTime / totalTimeLength;
                    }
                }
            }
        }

        double totalTimeLength = 0;

        bool validList = false;
        int invalidIndex = -1;

        private int GetIndex(double percent)
        {
            InitIfNeed();

            if (validList)
            {
                var last = _frameTextures.Count - 1;

                //优先检查缓存的索引值
                if (lastIndex >= 0 && lastIndex < last && percent > lastFrameTexture.beginPercent && percent <= _frameTextures[lastIndex + 1].beginPercent)
                {
                    return lastIndex;
                }

                //二分查找新的索引值
                var result = MathLibrary.BinarySearch(_frameTextures, (i, frameTexture) =>
                {
                    if (percent > frameTexture.beginPercent)
                    {
                        if (last == i || percent <= _frameTextures[i + 1].beginPercent)
                        {
                            return 0;
                        }
                        return 1;
                    }
                    return -1;
                });

                //找到新的有效值
                if (result.Item1 >= 0) return result.Item1;

                //返回开头或末尾
                return percent >= _frameTextures[last].beginPercent ? last : 0;
            }
            else
            {
                invalidIndex = ++invalidIndex % _frameTextures.Count;
                return invalidIndex;
            }
        }

        /// <summary>
        /// 标记脏
        /// </summary>
        public void MarkDirty()
        {
            needInit = true;
            lastIndex = -1;
            lastFrameTexture = null;
        }
    }

    /// <summary>
    /// Gif源
    /// </summary>
    [Name("Gif源")]
    public enum EGifSource
    {
        /// <summary>
        /// 文本资产
        /// </summary>
        [Name("文本资产")]
        [Tip("Gif文件的二进制文件")]
        TextAsset = 0,

        /// <summary>
        /// 序列帧
        /// </summary>
        [Name("序列帧")]
        [Tip("通过多张纹理与间隔时间设置的序列帧纹理列表")]
        SequenceFrames,
    }
}
