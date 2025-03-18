using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [Tool("自动")]
    [Name("自动激活游戏对象(根据运行时平台)")]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class AutoActiveGameObjectByByRuntimePlatform : InteractProvider
    {
        [Name("游戏对象列表")]
        [Tip("期望自动激活的游戏对象列表", "List of GameObjects expected to be activated automatically")]
        public List<GameObject> gameObjects = new List<GameObject>();

        [Name("运行时平台列表")]
        public List<RuntimePlatform> runtimePlatforms = new List<RuntimePlatform>();

        [Name("取反")]
        [Tip("为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；", "When it is true, setting enabled changes to setting disabled, and setting disabled changes to setting enabled; If it is false, it will not be processed;")]
        public bool reverse = false;

        [Name("保持一致")]
        [Tip("为True时，符合设置的运行时则设置激活，不符合设置的运行时则设置停用；为False时，符合设置的运行时则设置激活，不符合设置的运行时不做处理；", "When it is true, the setting is activated when it meets the setting, and deactivated when it does not meet the setting; When it is false, the setting is activated when the setting is met, and the setting is not processed when the setting is not met;")]
        public bool keepSame = true;

        [Name("总是激活")]
        [Tip("为True时，会一直尝试激活期望的游戏对象；为False时，仅在当前组件启用时尝试激活一次；", "When true, it will always try to activate the desired game object; When it is false, the activation is attempted only once when the current component is enabled;")]
        public bool alwaysActive = false;

        private void Active(GameObject gameObject, bool active)
        {
            if (!gameObject) return;
            if (keepSame || active)
            {
                gameObject.SetActive(reverse ? (!active) : active);
            }
        }

        private void ActiveGameObjects()
        {
            var active = runtimePlatforms.Any(p => p == Application.platform);
            foreach (var go in gameObjects)
            {
                Active(go, active);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveGameObjects();
        }

        protected void Update()
        {
            if (alwaysActive) ActiveGameObjects();
        }
    }
}
