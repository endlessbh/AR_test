using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginsCameras.Tools.Tweens;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机切换补间器
    /// </summary>
    [Name("相机切换补间器")]
    [DisallowMultipleComponent]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController), */nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Path)]
    public class CameraSwitchTweener : BaseCameraSwitchTweener
    {
        /// <summary>
        /// 补间规则
        /// </summary>
        [Name("补间规则")]
        [EnumPopup]
        public ETweenRule _tweenRule = ETweenRule.Default;

        /// <summary>
        /// 无补间:不做补间,到时间直接切换
        /// </summary>
        [Name("无补间")]
        [Tip("不做补间,到时间直接切换", "Do not make up the tween, and switch directly at the time")]
        [HideInSuperInspector]
        public NoneTweener noneTweener = new NoneTweener();

        /// <summary>
        /// 默认补间:使用相机变换器中默认自带的补间方法执行相机补间
        /// </summary>
        [Name("默认补间")]
        [Tip("使用相机变换器中默认自带的补间方法执行相机补间", "Use the default built-in interpolation method in the camera converter to perform camera interpolation")]
        [HideInSuperInspector]
        public DefaultTweener defaultTweener = new DefaultTweener();

        /// <summary>
        /// 获取补间器
        /// </summary>
        /// <returns></returns>
        public BaseTweener GetTweener()
        {
            switch (_tweenRule)
            {
                case ETweenRule.None: return noneTweener;
                case ETweenRule.Default:
                default: return defaultTweener;
            }
        }

        /// <summary>
        /// 本次切换使用的补间对象
        /// </summary>
        private BaseTweener _tweener;

        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>  
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public override bool OnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch)
        {
            _tweener = GetTweener();
            return _tweener.OnWillBeginMustSwitch(this, targetCameraController, directSwitch);
        }

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public override void OnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            if (_tweener == null) _tweener = GetTweener();
            _tweener.OnBeginSwitchTween(this, targetCameraController, duration, onCompleted);
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public override void OnEndSwitchTween(BaseCameraMainController newCurrentCameraController)
        {
            _tweener?.OnEndSwitchTween(this, newCurrentCameraController);
            _tweener = null;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (cameraTransformer is CameraTransformer transformer && !transformer.cameraSwitchTweener)
            {
                transformer.cameraSwitchTweener = this;
            }
        }
    }
}
