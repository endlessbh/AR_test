
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVuforia;
using UnityEditor;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras;
using XCSJ.EditorExtension.Base;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginVoxelTracker;

#if XDREAMER_VUFORIA
using Vuforia;
#endif

namespace XCSJ.EditorVuforia.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    public static class ToolsMenu
    {
#if XDREAMER_VUFORIA

        private static VuforiaBehaviour GetOrCreateARCamera()
        {
            CameraHelperExtension.HandleDefaultMainCamera();
            if (VuforiaBehaviour.Instance) return VuforiaBehaviour.Instance;

            EditorApplication.ExecuteMenuItem("GameObject/Vuforia Engine/AR Camera");

            if (VuforiaBehaviour.Instance)
            {
                VuforiaBehaviour.Instance.transform.XSetTransformParent(default(Transform));
            }
            return VuforiaBehaviour.Instance;
        }

        private static T Create<T>(string menuItemPath) where T: MonoBehaviour
        {
            var select = Selection.activeGameObject;
            if (select && !select.GetComponent<VuforiaManager>())
            {
                select = default;
            }
            var vuforia = GetOrCreateARCamera();
            if (!vuforia)
            {
                return default;
            }

            var behaviour = EditorHelper.ExecuteMenuItem<T>(menuItemPath);
            if (behaviour)
            {
                behaviour.transform.XSetTransformParent(default(Transform));
                behaviour.gameObject.XSetUniqueName(behaviour.name);

                UICommonFun.PingObject(behaviour);
            }

            if (select) Selection.activeGameObject = select;
            return behaviour;
        }

#endif

        /// <summary>
        /// 图像目标
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("图像目标")]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreateImageTargetBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<ImageTargetBehaviour>("GameObject/Vuforia Engine/Image Target");
#endif
        }

        /// <summary>
        /// 多目标行为
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("多目标")]
        [XCSJ.Attributes.Icon(EIcon.Target)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreateMultiTargetBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<MultiTargetBehaviour>("GameObject/Vuforia Engine/Multi Target");
#endif
        }

        /// <summary>
        /// 圆柱目标
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("圆柱目标")]
        [XCSJ.Attributes.Icon(EIcon.Cylinder)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreateCylinderTargetBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<CylinderTargetBehaviour>("GameObject/Vuforia Engine/Cylinder Target");
#endif
        }

        /// <summary>
        /// 模型目标
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("模型目标")]
        [XCSJ.Attributes.Icon(EIcon.Model)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreateModelTargetBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<ModelTargetBehaviour>("GameObject/Vuforia Engine/Model Target");
#endif
        }

        /// <summary>
        /// VuMark
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("VuMark")]
        [XCSJ.Attributes.Icon(EIcon.Marker)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreateVuMarkBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<VuMarkBehaviour>("GameObject/Vuforia Engine/VuMark");
#endif
        }

        /// <summary>
        /// 平面查找器
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VuforiaHelper.Title, nameof(VuforiaManager), groupRule = EToolGroupRule.Create)]
        [Name("平面查找器")]
        [XCSJ.Attributes.Icon(EIcon.WireFrame)]
        [RequireManager(typeof(VuforiaManager))]
        [Manual(typeof(VuforiaManager))]
        public static void CreatePlaneFinderBehaviour(ToolContext toolContext)
        {
#if XDREAMER_VUFORIA
            var behaviour = Create<PlaneFinderBehaviour>("GameObject/Vuforia Engine/Ground Plane/Plane Finder");
#endif
        }
    }
}
