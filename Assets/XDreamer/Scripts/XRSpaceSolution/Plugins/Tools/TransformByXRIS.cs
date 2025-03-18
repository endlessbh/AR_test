using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.UVRPN.Core;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 变换通过XRIS
    /// </summary>
    [Name("变换通过XRIS")]
    [Tip("通过XRIS控制变换的TRS(位置、旋转、缩放)", "Analog input and output of controller pose through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    //[Tool(XRITHelper.IO, nameof(AnalogController))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class TransformByXRIS : InteractProvider
    {
        /// <summary>
        /// 变换处理
        /// </summary>
        [Name("变换处理")]
        [OnlyMemberElements]
        public TransformHandler _transformHandler = new TransformHandler();

        Transform thisTransform;

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;

            thisTransform = transform;
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onXRAnswerReceived -= OnXRAnswerReceived;
        }

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (_transformHandler.Handle(answer))
            {
                var transformByXBox = GetComponents<TransformByXBox>().FirstOrDefault(t => t && t.usageForXRIS == _transformHandler.actionName);
                if (!transformByXBox)
                {
                    transformByXBox = this.XAddComponent<TransformByXBox>();
                    transformByXBox.usageForXRIS = _transformHandler.actionName;
                }

                transformByXBox._speed = _transformHandler.speed;
                transformByXBox._transformTRS = _transformHandler.transformTRS;
                transformByXBox.XSetEnable(_transformHandler.config.enable);

                var controlData = transformByXBox._controlData;

                controlData._nx = _transformHandler.nx.xboxConfig.enable ? _transformHandler.nx.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._nxDeadZone = _transformHandler.nx.xboxConfig.deadZone.ToVector2();

                controlData._px = _transformHandler.px.xboxConfig.enable ? _transformHandler.px.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._pxDeadZone = _transformHandler.px.xboxConfig.deadZone.ToVector2();

                controlData._ny = _transformHandler.ny.xboxConfig.enable ? _transformHandler.ny.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._nyDeadZone = _transformHandler.ny.xboxConfig.deadZone.ToVector2();

                controlData._py = _transformHandler.py.xboxConfig.enable ? _transformHandler.py.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._pyDeadZone = _transformHandler.py.xboxConfig.deadZone.ToVector2();

                controlData._nz = _transformHandler.nz.xboxConfig.enable ? _transformHandler.nz.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._nzDeadZone = _transformHandler.nz.xboxConfig.deadZone.ToVector2();

                controlData._pz = _transformHandler.pz.xboxConfig.enable ? _transformHandler.pz.xboxConfig.axisAndButton : EXBoxAxisAndButton.None;
                controlData._pzDeadZone = _transformHandler.pz.xboxConfig.deadZone.ToVector2();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_transformHandler.config.enable) return;

            var offset = _transformHandler.GetSpeedOffset() * Time.deltaTime;

            _transformHandler.transformTRS.TRS(thisTransform, offset);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _transformHandler.actionName = "变换TRS";
        }
    }
}
