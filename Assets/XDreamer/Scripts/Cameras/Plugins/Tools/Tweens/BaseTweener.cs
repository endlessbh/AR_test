using System;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Controllers;

namespace XCSJ.PluginsCameras.Tools.Tweens
{
    /// <summary>
    /// 基础补间器
    /// </summary>
    public abstract class BaseTweener
    {
        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>  
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public abstract bool OnWillBeginMustSwitch(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController targetCameraController, bool directSwitch);

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public abstract void OnBeginSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController targetCameraController, float duration, Action onCompleted);

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public abstract void OnEndSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController newCurrentCameraController);
    }

    /// <summary>
    /// 补间规则
    /// </summary>
    [Name("补间规则")]
    public enum ETweenRule
    {
        /// <summary>
        /// 无:不做补间,到时间直接切换
        /// </summary>
        [Name("无")]
        [Tip("不做补间,到时间直接切换", "Do not make up tween, and switch directly at the time")]
        None,

        /// <summary>
        /// 默认：使用默认补间规则
        /// </summary>
        [Name("默认")]
        [Tip("使用相机变换器中默认自带的补间方法执行相机补间", "Use the default built-in interpolation method in the camera converter to perform camera interpolation")]
        Default,
    }
}
