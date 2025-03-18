using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    [RequireComponent(typeof(GameObjectSet))]
    public abstract class TransformMotion<T> : Motion<T, TransformMotion<T>.Recorder> where T : TransformMotion<T>
    {
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        protected abstract void SetPercent(TransformRecorder.Info info, Percent percent);

        public class Recorder : TransformRecorder, IPercentRecorder<T>
        {
            private T motion;

            public void Record(T motion)
            {
                this.motion = motion;
                if (!motion.gameObjectSet) return;
                _records.Clear();
                Record(motion.gameObjectSet.objects);
            }

            public void SetPercent(Percent percent)
            {
                try
                {
                    foreach (var i in _records)
                    {
                        motion.SetPercent(i, percent);
                    }
                }
                catch { }
            }
        }
    }
}
