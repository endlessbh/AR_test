using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.UVRPN.Core;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 姿态IO通过XRIS
    /// </summary>
    [Name("姿态IO通过XRIS")]
    [Tip("通过XRIS模拟控制器姿态的输入输出", "Analog input and output of controller pose through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    public class PoseIOByXRIS : BaseAnalogControllerIO, IPoseIO
    {
        /// <summary>
        /// 姿态
        /// </summary>
        [Name("姿态处理")]
        [OnlyMemberElements]
        public PoseHandler _poseHandler = new PoseHandler();

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRAnswerReceived;
        }

        private void OnXRAnswerReceived(XRAnswer answer) => _poseHandler.Handle(answer);

        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation) => _poseHandler.TryGetPose(out position, out rotation) ? InputTrackingState.Position | InputTrackingState.Rotation : InputTrackingState.None;
    }
}
