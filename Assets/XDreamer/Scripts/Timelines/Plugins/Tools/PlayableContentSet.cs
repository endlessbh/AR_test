using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 可播放内容集合
    /// </summary>
    [Name("可播放内容集合")]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.List)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class PlayableContentSet : PlayableContentHostPlayer
    {
        /// <summary>
        /// 播放内容数据
        /// </summary>
        [Serializable]
        public class PlayableContentData
        {
            [Name("可播放内容")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public PlayableContent _playableContent;

            [Name("百分比")]
            public PercentRange _percentRange = new PercentRange();

            /// <summary>
            /// 名称
            /// </summary>
            public string name => _playableContent ? _playableContent.name : "";

            /// <summary>
            /// 开始百分比
            /// </summary>
            public double beginPercent { get => _percentRange.beginPercent; set => _percentRange.beginPercent = value; }

            /// <summary>
            /// 结束百分比
            /// </summary>
            public double endPercent { get => _percentRange.endPercent; set => _percentRange.endPercent = value; }

            /// <summary>
            /// 设置百分比
            /// </summary>
            /// <param name="percent01"></param>
            public void SetPercent(double percent01)
            {
                if (_playableContent)
                {
                    var length = _percentRange.percentLength;
                    if (MathX.ApproximatelyZero(length))
                    {
                        percent01 = 1;
                    }
                    else
                    {
                        percent01 = MathX.Clamp(percent01, _percentRange.beginPercent, _percentRange.endPercent);
                        percent01 = (percent01 - _percentRange.beginPercent) / length;
                    }

                    _playableContent.percent = percent01;
                }
            }
        }

        /// <summary>
        /// 可播放内容列表
        /// </summary>
        [Name("可播放内容数据列表")]
        public List<PlayableContentData> _playableContentDatas = new List<PlayableContentData>();

        private List<PlayableContent> validContents = new List<PlayableContent>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            validContents.AddRange(_playableContentDatas.Where(d => d._playableContent && d._playableContent != this).Cast(d => d._playableContent));

            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            validContents.Clear();
        }

        private List<IPlayableContent> _loadedContents = new List<IPlayableContent>();

        private EInteractResult loadResult = EInteractResult.None;

        /// <summary>
        /// 当内容加载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnLoad(PlayableData playableData)
        {
            if (loadResult != EInteractResult.Wait)
            {
                loadResult = EInteractResult.Wait;

                foreach (var subContent in validContents)
                {
                    if (subContent.isLoaded)
                    {
                        subContent.host = this;
                        _loadedContents.AddWithDistinct(subContent);
                    }
                    else
                    {
                        subContent.Load(subContent, this, isLoaded =>
                        {
                            if (isLoaded)
                            {
                                _loadedContents.AddWithDistinct(subContent);
                            }
                            else
                            {
                                loadResult = EInteractResult.Aborted;
                            }
                        });
                    }
                }
            }

            if (loadResult == EInteractResult.Wait)
            {
                if (_loadedContents.Count == validContents.Count)
                {
                    return loadResult = EInteractResult.Finished;
                }
            }

            return loadResult;
        }

        /// <summary>
        /// 当内容卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public override EInteractResult OnUnload(PlayableData playableData)
        {
            if (loadResult != EInteractResult.Wait)
            {
                loadResult = EInteractResult.Wait;
                foreach (var subContent in validContents)
                {
                    if (subContent.isLoaded)
                    {
                        subContent.Unload(subContent, this, isUnloaded =>
                        {
                            if (isUnloaded)
                            {
                                _loadedContents.Remove(subContent);
                            }
                            else// 有一个内容卸载失败，就调用传入回调
                            {
                                loadResult = EInteractResult.Aborted;
                            }
                        });
                    }
                    else
                    {
                        if (subContent.host == (IPlayableContentHost)this)
                        {
                            subContent.host = null;
                        }
                        _loadedContents.Remove(subContent);
                    }
                }
            }

            if (loadResult == EInteractResult.Wait)
            {
                if (_loadedContents.Count == 0)
                {
                    return loadResult = EInteractResult.Finished;
                }
            }

            return loadResult;
        }

        private double lastPercent = 0;

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            var p = percent.percent01OfWorkCurve;
            double percentOffset = p - lastPercent;

            var beforeRanges = new List<PlayableContentData>();
            var curRanges = new List<PlayableContentData>();
            var afterRanges = new List<PlayableContentData>();

            // 比较百分比区间
            foreach (var data in _playableContentDatas)
            {
                if (data._percentRange.In(p))
                {
                    curRanges.Add(data);
                }
                else if (data._percentRange.Right(p))
                {
                    beforeRanges.Add(data);
                }
                else
                {
                    afterRanges.Insert(0, data);
                }
            }

            // 从前到后
            if (percentOffset >= 0)
            {
                // 使用区间与lastTime比较
                // lastTime  |-----------| time
                // before  |---|
                // in         |---|
                // in             |---|
                // after             |---|
                foreach (var data in beforeRanges)
                {
                    if (data._percentRange.Right(lastPercent))
                    {
                        continue;
                    }
                    else
                    {
                        // 使用大值来设置
                        data.SetPercent(p);
                        // 回调右越界事件
                        data._playableContent.OnOutOfBounds(EOutOfBoundsMode.Right, p, playableData, lastPercent);
                    }
                }
            }
            // 从后到前
            else
            {
                // 使用区间与lastTime比较
                // time  |-----------| lastTime
                // before  |---|
                // in            |---|
                // in                |---|
                // after                |---|
                foreach (var data in afterRanges)
                {
                    if (data._percentRange.Left(lastPercent))
                    {
                        continue;
                    }
                    else
                    {
                        // 使用小值来设置
                        data.SetPercent(p);
                        // 回调左越界事件
                        data._playableContent.OnOutOfBounds(EOutOfBoundsMode.Left, p, playableData, lastPercent);
                    }
                }
            }

            // 设置当前片段动画
            foreach (var data in curRanges)
            {
                if (data._percentRange.Left(lastPercent))
                {
                    data._playableContent.OnOutOfBounds(EOutOfBoundsMode.LeftToIn, p, playableData, lastPercent);
                }
                else if (data._percentRange.Right(lastPercent))
                {
                    data._playableContent.OnOutOfBounds(EOutOfBoundsMode.RightToIn, p, playableData, lastPercent);
                }
                data.SetPercent(p);
            }

            lastPercent = p;
        }
    }
}
