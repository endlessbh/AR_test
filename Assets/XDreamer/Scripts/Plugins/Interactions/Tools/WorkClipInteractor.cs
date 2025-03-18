using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 工作剪辑交互器
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class WorkClipInteractor : Interactor, IBaseLoopWorkClip
    {
        /// <summary>
        /// 时长
        /// </summary>
        [Group("播放设置", textEN = "Play Settings", defaultIsExpanded = false)]
        [Name("时长")]
        public double _timeLength = TimeRange.DefaultTimeLength;

        /// <summary>
        /// 时长
        /// </summary>
        public virtual double timeLength { get => _timeLength; set => _timeLength = value; } 

        /// <summary>
        /// 循环类型
        /// </summary>
        [Name("循环类型")]
        [EnumPopup]
        public ELoopType _loopType = ELoopType.None;

        /// <summary>
        /// 单次循环时长
        /// </summary>
        [Name("单次循环时长")]
        [Tip("当前组件完整执行一次表现逻辑的期望时长", "The expected length of time for the current component to fully execute the presentation logic once")]
        [HideInSuperInspector(nameof(_loopType), EValidityCheckType.Equal, ELoopType.None)]
        public double _onceTimeLength = TimeRange.DefaultTimeLength;

        /// <summary>
        /// 工作曲线
        /// </summary>
        [Name("工作曲线")]
        [EndGroup(true)]
        public AnimationCurve _workCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>
        /// 循环类型
        /// </summary>
        public ELoopType loopType => _loopType;

        /// <summary>
        /// 循环数量
        /// </summary>
        public double loopCount => loop ? Math.Min(_timeLength / _onceTimeLength, 1) : 1;

        /// <summary>
        /// 是否循环
        /// </summary>
        public virtual bool loop { get => loopType != ELoopType.None; set => _loopType = ELoopType.Loop; }

        /// <summary>
        /// 循环一次百分比长度
        /// </summary>
        public double oncePercentLength => 1 / loopCount;

        /// <summary>
        /// 工作曲线
        /// </summary>
        public AnimationCurve workCurve => _workCurve;

        /// <summary>
        /// 继续循环
        /// </summary>
        public bool continueLoopAfterWorkRange => true;

        /// <summary>
        /// 超过工作曲线百分比
        /// </summary>
        public double percentOnAfterWorkRange => 1;

        /// <summary>
        /// 百分比数据
        /// </summary>
        protected Percent percentData 
        { 
            get
            {
                if (_percentData.loopWorkClip == null)
                {
                    _percentData.Init(this);
                }
                return _percentData;
            }
        }
        private Percent _percentData = new Percent();

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent { get => 0; set { } }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent { get => 1; set { } }

        /// <summary>
        /// 百分比长度
        /// </summary>
        public double percentLength { get => 1; set { } }
    }
}
