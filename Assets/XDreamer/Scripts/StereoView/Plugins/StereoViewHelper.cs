using UnityEngine;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.PluginStereoView
{
    /// <summary>
    /// 立体显示辅助类
    /// </summary>
    public static class StereoViewHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "立体显示";

        /// <summary>
        /// 锚点关联
        /// </summary>
        /// <param name="standardScreen"></param>
        /// <param name="standardScreenAnchor"></param>
        /// <param name="screen"></param>
        /// <param name="screenAnchor"></param>
        /// <param name="rotation"></param>
        /// <param name="standardAnchorOffset"></param>
        /// <param name="standardAnchorOffsetSpaceType"></param>
        /// <param name="anchorOffset"></param>
        /// <param name="anchorOffsetSpaceType"></param>
        /// <returns></returns>
        public static bool AnchorLink(BaseScreen standardScreen, ERectAnchor standardScreenAnchor, BaseScreen screen, ERectAnchor screenAnchor, Vector3 rotation, Vector3 standardAnchorOffset = default, ESpaceType standardAnchorOffsetSpaceType = ESpaceType.Local, Vector3 anchorOffset = default, ESpaceType anchorOffsetSpaceType = ESpaceType.Local)
        {
            if (!standardScreen || !screen) return false;

            var transform = screen.transform;
            transform.rotation = standardScreen.transform.rotation;//旋转量一致

            var standardAnchorPosition = standardScreen.GetAnchorPosition(standardScreenAnchor, standardAnchorOffset, standardAnchorOffsetSpaceType);
            var anchorPosition = screen.GetAnchorPosition(screenAnchor, anchorOffset, anchorOffsetSpaceType);

            var rotationPostion = standardAnchorPosition;//旋转点默认为基准锚点位置
            var offset = standardAnchorPosition - anchorPosition;//锚点重合的偏移量

            transform.position += offset;
            transform.RotateAround(rotationPostion, Vector3.right, rotation.x);
            transform.RotateAround(rotationPostion, Vector3.up, rotation.y);
            transform.RotateAround(rotationPostion, Vector3.forward, rotation.z);

            screen.CallScreenChanged();

            return true;
        }

        /// <summary>
        /// 获取锚点偏移量
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector3 GetAnchorOffset(this ERectAnchor anchor, Vector3 size) => AnchorHelper.GetAnchorOffset(anchor, size);

        /// <summary>
        /// 获取屏幕数目
        /// </summary>
        /// <param name="screenLayoutMode"></param>
        /// <returns></returns>
        public static int GetScreenCount(this EScreenLayoutMode screenLayoutMode) => ScreenCountAttribute.GetScreenCount(screenLayoutMode);

        /// <summary>
        /// 屏幕锚点关联到屏幕
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="screenAnchor"></param>
        /// <param name="linkRotation"></param>
        /// <param name="standardScreen"></param>
        /// <param name="standardScreenAnchor"></param>
        /// <returns></returns>
        public static ScreenAnchorLink AnchorLinkToScreen(this VirtualScreen screen, ERectAnchor screenAnchor, Vector3 linkRotation, VirtualScreen standardScreen, ERectAnchor standardScreenAnchor)
        {
            if (!screen || !standardScreen) return default;
            var anchorLink = screen.XGetOrAddComponent<ScreenAnchorLink>();
            anchorLink.standardScreen = standardScreen;
            anchorLink.standardScreenAnchor = standardScreenAnchor;
            anchorLink.screenAnchor = screenAnchor;
            anchorLink.linkRotation = linkRotation;
            return anchorLink;
        }

        /// <summary>
        /// 为主动立体设置相机属性
        /// </summary>
        public static void SetCameraPropertyForActiveStereo(this Camera camera)
        {
            if (!camera) return;

            camera.renderingPath = RenderingPath.Forward;
            camera.allowHDR = false;
            camera.allowMSAA = false;
        }
    }
}
