using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 自动启用组件(根据运行时平台)
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.ComponentEnable)]
    [Tool("自动")]
    [Name("自动启用组件(根据运行时平台)")]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class AutoEnableComponentByRuntimePlatform : InteractProvider
    {
        /// <summary>
        /// 组件列表:期望在特定运行时平台才启用的组件列表
        /// </summary>
        [Name("组件列表")]
        [Tip("期望在特定运行时平台才启用的组件列表", "List of components that are expected to be enabled on a specific runtime platform")]
        [ComponentPopup(searchFlags = ESearchFlags.Default, displayOnRuntime = false)]
        public List<Component> components = new List<Component>();

        /// <summary>
        /// 运行时平台列表
        /// </summary>
        [Name("运行时平台列表")]
        public List<RuntimePlatform> runtimePlatforms = new List<RuntimePlatform>();

        /// <summary>
        /// 取反:为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；
        /// </summary>
        [Name("取反")]
        [Tip("为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；", "When it is true, setting enabled changes to setting disabled, and setting disabled changes to setting enabled; If it is false, it will not be processed;")]
        public bool reverse = false;

        /// <summary>
        /// 保持一致:为True时，符合设置的运行时则设置启用，不符合设置的运行时则设置禁用；为False时，符合设置的运行时则设置启用，不符合设置的运行时不做处理；
        /// </summary>
        [Name("保持一致")]
        [Tip("为True时，符合设置的运行时则设置启用，不符合设置的运行时则设置禁用；为False时，符合设置的运行时则设置启用，不符合设置的运行时不做处理；", "When it is true, the runtime that meets the setting will be enabled, and the runtime that does not meet the setting will be disabled; When it is false, the runtime that meets the setting will be enabled, and the runtime that does not meet the setting will not be processed;")]
        public bool keepSame = true;

        /// <summary>
        /// 总是启用
        /// </summary>
        [Name("总是启用")]
        [Tip("为True时，会一直尝试启用期望的组件；为False时，仅在当前组件启用时尝试启用一次；", "When true, it will always try to enable the desired component; When it is false, it is only attempted to enable once when the current component is enabled;")]
        public bool alwaysEnable = false;

        private void EnableComponents()
        {
            var enable = runtimePlatforms.Any(p => p == Application.platform);
            foreach (var c in components)
            {
                EnableComponent(c, enable);
            }
        }

        private void EnableComponent(Component component, bool enable)
        {
            if (keepSame || enable)
            {
                component.XSetEnable(reverse ? (!enable) : enable);
            }
        }

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            EnableComponents();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (alwaysEnable) EnableComponents();
        }
    }
}
