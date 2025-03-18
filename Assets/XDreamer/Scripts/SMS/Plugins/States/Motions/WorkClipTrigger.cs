using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    public interface ITriggerPoint
    {
        double triggerPercent { get; set; }

        ETriggerDirection direction { get; set; }

        bool valid { get; set; }
    }

    [Name("触发方向")]
    public enum ETriggerDirection
    {
        [Name("递增方向")]
        Increase,

        [Name("递减方向")]
        Descending,

        [Name("双向")]
        Both,
    }

    public abstract class WorkClipTriggger<T> : WorkClip<T>, ITriggerPoint where T : WorkClipTriggger<T>
    {
        [Group("触发")]
        [Name("触发百分比")]
        [Range(0,1)]
        public double _triggerPercent = 0.05f;

        [Name("递增方向")]
        [EnumPopup]
        public ETriggerDirection _direction = ETriggerDirection.Increase;

        private bool _valid = true;

        protected double lastPercent = -1;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (valid) lastPercent = 0;
            //Debug.Log("OnEntry name:" + parent.name + ",valid:"+ valid+ ",lastPercent:"+ lastPercent);
        }

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (!valid) return;
            var p = percent.percentOfWorkCurve;
            // 初始化过后
            if (lastPercent >= 0)
            {
                switch(direction)
                {
                    case ETriggerDirection.Increase:
                        {
                            if (triggerPercent > lastPercent && triggerPercent < p)
                            {
                                OnTrigger();
                                    //Debug.Log("trigger name:" + parent.name + ",percent:" + percent + ",lastPercent:" + lastPercent + ",triggerPercent:" + triggerPercent);
                            }
                            break;
                        }
                    case ETriggerDirection.Descending:
                        {
                            if (triggerPercent < lastPercent && triggerPercent > p)
                            {
                                OnTrigger();
                            }
                            break;
                        }
                    case ETriggerDirection.Both:
                        {
                            if ((triggerPercent > lastPercent && triggerPercent < p) ||
                                (triggerPercent < lastPercent && triggerPercent > p))
                            {
                                OnTrigger();
                            }
                            break;
                        }
                }
            }
            //Debug.Log("trigger name 1:" + parent.name + ",percent:" + percent + ",lastPercent:" + lastPercent + ",triggerPercent:" + triggerPercent);
            lastPercent = p;
        }

        protected abstract void OnTrigger();

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
