#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens点击", typeof(HoloLensManager))]
    [Name("HoloLens点击", nameof(Click))]
    [XCSJ.Attributes.Icon(index = 34992)]
    [KeyNode(nameof(ITrigger), "触发器")]
    [Tip("HoloLens点击组件事件是检测用户是否凝视某个游戏对象并做出点击的手势的触发器。当条件满足后，组件切换为完成态。", "Hololens click component event is a trigger to detect whether a user stares at a game object and makes a click gesture. When the conditions are met, the component switches to the completed state.")]
    public class Click : HoloLensStateComponent<Click>
    {
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens点击", typeof(HoloLensManager))]
#endif
        [Name("HoloLens点击", nameof(Click))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens点击组件事件是检测用户是否凝视某个游戏对象并做出点击的手势的触发器。当条件满足后，组件切换为完成态。", "Hololens click component event is a trigger to detect whether a user stares at a game object and makes a click gesture. When the conditions are met, the component switches to the completed state.")]
        public static State CreateClick(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject go;

        [Name("点击类型")]
        [EnumPopup]
        public EClick clickType = EClick.Click;

        [Name("自动添加碰撞体")]
        [Tip("没有碰撞体，点击事件就不会产生！", "Without collision body, click event will not occur!")]
        public bool autoAddCollider = true;

        protected override GameObject gameObj => go;

        private Collider collider;

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
                inputListener.onDownCallback += OnInputDown;
                inputListener.onUpCallback += OnInputUp;
                inputListener.onClickCallback += OnInputClicked;
            }
        }

        public override void OnAfterExit(StateData data)
        {
            if (inputListener)
            {
                inputListener.onDownCallback -= OnInputDown;
                inputListener.onUpCallback -= OnInputUp;
                inputListener.onClickCallback -= OnInputClicked;
            }

            base.OnAfterExit(data);
        }

#if XDREAMER_HOLOLENS
        protected void OnInputDown(InputEventData eventData)
#else
        protected void OnInputDown(BaseEventData eventData)
#endif
        {
            if (clickType == EClick.Down)
            {
                finished = true;
            }
        }

#if XDREAMER_HOLOLENS
        protected void OnInputUp(InputEventData eventData)
#else
        protected void OnInputUp(BaseEventData eventData)
#endif
        {
            if (clickType == EClick.Up)
            {
                finished = true;
            }
        }

#if XDREAMER_HOLOLENS
        protected void OnInputClicked(InputClickedEventData eventData)
#else
        protected void OnInputClicked(BaseEventData eventData)
#endif
        {
            if(clickType== EClick.Click)
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
            return (go ? go.name : "") + "[" + CommonFun.Name(clickType) + "]";
        }
    }
}
