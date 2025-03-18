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
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// 交互IO通过XRIS
    /// </summary>
    [Name("交互IO通过XRIS")]
    [Tip("通过XRIS模拟控制器交互的输入输出", "Analog input and output of controller interaction through XRIS")]
    [RequireManager(typeof(XRSpaceSolutionManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    //[Tool(XRSpaceSolutionHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    public class InteractIOByXRIS : BaseAnalogControllerIO, IInteractIO
    {
        /// <summary>
        /// 交互处理
        /// </summary>
        [Name("交互处理")]
        [OnlyMemberElements]
        public InteractHandler _interactHandler = new InteractHandler();

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

        private void OnXRAnswerReceived(XRAnswer answer)
        {
            if (_interactHandler.Handle(answer))
            {
                var interact = this.XGetOrAddComponent<InteractIOByXBox>();
                if (interact)
                {
                    interact.RegistIO();

                    var allowButtonInteract = _interactHandler.allowButtonInteract;

                    if (_interactHandler.select?.xboxConfig is XBoxConfig select)
                    {
                        interact.buttonOfSelect = allowButtonInteract && select.enable ? select.axisAndButton : EXBoxAxisAndButton.None;
                    }

                    if (_interactHandler.activate?.xboxConfig is XBoxConfig activate)
                    {
                        interact.buttonOfActivate = allowButtonInteract && activate.enable ? activate.axisAndButton : EXBoxAxisAndButton.None;
                    }

                    if (_interactHandler.ui?.xboxConfig is XBoxConfig ui)
                    {
                        interact.buttonOfUI = allowButtonInteract && ui.enable ? ui.axisAndButton : EXBoxAxisAndButton.None;
                    }
                }
            }
        }

        /// <summary>
        /// 是激活按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _interactHandler.Activate();

        /// <summary>
        /// 是选择按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _interactHandler.Select();

        /// <summary>
        /// 是UI按钮按下
        /// </summary>
        /// <param name="analogController"></param>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _interactHandler.UI();
    }
}
