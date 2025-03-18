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
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
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
    /// 相机姿态通过XRIS
    /// </summary>
    [Name("相机姿态通过XRIS")]
    [Tip("通过XR网络控制相机的姿态（移动与旋转）", "Control camera pose (movement and rotation) through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraTransformer))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [DisallowMultipleComponent]
    [Owner(typeof(XRSpaceSolutionManager))]
    public class CameraPoseByXRIS : BaseCameraTransformController
    {
        /// <summary>
        /// 姿态处理
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

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if(_poseHandler.TryGetPose(out var position, out var rotation))
            {
                var mainTransform = cameraTransformer.mainTransform;
                mainTransform.localPosition = position;
                mainTransform.localRotation = rotation;
            }
        }

        private void OnXRAnswerReceived(XRAnswer answer) => _poseHandler.Handle(answer);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _poseHandler.actionName = CommonFun.Name(EActionName.HMD);;
        }
    }
}
