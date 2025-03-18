using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础相机工具控制器：为所有相机工具组件的基类
    /// </summary>
    public abstract class BaseCameraToolController : BaseCameraSubController, ICameraControllerEvent
    {
        #region 基础设置

        /// <summary>
        /// 运行时平台的可用性规则:在不同运行时平台本组件的可用规则，即根据不同的运行时平台确定本组件的可用性；本规则仅在组件初始化时执行一次；
        /// </summary>
        [Group("基础设置", defaultIsExpanded = false)]
        [Name("运行时平台的可用性规则")]
        [Tip("在不同运行时平台本组件的可用规则，即根据不同的运行时平台确定本组件的可用性；本规则仅在组件初始化时执行一次；", "Availability rules of this component in different runtime platforms, that is, determine the availability of this component according to different runtime platforms; This rule is executed only once during component initialization;")]
        public EBoolRuntimePlatformInfo _enableRuleByRuntimePlatform = new EBoolRuntimePlatformInfo();

        /// <summary>
        /// 切换时可用性规则：当前组件在所属相机控制器做切换操作时的处理规则
        /// </summary>
        [EndGroup(true)]
        [Name("切换时可用性规则")]
        [Tip("当前组件在所属相机控制器做切换操作时的处理规则", "The processing rules of the current component when switching the camera controller to which it belongs")]
        [EnumPopup]
        public EEnableRuleOnSwitch _enableRuleOnSwitch = EEnableRuleOnSwitch.BeginDisable_WillEndRecover;

        private bool tmpEnableOnSwitch = false;

        /// <summary>
        /// 切换时可用性规则
        /// </summary>
        [Name("切换时可用性规则")]
        public enum EEnableRuleOnSwitch
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            [Name("开始时禁用-将要结束时还原")]
            BeginDisable_WillEndRecover,

            /// <summary>
            /// 开始时禁用-结束时还原
            /// </summary>
            [Name("开始时禁用-结束时还原")]
            BeginDisable_EndRecover,

            /// <summary>
            /// 开始时禁用-将要结束时启用
            /// </summary>
            [Name("开始时禁用-将要结束时启用")]
            BeginDisable_WillEndEnable,

            /// <summary>
            /// 开始时禁用-结束时启用
            /// </summary>
            [Name("开始时禁用-结束时启用")]
            BeginDisable_EndEnable,
        }

        #endregion

        #region 相机控制器事件ICameraControllerEvent

        /// <summary>
        /// 当将要开始切换相机控制器之前回调，即将由旧相机控制器切换到新相机控制器；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public virtual void OnBeginSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            if (from != cameraController) return;
            switch (_enableRuleOnSwitch)
            {
                case EEnableRuleOnSwitch.BeginDisable_WillEndRecover:
                case EEnableRuleOnSwitch.BeginDisable_EndRecover:
                    {
                        tmpEnableOnSwitch = this.enabled;
                        this.enabled = false;
                        break;
                    }
                case EEnableRuleOnSwitch.BeginDisable_WillEndEnable:
                case EEnableRuleOnSwitch.BeginDisable_EndEnable:
                    {
                        this.enabled = false;
                        break;
                    }
            }
        }

        /// <summary>
        /// 当将要结束切换之前回调的事件；即旧相机控制器(即当前相机控制器)已经切换到新相机控制器的位置与旋转（如果需要补间）之后回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public virtual void OnWillEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            if (from != cameraController) return;
            switch (_enableRuleOnSwitch)
            {
                case EEnableRuleOnSwitch.BeginDisable_WillEndRecover:
                    {
                        this.enabled = tmpEnableOnSwitch;
                        break;
                    }
                case EEnableRuleOnSwitch.BeginDisable_EndEnable:
                    {
                        this.enabled = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；空函数；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        public virtual void OnWillSwitchToLast(BaseCameraMainController from) { }

        /// <summary>
        /// 当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；空函数；
        /// </summary>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public virtual void OnSwitchedToCurrent(BaseCameraMainController to) { }

        /// <summary>
        /// 当将要已经切换相机控制器之后回调的事件；
        /// </summary>
        /// <param name="from">旧相机控制器</param>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public virtual void OnEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            if (from != cameraController) return;
            switch (_enableRuleOnSwitch)
            {
                case EEnableRuleOnSwitch.BeginDisable_EndRecover:
                    {
                        this.enabled = tmpEnableOnSwitch;
                        break;
                    }
                case EEnableRuleOnSwitch.BeginDisable_EndEnable:
                    {
                        this.enabled = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 当相机主控制器组件启用时回调的事件；空函数；
        /// </summary>
        /// <param name="cameraController"></param>
        public virtual void OnEnabled(BaseCameraMainController cameraController) { }

        /// <summary>
        /// 当相机主控制器组件禁用时回调的事件；空函数；
        /// </summary>
        /// <param name="cameraController"></param>
        public virtual void OnDisabled(BaseCameraMainController cameraController) { }

        #endregion

        #region MB方法

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public virtual void Awake()
        {
            CameraControllerEvent.Register(this);
            if (enabled)//需要保证当前是可用的
            {
                this.XSetEnable(_enableRuleByRuntimePlatform.GetValueOfCurrentRuntimePlatform());
            }
        }

        /// <summary>
        /// 销毁:由事件各自销毁
        /// </summary>
        public virtual void OnDestroy()
        {
            CameraControllerEvent.Unregister(this);
        }

        #endregion
    }

    /// <summary>
    /// 相机控制器事件枚举
    /// </summary>
    [LanguageFileOutput]
    [Name("相机控制器事件")]
    public enum ECameraControllerEvent
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 当开始切换
        /// </summary>
        [Name("当开始切换")]
        OnBeginSwitch,

        /// <summary>
        /// 当将要结束切换
        /// </summary>
        [Name("当将要结束切换")]
        OnWillEndSwitch,

        /// <summary>
        /// 当将要切换为上个
        /// </summary>
        [Name("当将要切换为上个")]
        OnWillSwitchToLast,

        /// <summary>
        /// 当已切换为当前
        /// </summary>
        [Name("当已切换为当前")]
        OnSwitchedToCurrent,

        /// <summary>
        /// 当结束切换
        /// </summary>
        [Name("当结束切换")]
        OnEndSwitch,

        /// <summary>
        /// 当已启用
        /// </summary>
        [Name("当已启用")]
        OnEnabled,

        /// <summary>
        /// 当已禁用
        /// </summary>
        [Name("当已禁用")]
        OnDisabled,
    }
}
