#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens聚焦", typeof(HoloLensManager))]
    [Name("HoloLens聚焦", nameof(Focus))]
    [XCSJ.Attributes.Icon(index = 34994)]
    [KeyNode(nameof(ITrigger),"触发器")]
    [Tip("HoloLens聚焦组件是用于判断用户是否凝视在某个游戏对象的触发器。当用户凝视在指定的游戏对象上后，组件切换为完成态。", "The hololens focus component is a trigger used to determine whether the user is staring at a game object. When the user stares at the specified game object, the component switches to the completed state.")]
    public class Focus : HoloLensStateComponent<Focus>, ITrigger
    {
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens聚焦", typeof(HoloLensManager))]
#endif
        [Name("HoloLens聚焦", nameof(Focus))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens聚焦组件是用于判断用户是否凝视在某个游戏对象的触发器。当用户凝视在指定的游戏对象上后，组件切换为完成态。", "The hololens focus component is a trigger used to determine whether the user is staring at a game object. When the user stares at the specified game object, the component switches to the completed state.")]
        public static State CreateFocus(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go;

        private Collider collider;

        [Name("聚焦类型")]
        [EnumPopup]
        public EFocus fucusType = EFocus.Enter;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool autoAddCollider = true;

        protected override GameObject gameObj => go;

        public override bool Init(StateData data)
        {
            if (go)
            {
                collider = go.GetComponent<Collider>();
                if (!collider && autoAddCollider)
                {
                    collider = go.GetComponent<MeshRenderer>() ? (go.AddComponent<MeshCollider>() as Collider) : (go.AddComponent<BoxCollider>() as Collider);
                }
            }
            return base.Init(data);
        }

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (inputListener)
            {
                inputListener.onFocusEnterCallback += OnFocusEnter;
                inputListener.onFocusExitCallback += OnFocusExit;
            }
        }

        public override void OnAfterExit(StateData data)
        {
            if(inputListener)
            {
                inputListener.onFocusEnterCallback -= OnFocusEnter;
                inputListener.onFocusExitCallback -= OnFocusExit;
            }

            base.OnAfterExit(data);
        }

        protected void OnFocusEnter()
        {
            if (fucusType == EFocus.Enter)
            {
                finished = true;
            }
        }

        protected void OnFocusExit()
        {
            if (fucusType == EFocus.Exit)
            {
                finished = true;
            }
        }

        public override bool DataValidity()
        {
            return go;
        }

        public override string ToFriendlyString()
        {
            return (go ? go.name : "") +"["+CommonFun.Name(fucusType)+"]";
        }
    }
}
