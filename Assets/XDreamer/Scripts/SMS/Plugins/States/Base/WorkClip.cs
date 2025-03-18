using System;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    [Name("工作剪辑")]
    [KeyNode(nameof(IWorkClip), "工作剪辑")]
    [DisallowMultipleComponent]
    public abstract class WorkClip : StateComponent, ILoopWorkClip
    {
        /// <summary>
        /// 工作区间:当前组件在状态生命周期内的工作区间(时间与百分比)信息
        /// </summary>
        [Name("工作区间")]
        [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息", "Loopsetting the working interval (time and percentage) information of the current component in the state life cycle")]
        [OnlyMemberElements]
        public WorkRange workRange = new WorkRange();

        #region 基础设置-变量

        [Group("基础设置",textEN = "Basic Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("使用初始化数据")]
        [Tip("为True时，则使用初始化时记录的数据信息进行后续的更新与处理；为False时，则使用进入时(即状态启用或再次启用时)记录的数据信息进行后续的更新与处理", "When the basic setting is true, the data information recorded during initialization is used for subsequent update and processing; If it is false, the data information recorded when entering (i.e. when the status is enabled or enabled again) is used for subsequent update and processing")]
        public bool useInitData = true;

        [Name("进入时设置百分比")]
        [Tip("为True时,在当前状态组件进入时设置百分比进度为0;为False时,不做处理;", "When true, set the percentage progress to 0 when the current state component enters; If it is false, it will not be processed;")]
        public bool setPercentOnEntry = true;

        [Name("进入时百分比")]
        [Tip("当状态组件进入后,将当前状态组件逻辑数尝试保持在本百分比进度;", "When the status component enters, try to keep the current status component logic number at this percentage progress;")]
        [HideInSuperInspector(nameof(setPercentOnEntry), EValidityCheckType.False)]
        public double percentOnEntry = 0;

        [Name("退出时设置百分比")]
        [Tip("为True时,在当前状态组件退出时设置百分比进度为1;为False时,不做处理;", "When true, set the percentage progress to 1 when the current state component exits; If it is false, it will not be processed;")]
        public bool setPercentOnExit = true;

        [EndGroup(true)]
        [Name("退出时百分比")]
        [Tip("当状态组件退出后,将当前状态组件逻辑数据保持在本百分比进度;值为0,可将数据尽量还原到初始化/进入的状态;", "When the status component exits, keep the logic data of the current status component at this percentage progress; If the value is 0, the data can be restored to the initialization / entry state as much as possible;")]
        [HideInSuperInspector(nameof(setPercentOnExit), EValidityCheckType.False)]
        public double percentOnExit = 1;

        #endregion

        #region 循环-变量

        [Group("循环设置", textEN = "Loop Settings", needBoundBox = true, defaultIsExpanded = false)]
        [Name("循环类型")]
        [Tip("当前组件在其有效时间区间(百分比区间)内执行逻辑的循环类型;", "The type of loop in which the current component executes logic within its effective time interval (percentage interval);")]
        [EnumPopup]
        public ELoopType loopType = ELoopType.None;

        [Name("单次时长")]
        [Tip("当前状态组件完整执行一次表现逻辑的期望时长", "The expected length of time for the current state component to fully execute the presentation logic once")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        [FormerlySerializedAs(nameof(onceTimeLength))]
        [SerializeField]
        public double _onceTimeLength = 3;

        [Name("最少循环次数")]
        [Tip("当已经循环次数大于等于本值时,本状态组件设置为完成态;", "When the number of cycles is greater than or equal to this value, the component in this status is set to the completed status;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        public double leastLoopCount = 0;

        [Name("超出工作区间继续循环")]
        [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件是否继续执行循环逻辑;为True时,继续执行循环逻辑;为False时,不再继续执行循环逻辑;", "If the working time (percentage) of the current state component exceeds (exceeds) the right value of the working effective time (percentage) interval, whether the current state component continues to execute the loop logic; When true, continue to execute the loop logic; When it is false, the loop logic will not be executed again;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal, ELoopType.None)]
        public bool continueLoopAfterWorkRange = true;

        [EndGroup(true)]
        [Name("超出工作区间时百分比")]
        [Tip("当前状态组件所在状态的工作时间(百分比)超出(大于)工作有效时间(百分比)区间右值之后,当前状态组件将保持当前百分比设定的值所对应的状态;", "After the working time (percentage) of the current status component exceeds (exceeds) the right value of the working effective time (percentage) interval, the current status component will maintain the state corresponding to the value set by the current percentage;")]
        [HideInSuperInspector(nameof(loopType), EValidityCheckType.Equal | EValidityCheckType.Or, ELoopType.None, nameof(continueLoopAfterWorkRange), EValidityCheckType.True)]
        public double percentOnAfterWorkRange = 1;

        #endregion

        #region 工作曲线-变量

        [Group("工作曲线设置", textEN = "Working Curve Setting", needBoundBox = true, defaultIsExpanded = false)]
        [EndGroup]
        [Name("工作曲线")]
        [Tip("工作曲线仅在当前组件的工作区间的有效百分比区间内生效", "The working curve takes effect only within the effective percentage range of the working range of the current component")]
        public AnimationCurve workCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region 其它变量

        [Name("锁定比例")]
        [Tip("锁定百分比与时间的比例关系,根据锁定时当前状态组件总时长,对二者进行等比例同步调整;即其中一区间数据发生修改，另一区间数据将同步进行等比例的数据修改;", "The proportional relationship between locking percentage and time, and the two are adjusted synchronously in equal proportion according to the total time of components in the current state at the time of locking; That is, if one section data is modified, the other section data will be modified in equal proportion synchronously;")]
        [XCSJ.Attributes.Icon(EIcon.Lock)]
        [HideInSuperInspector]
        public bool lockRatioOfWorkRange = false;

        /// <summary>
        /// 锁定比例的总时长
        /// </summary>
        [Name("锁定比例的总时长", "Total duration of lock ratio")]
        [HideInSuperInspector]
        public double ttlOfLockRatio = 3f;

        /// <summary>
        /// 同步时长:将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)
        /// </summary>
        [Name("同步时长")]
        [Tip("将时长实时自动同步为当前状态组件中某些有效字段成员所承载的内容时长;同步时保证起始时间不变;TL,即时长(Time Length缩写)", "Automatically synchronize the duration in real time to the content duration carried by some effective field members in the current status component; Ensure that the starting time remains unchanged during synchronization; TL, time length (short for time length)")]
        [HideInSuperInspector]
        [XCSJ.Attributes.Icon(EIcon.Update)]
        public bool syncTL = true;

        /// <summary>
        /// 同步OTL:将单次时长与当前状态组件的有效时长保持同步,即二者修改会互相影响;OTL,即单次时长(Once Time Length缩写)
        /// </summary>
        [Name("同步OTL")]
        [Tip("将单次时长与当前状态组件的有效时长保持同步,即二者修改会互相影响;OTL,即单次时长(Once Time Length缩写)", "Keep the single time duration synchronized with the effective duration of the current state component, that is, the modification of the two will affect each other; OTL, i.e. single time length (abbreviation for once time length)")]
        [HideInSuperInspector]
        public bool syncOTL = true;

        #endregion

        #region 基类接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public override bool Init(StateData stateData)
        {
            percent.Init(this);
            return base.Init(stateData);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => base.Finished() || MathX.ApproximatelyZero(timeLength);

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            percent.Reset();
            base.OnBeforeEntry(stateData);
        }

        /// <summary>
        /// 当进入之后
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            if (setPercentOnEntry)
            {
                SetPercent(percentOnEntry, stateData);
            }
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            SetTimeOfState(parent.timeLengthWithSpeedSinceEntry, stateData);
        }

        /// <summary>
        /// 当退出之前
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeExit(StateData stateData)
        {
            if (setPercentOnExit)
            {
                SetPercent(percentOnExit, stateData);
            }
            base.OnBeforeExit(stateData);
        }

        #endregion

        #region IProgress && ILoopWorkClip

        /// <summary>
        /// 百分比
        /// </summary>
        [Name("百分比")]
        public Percent percent { get; private set; } = new Percent();

        /// <summary>
        /// 进度
        /// </summary>
        public override double progress { get => percent.percent01OfWorkCurve; set => SetPercent(value); }

        /// <summary>
        /// 标识是否循环
        /// </summary>
        public bool loop => loopType != ELoopType.None;

        /// <summary>
        /// 循环次数
        /// </summary>
        public double loopCount => loop ? TimeScaleByOTL(timeLength) : 1;

        ELoopType IBaseLoopWorkClip.loopType => loopType;

        AnimationCurve IBaseLoopWorkClip.workCurve => workCurve;

        bool IBaseLoopWorkClip.continueLoopAfterWorkRange => continueLoopAfterWorkRange;

        double IBaseLoopWorkClip.percentOnAfterWorkRange => percentOnAfterWorkRange;

        /// <summary>
        /// 单次时长：简写OTL
        /// </summary>
        public double onceTimeLength
        {
            get => loop ? _onceTimeLength : timeLength;
            set
            {
                if(syncOTL || !loop)
                {
                    timeLength = value;
                }
                else
                {
                    this.XModifyProperty(ref _onceTimeLength, value);
                }
            }
        }

        /// <summary>
        /// 单次百分比长
        /// </summary>
        public double oncePercentLength => loop ? MathX.Scale(percentLength * _onceTimeLength, timeLength) : percentLength;

        /// <summary>
        /// 总时长
        /// </summary>
        public double totalTimeLength { get => workRange.totalTimeLength; set => workRange.totalTimeLength = value; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime { get => workRange.beginTime; set => workRange.beginTime = value; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public double endTime { get => workRange.endTime; set => workRange.endTime = value; }

        /// <summary>
        /// 时长
        /// </summary>
        public double timeLength
        {
            get => workRange.timeLength;
            set
            {
                this.XModifyProperty(() =>
                {
                    workRange.timeLength = value;
                    if (syncOTL) _onceTimeLength = value;
                });
            }
        }

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent { get => workRange.beginPercent; set => workRange.beginPercent = value; }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent { get => workRange.endPercent; set => workRange.endPercent = value; }

        /// <summary>
        /// 百分比长
        /// </summary>
        public double percentLength { get => workRange.percentLength; set => workRange.percentLength = value; }

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTime(double time) => InternelSetPercentOfState(MathX.Scale(time + beginTime, totalTimeLength), null);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTime(double time, StateData stateData) => InternelSetPercentOfState(MathX.Scale(time + beginTime, totalTimeLength), stateData);

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTimeOfState(double time) => InternelSetPercentOfState(MathX.Scale(time, totalTimeLength), null);

        /// <summary>
        /// 设置状态时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetTimeOfState(double time, StateData stateData) => InternelSetPercentOfState(MathX.Scale(time, totalTimeLength), stateData);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercent(double percent) => InternelSetPercentOfState(percent + beginPercent, null);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercent(double percent, StateData stateData) => InternelSetPercentOfState(percent + beginPercent, stateData);

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public bool SetPercentOfState(double percent) => InternelSetPercentOfState(percent, null);

        /// <summary>
        /// 设置状态百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public bool SetPercentOfState(double percent, StateData stateData) => InternelSetPercentOfState(percent, stateData);

        private bool InternelSetPercentOfState(double percentOfState, StateData stateData)
        {
            try
            {
                OnSetPercent(percent.Update(percentOfState), stateData);
                return true;
            }
            catch (Exception ex)
            {
                LogException(this, nameof(OnSetPercent), ex);
                return false;
            }
            finally
            {
                if (!finished && percentOfState >= 0)
                {
                    switch (loopType)
                    {
                        case ELoopType.None:
                            {
                                finished = percent.percent >= 1
                                    || MathX.Approximately(percent.percent01, 1)
                                    || MathX.ApproximatelyZero(timeLength);
                                break;
                            }
                        case ELoopType.Loop:
                        case ELoopType.PingPong:
                            {
                                finished = percent.percent >= leastLoopCount
                                    || MathX.Approximately(percent.percent, leastLoopCount)
                                    || MathX.ApproximatelyZero(timeLength)
                                    || percent.percent * _onceTimeLength >= totalTimeLength;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 当设置百分比时回调
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected abstract void OnSetPercent(Percent percent, StateData stateData);

        /// <summary>
        /// 时间缩放通过单次时长（OTL）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected double TimeScaleByOTL(double time) => MathX.Scale(time, _onceTimeLength);

        /// <summary>
        /// 百分比缩放通过单次时长（OTL）
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        protected double PercentScaleByOTL(double percent) => MathX.Scale(percent * timeLength, percentLength * _onceTimeLength);

        /// <summary>
        /// 验证总时长有效性
        /// </summary>
        /// <param name="ttl">期望的总时长</param>
        /// <returns>如期望总时长与计算得到的总是长相等(在允许误差的范围内即认为相等)时返回True；否则返回False</returns>
        public virtual bool Validity(double ttl)
        {
            return MathX.Approximately(ttl, totalTimeLength, Epsilon) && WorkClipValidity(this);
        }

        /// <summary>
        /// 检测工作剪辑的有效性；主要检测时间与百分比的比例关系等；
        /// </summary>
        /// <param name="workClip">待检测的工作剪辑对象</param>
        /// <returns>有效返回True；否则返回False</returns>
        public static bool WorkClipValidity(IWorkClip workClip)
        {
            if (workClip == null) return false;

            //优先检查时长与百分比长
            if (MathX.ApproximatelyZero(workClip.timeLength, Epsilon) || MathX.ApproximatelyZero(workClip.percentLength, Epsilon))
            {
                return false;
            }

            var ttl = workClip.totalTimeLength;
            //需保证起始/结束的时间与百分比相对总时长必须成比例
            return MathX.Approximately(ttl * workClip.beginPercent, workClip.beginTime, Epsilon) && MathX.Approximately(ttl * workClip.endPercent, workClip.endTime, Epsilon);
        }

        /// <summary>
        /// 当越界发生时回调；
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="outOfBoundsMode">越界模式</param>
        /// <param name="percent">当前百分比</param>
        /// <param name="stateData">状态数据对象</param>
        /// <param name="lastPercent">上一次的百分比</param>
        /// <param name="stateWorkClip">状态工作剪辑对象</param>
        public virtual void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip) {

            switch(outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        if (setPercentOnEntry)
                        {
                            SetPercent(percentOnEntry, stateData);
                        }
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        if (setPercentOnExit)
                        {
                            SetPercent(percentOnExit, stateData);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 工作剪辑播放器对象；进入播放状态对象有效，停止时对象无效；
        /// </summary>
        public IWorkClipPlayer workClipPlayer { get; private set; } = null;

        /// <summary>
        /// 当工作剪辑播放器的播放状态发生变化时回调
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="lastPlayerState">上次的工作剪辑播放器的播放状态</param>
        public virtual void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            switch(player.playerState)
            {
                case EPlayerState.Play:
                    {
                        workClipPlayer = player;
                        break;
                    }
                case EPlayerState.Stop:
                    {
                        workClipPlayer = null;
                        break;
                    }
            }
        }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        /// <summary>
        /// 误差
        /// </summary>
        public const double Epsilon = Percent.Epsilon;

        #endregion

        #region 用于计算的百分比

        /// <summary>
        /// 获取百分比
        /// </summary>
        /// <param name="percentOfState">基于状态的百分比：状态的百分比进度(范围[-∞,+∞])</param>
        /// <returns></returns>
        [Name("获取百分比")]
        public Percent GetPercent(double percentOfState)
        {
            var percent = new Percent();
            percent.Init(this);
            percent.Update(percentOfState);
            return percent;
        }

        #endregion
    }

    /// <summary>
    /// 工作剪辑模版抽象类
    /// </summary>
    /// <typeparam name="T">子类模版</typeparam>
    [Name("工作剪辑<模版>")]
    public abstract class WorkClip<T> : WorkClip where T : WorkClip<T>
    {
        /// <summary>
        /// 自身对象
        /// </summary>
        public T self => (T)this;

        /// <summary>
        /// 创建携带当前状态组件的普通状态
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的普通状态会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static NormalState CreateNormalState(IGetStateCollection obj, Action<NormalState> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateNormalState<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 创建携带当前状态组件的子状态机
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的子状态机会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static SubStateMachine CreateSubStateMachine(IGetStateCollection obj, Action<SubStateMachine> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateSubStateMachine<T>(init, stateComponentTypes);
        }
    }
}
