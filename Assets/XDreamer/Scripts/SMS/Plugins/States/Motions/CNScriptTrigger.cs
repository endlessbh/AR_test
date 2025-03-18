using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 脚本触发
    /// </summary>
    [Name("脚本触发")]
    public enum ECNScriptTrigger
    {
        [Name("工作剪辑触发")]
        [Tip("在工作剪辑的某个百分比上触发", "Triggered on a percentage of the working clip")]
        OnTrigger,
    }

    /// <summary>
    /// 脚本触发函数
    /// </summary>
    [Name("脚本触发函数")]
    [Serializable]
    public class CNScriptTriggerFunction : EnumFunction<ECNScriptTrigger> { }

    /// <summary>
    /// 脚本触发函数集合
    /// </summary>
    [Name("脚本触发函数集合")]
    [Serializable]
    public class CNScriptTriggerFunctionCollection : EnumFunctionCollection<ECNScriptTrigger, CNScriptTriggerFunction> { }

    /// <summary>
    /// 脚本触发:使用中文脚本编写控制逻辑,并在某个区间中触发
    /// </summary>
    [Serializable]
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(CNScriptTrigger))]
    [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发", "Use Chinese script to write control logic and trigger it in a certain interval")]
    [XCSJ.Attributes.Icon(index = 33616)]
    [KeyNode(nameof(IWorkClip), "工作剪辑")]
    public class CNScriptTrigger : StateScriptComponent<CNScriptTrigger, ECNScriptTrigger, CNScriptTriggerFunction, CNScriptTriggerFunctionCollection>, 
        IWorkClip, 
        ITriggerPoint
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "脚本触发";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(CNScriptTrigger))]
        [Tip("使用中文脚本编写控制逻辑,并在某个区间中触发", "Use Chinese script to write control logic and trigger it in a certain interval")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 工作区间
        /// </summary>
        [Group("区间触发")]
        [Name("工作区间")]
        [Tip("当前组件在状态生命周期内的工作区间(时间与百分比)信息", "Information about the working interval (time and percentage) of the current component in the state life cycle")]
        [OnlyMemberElements]
        public WorkRange workRange = new WorkRange();

        public bool loop => false;

        public double onceTimeLength => workRange.totalTimeLength;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            _progress = 0;
            base.OnEntry(data);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            SetTimeOfState(parent.timeLengthWithSpeedSinceEntry);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
        }

        #region 工作剪辑 && 进度

        public double totalTimeLength
        {
            get { return workRange.totalTimeLength; }
            set { workRange.totalTimeLength = value; }
        }

        protected double _progress = 0;

        public override double progress
        {
            get { return MathX.Clamp01(_progress); }
            set { SetPercent(value); }
        }

        public virtual double beginTime
        {
            get { return workRange.timeRange.timeRange.x; }
            set { workRange.timeRange.timeRange.x = value; }
        }

        public virtual double endTime
        {
            get { return workRange.timeRange.timeRange.y; }
            set { workRange.timeRange.timeRange.y = value; }
        }

        public virtual double timeLength
        {
            get { return endTime - beginTime; }
            set { endTime = beginTime + value; }
        }

        public virtual double beginPercent
        {
            get { return workRange.percentRange.percentRange.x; }
            set { workRange.percentRange.percentRange.x = value; }
        }

        public virtual double endPercent
        {
            get { return workRange.percentRange.percentRange.y; }
            set { workRange.percentRange.percentRange.y = value; }
        }

        public virtual double percentLength
        {
            get { return endPercent - beginPercent; }
            set { endPercent = MathX.Clamp01(beginPercent + value); }
        }

        public virtual bool SetTimeOfState(double time) => SetTimeOfState(time, null);

        public virtual bool SetTime(double time) => SetTime(time, null);

        public virtual bool SetPercentOfState(double percent) => SetPercentOfState(percent, null);

        public virtual bool SetPercent(double percent) => SetPercent(percent, null);
        
        protected virtual void CheckFinished()
        {
            if (!finished)
            {
                finished = MathX.Approximately(progress, 1) || MathX.ApproximatelyZero(timeLength);
            }
        }

        public bool Validity(double ttl) => MathX.Approximately(ttl, totalTimeLength, WorkClip.Epsilon) && WorkClip.WorkClipValidity(this);


        public bool SetTime(double time, StateData stateData)
        {
            return SetPercent(workRange.timeRange.NormalizeOfRelativeLeft(time), stateData);
        }

        public bool SetTimeOfState(double time, StateData stateData)
        {
            return SetTime(time - beginTime, stateData);
        }

        public bool SetPercentOfState(double percent, StateData stateData)
        {
            return SetPercent(workRange.percentRange.Normalize(percent), stateData);
        }

        public bool SetPercent(double percent, StateData stateData)
        {
            if (!valid) return false;

            // 初始化过后
            if (lastPercent >= 0)
            {
                switch (direction)
                {
                    case ETriggerDirection.Increase:
                        {
                            if (triggerPercent > lastPercent && triggerPercent < percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Descending:
                        {
                            if (triggerPercent < lastPercent && triggerPercent > percent)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Both:
                        {
                            if ((triggerPercent > lastPercent && triggerPercent < percent) ||
                                (triggerPercent < lastPercent && triggerPercent > percent))
                            {
                                OnTrigger();
                            }
                            break;
                        }
                }
            }
            lastPercent = percent;

            _progress = percent;
            CheckFinished();
            return true;
        }

        #endregion

        /// <summary>
        /// 触发百分比
        /// </summary>
        [Name("触发百分比")]
        [Range(0, 1)]
        public double _triggerPercent = 0.05f;

        /// <summary>
        /// 递增方向
        /// </summary>
        [Name("递增方向")]
        [EnumPopup]
        public ETriggerDirection _direction = ETriggerDirection.Increase;

        /// <summary>
        /// 上次百分比
        /// </summary>
        protected double lastPercent = 0;

        private bool _valid = true;

        /// <summary>
        /// 当触发
        /// </summary>
        protected virtual void OnTrigger()
        {
            ExecuteScriptEvent(ECNScriptTrigger.OnTrigger);
        }

        public void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip) { }

        public void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState) { }

        void IPlayerEvent.OnPlayerStateChanged(IPlayer player, EPlayerState lastPlayerState) => OnPlayerStateChanged(player as IWorkClipPlayer, lastPlayerState);

        public virtual double triggerPercent
        {
            get { return this._triggerPercent; }
            set { this._triggerPercent = value; }
        }

        public virtual ETriggerDirection direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }

        public virtual bool valid
        {
            get { return _valid; }
            set { _valid = value; }
        }
    }
}
