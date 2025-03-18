using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXRSpaceSolution.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorXRSpaceSolution
{
    /// <summary>
    /// 编辑器XR空间解决方案组手
    /// </summary>
    [LanguageFileOutput]
    public static class EditorXRSpaceSolutionHelper
    {
        /// <summary>
        /// 绘制选中管理器
        /// </summary>
        [LanguageTuple("Select [{0}] Manager", "选中[{0}]管理器")]
        public static void DrawSelectManager()
        {
            if (GUILayout.Button(string.Format("Select [{0}] Manager".Tr(typeof(EditorXRITHelper)), typeof(XRSpaceSolutionManager).Tr())) && XRSpaceSolutionManager.instance)
            {
                Selection.activeObject = XRSpaceSolutionManager.instance;
            }
        }

        /// <summary>
        /// 创建带XBox的XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace_XBox(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            var xrSpace = CreateXRSpace(screenCount, onScreenCreated, () =>
            {
                var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
                if (origin)//XRRig->XROrigin
                {
                    if (leftHand)
                    {
                        var interact = leftHand.XGetOrAddComponent<InteractIOByXBox>();
                        if (interact)
                        {
                        }
                    }

                    if (rightHand)
                    {
                        var interact = rightHand.XGetOrAddComponent<InteractIOByXBox>();
                        if (interact)
                        {
                            interact.buttonOfActivate = EXBoxAxisAndButton.RightTrigger;
                            interact.buttonOfSelect = EXBoxAxisAndButton.RightBumper;
                            interact.buttonOfUI = EXBoxAxisAndButton.RightTrigger;
                        }
                    }
                }
                return (origin, hmd, leftHand, rightHand);
            }, onCameraLinkedToScreen, xrSpaceName);
            if (xrSpace)
            {
                var move = xrSpace.XAddComponent<TransformByXBox>();
                move.SetDefaultMove();

                var rotate = xrSpace.XAddComponent<TransformByXBox>();
                rotate.SetDefaultRotateWorldY();
            }
            return xrSpace;
        }

        /// <summary>
        /// 创建带XBox的XR空间
        /// </summary>
        /// <param name="screenCount"></param>
        /// <param name="onScreenCreated"></param>
        /// <param name="createXROriginFunc"></param>
        /// <param name="onCameraLinkedToScreen"></param>
        /// <returns></returns>
        public static XRSpace CreateXRSpace(int screenCount, Action<ScreenGroup, List<VirtualScreen>> onScreenCreated, Func<(MonoBehaviour, CameraController, Transform, Transform)> createXROriginFunc, Action<int, Camera, VirtualScreen> onCameraLinkedToScreen, string xrSpaceName)
        {
            if (screenCount < 1)
            {
                Debug.LogWarning("创建XR空间时，构建的屏幕数" + screenCount.ToString() + "不能低于1个！");
                return null;
            }
            if (createXROriginFunc == null)
            {
                Debug.LogWarning("创建XR空间时，构建XR装备的方法不能为空！");
                return null;
            }

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else
            #region XR原点

            var (origin, hmd, leftHand, rightHand) = createXROriginFunc();
            if (!origin)
            {
                Debug.LogWarning("创建XR空间时，构建XR原点的方法返回值无效！");
                return null;
            }

            #endregion

            var (xrSpace, spaceOffset, screenGroup) = CreateXRSpace(origin);

            #region 屏幕组

            List<VirtualScreen> screens = new List<VirtualScreen>();
            if (screenGroup)
            {
                var screenGroupTransform = screenGroup.transform;
                for (int i = 0; i < screenCount; i++)
                {
                    screens.Add(VirtualScreen.CreateScreen("屏幕" + i.ToString(), screenGroupTransform));
                }
                onScreenCreated?.Invoke(screenGroup, screens);
            }

            #endregion

            #region 相机与虚拟屏幕关联

            if (hmd)
            {
                var cameraParent = hmd.cameraEntityController;
                var cameraParentTransform = cameraParent.transform;

                var camera0 = cameraParent.mainCamera;
                var screen0 = screens[0];

                camera0.XSetName("相机0");
                var cameraProjection1 = camera0.XGetOrAddComponent<CameraProjection>();
                cameraProjection1.screen = screen0;
                onCameraLinkedToScreen?.Invoke(0, camera0, screen0);
                camera0.XGetOrAddComponent<AudioListener>().XSetEnable(true);

                for (int i = 1; i < screenCount; i++)
                {
                    var camera = camera0.gameObject.XCloneObject().GetComponent<Camera>();//通过组件所在游戏对象完成克隆，才能支持撤销

                    camera.XSetName("相机" + i.ToString());
                    camera.transform.XSetTransformParent(cameraParentTransform);
                    camera.transform.XResetLocalPRS();

                    var screen = screens[i];
                    camera.XGetOrAddComponent<CameraProjection>().screen = screen;
                    onCameraLinkedToScreen?.Invoke(i, camera, screen);

                    camera.GetComponent<AudioListener>().XSetEnable(false);
                }

                //刷新相机实体控制器的相机列表
                cameraParent.UpdateCamears();
            }

            #endregion

            return xrSpace;
#endif
        }

        private static CameraController CreateCamera() => XRSpaceSolutionHelper.CreateHMDFlyCamera<CameraPoseByXRIS>();

        private static Vector3 DefaultScreenGroupLocalPosition { get; } = new Vector3(0, 1, 2);

        /// <summary>
        /// 创建XR空间
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        private static (XRSpace xrSpace, Transform spaceOffset, ScreenGroup screenGroup) CreateXRSpace(MonoBehaviour origin)
        {
            //创建XR空间组件
            var xrSpace = origin.XGetOrAddComponent<XRSpace>();

            //空间偏移
            var spaceOffset = xrSpace.XCreateChild<Transform>("空间偏移");
            xrSpace.Reset();
            xrSpace.spaceOffset = spaceOffset;

            //屏幕组
            var screenGroup = spaceOffset.XCreateChild<ScreenGroup>("屏幕组");
            screenGroup.transform.XSetLocalPosition(DefaultScreenGroupLocalPosition);
            xrSpace.screenGroup = screenGroup;

            return (xrSpace, spaceOffset, screenGroup);
        }

        /// <summary>
        /// 创建XR交互空间
        /// </summary>
        /// <param name="xrSpaceName"></param>
        /// <param name="hmd"></param>
        /// <param name="leftHand"></param>
        /// <param name="rightHand"></param>
        /// <returns></returns>
        public static XRSpace CreateXRIS(string xrSpaceName)
        {
            var (origin, hmd, leftHand, rightHand, locomotionSystem) = EditorXRITHelper.Create(CreateCamera);

            if (!origin) return default;

            // 创建XR空间
            var (xrSpace, spaceOffset, screenGroup) = CreateXRSpace(origin);
            xrSpace.gameObject.XSetUniqueName(xrSpaceName ?? "XR交互空间");

            if (xrSpace)
            {
                var spaceTranslate = xrSpace.XAddComponent<TransformByXRIS>();
                spaceTranslate._transformHandler.actionName = CommonFun.Name(EActionName.SpaceTranslate);

                var spaceRotate = xrSpace.XAddComponent<TransformByXRIS>();
                spaceRotate._transformHandler.actionName = CommonFun.Name(EActionName.SpaceRotate);
            }

            #region 屏幕组

            if (screenGroup)
            {
                VirtualScreen.CreateScreen("屏幕", screenGroup.transform);

                //与相机透视尝试创建关联
                foreach (var p in hmd.GetComponentsInChildren<CameraProjection>())
                {
                    if (p)
                    {
                        //置空后使其尝试关联新创建的屏幕对象
                        p._screen = default;
                        if (p.screen) { }
                    }
                }
            }

            #endregion

            #region XR装备/原点

            //左手
            if (leftHand)
            {
                var actionName = CommonFun.Name(EActionName.LeftHand);
                var analogController = leftHand.XGetOrAddComponent<AnalogController>();
                if (analogController.analogMouseInput) { }

                var pose = analogController.XGetOrAddComponent<PoseIOByXRIS>();
                pose._poseHandler.actionName = actionName;
                analogController._poseIO = pose;

                var interact = analogController.XGetOrAddComponent<InteractIOByXRIS>();
                interact._interactHandler.actionName = actionName;
            }

            //右手
            if (rightHand)
            {
                var actionName = CommonFun.Name(EActionName.RigthHand);
                var analogController = rightHand.XGetOrAddComponent<AnalogController>();
                if (analogController.analogMouseInput) { }

                var pose = analogController.XGetOrAddComponent<PoseIOByXRIS>();
                pose._poseHandler.actionName = actionName;
                analogController._poseIO = pose;

                var interact = analogController.XGetOrAddComponent<InteractIOByXRIS>();
                interact._interactHandler.actionName = actionName;
            }

            #endregion

            return xrSpace;
        }
    }
}
