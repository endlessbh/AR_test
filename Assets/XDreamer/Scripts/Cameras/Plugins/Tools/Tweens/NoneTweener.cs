using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Controllers;

namespace XCSJ.PluginsCameras.Tools.Tweens
{
    /// <summary>
    /// 无补间:不做补间,到时间直接切换
    /// </summary>
    [Name("无补间")]
    [Tip("不做补间,到时间直接切换", "Do not make up the room, and switch directly at the time")]
    [Serializable]
    public class NoneTweener : BaseTweener
    {
        /// <summary>
        /// 延时动作对象
        /// </summary>
        DelayAction delayAction;

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public override void OnBeginSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            delayAction = CommonFun.DelayCall(onCompleted, duration);
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="cameraSwitchTweener">相机切换补间器</param>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public override void OnEndSwitchTween(CameraSwitchTweener cameraSwitchTweener, BaseCameraMainController newCurrentCameraController)
        {
            delayAction = null;
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
            delayAction?.Kill();
            delayAction = null;
            return true;
        }
    }
}
