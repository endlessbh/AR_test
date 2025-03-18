using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    public abstract class SingleComponentEnable<TStateCmponent, TComponent> : StateComponent<TStateCmponent>
        where TStateCmponent : SingleComponentEnable<TStateCmponent, TComponent>
        where TComponent : Component
    {
        [Name("组件")]
        [ComponentPopup]
        public TComponent component;

        [Name("初始化启用")]
        [EnumPopup]
        public EBool initEnable = EBool.None;

        [Name("进入启用")]
        [EnumPopup]
        public EBool entryEnable = EBool.Yes;

        [Name("退出启用")]
        [EnumPopup]
        public EBool exitEnable = EBool.No;

        public override bool Init(StateData data)
        {
            SetEnable(initEnable);
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetEnable(entryEnable);
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            SetEnable(exitEnable);
        }

        public override bool Finished()
        {
            return true;
        }

        public void SetEnable(EBool enable)
        {
            component.XSetEnable(enable);
        }
    }
}
