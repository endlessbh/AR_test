using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;
using UnityEngine.Events;
using XCSJ.Extension.Base.XUnityEngine.XEvents;
using System.Reflection;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机控制器事件
    /// </summary>
    [Name("相机控制器事件")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraController))]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [DisallowMultipleComponent]
    public class CameraControllerEvent : BaseCameraToolController, IDynamicLabel
    {
        /// <summary>
        /// 事件列表
        /// </summary>
        [Name("事件列表")]
        public List<CameraControllerCallbackEvent> _events = new List<CameraControllerCallbackEvent>();

        private void Handle(ECameraControllerEvent cameraControllerEvent, BaseCameraMainController from, BaseCameraMainController to)
        {
            foreach (var e in _events)
            {
                e.Invoke(cameraControllerEvent, from, to);
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="cameraControllerEvent"></param>
        /// <param name="gameObject"></param>
        /// <param name="active"></param>
        public void Add(ECameraControllerEvent cameraControllerEvent, GameObject gameObject, bool active)
        {
            this.XModifyProperty(() =>
            {
                var e = _events.FirstOrNew(ce => ce._cameraControllerEvent == cameraControllerEvent, ce =>
                {
                    ce._cameraControllerEvent = cameraControllerEvent;
                    _events.Add(ce);
                });
                new UnityEventBase_LinkType(e).AddBoolPersistentListener(gameObject.SetActive, active);
            });
        }

        /// <summary>
        /// 相机控制器回调事件
        /// </summary>
        [Serializable]
        public class CameraControllerCallbackEvent : UnityEvent<BaseCameraMainController, BaseCameraMainController>
        {
            /// <summary>
            /// 相机控制器事件
            /// </summary>
            [Name("相机控制器事件")]
            [EnumPopup]
            public ECameraControllerEvent _cameraControllerEvent = ECameraControllerEvent.None;

            /// <summary>
            /// 调用
            /// </summary>
            /// <param name="cameraControllerEvent"></param>
            /// <param name="from"></param>
            /// <param name="to"></param>
            public void Invoke(ECameraControllerEvent cameraControllerEvent, BaseCameraMainController from, BaseCameraMainController to)
            {
                if (_cameraControllerEvent != cameraControllerEvent) return;
                Invoke(from, to);
            }
        }

        #region 事件

        /// <summary>
        /// 当将要开始切换相机控制器之前回调，即将由旧相机控制器切换到新相机控制器；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public override void OnBeginSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnBeginSwitch(from, to);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnBeginSwitch, from, to);
        }

        /// <summary>
        /// 当将要结束切换之前回调的事件；即旧相机控制器(即当前相机控制器)已经切换到新相机控制器的位置与旋转（如果需要补间）之后回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public override void OnWillEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnWillEndSwitch(from, to);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnWillEndSwitch, from, to);
        }

        /// <summary>
        /// 当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        public override void OnWillSwitchToLast(BaseCameraMainController from)
        {
            //base.OnWillSwitchToLast(from);
            if (from != cameraController) return;

            Handle(ECameraControllerEvent.OnWillSwitchToLast, from, default);
        }

        /// <summary>
        /// 当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；
        /// </summary>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public override void OnSwitchedToCurrent(BaseCameraMainController to)
        {
            //base.OnSwitchedToCurrent(to);
            if (to != cameraController) return;

            Handle(ECameraControllerEvent.OnSwitchedToCurrent, default, to);
        }

        /// <summary>
        /// 当将要已经切换相机控制器之后回调的事件；
        /// </summary>
        /// <param name="from">旧相机控制器</param>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public override void OnEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            base.OnEndSwitch(from, to);
            if (to != cameraController) return;

            Handle(ECameraControllerEvent.OnEndSwitch, from, to);
        }

        /// <summary>
        /// 当相机主控制器组件启用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public override void OnEnabled(BaseCameraMainController cameraController)
        {
            //base.OnEnabled(cameraController);
            if (this.cameraController != cameraController) return;

            Handle(ECameraControllerEvent.OnEnabled, cameraController, cameraController);
        }

        /// <summary>
        /// 当相机主控制器组件禁用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public override void OnDisabled(BaseCameraMainController cameraController)
        {
            //base.OnEnabled(cameraController);
            if (this.cameraController != cameraController) return;

            Handle(ECameraControllerEvent.OnEndSwitch, cameraController, cameraController);
        }

        #endregion

        #region IDynamicLabel

        GUIContent IDynamicLabel.GetDynamicLabel(string propertyPath, FieldInfo fieldInfo, GUIContent label)
        {
            switch (fieldInfo.Name)
            {
                case nameof(_events):
                    {
                        if (PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out var index))
                        {
                            var c = CommonFun.NameTip(_events[index]._cameraControllerEvent);
                            return new GUIContent((index + 1).ToString() + "." + c.text, c.tooltip);
                        }
                        break;
                    }
            }
            return null;
        }

        #endregion
    }
}
