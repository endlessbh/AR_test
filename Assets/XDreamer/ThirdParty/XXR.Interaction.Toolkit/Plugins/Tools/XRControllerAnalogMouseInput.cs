using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.PluginsCameras.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginTools.Inputs;
using System.Collections.Generic;
using XCSJ.Collections;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// XR控制器模拟鼠标输入源
    /// </summary>
    [Name("XR控制器模拟鼠标输入源")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    public class XRControllerAnalogMouseInput : AnalogMouseInputSource
    {
#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// XR控制器
        /// </summary>
        [Name("XR控制器")]
        [ComponentPopup]
        public XRBaseController _xrController;

        /// <summary>
        /// XR控制器
        /// </summary>
        public XRBaseController xrController => this.XGetComponent(ref _xrController);

#endif
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {

#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (xrController) { }
#endif
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (xrController) { }
#endif
        }

        /// <summary>
        /// 尝试获取鼠标输入左右键
        /// </summary>
        /// <param name="leftPressed"></param>
        /// <param name="rightPressed"></param>
        /// <returns></returns>
        public override bool TryGetMouseInput(out bool leftPressed, out bool rightPressed)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (_xrController)
            {
                leftPressed = _xrController.selectInteractionState.active;
                rightPressed = _xrController.activateInteractionState.active;
                return true;
            }
#endif
            leftPressed = false;
            rightPressed = false;
            return false;
        }
    }
}
