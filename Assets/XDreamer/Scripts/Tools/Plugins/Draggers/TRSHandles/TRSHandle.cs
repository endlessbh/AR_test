using System;
using System.Collections;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;
using Application = UnityEngine.Application;

namespace XCSJ.PluginTools.Draggers.TRSHandles
{
    /// <summary>
    /// 平移旋转缩放工具
    /// </summary>
    [Name("平移旋转缩放工具")]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Drag)]
    [RequireManager(typeof(ToolsManager))]
    public class TRSHandle : Dragger
    {
        #region 基础属性

        /// <summary>
        /// 变换空间
        /// </summary>
        [Name("变换空间")]
        [EnumPopup]
        public ESpace _space = ESpace.Local;

        /// <summary>
        /// 工具类型
        /// </summary>
        public ETRSToolType trsToolType
        {
            get => _trsToolType;
            set
            {
                if (_trsToolType != value)
                {
                    var old = _trsToolType;
                    _trsToolType = value;

                    RebuildGizmoMesh(Vector3.one);

                    onTRSHandleToolTypeChanged?.Invoke(this, old);
                }
            }
        }

        [Readonly]
        [Name("TRS工具类型")]
        [EnumPopup]
        public ETRSToolType _trsToolType = ETRSToolType.None;

        /// <summary>
        /// 变换空间
        /// </summary>
        public ESpace space
        {
            get => _space;
            set
            {
                if (_space != value)
                {
                    var old = _space;
                    _space = value;
                    onTRSHandleSpaceChanged?.Invoke(this, old);
                    Rebuild();
                }
            }
        }

        /// <summary>
        /// 本地空间
        /// </summary>
        public bool localSpace { get => space == ESpace.Local; set => space = value ? ESpace.Local : ESpace.World; }

        /// <summary>
        /// 变换空间变化回调函数,参数2为上一次的变换空间规则
        /// </summary>
        public static event Action<TRSHandle, ESpace> onTRSHandleSpaceChanged;

        /// <summary>
        /// 点击轴最大识别距离
        /// </summary>
        [Name("点击轴最大识别距离")]
        [Min(1)]
        public int _clickAxisMaxDistance = 15;

        /// <summary>
        /// 选择集修改器
        /// </summary>
        [Name("选择集修改器")]
        [Tip("点击三轴拖拽时禁用修改选择集,否则可能因点空白而取消选择集，导致拖拽对象无效", "Disable modifying the selection set when clicking on the three axis drag, otherwise the selection set may be cancelled due to a blank point, resulting in invalid drag objects")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SelectionModify _selectionModify;

        #endregion

        #region 平移属性

        [Group("平移设置", textEN ="Move Settings", defaultIsExpanded = false)]
        /// <summary>
        /// 平移启用轴
        /// </summary>
        [Name("平移启用轴")]
        public EAxis _moveEnableAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 平移启用轴
        /// </summary>
        public EAxis moveEnableAxis
        {
            get => _moveEnableAxis;
            set
            {
                if (_moveEnableAxis != value)
                {
                    _moveEnableAxis = value;
                    Rebuild();
                }
            }
        }

        /// <summary>
        /// 单次移动距离
        /// </summary>
        [Name("移动步长")]
        [Min(0)]
        public float _positionSnapValue = 0.01f;

        /// <summary>
        /// 轴大小
        /// </summary>
        [Name("轴大小")]
        [Min(1)]
        public float _handleSize = 90f;

        /// <summary>
        /// 椎体网格
        /// </summary>
        [Name("轴椎体网格模型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Mesh _coneMesh;

        /// <summary>
        /// 椎体大小
        /// </summary>
        [Name("椎体大小")]
        [Min(0.01f)]
        public float _capSize = 0.1f;

        #endregion

        #region 旋转属性

        [Group("旋转设置", textEN = "Rotation Settings", defaultIsExpanded = false)]
        /// <summary>
        /// 旋转启用轴
        /// </summary>
        [Name("旋转启用轴")]
        public EAxis _rotationEnableAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 旋转启用轴
        /// </summary>
        public EAxis rotationEnableAxis
        {
            get => _rotationEnableAxis;
            set
            {
                if (_rotationEnableAxis != value)
                {
                    _rotationEnableAxis = value;
                    Rebuild();
                }
            }
        }

        /// <summary>
        /// 旋转值
        /// </summary>
        [Name("旋转最小角")]
        [Min(0)]
        public float _rotationSnapValue = 0.2f;

        /// <summary>
        /// 旋转速度系数
        /// </summary>
        [Name("旋转速度系数")]
        public float _rotationSpeed = 1;

        /// <summary>
        /// 旋转线材质
        /// </summary>
        [Name("旋转线材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _rotateLineMaterial = null;

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

        #endregion

        #region 缩放

        [Group("缩放设置", textEN = "Scale Settings", defaultIsExpanded = false)]
        /// <summary>
        /// 缩放启用轴
        /// </summary>
        [Name("缩放启用轴")]
        public EAxis _scaleEnableAxis = EAxis.X | EAxis.Y | EAxis.Z;

        /// <summary>
        /// 缩放启用轴
        /// </summary>
        public EAxis scaleEnableAxis
        {
            get => _scaleEnableAxis;
            set
            {
                if (_scaleEnableAxis != value)
                {
                    _scaleEnableAxis = value;
                    Rebuild();
                }
            }
        }

        /// <summary>
        /// 缩放最小倍数
        /// </summary>
        [Name("缩放最小倍数")]
        [Min(0)]
        public float _scaleSnapValue = .01f;

        /// <summary>
        /// 缩放立方体网格模型
        /// </summary>
        [Name("缩放立方体网格模型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Mesh _cubeMesh;

        /// <summary>
        /// 立方体大小
        /// </summary>
        [Name("立方体大小")]
        [Min(0.1f)]
        public float _boxSize = .25f;

        /// <summary>
        /// 平移缩放轴材质
        /// </summary>
        [Name("平移缩放轴材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _handleOpaqueMaterial = null;

        /// <summary>
        /// 平面材质
        /// </summary>
        [Name("平面材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _handleTransparentMaterial = null;

        #endregion

        #region TRS记录

        public void SetTRS(GameObject go)
        {
            if (go)
            {
                trs.position = go.transform.position;
                trs.rotation = _space == ESpace.Local ? go.transform.rotation : Quaternion.Euler(0, 0, 0);
                trs.localScale = Vector3.one;

                RebuildGizmoMatrix();
            }
        }

        /// <summary>
        /// 空间
        /// </summary>
        [Name("空间")]
        public enum ESpace
        {
            [Name("本地")]
            Local,

            [Name("世界")]
            World,
        }

        private bool validMaterial = false;

        /// <summary>
        /// 控制类型变化回调函数,参数2为上一次工具类型
        /// </summary>
        public static Action<TRSHandle, ETRSToolType> onTRSHandleToolTypeChanged = null;

        private Transform _trs;

        private Transform trs
        {
            get
            {
                if (!_trs && Application.isPlaying)
                {
                    _trs = (new GameObject(CommonFun.Name(GetType()) + "_轴参考对象")).transform;
                    _trs.SetParent(transform);
                }
                return _trs;
            }
        }

        /// <summary>
        /// 拖拽轴数量
        /// </summary>
        private int draggingAxesCount = 0;

        private HandleTransform handleOrigin = HandleTransform.identity;

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            validMaterial = _handleOpaqueMaterial && _handleTransparentMaterial && _rotateLineMaterial;
            if (!validMaterial)
            {
                Debug.LogError("平移旋转缩放绘制所需材质无效");
            }

            ToolsManager.onTransformHasChanged += OnDragGameObjectTransformChanged;

            Rebuild();
        }

        /// <summary>
        /// 启用：解除选择事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ToolsManager.onTransformHasChanged -= OnDragGameObjectTransformChanged;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            DebugDrawOnSceneView();

            OnCameraMove();
        }

        private DragOrientation drag = new DragOrientation();

        private void DebugDrawOnSceneView()
        {
            if (!draggingHandle) return;

            var dir = drag.axis * 2f;
            var color = new Color(Mathf.Abs(drag.axis.x), Mathf.Abs(drag.axis.y), Mathf.Abs(drag.axis.z), 1f);
            var lineTime = 0f;
            Debug.DrawRay(drag.origin, dir * 1f, color, lineTime, false);
            var orgOffset1 = drag.origin + dir;
            var orgOffset2 = drag.origin + dir * .9f;
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.up * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.forward * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (trs.right * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.up * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.forward * .1f), color, lineTime, false);
            Debug.DrawLine(orgOffset1, orgOffset2 + (-trs.right * .1f), color, lineTime, false);

            Debug.DrawLine(drag.origin, drag.origin + drag.mouse, Color.red, lineTime, false);
            Debug.DrawLine(drag.origin, drag.origin + drag.cross, Color.black, lineTime, false);
        }

        private GameObject currentDragGameObject = null;

        /// <summary>
        /// 渲染
        /// </summary>
        private void OnRenderObject()
        {
            if (!validMaterial) return;
            if (!currentDragGameObject) return;

            switch (trsToolType)
            {
                case ETRSToolType.Position:
                case ETRSToolType.Scale:
                    {
                        _handleOpaqueMaterial.SetPass(0);
                        Graphics.DrawMeshNow(_handleLineMesh, _handleMatrix);
                        Graphics.DrawMeshNow(_handleSolidMesh, _handleMatrix, 1);  // Cones

                        // 当拖拽轴不为1时，才显示中间的模型网格
                        if (draggingAxesCount != 1)
                        {
                            _handleTransparentMaterial.SetPass(0);
                            Graphics.DrawMeshNow(_handleSolidMesh, _handleMatrix, 0);  // Box
                        }
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        _rotateLineMaterial.SetPass(0);
                        Graphics.DrawMeshNow(_handleLineMesh, _handleMatrix);
                        break;
                    }
            }
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 抓状态
        /// </summary>
        public override EGrabState grabState 
        { 
            get => base.grabState; 
            protected set
            {
                base.grabState = value;
                switch (value)
                {
                    case EGrabState.GrabEnter:
                        {
                            _selectionModify.canInteract = false;
                            break;
                        }
                    case EGrabState.ReleaseEnter:
                        {
                            _selectionModify.canInteract = true;
                            break;
                        }
                }
            }
        }

        [Serializable]
        [Name("拖拽操作")]
        public class DragOrientation
        {
            public Vector3 origin = Vector3.zero;
            public Vector3 axis = Vector3.zero;
            public Vector3 mouse = Vector3.zero;
            public Vector3 cross = Vector3.zero;
            public Vector3 offset = Vector3.zero;
            public Plane plane = new Plane(Vector3.up, Vector3.zero);
        }

        float axisAngle = 0f;

        public bool draggingHandle => _dragAxis != 0;

        private EAxis _dragAxis = 0;

        private Vector2 _lastMouse = Vector2.zero;

        /// <summary>
        /// 拖拽有效性检测
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected override bool CanGrab(InteractData interactData, Grabbable grabbable)
        {
            if (base.CanGrab(interactData, currentDragGameObject ? currentDragGameObject.GetComponent<Grabbable>() : default))
            {
                EAxis mask = 0;
                switch (_trsToolType)
                {
                    case ETRSToolType.Position: mask = _moveEnableAxis; break;
                    case ETRSToolType.Rotate: mask = _rotationEnableAxis; break;
                    case ETRSToolType.Scale: mask = _scaleEnableAxis; break;
                }
                _dragAxis = PickHandleAxis(Input.mousePosition) & mask;
            }
            return draggingHandle;
        }

        private void OnDragGameObjectTransformChanged(Transform transform)
        {
            if (currentDragGameObject && transform == currentDragGameObject.transform)
            {
                SetTRS(currentDragGameObject);
            }
        }

        protected override Grabbable GetGrabbable() => currentDragGameObject ? currentDragGameObject.GetComponent<Grabbable>() : default;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        protected override void OnGrabEnter()
        {
            base.OnGrabEnter();

            if (!draggingHandle) return;

            drag.offset = Vector3.zero;

            axisAngle = 0f;

            _lastMouse = Input.mousePosition;

            drag.axis = Vector3.zero;
            draggingAxesCount = 0;

            Color xA, yA, zA, xP, yP, zP, selectedAColor, selectedPColor;
            xA = yA = zA = xP = yP = zP = Color.gray;
            xP.a = yP.a = zP.a = HandleMesh.AphlaPlane;
            selectedPColor = selectedAColor = Color.yellow;
            selectedPColor.a = HandleMesh.AphlaPlane;

            if ((_dragAxis & EAxis.X) == EAxis.X)
            {
                draggingAxesCount++;
                drag.axis = trs.right;
                drag.plane.SetNormalAndPosition(trs.right, trs.position);

                xA = selectedAColor;
            }

            if ((_dragAxis & EAxis.Y) == EAxis.Y)
            {
                draggingAxesCount++;
                var n = draggingAxesCount > 1 ? Vector3.Cross(drag.axis, trs.up) : trs.up;
                drag.plane.SetNormalAndPosition(n, trs.position);
                drag.axis += trs.up;

                yA = selectedAColor;
            }

            if ((_dragAxis & EAxis.Z) == EAxis.Z)
            {
                draggingAxesCount++;
                var n = draggingAxesCount > 1 ? Vector3.Cross(drag.axis, trs.forward) : trs.forward;
                drag.plane.SetNormalAndPosition(n, trs.position);
                drag.axis += trs.forward;

                zA = selectedAColor;
            }

            var ray = grabData.ray;
            if (!ray.HasValue)
            {
                return;
            }

            // 拖拽轴为1时
            if (draggingAxesCount == 1)
            {
                if (MathLibrary.ClosestPointsOnTwoRay(new Ray(trs.position, drag.axis), ray.Value, out Vector3 a, out Vector3 b))
                {
                    drag.offset = a - trs.position;
                }
                if (drag.plane.Raycast(ray.Value, out float hit))
                {
                    drag.mouse = (ray.Value.GetPoint(hit) - trs.position).normalized;
                    drag.cross = Vector3.Cross(drag.axis, drag.mouse);
                }
            }
            else if (draggingAxesCount > 1)
            {
                if (drag.plane.Raycast(ray.Value, out float hit))
                {
                    drag.offset = ray.Value.GetPoint(hit) - trs.position;
                    drag.mouse = drag.offset.normalized;
                    drag.cross = Vector3.Cross(drag.axis, drag.mouse);
                }

                // 设置面颜色
                switch (trsToolType)
                {
                    case ETRSToolType.Position:
                        {
                            if (xA != selectedAColor)
                            {
                                xP = selectedPColor;
                            }

                            if (yA != selectedAColor)
                            {
                                yP = selectedPColor;
                            }

                            if (zA != selectedAColor)
                            {
                                zP = selectedPColor;
                            }
                            break;
                        }
                    case ETRSToolType.Rotate:
                        {
                            break;
                        }
                    case ETRSToolType.Scale:
                        {
                            xA = yA = zA = selectedAColor;
                            xP = yP = zP = selectedPColor;
                            break;
                        }
                }
            }

            // 设定拖拽三轴模型颜色
            HandleMesh.SetColor(xA, yA, zA, xP, yP, zP);
            RebuildGizmoMesh(Vector3.one);

            handleOrigin.SetTRS(trs);
            trsTransformOnBegin = new HandleTransform(trs.position, trs.rotation, grabbedObject.transform.lossyScale);
            dragObjectTransformOnBegin = new HandleTransform(grabbedObject.transform);

            dragAndTrsOffsetOnBegin = dragObjectTransformOnBegin - trsTransformOnBegin;

            dragOrgScale = grabbedObject.transform.localScale;
        }

        private Vector3 dragOrgScale = Vector3.one;

        /// <summary>
        /// 拖拽中
        /// </summary>
        protected override void OnHolding()
        {
            base.OnHolding();

            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return;

            if (!draggingHandle) return;

            var ray = holdData.ray;
            if (!ray.HasValue) return;

            var point = Vector3.zero;

            if (draggingAxesCount < 2 && trsToolType != ETRSToolType.Rotate)
            {
                if (!MathLibrary.ClosestPointsOnTwoRay(new Ray(trs.position, drag.axis), ray.Value, out point, out Vector3 b))
                {
                    return;
                }
            }
            else
            {
                if (drag.plane.Raycast(ray.Value, out float hit))
                {
                    point = ray.Value.GetPoint(hit);
                }
                else
                {
                    return;
                }
            }

            drag.origin = trs.position;
            switch (trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        trs.position = HandleHelper.Snap(point - drag.offset, _positionSnapValue);
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        var delta = (Vector2)Input.mousePosition - _lastMouse;
                        var deltaMagnitude = delta.magnitude;
                        if (MathX.ApproximatelyZero(deltaMagnitude))
                        {
                            return;
                        }
                        _lastMouse = Input.mousePosition;
                        var angleOffset = deltaMagnitude * HandleHelper.CalcMouseDeltaSignWithAxes(cam, drag.origin, drag.axis, drag.cross, delta) * _rotationSpeed;
                        axisAngle += angleOffset;
                        trs.rotation = Quaternion.AngleAxis(HandleHelper.Snap(axisAngle, _rotationSnapValue), drag.axis) * handleOrigin.rotation;

                        if (_space == ESpace.Local)
                        {
                            dragHoldRotation = dragAndTrsOffsetOnBegin.rotation * trs.rotation;
                        }
                        else
                        {
                            var oldRotation = grabbedObject.transform.rotation;
                            grabbedObject.transform.Rotate(drag.axis, angleOffset, Space.World);
                            dragHoldRotation = grabbedObject.transform.rotation;
                            grabbedObject.transform.rotation = oldRotation;
                        }

                        break;
                    }
                case ETRSToolType.Scale:
                    {
                        var p = point - drag.offset - trs.position;
                        var v = Vector3.one + (draggingAxesCount > 1 ? GetUniformMagnitude(p) : Quaternion.Inverse(handleOrigin.rotation) * p);

                        var scale = HandleHelper.Snap(v, _scaleSnapValue);
                        RebuildGizmoMesh(scale);
                        dragHoldLocalScale = Vector3.Scale(dragOrgScale, scale);
                        break;
                    }
            }

            RebuildGizmoMatrix();
        }

        private Quaternion dragHoldRotation;
        private Vector3 dragHoldLocalScale;

        /// <summary>
        /// 尝试获取拖拽位置数据
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool TryGetDragPosition(out Vector3 position)
        {
            switch (trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        position = trs.position;
                        return true;
                    }
            }
            return base.TryGetDragPosition(out position);
        }

        /// <summary>
        /// 尝试获取拖拽旋转数据
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        protected override bool TryGetDragRotation(out Quaternion rotation)
        {
            switch (trsToolType)
            {
                case ETRSToolType.Rotate:
                    {
                        rotation = dragHoldRotation;
                        return true;
                    }
            }
            return base.TryGetDragRotation(out rotation);
        }

        /// <summary>
        /// 尝试获取拖拽缩放数据
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected override bool TryGetDragScale(out Vector3 scale)
        {
            switch (trsToolType)
            {
                case ETRSToolType.Scale:
                    {
                        scale = dragHoldLocalScale;
                        return true;
                    }
            }
            return base.TryGetDragScale(out scale);
        }

        private HandleTransform trsTransformOnBegin;
        private HandleTransform dragObjectTransformOnBegin;
        private HandleTransform dragAndTrsOffsetOnBegin;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        protected override void OnReleaseEnter()
        {
            base.OnReleaseEnter();

            draggingAxesCount = 0;
            HandleMesh.RecoverColor();
            RebuildGizmoMesh(Vector3.one, true);

            StartCoroutine(DelaySetDraggingFalse());
        }

        /// <summary>
        /// 选择集发生改变
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            base.OnSelectionChanged(oldSelections, flag);

            if (_selectionModify)
            {
                currentDragGameObject = _selectionModify.currentSelection;
                Rebuild();
            }
        }

        /// <summary>
        /// 设置所有向量长度为最大长度
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private Vector3 GetUniformMagnitude(Vector3 a)
        {
            var absX = Mathf.Abs(a.x);
            var absY = Mathf.Abs(a.y);
            var absZ = Mathf.Abs(a.z);
            
            float max = absX > absY && absX > absZ ? a.x : absY > absZ ? a.y : a.z;
            a.x = a.y = a.z = max;
            return a;
        }

        private IEnumerator DelaySetDraggingFalse()
        {
            yield return new WaitForEndOfFrame();
            _dragAxis = 0;
        }

        #endregion

        #region 拾取轴

        /// <summary>
        /// 检测点击轴或面
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        private EAxis PickHandleAxis(Vector2 mousePosition)
        {
            switch (trsToolType)
            {
                case ETRSToolType.Position:
                case ETRSToolType.Scale:
                    {
                        return PickPositionScaleLineOrPlane(mousePosition);
                    }
                case ETRSToolType.Rotate:
                    {
                        return PickRotationLine(mousePosition);
                    }
            }

            return 0;
        }

        private EAxis PickPositionScaleLineOrPlane(Vector2 mousePosition)
        {
            EAxis plane = 0;

            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return plane;

            // 将空间坐标转为平面坐标进行求解
            var position = trs.position;
            var sceneHandleSize = (1 + _capSize) * HandleHelper.GetScreenAndWorldRatio(position) * _handleSize;
            var center = cam.WorldToScreenPoint(position);
            var up = cam.WorldToScreenPoint(position + trs.up * sceneHandleSize);
            var right = cam.WorldToScreenPoint(position + trs.right * sceneHandleSize);
            var forward = cam.WorldToScreenPoint(position + trs.forward * sceneHandleSize);

            // 检查平面是否激活
            var cameraMask = HandleHelper.DirectionMask(trs, cam.transform.forward);

            var p_right = center + (right - center) * cameraMask.x * _boxSize;
            var p_up = center + (up - center) * cameraMask.y * _boxSize;
            var p_forward = center + (forward - center) * cameraMask.z * _boxSize;

            if (MathLibrary.PointInPolygon(new Vector2[] { center, p_up, p_up, (p_up + p_forward) - center, (p_up + p_forward) - center, p_forward, p_forward, center }, mousePosition))// x plane
            {
                plane = EAxis.Y | EAxis.Z;
            }
            else if (MathLibrary.PointInPolygon(new Vector2[] { center, p_right, p_right, (p_right + p_forward) - center, (p_right + p_forward) - center, p_forward, p_forward, center }, mousePosition))// y plane
            {
                plane = EAxis.X | EAxis.Z;
            }
            else if (MathLibrary.PointInPolygon(new Vector2[] { center, p_up, p_up, (p_up + p_right) - center, (p_up + p_right) - center, p_right, p_right, center }, mousePosition))// z plane
            {
                plane = EAxis.X | EAxis.Y;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, up) < _clickAxisMaxDistance)
            {
                plane = EAxis.Y;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, right) < _clickAxisMaxDistance)
            {
                plane = EAxis.X;
            }
            else if (MathLibrary.DistancePointLineSegment(mousePosition, center, forward) < _clickAxisMaxDistance)
            {
                plane = EAxis.Z;
            }
            return plane;
        }

        private EAxis PickRotationLine(Vector2 mousePosition)
        {
            EAxis plane = 0;

            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return plane;

            var best = Mathf.Infinity;
            var vertices = HandleMesh.GetRotationVertices(16, _rotationRadius);
            Vector2 cur, prev = Vector2.zero;

            for (int i = 0; i < 3; i++)
            {
                cur = cam.WorldToScreenPoint(vertices[i][0]);

                for (int n = 0; n < vertices[i].Length - 1; n++)
                {
                    prev = cur;
                    cur = cam.WorldToScreenPoint(_handleMatrix.MultiplyPoint3x4(vertices[i][n + 1]));

                    float dist = MathLibrary.DistancePointLineSegment(mousePosition, prev, cur);

                    if (dist < best && dist < _clickAxisMaxDistance)
                    {
                        var viewDir = (_handleMatrix.MultiplyPoint3x4((vertices[i][n] + vertices[i][n + 1]) * .5f) - cam.transform.position).normalized;
                        var nrm = transform.TransformDirection(vertices[i][n]).normalized;

                        if (Vector3.Dot(nrm, viewDir) > .5f) continue;

                        best = dist;

                        switch (i)
                        {
                            case 0:
                                {
                                    plane = EAxis.Y;
                                    break;
                                }
                            case 1:
                                {
                                    plane = EAxis.Z;
                                    break;
                                }
                            case 2:
                                {
                                    plane = EAxis.X;
                                    break;
                                }
                        }
                    }
                }
            }

            return (best < _clickAxisMaxDistance + .1f) ? plane : 0;
        }

        #endregion

        #region 三轴创建与渲染

        private Mesh _handleLineMesh;

        private Mesh _handleSolidMesh;

        /// <summary>
        /// 渲染矩阵
        /// </summary>
        private Matrix4x4 _handleMatrix;

        private void Rebuild()
        {
            SetTRS(currentDragGameObject);
            RebuildGizmoMesh(Vector3.one, true);// 重建拖拽轴模型
        }

        private void OnCameraMove()
        {
            var cam = CameraHelperExtension.currentCamera;
            if (cam && cam.transform.hasChanged)
            {
                cam.transform.hasChanged = false;
                RebuildGizmoMesh(Vector3.one, true);
            }           
        }

        /// <summary>
        /// 构建矩阵
        /// </summary>
        private void RebuildGizmoMatrix()
        {
            var handleSize = HandleHelper.GetScreenAndWorldRatio(trs.position);
            _handleMatrix = trs.localToWorldMatrix * Matrix4x4.Scale(Vector3.one * handleSize * _handleSize);
        }

        /// <summary>
        /// 构建网格模型
        /// </summary>
        /// <param name="scale"></param>
        private void RebuildGizmoMesh(Vector3 scale, bool rebuildMatrix = false)
        {
            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return;

            switch (trsToolType)
            {
                case ETRSToolType.Position:
                    {
                        HandleMesh.CreateAxisLineMesh(ref _handleLineMesh, trs, scale, cam, _moveEnableAxis);
                        HandleMesh.CreateAxisTopMesh(ref _handleSolidMesh, trs, scale, cam, _coneMesh, _boxSize, _capSize, _moveEnableAxis);
                        break;
                    }
                case ETRSToolType.Rotate:
                    {
                        HandleMesh.CreateRotateMesh(ref _handleLineMesh, _circleSegment, _rotationRadius, _rotationEnableAxis);
                        break;
                    }
                case ETRSToolType.Scale:
                    {
                        HandleMesh.CreateAxisLineMesh(ref _handleLineMesh, trs, scale, cam, _scaleEnableAxis);
                        HandleMesh.CreateAxisTopMesh(ref _handleSolidMesh, trs, scale, cam, _cubeMesh, _boxSize, _capSize, _scaleEnableAxis);
                        break;
                    }
            }

            if (rebuildMatrix)
            {
                RebuildGizmoMatrix();
            }
        }

        #endregion
    }

    /// <summary>
    /// TRS工具类型
    /// </summary>
    [Name("TRS工具类型")]
    public enum ETRSToolType
    {
        [Name("无")]
        None,

        [Name("平移")]
        Position,

        [Name("旋转")]
        Rotate,

        [Name("缩放")]
        Scale,
    }
}
