using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginsCameras.Tools.Controllers;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// XR空间:用于构建单机单（多）通道环境的XR空间功能组件
    /// </summary>
    [Name("XR空间")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    public class XRSpace : InteractProvider
    {
        /// <summary>
        /// XR原点
        /// </summary>
        [Name("XR原点")]
        public XROriginOwner _rigOwner;

        /// <summary>
        /// XR原点
        /// </summary>
        public XROriginOwner rigOwner => this.XGetComponentInChildren(ref _rigOwner);

        /// <summary>
        /// 空间偏移
        /// </summary>
        [Name("空间偏移")]
        public Transform _spaceOffset;

        /// <summary>
        /// 空间偏移
        /// </summary>
        public Transform spaceOffset
        {
            get
            {
                if (!_spaceOffset)
                {
                    var sot = transform.Find("空间偏移");
                    if (sot)
                    {
                        this.XModifyProperty(ref _spaceOffset, sot);
                    }
                    else if (transform.childCount == 1)//如果只有一个子级对象，直接使用该对象
                    {
                        this.XModifyProperty(ref _spaceOffset, transform.GetChild(0));
                    }
                }
                return _spaceOffset;
            }
            set => this.XModifyProperty(ref _spaceOffset, value);
        }

        /// <summary>
        /// 屏幕组
        /// </summary>
        [Name("屏幕组")]
        public ScreenGroup _screenGroup;

        /// <summary>
        /// 屏幕组
        /// </summary>
        public ScreenGroup screenGroup
        {
            get => this.XGetComponentInChildren(ref _screenGroup);
            set => this.XModifyProperty(ref _screenGroup, value);
        }

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

        private void OnXRAnswerReceived(XRAnswer answer) => OnHandle(answer as XRSpaceConfigA);

        /// <summary>
        /// 空间配置实例
        /// </summary>
        public static XRSpaceConfigA spaceConfigInstance { get; private set; } = new XRSpaceConfigA();

        private void OnHandle(XRSpaceConfigA xRSpaceConfigA)
        {
            if (xRSpaceConfigA == null) return;

            spaceConfigInstance = xRSpaceConfigA;

            var link = xRSpaceConfigA.enableScreenCameraLink;

            //空间偏移
            var spaceOffset = this.spaceOffset;
            if (spaceOffset)
            {
                spaceOffset.XSetLocalPosition(xRSpaceConfigA.spaceOffset.position);
                spaceOffset.XSetLocalRotation(xRSpaceConfigA.spaceOffset.rotation);
            }

            //屏幕组
            var screenGroup = this.screenGroup;
            if (!screenGroup) return;

            //屏幕组偏移
            {
                var screenGroupTransform = screenGroup.transform;
                screenGroupTransform.XSetLocalPosition(xRSpaceConfigA.screenGroupOffset.position);
                screenGroupTransform.XSetLocalRotation(xRSpaceConfigA.screenGroupOffset.rotation);
            }

            //装备拥有者
            var rigOwner = this.rigOwner;
            if (!rigOwner) return;

            //相机偏移：装备偏移
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var rig = this.rigOwner.xrRig;
            if (rig)
            {
                //同步调整XRIT中的高度信息
                rig.CameraYOffset = xRSpaceConfigA.cameraOffset.position.y;
            }
#endif
            var cameraOffset = rigOwner.cameraOffset;
            if (cameraOffset)
            {
                cameraOffset.XSetLocalPosition(xRSpaceConfigA.cameraOffset.position);
                cameraOffset.XSetLocalRotation(xRSpaceConfigA.cameraOffset.rotation);
            }

            #region 屏幕

            var screenCount = xRSpaceConfigA.screens.Count;
            var screens = screenGroup.screens;//当前已经有的屏幕组
            BaseScreen cloneFromScreen = screens.FirstOrDefault();//克隆来源屏幕

            for (int i = screenCount; i < screens.Length; i++)//将可能多出的屏幕隐藏
            {
                var s = screens[i];
                if (s)
                {
                    s.gameObject.XSetActive(false);
                    s.XSetName(i.ToString() + "___已禁用");
                }
            }
            Dictionary<string, BaseScreen> tempScreens = new Dictionary<string, BaseScreen>();
            List<(BaseScreen, ScreenInfo)> anchorLinkScreens = new List<(BaseScreen, ScreenInfo)>();

            BaseScreen defaultStandardScreen = default;
            for (int i = 0; i < screenCount; i++)
            {
                var screenInfo = xRSpaceConfigA.screens[i];
                var screen = i < screens.Length ? screens[i] : VirtualScreen.CloneOrCreateScreen(cloneFromScreen, screenInfo.name, screenGroup.transform);

                screen.gameObject.XSetActive(true);//保证游戏对象激活
                screen.XSetName(screenInfo.name);
                screen.screenSize = screenInfo.screenSize.ToVector3();

                if (!tempScreens.ContainsKey(screenInfo.name))
                {
                    tempScreens.Add(screenInfo.name, screen);
                }

                var renderer = screen.XGetOrAddComponent<GizmoRenderer>();
                renderer.text = screenInfo.displayName;

                switch (screenInfo.screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            if (!defaultStandardScreen) defaultStandardScreen = screen;//至少有一个默认的标准屏幕

                            var screenTransform = screen.transform;
                            screenTransform.XSetLocalPosition(screenInfo.screenPose.position);
                            screenTransform.XSetLocalRotation(screenInfo.screenPose.rotation);

                            var screenAnchorLink = screen.GetComponent<ScreenAnchorLink>();
                            if (screenAnchorLink)
                            {
                                screenAnchorLink.XSetEnable(false);
                            }
                            break;
                        }
                    case EScreenPoseMode.AnchorLink:
                        {
                            anchorLinkScreens.Add((screen, screenInfo));
                            break;
                        }
                }
            }

            if (!defaultStandardScreen)
            {
                Debug.LogError("未找到有效的标准屏幕!");
            }
            foreach (var kv in anchorLinkScreens)
            {
                var screenInfo = kv.Item2;
                var screen = kv.Item1;
                var screenAnchorLink = screen.XGetOrAddComponent<ScreenAnchorLink>();
                screenAnchorLink.XSetEnable(true);

                var screenAnchorLinkInfo = screenInfo.screenAnchorLinkInfo;
                if (tempScreens.TryGetValue(screenAnchorLinkInfo.standardScreen, out var standardScreen))
                {
                    screenAnchorLink.standardScreen = standardScreen;
                }
                else
                {
                    Debug.LogErrorFormat("屏幕[{0}]依赖锚点关联时未找到基准屏幕[{1}],使用[{2}]替代",
                        screenInfo.name,
                        screenAnchorLinkInfo.standardScreen,
                        defaultStandardScreen ? defaultStandardScreen.name : "");
                }

                screenAnchorLink.standardScreenAnchor = screenAnchorLinkInfo.standardScreenAnchor;
                screenAnchorLink._standardScreenAnchorOffset = screenAnchorLinkInfo.standardScreenAnchorOffset.ToVector3();
                screenAnchorLink._standardScreenAnchorOffsetSpaceType = screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType == 0 ? ESpaceType.World : ESpaceType.Local;

                screenAnchorLink.screenAnchor = screenAnchorLinkInfo.screenAnchor;
                screenAnchorLink._screenAnchorOffset = screenAnchorLinkInfo.screenAnchorOffset.ToVector3();
                screenAnchorLink._screenAnchorOffsetSpaceType = screenAnchorLinkInfo.screenAnchorOffsetSpaceType == 0 ? ESpaceType.World : ESpaceType.Local;

                screenAnchorLink.linkRotation = screenAnchorLinkInfo.linkRotation.ToVector3();
            }

            #endregion

            #region 相机

            var cameraCount = xRSpaceConfigA.cameras.Count;
            var cameras = rigOwner.hmd.cameraEntityController.cameras;//当前已经有的相机组
            var defaultCamera = cameras.FirstOrDefault();
            for (int i = cameraCount; i < cameras.Length; i++)//将可能多出的相机隐藏
            {
                var c = cameras[i];
                if (c)
                {
                    c.gameObject.XSetActive(false);
                    c.XSetName(i.ToString() + "___已禁用");
                }
            }

            var parent = rigOwner.hmd.cameraEntityController.transform;
            for (int i = 0; i < cameraCount; i++)
            {
                var cameraInfo = xRSpaceConfigA.cameras[i];
                var camera = i < cameras.Length ? cameras[i] : CameraHelperExtension.CloneOrCreateCamera(defaultCamera, parent);

                //设置相机基础信息
                camera.gameObject.XSetActive(true);//保证游戏对象激活
                camera.XSetName(cameraInfo.name);

                //相机透视组件信息设置
                var cameraProjection = camera.XGetOrAddComponent<CameraProjection>();
                cameraProjection.XSetEnable(true);//保证组件启用
                cameraProjection.LREyeMatrixMode = xRSpaceConfigA.unityCameraControl.LREyeMatrixMode;//设置左右眼矩阵计算模式
                if (link)//根据关联关系设置其更新模式
                {
                    cameraProjection.updateMode = CameraProjection.EUpdateMode.CustomVirtualScreen;
                    if (!tempScreens.TryGetValue(cameraInfo.screen, out var screen) || !(screen is VirtualScreen virtualScreen) || !virtualScreen)
                    {
                        Debug.LogError("相机[" + cameraInfo.name + "]未找到期望关联的屏幕:" + cameraInfo.screen);
                        cameraProjection.screen = default;//解除屏幕关联
                        continue;
                    }
                    cameraProjection.screen = virtualScreen;//关联屏幕
                }
                else
                {
                    cameraProjection.updateMode = CameraProjection.EUpdateMode.UnityVirtualScreen;
                }

                //设置相机的全局配置
                xRSpaceConfigA.unityCameraConfig.SetCameraConfig(camera);

                //设置相机的独立参数-显示信息
                camera.stereoTargetEye = (StereoTargetEyeMask)cameraInfo.stereoTargetEye;
                camera.targetDisplay = (int)cameraInfo.targetDisplay;

                //设置相机的独立参数-变换信息
                var cameraTransform = camera.transform;
                cameraTransform.XResetLocalPRS();//强制重置所有值
                cameraTransform.XSetLocalPosition(cameraInfo.cameraOffset_Position);
                cameraTransform.XSetLocalRotation(cameraInfo.cameraOffset_Rotation);

                //设置相机的独立参数-视口信息
                camera.XModifyProperty(() =>
                {
                    camera.rect = cameraInfo.viewportRect.ToRect();
                });

                //确保只有一个音频监听器可用
                camera.GetComponent<AudioListener>().XSetEnable(i == 0);
            }

            //更新相机组
            rigOwner.hmd.cameraEntityController.UpdateCamears();

            #endregion

            #region 相机控制

            //相机移动
            var cameraMoveControllers = rigOwner.hmd.GetComponentsInChildren<BaseCameraMoveController>();
            foreach (var cmc in cameraMoveControllers)
            {
                cmc.XSetEnable(xRSpaceConfigA.allowMoveControl);
            }

            //允许屏幕边界控制
            var screenBoundaryControl = rigOwner.hmd.cameraTransformer.GetComponent<CameraMoveByMouse>();
            screenBoundaryControl.XSetEnable(xRSpaceConfigA.allowScreenBoundaryControl && xRSpaceConfigA.allowMoveControl);

            //相机旋转
            var cameraRotateControllers = rigOwner.hmd.GetComponentsInChildren<BaseCameraRotateController>();
            foreach (var crc in cameraRotateControllers)
            {
                crc.XSetEnable(xRSpaceConfigA.allowRotateControl);
            }

            //相机变换
            var cameraTransformControllers = rigOwner.hmd.GetComponentsInChildren<BaseCameraTransformController>();
            foreach (var ctc in cameraTransformControllers)
            {
                ctc.XSetEnable(xRSpaceConfigA.allowDirectTransformControl);
            }

            //相机变换处理规则
            switch (xRSpaceConfigA.cameraTransformHandleRule)
            {
                case ECameraTransformHandleRule.Reset:
                    {
                        rigOwner.hmd.cameraTransformer.ResetCamera();
                        break;
                    }
                case ECameraTransformHandleRule.ResetToStart:
                    {
                        rigOwner.hmd.cameraTransformer.ResetCameraToStart();
                        break;
                    }
            }

            #endregion
        }

        /// <summary>
        /// 设置屏幕相机关联
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public void SetScreenCameraLink(bool link)
        {
            var rigOwner = this.rigOwner;
            if (!rigOwner) return;
            var hmd = rigOwner.hmd;
            if (!hmd) return;
            var CameraProjections = hmd.cameraEntityController.GetComponentsInChildren<CameraProjection>(true);
            foreach (var cameraProjection in CameraProjections)
            {
                cameraProjection.XSetEnable(link);
            }
        }

        /// <summary>
        /// 所有屏幕相机关联
        /// </summary>
        /// <returns></returns>
        public bool AllScreenCameraLinked()
        {
            var rigOwner = this.rigOwner;
            if (!rigOwner) return false;
            var hmd = rigOwner.hmd;
            if (!hmd) return false;

            return hmd.cameraEntityController.GetComponentsInChildren<CameraProjection>(true).All(cameraProjection => cameraProjection.enabled);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (spaceOffset) { }
            if (screenGroup) { }
            if (rigOwner) { }
        }
    }
}
