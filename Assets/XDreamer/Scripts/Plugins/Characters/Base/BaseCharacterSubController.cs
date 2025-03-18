using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Components;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Characters.Base
{
    /// <summary>
    /// 基础角色子控制器
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class BaseCharacterSubController : BaseSubController<XCharacterController>
    {
        /// <summary>
        /// 角色运动
        /// </summary>
        public CharacterMovement movement => mainController.movement;

        /// <summary>
        /// 角色变换
        /// </summary>
        public Transform characterTransform => mainController.characterTransform;

        /// <summary>
        /// 角色相机控制器
        /// </summary>
        public BaseCharacterCameraController characterCameraController => mainController.characterCameraController;

        /// <summary>
        /// 相机控制器
        /// </summary>
        public BaseCameraMainController cameraController => characterCameraController.cameraMainController;

        /// <summary>
        /// 角色相机
        /// </summary>
        public Camera characterCamera => characterCameraController.mainCamera;

        /// <summary>
        /// 角色变换器
        /// </summary>
        public BaseCharacterTransformer characterTransformer => mainController.characterTransformer;
    }

    /// <summary>
    /// 基础角色核心控制器
    /// </summary>
    public abstract class BaseCharacterCoreController : BaseCharacterSubController { }

    /// <summary>
    /// 基础角色工具控制器
    /// </summary>
    public abstract class BaseCharacterToolController : BaseCharacterSubController
    {
        /// <summary>
        /// 运行时平台的可用性规则:在不同运行时平台本组件的可用规则，即根据不同的运行时平台确定本组件的可用性；本规则仅在组件初始化时执行一次；
        /// </summary>
        [Group("基础设置", defaultIsExpanded = false)]
        [EndGroup]
        [Name("运行时平台的可用性规则")]
        [Tip("在不同运行时平台本组件的可用规则，即根据不同的运行时平台确定本组件的可用性；本规则仅在组件初始化时执行一次；", "Availability rules of this component in different runtime platforms, that is, determine the availability of this component according to different runtime platforms; This rule is executed only once during component initialization;")]
        public EBoolRuntimePlatformInfo _enableRuleByRuntimePlatform = new EBoolRuntimePlatformInfo();

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public virtual void Awake()
        {
            this.XSetEnable(_enableRuleByRuntimePlatform.GetValueOfCurrentRuntimePlatform());
        }
    }
}
