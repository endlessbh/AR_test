using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Motions
{
    public abstract class Motion<T> : WorkClip<T>, IMotion
        where T : Motion<T>, IMotion
    {
        public override bool Init(StateData data)
        {
            base.Init(data);
            initRecorder.Record(self);
            return true;
        }

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            recorder.SetPercent(percent);
        }

        public override void OnEntry(StateData data)
        {
            entryRecorder.Clear();
            entryRecorder.Record(self);
            base.OnEntry(data);
        }

        public virtual IPercentRecorder<T> recorder => useInitData ? initRecorder : entryRecorder;

        public abstract IPercentRecorder<T> initRecorder { get; }

        public abstract IPercentRecorder<T> entryRecorder { get; }

        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch(data.dataRule)
            {
                case EDataRule.Init:
                    {
                        initRecorder.Recover();
                        break;
                    }
                case EDataRule.Entry:
                    {
                        entryRecorder.Recover();
                        break;
                    }
            }
        }
    }

    public abstract class Motion<T, TRecorder> : Motion<T>
        where T : Motion<T, TRecorder>
        where TRecorder : class, IPercentRecorder<T>, new()
    {
        public override IPercentRecorder<T> initRecorder { get; } = new TRecorder();

        public override IPercentRecorder<T> entryRecorder { get; } = new TRecorder();
    }

    public interface IMotion : ILoopWorkClip { }

}
