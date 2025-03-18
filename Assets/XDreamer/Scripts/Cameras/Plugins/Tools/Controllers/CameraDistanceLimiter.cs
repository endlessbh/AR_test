using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机距离限定器:默认通过输入鼠标XY与滚轮的偏移量控制相机的移动
    /// </summary>
    [Name("相机距离限定器")]
    [Tip("默认通过输入鼠标XY与滚轮的偏移量控制相机的移动", "By default, the movement of the camera is controlled by entering the offset between the mouse XY and the wheel")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController), */nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraDistanceLimiter : BaseCameraLimiter
    {
        /// <summary>
        /// 距离区间
        /// </summary>
        [Name("距离区间")]
        [LimitRange(0.001f, 1000f)]
        public Vector2 _distanceRange = new Vector2(0.01f, 100);

        /// <summary>
        /// 距离限定规则
        /// </summary>
        [Name("距离限定规则")]
        public enum EDistanceLimitRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 目标:以相机目标对象位置点为测距点，限定相机与测距点的距离
            /// </summary>
            [Name("目标")]
            [Tip("以相机目标对象位置点为测距点，限定相机与测距点的距离", "Taking the location point of the camera target object as the ranging point, the distance between the camera and the ranging point is limited")]
            Target,

            /// <summary>
            /// 射线:限定以相机为原点的射线到命中碰撞体点的距离
            /// </summary>
            [Name("射线")]
            [Tip("以相机为原点的射线命中碰撞体的点为测距点，限定相机与测距点的距离", "The point where the ray hitting the collision body with the camera as the origin is the ranging point, which limits the distance between the camera and the ranging point")]
            Ray,

            /// <summary>
            /// 位置:以已知世界坐标位置点为测距点，限定相机与测距点的距离
            /// </summary>
            [Name("位置")]
            [Tip("以已知世界坐标位置点为测距点，限定相机与测距点的距离", "Taking the known world coordinate position point as the ranging point, the distance between the camera and the ranging point is limited")]
            Position,

            /// <summary>
            /// 变换位置:以变换对象的世界坐标位置点为测距点，限定相机与测距点的距离
            /// </summary>
            [Name("变换位置")]
            [Tip("以变换对象的世界坐标位置点为测距点，限定相机与测距点的距离", "Taking the world coordinate position point of the transformed object as the ranging point, the distance between the camera and the ranging point is limited")]
            TransformPosition,
        }

        /// <summary>
        /// 距离限定规则
        /// </summary>
        [Name("距离限定规则")]
        [EnumPopup]
        public EDistanceLimitRule _distanceLimitRule = EDistanceLimitRule.Target;

        /// <summary>
        /// 测距点
        /// </summary>
        [Name("测距点")]
        [Readonly]
        public Vector3 _rangingPoint = new Vector3();

        /// <summary>
        /// 距离：相机与测距点距离
        /// </summary>
        [Name("距离")]
        [Tip("相机与测距点距离", "Distance between camera and ranging point")]
        public float _distance = 0;

        /// <summary>
        /// 测距点位置
        /// </summary>
        [Name("测距点位置")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Position)]
        public Vector3 _rangingPointPosition = new Vector3();

        /// <summary>
        /// 测距点变换
        /// </summary>
        [Name("测距点变换")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.TransformPosition)]
        public Transform _rangingPointTransform;

        #region 射线属性

        /// <summary>
        /// 有命中:标识射线检测是否命中
        /// </summary>
        [Name("有命中")]
        [Tip("标识射线检测是否命中", "Identify whether the radiographic test is hit")]
        [Readonly]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public bool _hasHit = false;

        [Name("命中变换")]
        [Tip("标识射线检测命中的变换对象", "Identifies the transform object that the radiographic test hit")]
        [Readonly]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public Transform _hitTransform = null;

        /// <summary>
        /// 图层蒙版:射线检测的图层蒙版
        /// </summary>
        [Name("图层蒙版")]
        [Tip("射线检测的图层蒙版", "Layer mask for radiographic testing")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public LayerMask _layerMask = 1;

        /// <summary>
        /// 未命中时规则
        /// </summary>
        public enum ERuleOnHitMiss
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 相机背向移动
            /// </summary>
            [Name("相机背向移动")]
            CameraBackMove
        }

        /// <summary>
        /// 未命中时规则
        /// </summary>
        [Name("未命中时规则")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public ERuleOnHitMiss _ruleOnHitMiss = ERuleOnHitMiss.CameraBackMove;

        /// <summary>
        /// 相机背向移动次数
        /// </summary>
        [Name("相机背向移动次数")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EDistanceLimitRule.Ray, nameof(_ruleOnHitMiss), EValidityCheckType.NotEqual, ERuleOnHitMiss.CameraBackMove)]
        public int _cameraBackMoveCount = 1;

        /// <summary>
        /// 相机背向移动距离
        /// </summary>
        [Name("相机背向移动距离")]
        [Tip("当未命中时相机背向移动的距离")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EDistanceLimitRule.Ray, nameof(_ruleOnHitMiss), EValidityCheckType.NotEqual, ERuleOnHitMiss.CameraBackMove)]
        public float _cameraBackMoveDistance = 10;

        /// <summary>
        /// 射线方向规则
        /// </summary>
        [Name("射线方向规则")]
        public enum ERayDirRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 相机朝向
            /// </summary>
            [Name("相机朝向")]
            CameraForward,

            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom,
        }

        /// <summary>
        /// 射线方向规则
        /// </summary>
        [Name("射线方向规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public ERayDirRule _rayDirRule = ERayDirRule.CameraForward;

        /// <summary>
        /// 射线方向
        /// </summary>
        [Name("射线方向")]
        [Tip("自定义的射线方向", "Custom ray direction")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EDistanceLimitRule.Ray, nameof(_rayDirRule), EValidityCheckType.NotEqual, ERayDirRule.Custom)]
        public Vector3 _rayDir = new Vector3(0, 0, -1);

        /// <summary>
        /// 尝试获取射线方向
        /// </summary>
        /// <param name="rayDir"></param>
        /// <returns></returns>
        public bool TryGetRayDir(out Vector3 rayDir)
        {
            switch (_rayDirRule)
            {
                case ERayDirRule.CameraForward:
                    {
                        rayDir = cameraTransformer.forward;
                        return true;
                    }
                case ERayDirRule.Custom:
                    {
                        rayDir = _rayDir;
                        return true;
                    }
            }
            rayDir = default;
            return false;
        }

        /// <summary>
        /// 同步到相机目标位置
        /// </summary>
        public enum ESyncToCameraTargetPosition
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 命中点
            /// </summary>
            [Name("命中点")]
            [Tip("将射线的命中点世界坐标尝试同步到相机目标位置", "Try to synchronize the world coordinates of the ray's midpoint to the camera target position")]
            HitPoint,
        }

        /// <summary>
        /// 同步到相机目标位置
        /// </summary>
        [Name("同步到相机目标位置")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public ESyncToCameraTargetPosition _syncToCameraTargetPosition = ESyncToCameraTargetPosition.HitPoint;

        /// <summary>
        /// 同步到相机目标旋转
        /// </summary>
        public enum ESyncToCameraTargetRotation
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 相机前向量投影到世界X0Z面
            /// </summary>
            [Name("相机前向量投影到世界X0Z面")]
            [Tip("将当前相机控制器的前向量投影到世界X0Z面作为新的前向量，世界标准Y向量做为新的Y向量；", "Project the front vector of the current camera controller onto the world x0z plane as the new front vector, and the world standard y vector as the new y vector;")]
            CameraForward_ProjectTo_WorldX0Z,
        }

        /// <summary>
        /// 同步到相机目标旋转
        /// </summary>
        [Name("同步到相机目标旋转")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public ESyncToCameraTargetRotation _syncToCameraTargetRotation = ESyncToCameraTargetRotation.CameraForward_ProjectTo_WorldX0Z;

        /// <summary>
        /// 同步到相机目标包围盒
        /// </summary>
        public enum ESyncToCameraTargetBounds
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 命中点到中心
            /// </summary>
            [Name("命中点到中心")]
            [Tip("将射线的命中点世界坐标尝试同步到相机目标包围盒中心", "Try to synchronize the world coordinates of the ray's midpoint to the center of the camera target bounding box")]
            HitPointToCenter,
        }

        /// <summary>
        /// 同步到相机目标包围盒
        /// </summary>
        [Name("同步到相机目标包围盒")]
        [HideInSuperInspector(nameof(_distanceLimitRule), EValidityCheckType.NotEqual, EDistanceLimitRule.Ray)]
        public ESyncToCameraTargetBounds _syncToCameraTargetBounds = ESyncToCameraTargetBounds.HitPointToCenter;

        #endregion

        /// <summary>
        /// 距离保持规则
        /// </summary>
        [Name("距离保持规则")]
        public enum EDistanceKeepRule
        {
            [Name("无")]
            None,

            [Name("尝试保持距离")]
            [Tip("尝试保持相机与测距点距离恒定", "Try to keep the distance between the camera and the ranging point constant")]
            TryKeepDistance,
        }

        /// <summary>
        /// 距离保持规则
        /// </summary>
        [Name("距离保持规则")]
        [EnumPopup]
        public EDistanceKeepRule _distanceKeepRule = EDistanceKeepRule.None;

        /// <summary>
        /// 尝试限定距离
        /// </summary>
        /// <param name="cameraPosition"></param>
        /// <param name="rangingPoint"></param>
        /// <returns>如果修改了相机位置，即修改前距离不在距离区间的限定范围内，则返回True；否则返回False；</returns>
        private bool TryLimitDistance(Vector3 cameraPosition, Vector3 rangingPoint)
        {
            _rangingPoint = rangingPoint;
            var dir = cameraPosition - rangingPoint;
            var newDistance = dir.magnitude;//新的距离
            if (newDistance < _distanceRange.x)
            {
                newDistance = _distanceRange.x;
            }
            else if (newDistance > _distanceRange.y)
            {
                newDistance = _distanceRange.y;
            }
            else
            {
                switch(_distanceKeepRule)
                {
                    case EDistanceKeepRule.TryKeepDistance:
                        {
                            newDistance = Mathf.Clamp(_distance, _distanceRange.x, _distanceRange.y);
                            break;
                        }
                    default:
                        {
                            _distance = newDistance;
                            return false;
                        }
                }
            }
            _distance = newDistance;
            cameraTransformer.position = rangingPoint + dir.normalized * newDistance;
            return true;
        }

        private bool TryRaycast(Vector3 origin,Vector3 dir, out Vector3 rangingPoint)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, _distanceRange.y * 2, _layerMask))
            {
                _hasHit = true;
                _hitTransform = hitInfo.transform;

                rangingPoint = hitInfo.point;

                //位置
                switch (_syncToCameraTargetPosition)
                {
                    case ESyncToCameraTargetPosition.HitPoint:
                        {
                            cameraTargetController.TrySetTargetPosition(rangingPoint);
                            break;
                        }
                }
                //旋转
                switch (_syncToCameraTargetRotation)
                {
                    case ESyncToCameraTargetRotation.CameraForward_ProjectTo_WorldX0Z:
                        {
                            var forward = Vector3.ProjectOnPlane(cameraTransformer.forward, Vector3.up);
                            var rotation = Quaternion.FromToRotation(Vector3.forward, forward);
                            cameraTargetController.TrySetTargetRotation(rotation);
                            break;
                        }
                }
                //包围盒
                switch (_syncToCameraTargetBounds)
                {
                    case ESyncToCameraTargetBounds.HitPointToCenter:
                        {
                            cameraTargetController.TrySetTargetBoundsCenter(rangingPoint);
                            break;
                        }
                }
                return true;
            }
            else
            {
                _hasHit = false;
                _hitTransform = null;
            }

            rangingPoint = default;
            return false;
        }

        /// <summary>
        /// 尝试获取测距点
        /// </summary>
        /// <param name="rangingPoint"></param>
        /// <returns></returns>
        private bool TryGetRangingPoint(out Vector3 rangingPoint)
        {
            switch (_distanceLimitRule)
            {
                case EDistanceLimitRule.Target:
                    {
                        rangingPoint = cameraTargetController.targetPosition;
                        return true;
                    }
                case EDistanceLimitRule.Ray:
                    {
                        if (!TryGetRayDir(out var dir))
                        {
                            _hasHit = false;
                            _hitTransform = null;
                            break;
                        }

                        //射线检测
                        if (TryRaycast(cameraTransformer.position, dir, out rangingPoint))
                        {                            
                            return true;
                        }
                        else
                        {
                            switch (_ruleOnHitMiss)
                            {
                                case ERuleOnHitMiss.CameraBackMove:
                                    {
                                        var position = cameraTransformer.position;
                                        var back = -cameraTransformer.forward;
                                        for (var i = 1; i < _cameraBackMoveCount; i++)
                                        {
                                            if (TryRaycast(position + back * i, dir, out rangingPoint)) return true;
                                        }
                                        break;
                                    }
                            }
                        }

                        break;
                    }
                case EDistanceLimitRule.Position:
                    {
                        rangingPoint = _rangingPointPosition;
                        return true;
                    }
                case EDistanceLimitRule.TransformPosition:
                    {
                        if(_rangingPointTransform)
                        {
                            rangingPoint = _rangingPointTransform.position;
                            return true;
                        }
                        break;
                    }
            }
            rangingPoint = default;
            return false;
        }

        /// <summary>
        /// 尝试更新距离
        /// </summary>
        /// <returns></returns>
        public bool TryUpdateDistance()
        {
            if (TryGetRangingPoint(out var rangingPoint))
            {
                _rangingPoint = rangingPoint;
                var dir = cameraTransformer.position - rangingPoint;
                _distance = dir.magnitude;
                return true;
            }
            return false;
        }

        private bool TryUpdateDistanceAndLimit()
        {
            return TryUpdateDistance() && TryLimitDistance(cameraTransformer.position, _rangingPoint);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!enabled) return;
            inTween = false;
            TryUpdateDistance();

            BaseCameraTransformer.onBeforeTransformTo += OnBeforeTransformTo;
            BaseCameraTransformer.onAfterTransformTo += OAfterTransformTo;

            BaseCameraTransformer.onBeforeTranslate += OnBeforeTranslate;
            BaseCameraTransformer.onAfterTranslate += OnAfterTranslate;
            BaseCameraTransformer.onAfterRotate += OnAfterRotate;
            BaseCameraTransformer.onAfterRotateAround += OnAfterRotateAround;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            BaseCameraTransformer.onBeforeTransformTo -= OnBeforeTransformTo;
            BaseCameraTransformer.onAfterTransformTo -= OAfterTransformTo;

            BaseCameraTransformer.onBeforeTranslate -= OnBeforeTranslate;
            BaseCameraTransformer.onAfterTranslate -= OnAfterTranslate;
            BaseCameraTransformer.onAfterRotate -= OnAfterRotate;
            BaseCameraTransformer.onAfterRotateAround -= OnAfterRotateAround;
        }

        private void OnBeforeTransformTo(BaseCameraTransformer cameraTransformer, Vector3 dstPosition, Quaternion dstRotation, float time)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;

            RecordLastCameraPosition();
        }

        private void OAfterTransformTo(BaseCameraTransformer cameraTransformer, Vector3 srcPosition, Quaternion srcRotation, float time)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;

            UpdateDistance();
        }

        private void OnBeforeTranslate(BaseCameraTransformer cameraTransformer, Vector3 translation, Space relativeTo)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;
            inTween = true;
            RecordLastCameraPosition();
        }

        private void OnAfterTranslate(BaseCameraTransformer cameraTransformer, Vector3 translation, Space relativeTo)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;

            inTween = false;
            UpdateDistance();
        }

        bool inTween = false;

        Vector3 lastCameraPosition;

        private void RecordLastCameraPosition()
        {
            lastCameraPosition = cameraTransformer.position;
        }

        private void UpdateDistance()
        {
            if (TryGetRangingPoint(out var rangingPosition))
            {
                var currentCameraPosition = cameraTransformer.position;
                var moveDir = currentCameraPosition - lastCameraPosition;
                var dir = currentCameraPosition - rangingPosition;

                if (moveDir == Vector3.zero) return;

                var angle = Vector3.Angle(moveDir, dir);
                if (Mathf.Approximately(angle, 0))
                {
                    _distance += moveDir.magnitude;
                }
                else if (Mathf.Approximately(angle, 180))
                {
                    _distance -= moveDir.magnitude;
                }
            }
        }

        private void OnAfterRotate(BaseCameraTransformer cameraTransformer, Vector3 eulers, Space relativeTo)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;
            TryUpdateDistance();
        }

        private void OnAfterRotateAround(BaseCameraTransformer cameraTransformer, Vector3 point, Vector3 axis, float angle)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_distanceKeepRule != EDistanceKeepRule.TryKeepDistance) return;
            TryUpdateDistance();
        }

        /// <summary>
        /// 尝试限制
        /// </summary>
        /// <returns></returns>
        public bool TryLimit()
        {
            return TryGetRangingPoint(out var rangingPoint) && TryLimitDistance(cameraTransformer.position, rangingPoint);
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        private void LateUpdate()
        {
            if (inTween) return;//补间中不做处理
            TryLimit();
        }

        private void OnDrawGizmosSelected()
        {
            var cameraPosition = cameraTransformer.position;
            var dir = (cameraPosition - _rangingPoint).normalized;
            var xPosition = _rangingPoint + dir * _distanceRange.x;
            var yPosition = _rangingPoint + dir * _distanceRange.y;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_rangingPoint, Mathf.Max(_distance * 0.02f, 0.1f));
            //Gizmos.DrawLine(_currentRangingPoint, cameraPosition);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(xPosition, yPosition);
        }

        /// <summary>
        /// 在编辑态执行限定
        /// </summary>
        protected override void ExcuteLimitInEdit()
        {
            base.ExcuteLimitInEdit();
            TryUpdateDistanceAndLimit();
        }
    }
}
