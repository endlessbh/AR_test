using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPico;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit;
#endif

#if XDREAMER_PICO
using Unity.XR.PXR;
#endif

namespace XCSJ.EditorPico.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
        /// <summary>
        /// PICO型XR交互空间
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(PicoHelper.Title)]
        [Name("PICO型XR交互空间")]
        [Tip("创建基于PICO的XR交互空间；", "Create an XR interactive space based on PICO;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(PicoManager))]
        [Manual(typeof(PicoManager))]
        public static void CreateXRSpace_Pico(ToolContext toolContext)
        {
            var origin = EditorXRITHelper.CreateUnity(out Transform mainCamera, out Transform leftHand, out Transform rightHand, out _);
            if (!origin) return;

#if XDREAMER_PICO
            origin.XGetOrAddComponent<PXR_Manager>();
#endif
        }
    }
}
