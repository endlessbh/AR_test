using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginsCameras.States
{
    /// <summary>
    /// 相机控制器切换事件
    /// </summary>
    [Name(Title, nameof(CameraControllerSwitchEvent))]
    [ComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Switch)]
    [Owner(typeof(CameraManager))]
    public class CameraControllerSwitchEvent : Trigger<CameraControllerSwitchEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机控制器切换事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CameraHelperExtension.StateLibCategoryName, typeof(CameraManager))]
        [StateComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
        [Name(Title, nameof(CameraControllerSwitchEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _cameraController;

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnSwitchedTo;

        private EEventType tmpEventType = EEventType.None;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            tmpEventType = _eventType;
            switch (tmpEventType)
            {
                case EEventType.None:
                    {
                        finished = true;
                        break;
                    }
                case EEventType.OnWillSwitchFrom:
                case EEventType.OnWillSwitchTo:
                    {
                        CameraControllerEvent.onWillEndSwitch += OnSwitch;
                        break;
                    }
                case EEventType.OnBeginSwitchFrom:
                case EEventType.OnBeginSwitchTo:
                    {
                        CameraControllerEvent.onBeginSwitch += OnSwitch;
                        break;
                    }
                case EEventType.OnWillEndSwitchFrom:
                case EEventType.OnWillEndSwitchTo:
                    {
                        CameraControllerEvent.onWillEndSwitch += OnSwitch;
                        break;
                    }
                case EEventType.OnWillSwitchToLast:
                    {
                        CameraControllerEvent.onWillSwitchToLast += OnSwitch;
                        break;
                    }
                case EEventType.OnSwitchedToCurrent:
                    {
                        CameraControllerEvent.onSwitchedToCurrent += OnSwitch;
                        break;
                    }
                case EEventType.OnEndSwitchFrom:
                case EEventType.OnEndSwitchTo:
                    {
                        CameraControllerEvent.onEndSwitch += OnSwitch;
                        break;
                    }
                case EEventType.OnSwitchedFrom:
                case EEventType.OnSwitchedTo:
                    {
                        CameraControllerEvent.onEndSwitch += OnSwitch;
                        break;
                    }
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            switch (tmpEventType)
            {
                case EEventType.OnWillSwitchFrom:
                case EEventType.OnWillSwitchTo:
                    {
                        CameraControllerEvent.onWillEndSwitch -= OnSwitch;
                        break;
                    }
                case EEventType.OnBeginSwitchFrom:
                case EEventType.OnBeginSwitchTo:
                    {
                        CameraControllerEvent.onBeginSwitch -= OnSwitch;
                        break;
                    }
                case EEventType.OnWillEndSwitchFrom:
                case EEventType.OnWillEndSwitchTo:
                    {
                        CameraControllerEvent.onWillEndSwitch -= OnSwitch;
                        break;
                    }
                case EEventType.OnWillSwitchToLast:
                    {
                        CameraControllerEvent.onWillSwitchToLast -= OnSwitch;
                        break;
                    }
                case EEventType.OnSwitchedToCurrent:
                    {
                        CameraControllerEvent.onSwitchedToCurrent -= OnSwitch;
                        break;
                    }
                case EEventType.OnEndSwitchFrom:
                case EEventType.OnEndSwitchTo:
                    {
                        CameraControllerEvent.onEndSwitch -= OnSwitch;
                        break;
                    }
                case EEventType.OnSwitchedFrom:
                case EEventType.OnSwitchedTo:
                    {
                        CameraControllerEvent.onEndSwitch -= OnSwitch;
                        break;
                    }
            }
        }

        void OnSwitch(BaseCameraMainController cameraController)
        {
            if (finished) return;
            switch (tmpEventType)
            {
                case EEventType.OnWillSwitchToLast:
                case EEventType.OnSwitchedToCurrent:
                    {
                        finished = _cameraController == cameraController;
                        break;
                    }
            }
        }

        void OnSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            if (finished) return;
            switch (tmpEventType)
            {
                case EEventType.OnWillSwitchFrom:
                case EEventType.OnBeginSwitchFrom:
                case EEventType.OnWillEndSwitchFrom:
                case EEventType.OnEndSwitchFrom:
                case EEventType.OnSwitchedFrom:
                    {
                        finished = _cameraController == from;
                        break;
                    }
                case EEventType.OnWillSwitchTo:
                case EEventType.OnBeginSwitchTo:
                case EEventType.OnWillEndSwitchTo:
                case EEventType.OnEndSwitchTo:
                case EEventType.OnSwitchedTo:
                    {
                        finished = _cameraController == to;
                        break;
                    }
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _cameraController ? _cameraController.name : "";

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _cameraController;
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当将要从相机控制器切换
            /// </summary>
            [Name("当将要从相机控制器切换")]
            OnWillSwitchFrom,

            /// <summary>
            /// 当将要切换到相机控制器
            /// </summary>
            [Name("当将要切换到相机控制器")]
            OnWillSwitchTo,

            /// <summary>
            /// 当开始从相机控制器切换
            /// </summary>
            [Name("当开始从相机控制器切换")]
            OnBeginSwitchFrom,

            /// <summary>
            /// 当开始切换到相机控制器
            /// </summary>
            [Name("当开始切换到相机控制器")]
            OnBeginSwitchTo,

            /// <summary>
            /// 当将要结束从相机控制器切换
            /// </summary>
            [Name("当将要结束从相机控制器切换")]
            OnWillEndSwitchFrom,

            /// <summary>
            /// 当将要结束切换到相机控制器
            /// </summary>
            [Name("当将要结束切换到相机控制器")]
            OnWillEndSwitchTo,

            /// <summary>
            /// 当将要切换相机控制器到上一个
            /// </summary>
            [Name("当将要切换相机控制器到上一个")]
            OnWillSwitchToLast,

            /// <summary>
            /// 当已切换相机控制器到当前
            /// </summary>
            [Name("当已切换相机控制器到当前")]
            OnSwitchedToCurrent,

            /// <summary>
            /// 当结束从相机控制器切换
            /// </summary>
            [Name("当结束从相机控制器切换")]
            OnEndSwitchFrom,

            /// <summary>
            /// 当结束切换到相机控制器
            /// </summary>
            [Name("当结束切换到相机控制器")]
            OnEndSwitchTo,

            /// <summary>
            /// 当已从相机控制器切换
            /// </summary>
            [Name("当已从相机控制器切换")]
            OnSwitchedFrom,

            /// <summary>
            /// 当已切换到相机控制器
            /// </summary>
            [Name("当已切换到相机控制器")]
            OnSwitchedTo,
        }
    }
}
