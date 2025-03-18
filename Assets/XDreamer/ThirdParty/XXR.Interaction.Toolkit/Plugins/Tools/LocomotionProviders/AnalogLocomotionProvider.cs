using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System;
using System.Linq;
using XCSJ.Collections;
using System.Collections.Generic;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginTools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
    /// <summary>
    /// 模拟运动提供者
    /// </summary>
    [Name("模拟运动提供者")]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(ToolsManager))]
    [DisallowMultipleComponent]
    public class AnalogLocomotionProvider
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : LocomotionProvider
#else
        : InteractProvider
#endif
    {
        /// <summary>
        /// 提供者IO列表
        /// </summary>
        public HashSet<BaseAnalogLocomotionProviderIO> _providerIOs = new HashSet<BaseAnalogLocomotionProviderIO>();

        /// <summary>
        /// 注册IO
        /// </summary>
        /// <param name="providerIO"></param>
        public bool RegistIO(BaseAnalogLocomotionProviderIO providerIO)
        {
            return providerIO ? _providerIOs.Add(providerIO) : false;
        }

        /// <summary>
        /// 注册IO
        /// </summary>
        /// <param name="providerIO"></param>
        public void UnregistIO(BaseAnalogLocomotionProviderIO providerIO)
        {
            _providerIOs.Remove(providerIO);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            foreach (var pIO in _providerIOs)
            {
                if (pIO) pIO.UpdateIO(this);
            }
        }

        /// <summary>
        /// 尝试开始运动
        /// </summary>
        /// <returns></returns>
        public bool TryBeginLocomotion()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return BeginLocomotion();
#else
            return false;
#endif
        }

        /// <summary>
        /// 尝试结束运动
        /// </summary>
        /// <returns></returns>
        public bool TryEndLocomotion()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return EndLocomotion();
#else
            return false;
#endif
        }
    }
}
