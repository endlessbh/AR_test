using System;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Controllers;

namespace XCSJ.PluginsCameras.Tools.Tweens
{
    /// <summary>
    /// 默认补间:使用相机变换器中默认自带的补间方法执行相机补间
    /// </summary>
    [Name("默认补间")]
    [Tip("使用相机变换器中默认自带的补间方法执行相机补间", "Use the default built-in interpolation method in the camera converter to perform camera interpolation")]
    [Serializable]
    public class DefaultTweener : BaseTweener
    {
        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public override void OnBeginSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            cameraSwitchTweener.cameraTransformer.DefaultOnBeginSwitchTween(targetCameraController, duration, onCompleted);
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public override void OnEndSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController newCurrentCameraController)
        {
            cameraSwitchTweener.cameraTransformer.DefaultOnEndSwitchTween(newCurrentCameraController);
        }

        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>  
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public override bool OnWillBeginMustSwitch(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController targetCameraController, bool directSwitch)
        {
            return cameraSwitchTweener.cameraTransformer.DefaultOnWillBeginMustSwitch(targetCameraController, directSwitch);
        }
    }
}
