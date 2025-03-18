using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.GameObjects;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    [RequireComponent(typeof(GameObjectSet))]
    public abstract class RendererRangeHandle<T> : RangeHandle<T, RendererRangeHandle<T>.Recorder>, IRendererRangeHandle
        where T : RendererRangeHandle<T>
    {
        [Name("操作对象")]
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("包含成员")]
        public bool includeChildren = true;

        //public override void OnExit(StateData data)
        //{
        //    base.OnExit(data);

        //    if (!setPercentOnExit)
        //    {
        //        recorder.Recover();
        //    }
        //}

        protected abstract void OnSetPercent(Recorder recorder, Percent percent);

        protected abstract void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate);

        public class Recorder : RendererRecorder, IRangeHandleRecorder<T>
        {
            public T rangeHandle;

            public void Record(T rangeHandle)
            {
                this.rangeHandle = rangeHandle;
                if (!rangeHandle.gameObjectSet) return;
                _records.Clear();

                foreach (var go in rangeHandle.gameObjectSet.objects)
                {
                    if (go)
                    {
                        Record(go);
                        if (rangeHandle.includeChildren) Record(go.GetComponentsInChildren<Renderer>());
                    }
                }
            }

            public void SetPercent(Percent percent)
            {
                rangeHandle.OnSetPercent(this, percent);
            }

            public void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
            {
                rangeHandle.OnSetPercent(this, boolValue, lifecycleEventLite);
            }
        }
    }

    public interface IRendererRangeHandle : IRangeHandle { }

    public abstract class RendererRangeHandle<T, TValue> : RendererRangeHandle<T>
       where T : RendererRangeHandle<T, TValue>
    {
        [Name("进入时值")]
        [HideInSuperInspector(nameof(onEntry), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onEntry), EValidityCheckType.Equal, EBool.None)]
        public TValue onEntryValue = default;

        [Name("区间左值")]
        [HideInSuperInspector(nameof(leftRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(leftRange), EValidityCheckType.Equal, EBool.None)]
        public TValue leftValue = default;

        [Name("区间内值")]
        [HideInSuperInspector(nameof(inRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(inRange), EValidityCheckType.Equal, EBool.None)]
        public TValue inValue = default;

        [Name("区间右值")]
        [HideInSuperInspector(nameof(rightRange), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(rightRange), EValidityCheckType.Equal, EBool.None)]
        public TValue rightValue = default;

        [Name("退出时值")]
        [HideInSuperInspector(nameof(onExit), EValidityCheckType.Equal | EValidityCheckType.Or, EBool.No, nameof(onExit), EValidityCheckType.Equal, EBool.None)]
        public TValue onExitValue = default;

        protected bool onEntrySwitchFlag = false;
        protected bool leftSwitchFlag = false;
        protected bool inSwitchFlag = false;
        protected bool rightSwitchFlag = false;
        protected bool onExitSwitchFlag = false;

        protected override void OnSetPercent(Recorder recorder, Percent percent)
        {
            if (percent.leftRange)
            {
                OnSetValue(recorder, leftRange, ref leftValue, ref leftSwitchFlag);
            }
            else if (percent.rightRange)
            {
                OnSetValue(recorder, rightRange, ref rightValue, ref rightSwitchFlag);
            }
            else
            {
                OnSetValue(recorder, inRange, ref inValue, ref inSwitchFlag);
            }
        }

        protected override void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
        {
            switch (lifecycleEventLite)
            {
                case ELifecycleEventLite.OnEntry:
                    {
                        OnSetValue(recorder, onEntry, ref onEntryValue, ref onEntrySwitchFlag);
                        break;
                    }
                case ELifecycleEventLite.OnExit:
                    {
                        OnSetValue(recorder, onExit, ref onExitValue, ref onExitSwitchFlag);
                        break;
                    }
            }
        }

        protected abstract void OnSetValue(Recorder recorder, EBool boolValue, ref TValue value, ref bool switchFlag);
    }
}
