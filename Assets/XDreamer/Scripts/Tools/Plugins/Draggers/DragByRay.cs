using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.Base;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 射线拖拽工具
    /// </summary>
    [Name("射线拖拽工具")]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class DragByRay : Dragger
    {
        public enum ERayDragRule
        {
            [Name("固定距离")]
            FixedDistance,

            [Name("射线投射命中")]
            RayCastHit
        }

        [Group("射线拖拽设置", textEN = "Ray Drag Settings")]
        [Name("射线拖拽规则")]
        [EnumPopup]
        public ERayDragRule _rayDragRule = ERayDragRule.RayCastHit;

        /// <summary>
        /// 上次距离
        /// </summary>
        //[Name("上次距离")]
        //[Readonly]
        public float _lastDistance { get; private set; } = 0;

        #region 固定距离

        // 拖拽参考对象
        private Transform dragReferenceObject
        {
            get
            {
                if (!_dragReferenceObject)
                {
                    _dragReferenceObject = (new GameObject(CommonFun.Name(GetType()) + "_拖拽参考对象")).transform;
                    _dragReferenceObject.SetParent(transform);
                }
                return _dragReferenceObject;
            }
        }
        private Transform _dragReferenceObject;

        private Quaternion grabbedObjectRotationOnGrab = Quaternion.identity;
        private Vector3 rayDirectionOnGrab = Vector3.zero;

        #endregion

        #region 射线投射命中

        /// <summary>
        /// 放置规则
        /// </summary>
        [Name("放置规则")]
        [EnumPopup]
        public EPutRule _putRule = EPutRule.BoundsTangent;

        /// <summary>
        /// 使用射线拾取对象面中心
        /// </summary>
        [Name("使用射线拾取对象面中心")]
        [Tip("启用时，碰撞点会被换算为碰撞面的中心；否则为碰撞点", "When on, the collision point is converted to the center of the collision surface; Otherwise, it is the collision point")]
        public bool _useHitObjectFaceCenter = false;

        /// <summary>
        /// 摆放包围盒
        /// </summary>
        private Bounds dragObjectBounds = new Bounds();

        private Vector3 boundsOffsetOnGrab = Vector3.zero;

        private Vector3 rayPositionOnGrabbed;

        private Vector3 grabbedPositionOnGrab = Vector3.zero;

        private bool init = true;

        private void InitGrabbedPosition(Ray ray, Grabbable grabbable)
        {
            if (init)
            {
                init = false;

                var dragTransform = grabbable.transform;
                rayPositionOnGrabbed = GetPositionByRay(dragTransform, ray, grabData.rayMaxDistance, grabData.layerMask);

                // 监测拖拽对象的Y值与射线求解的Y值不相等时，将强行把游戏对象设置到射线定位高度上
                var position = dragTransform.position;
                if (!Mathf.Approximately(rayPositionOnGrabbed.y, position.y))
                {
                    position.y = rayPositionOnGrabbed.y;
                    dragTransform.position = position;
                }
                grabbedPositionOnGrab = dragTransform.position;
            }
        }

        /// <summary>
        /// 求解拖拽对象包围
        /// </summary>
        /// <param name="dragTransform"></param>
        public void CalculateDragBounds(Transform dragTransform)
        {
            if (CommonFun.GetBounds(out dragObjectBounds, dragTransform, true, false, false))
            {
                boundsOffsetOnGrab = dragTransform.position - dragObjectBounds.center;
            }
        }

        #endregion

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

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        protected override void OnGrabEnter()
        {
            base.OnGrabEnter();

            var dragTransform = grabbedObject.targetTransform;
            CalculateDragBounds(dragTransform);

            if (grabData == null) return;

            var grabRay = grabData.ray;
            if (!grabRay.HasValue) return;

            var ray = grabRay.Value;
            if (grabData.raycastHit.HasValue)
            {
                var hitPoint = grabData.raycastHit.Value.point;
                _lastDistance = Vector3.Distance(ray.origin, hitPoint);
            }
            else
            {
                _lastDistance = Vector3.Distance(ray.origin, dragTransform.position);
            }

            switch (_rayDragRule)
            {
                case ERayDragRule.FixedDistance:
                    {
                        if (grabData.raycastHit.HasValue)
                        {
                            var hitPoint = grabData.raycastHit.Value.point;
                            dragReferenceObject.position = hitPoint;
                            dragReferenceObject.LookAt(hitPoint + ray.direction);

                            grabbedObjectRotationOnGrab = grabbedObject.rotation;
                            rayDirectionOnGrab = ray.direction;
                        }
                        break;
                    }
                case ERayDragRule.RayCastHit:
                    {
                        InitGrabbedPosition(ray, grabbedObject);
                        break;
                    }
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="gameObjects"></param>
        protected override void OnHolding()
        {
            base.OnHolding();

            if (grabbedObject)
            {
                SetTRS(grabbedObject.gameObject);
            }
        }

        /// <summary>
        /// 尝试获取拖拽位置数据
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool TryGetDragPosition(out Vector3 position)
        {
            if (holdData.ray.HasValue)
            {
                var ray = holdData.ray.Value;

                switch (_rayDragRule)
                {
                    case ERayDragRule.FixedDistance:
                        {
                            dragReferenceObject.position = ray.origin + ray.direction * _lastDistance;
                            dragReferenceObject.LookAt(dragReferenceObject.position + ray.direction);

                            position = dragReferenceObject.position;
                            return true;
                        }
                    case ERayDragRule.RayCastHit:
                        {
                            var rayPosition = GetPositionByRay(grabbedObject.targetTransform, ray, holdData.rayMaxDistance, holdData.layerMask);

                            position = grabbedPositionOnGrab + rayPosition - rayPositionOnGrabbed;

                            return true;
                        }
                }
            }

            return base.TryGetDragPosition(out position);
        }

        /// <summary>
        /// 尝试获取拖拽旋转量
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        protected override bool TryGetDragRotation(out Quaternion rotation)
        {
            if (holdData.ray.HasValue)
            {
                switch (_rayDragRule)
                {
                    case ERayDragRule.FixedDistance:
                        {
                            // 结果旋转量 = 抓时的旋转量 * 当前射线与抓时射线的构建的旋转量
                            rotation = grabbedObjectRotationOnGrab * Quaternion.FromToRotation(rayDirectionOnGrab, holdData.ray.Value.direction);
                            return true;
                        }
                }
            }
            return base.TryGetDragRotation(out rotation);
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnReleaseEnter()
        {
            base.OnReleaseEnter();

            switch (_rayDragRule)
            {
                case ERayDragRule.FixedDistance:
                    {
                        break;
                    }
                case ERayDragRule.RayCastHit:
                    {
                        init = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 计算射线位置
        /// </summary>
        /// <param name="dragTransform"></param>
        /// <param name="rayInteractData"></param>
        /// <returns></returns>
        public bool TryGetPositionByRay(Transform dragTransform, RayInteractData rayInteractData, out Vector3 position)
        {
            if (rayInteractData!=null && rayInteractData.ray.HasValue)
            {
                position = GetPositionByRay(dragTransform, rayInteractData.ray.Value, rayInteractData.rayMaxDistance, rayInteractData.layerMask);
                return true;
            }

            position = default;
            return false;
        }

        /// <summary>
        /// 通过射线获取位置
        /// </summary>
        /// <param name="dragTransform"></param>
        /// <returns></returns>
        private Vector3 GetPositionByRay(Transform dragTransform, Ray ray, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
        {
            var hits = RayHitter.GetHitAll(ray, maxDistance, layerMask);
            foreach (var hitInfo in hits)
            {
                // 排除拾取对象为拖拽对象或其子对象
                if (hitInfo.transform != dragTransform && !hitInfo.transform.IsChildOf(dragTransform))
                {
                    _lastDistance = hitInfo.distance;//记录上次距离

                    var pickPoint = hitInfo.point;
                    // 射线点转换为拾取碰撞对象面中心
                    if (_useHitObjectFaceCenter)
                    {
                        pickPoint = PutHelper.GetRayHitObjectFaceCenter(hitInfo, dragObjectBounds);
                    }

                    if (PutHelper.TryGetPutPosition(_putRule, pickPoint, hitInfo.normal, dragObjectBounds, boundsOffsetOnGrab, hitInfo.transform, out var position))
                    {
                        return position;
                    }

                    return pickPoint;
                }
            }
            return ray.origin + ray.direction * _lastDistance;
        }

        #region 绘制

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            validDraw = _handleOpaqueMaterial && _rotateLineMaterial;
            if (!validDraw)
            {
                Debug.LogWarning("平移旋转缩放绘制所需材质无效");
            }

            Rebuild();
        }

        /// <summary>
        /// 选择集发生改变
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            Rebuild(Selection.selection);
        }

        private bool validDraw = false;

        private Mesh _handleLineMesh;

        private Mesh _handleRotationMesh;

        /// <summary>
        /// 渲染矩阵
        /// </summary>
        private Matrix4x4 _handleMatrix;

        [Group("绘制设置", textEN = "Draw Settings")]
        [Name("绘制拖拽小发明")]
        public bool _drawDragGizmo = true;

        /// <summary>
        /// 平移缩放轴材质
        /// </summary>
        [Name("平移缩放轴材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _handleOpaqueMaterial = null;

        /// <summary>
        /// 旋转线材质
        /// </summary>
        [Name("旋转线材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _rotateLineMaterial = null;

        /// <summary>
        /// 轴大小
        /// </summary>
        [Name("轴大小")]
        [Min(1)]
        public float _handleSize = 90f;

        /// <summary>
        /// 圆形段数
        /// </summary>
        [Name("圆形段数")]
        [Min(1)]
        public int _circleSegment = 48;

        /// <summary>
        /// 旋转半径
        /// </summary>
        [Name("旋转半径")]
        [Min(0.1f)]
        public float _rotationRadius = 1;

        private Transform _trs;

        private void Rebuild()
        {
            if (grabbedObject)
            {
                Rebuild(grabbedObject.gameObject);
            }
        }

        private void Rebuild(GameObject go)
        {
            if (go)
            {
                SetTRS(go);
                RebuildGizmoMesh(Vector3.one, true);// 重建拖拽轴模型
            }
        }

        public void SetTRS(GameObject go)
        {
            if (go && go.activeInHierarchy)
            {
                _trs = go.transform;

                RebuildGizmoMatrix();
            }
        }

        /// <summary>
        /// 构建网格模型
        /// </summary>
        /// <param name="scale"></param>
        private void RebuildGizmoMesh(Vector3 scale, bool rebuildMatrix = false)
        {
            if (!validDraw || !_drawDragGizmo || !_trs) return;

            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return;

            HandleMesh.CreateAxisLineMesh(ref _handleLineMesh, _trs, scale, cam, EAxis.X | EAxis.Z);
            HandleMesh.CreateRotateMesh(ref _handleRotationMesh, _circleSegment, _rotationRadius, EAxis.Y);

            if (rebuildMatrix)
            {
                RebuildGizmoMatrix();
            }
        }

        /// <summary>
        /// 构建矩阵
        /// </summary>
        private void RebuildGizmoMatrix()
        {
            var handleSize = HandleHelper.GetScreenAndWorldRatio(_trs.position);
            var k = _trs.localScale;
            _handleMatrix = _trs.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1/k.x, 1/k.y, 1/k.z) * handleSize * _handleSize);
        }

        /// <summary>
        /// 渲染
        /// </summary>
        private void OnRenderObject()
        {
            if (!validDraw || !_drawDragGizmo) return;

            if (!grabbedObject) return;

            if (_handleLineMesh)
            {
                _handleOpaqueMaterial.SetPass(0);
                Graphics.DrawMeshNow(_handleLineMesh, _handleMatrix);
            }

            if (_handleRotationMesh)
            {
                _rotateLineMaterial.SetPass(0);
                Graphics.DrawMeshNow(_handleRotationMesh, _handleMatrix);
            }
        }

        #endregion
    }
}
