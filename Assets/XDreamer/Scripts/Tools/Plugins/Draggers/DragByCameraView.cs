using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 相机视图平面拖拽工具：
    /// 1、基于相机朝向和参考点构建一个平面，游戏对象在该平面上运动
    /// 2、被抓对象在3D场景中，则使用被抓对象世界位置作为参考点
    /// 3、如果被抓对象有插槽适配对象，插槽位置作为参考点
    /// </summary>
    [Name("相机视图平面拖拽工具")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class DragByCameraView : Dragger
    {
        /// <summary>
        /// 缺省距离
        /// </summary>
        [Name("缺省距离")]
        [Min(0.1f)]
        public float _defaultDistance = 3;

        /// <summary>
        /// 使用插槽对齐
        /// </summary>
        [Name("使用插槽对齐")]
        public bool _useSocketAlign = true;

        /// <summary>
        /// 是否拖拽
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected override bool CanGrab(InteractData interactData, Grabbable grabbable)
        {
            // 判断射线是否在选择集对象上
            return base.CanGrab(interactData, grabbable) && grabbable && ToolsExtensionHelper.IsSelection(grabbable.transform);
        }

        // 变换位置与碰撞点的偏移量
        private Vector3 offsetGrabObjPositionToHitPoint = Vector3.zero;

        // 以射线碰撞点为点，相机朝向法线构建的平面
        private Plane camDirPlaneOnGrab;

        private GameObjectSocketCache gameObjectSocketCache;

        /// <summary>
        /// 开启拖拽
        /// </summary>
        protected override void OnGrabEnter()
        {
            base.OnGrabEnter();

            var cam = CameraHelperExtension.currentCamera;
            if (cam && grabData!=null)
            {
                var rayHit = grabData.raycastHit;
                if (rayHit.HasValue)
                {
                    offsetGrabObjPositionToHitPoint = grabbedObject.transform.position - rayHit.Value.point;
                    camDirPlaneOnGrab = new Plane(cam.transform.forward, rayHit.Value.point);
                }
            }

            gameObjectSocketCache = this.XGetComponentInParentOrGlobal<GameObjectSocketCache>();
        }

        /// <summary>
        /// 获取位置信息
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool TryGetDragPosition(out Vector3 position)
        {
            var rayData = holdData.ray;
            if (rayData.HasValue)
            {
                var ray = rayData.Value;
                var origin = ray.origin;
                var dir = ray.direction;

                float distance = -1;
                // 拖拽对象有插槽时，使用插槽对齐
                if (_useSocketAlign && gameObjectSocketCache && gameObjectSocketCache.currentSocket is GameObjectSocket socket)
                {
                    distance = Mathf.Abs(Vector3.Dot(dir, socket.pose.position - origin));
                }
                else if (camDirPlaneOnGrab.Raycast(ray, out var enter)) // 使用相机平面
                {
                    distance = enter;
                }

                if (distance < 0)
                {
                    distance = _defaultDistance;
                }
                position = origin + dir * distance + offsetGrabObjPositionToHitPoint;
                return true;
            }
            return base.TryGetDragPosition(out position);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        protected override void OnReleaseEnter()
        {
            base.OnReleaseEnter();

            offsetGrabObjPositionToHitPoint = Vector3.zero;
        }
    }
}
