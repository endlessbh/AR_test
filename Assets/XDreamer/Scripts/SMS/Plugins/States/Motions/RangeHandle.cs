using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.Scripts;
using XCSJ.Interfaces;

namespace XCSJ.PluginSMS.States.Motions
{
    public abstract class RangeHandle<T, TRecorder> : Motion<T, TRecorder>, IRangeHandle
        where T : RangeHandle<T, TRecorder>
        where TRecorder : class, IRangeHandleRecorder<T>, new()
    {
        [Group("区间设置")]
        [Name("进入时")]
        [EnumPopup]
        public EBool onEntry = EBool.No;

        [Name("区间左")]
        [EnumPopup]
        public EBool leftRange = EBool.No;

        [Name("区间内")]
        [EnumPopup]
        [FormerlySerializedAs("enabled")]
        [FormerlySerializedAs("active")]
        public EBool inRange = EBool.Yes;

        [Name("区间右")]
        [EnumPopup]
        public EBool rightRange = EBool.No;

        [Name("退出时")]
        [EnumPopup]
        public EBool onExit = EBool.No;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            (recorder as IRangeHandleRecorder<T>)?.SetRange(onEntry, ELifecycleEventLite.OnEntry);
        }

        public override void OnExit(StateData stateData)
        {
            (recorder as IRangeHandleRecorder<T>)?.SetRange(onExit, ELifecycleEventLite.OnExit);
            base.OnExit(stateData);
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
        public override void OnOutOfBounds(IWorkClipPlayer player, EOutOfBoundsMode outOfBoundsMode, double percent, StateData stateData, double lastPercent, IStateWorkClip stateWorkClip)
        {
            switch (outOfBoundsMode)
            {
                case EOutOfBoundsMode.Left:
                    {
                        if (setPercentOnEntry)
                        {
                            SetPercent(percentOnEntry, stateData);
                        }
                        (recorder as IRangeHandleRecorder<T>)?.SetRange(onEntry, ELifecycleEventLite.OnEntry);
                        break;
                    }
                case EOutOfBoundsMode.Right:
                    {
                        if (setPercentOnExit)
                        {
                            SetPercent(percentOnExit, stateData);
                        }
                        (recorder as IRangeHandleRecorder<T>)?.SetRange(onExit, ELifecycleEventLite.OnExit);
                        break;
                    }
            }
        }
    }

    public interface IRangeHandle : IMotion { }
    public interface IRangeHandleRecorder<T> : IPercentRecorder<T>
    {
        void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate);
    }
}
