using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses;
using XCSJ.PluginTools.Base;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 刚体抓取器：
    /// 1、使用物理计算的方式完成对可抓对象的抓、抓住、放置、丢和扔的模拟
    /// 2、被抓对象或其父级必须存在刚体组件
    /// 3、抓分为【拖拽】和【手持】两种模式
    ///     【拖拽】：在可抓对象原有位置基础上施力进行拖拽
    ///     【抓持】：将可抓对象从远处抓到手中，或用手接触对象，并抓到手中
    /// </summary>
    [Name("刚体抓取器")]
    [Tool("选择集", rootType = typeof(ToolsManager), index = InteractionCategory.InteractorIndex)]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class RigidbodyGrabber : Dragger
    {
        #region 属性

        /// <summary>
        /// 自身刚体
        /// </summary>
        protected Rigidbody ownRigidbody
        {
            get
            {
                if (!_rigidbody)
                {
                    _rigidbody = GetComponent<Rigidbody>();
                    _rigidbody.useGravity = false;
                    _rigidbody.isKinematic = true;
                }
                return _rigidbody;
            }
        }

        private Rigidbody _rigidbody;

        /// <summary>
        /// 抓模式
        /// </summary>
        public enum EGrabMode
        {
            [Name("拖拽")]
            [Tip("在可抓对象原有位置基础上施力进行拖拽")]
            Drag,

            [Name("拿")]
            [Tip("将可抓对象抓到指定位置并保持")]
            Carry,

            [Name("自动")]
            [Tip("运行时通过可抓对象信息判断使用【拖拽】或【拿】模式")]
            Auto,
        }

        /// <summary>
        /// 抓模式
        /// </summary>
        [Name("抓模式")]
        [EnumPopup]
        public EGrabMode _grabMode = EGrabMode.Auto;

        /// <summary>
        /// 抓住物体最大距离
        /// </summary>
        [Name("抓住物体最大距离")]
        [Tip("抓取器与物体的距离超过本值时无法执行抓命令")]
        [Min(0)]
        public float _grabMaxDistance = 10;

        private EGrabMode grabModeOnRuntime = EGrabMode.Drag;

        protected override bool countUI => grabModeOnRuntime == EGrabMode.Drag;

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (ownRigidbody) { }
            handColliders = GetComponentsInChildren<Collider>();

            if (!_holdPoint)
            {
                _holdPoint = transform;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public override void ResetData()
        {
            base.ResetData();

            _outTimeCounter = 0;
            touchColliders.Clear();
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (grabState == EGrabState.Holding)
            {
                if (grabModeOnRuntime == EGrabMode.Carry)
                {
                    TryInteract(base.CreateHoldData(), out _);
                }
            }
        }

        /// <summary>
        /// 计算当前抓模式        
        /// </summary>
        private EGrabMode CalculateGrabMode()
        {
            if (grabbedObject.isConnectToOther) // 被抓对象连接了其他对象时, 使用拖拽模式，优先级最高
            {
                return EGrabMode.Drag;
            }
            else if (_grabMode == EGrabMode.Auto)
            {
                if (_holdPoint)
                {
                    if (grabbedObject._grabPoint)// 被抓对象有持有点时
                    {
                        return EGrabMode.Carry;
                    }
                    else // 根据现实情况人拖拽物体的质量比手持物体的质量要大，因为手持需要克服重力
                    {
                        return grabbedObject.targetRigidbody.mass < _holdMaxMass ? EGrabMode.Carry : EGrabMode.Drag;
                    }
                }
                else // 无持有参考点时，使用拖拽
                {
                    return EGrabMode.Drag;
                }
            }
            else
            {
                return _grabMode;
            }
        }

        #endregion

        #region Dragger方法

        /// <summary>
        /// 拖拽保持力系数
        /// </summary>
        [Group("拖拽设置", textEN = "Drag Settings")]
        [Name("拖拽力系数")]
        [Tip("系数越大抓力越大")]
        [Min(0)]
        public float _dragHoldForceCoefficient = 200f;

        public override EGrabState grabState 
        { 
            get => base.grabState; 
            protected set
            {
                base.grabState = value;

                // 开始抓之后设置IK为false，需要物理控制抓的过程
                if (base.grabState == EGrabState.Grabing)
                {
                    var rig = grabbedObject.GetComponent<Rigidbody>();
                    if (rig)
                    {
                        rig.isKinematic = false;
                    }
                }
            }
        }

        /// <summary>
        /// 能否抓
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected override bool CanGrab(InteractData interactData, Grabbable grabbable)
        {
            return base.CanGrab(interactData, grabbable) && grabbable && grabbable.targetRigidbody && grabData.raycastHit.Value.distance < _grabMaxDistance;
        }

        /// <summary>
        /// 抓进入
        /// </summary>
        protected override void OnGrabEnter()
        {
            base.OnGrabEnter();

            grabModeOnRuntime = CalculateGrabMode();

            _outTimeCounter = 0;

            switch (grabModeOnRuntime)
            {
                case EGrabMode.Drag: OnGrabEnter_Drag(); break;
                case EGrabMode.Carry: OnGrabEnter_Carry(); break; 
            }
        }

        /// <summary>
        /// 执行抓的过渡动作
        /// </summary>
        protected override bool IsGrabbing()
        {
            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry: return IsGrabbing_Carry();
            }

            return base.IsGrabbing();
        }

        /// <summary>
        /// 抓取中
        /// </summary>
        protected override void OnGrabbing()
        {
            base.OnGrabbing();

            _outTimeCounter += Time.deltaTime;
            if (_outTimeCounter > _grabOrReleaseMaxTime)
            {
                ResetData();
            }
        }

        /// <summary>
        /// 抓对象处理
        /// 1、忽略抓对象与抓取器之间的碰撞
        /// 2、设置抓对象刚体线性及旋转速度为0
        /// 3、抓对象为自由对象时，将抓对象移动到抓取器上；抓对象被另外一个作用力作用时，将抓取器移动至被抓对象上
        /// </summary>
        protected override void OnGrabExit()
        {
            base.OnGrabExit();

            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry: OnGrabExit_Carry(); break;
            }
        }

        /// <summary>
        /// 获取可抓对象
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override Grabbable GetGrabbable(IInteractable interactable)
        {
            // 接触的优先级高于射线
            var grabbable = GetGrabbableInTouchColliders();
            if (!grabbable)
            {
                grabbable = GetGrabbableInternal(interactable);
            }
            return grabbable;
        }

        private Grabbable GetGrabbableInTouchColliders()
        {
            Grabbable grabbable = null;
            for (int i = touchColliders.Count - 1; i >= 0; --i)
            {
                var collider = touchColliders[i];
                if (!collider || !collider.gameObject.activeSelf)// 当前对象非激活，则移除该对象
                {
                    touchColliders.RemoveAt(i);
                    continue;
                }
                if (!grabbable)
                {
                    grabbable = collider.GetComponentInParent<Grabbable>();
                }
            }
            return grabbable;
        }

        /// <summary>
        /// 抓住
        /// </summary>
        protected override void OnHolding()
        {
            base.OnHolding();

            switch (grabModeOnRuntime)
            {
                case EGrabMode.Drag: OnHolding_Drag(); break; 
                case EGrabMode.Carry: OnHolding_Carry(); break;
            }
        }

        /// <summary>
        /// 创建拖拽数据
        /// </summary>
        /// <returns></returns>
        protected override GrabInteractData CreateHoldData()
        {
            try
            {
                switch (grabModeOnRuntime)
                {
                    case EGrabMode.Drag:
                        {
                            if (hitOffset.HasValue && hitOffsetScale.HasValue && hitPosition.HasValue)
                            {
                                var dragPosition = grabbedObject.transform.TransformPoint(hitLocalPointOnGrab) + hitOffset.Value;
                                return new GrabInteractData(dragPosition, transform.rotation, hitOffset.Value, hitOffsetScale.Value, hitPosition.Value, holdData.cmdName, this, holdData.interactables);
                            }
                            break;
                        }
                }
                return base.CreateHoldData();
            }
            finally
            {
                hitOffset = null;
                hitOffsetScale = null;
                hitPosition = null;
            }
        }

        /// <summary>
        /// 获取拖拽位置数据
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override bool TryGetDragPosition(out Vector3 position)
        {
            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry:
                    {
                        position = grabbedObjectFollowPosition;
                        return true;
                    }
            }
            return base.TryGetDragPosition(out position);
        }

        /// <summary>
        /// 获取拖拽旋转数据
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        protected override bool TryGetDragRotation(out Quaternion rotation)
        {
            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry:
                    {
                        rotation = grabbedObjectFollowRotation;
                        return true;
                    }
            }
            return base.TryGetDragRotation(out rotation);
        }

        /// <summary>
        /// 释放进入
        /// </summary>
        protected override void OnReleaseEnter()
        {
            base.OnReleaseEnter();

            _outTimeCounter = 0;

            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry: OnReleaseEnter_Carry(); break;
            }
        }

        /// <summary>
        /// 执行放的过渡动作
        /// </summary>
        protected override bool IsReleaseing()
        {
            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry: return IsReleaseing_Carry(); 
            }
            return base.IsReleaseing();
        }

        /// <summary>
        /// 释放中
        /// </summary>
        protected override void OnReleaseing()
        {
            base.OnReleaseing();

            _outTimeCounter += Time.deltaTime;
            if (_outTimeCounter > _grabOrReleaseMaxTime)
            {
                ResetData();
            }
        }

        /// <summary>
        /// 放置
        /// </summary>
        protected override void OnReleaseExit()
        {
            base.OnReleaseExit();

            switch (grabModeOnRuntime)
            {
                case EGrabMode.Carry: OnReleaseExit_Carry(); break;
            }

            // 延迟调用恢复碰撞关系，以免对象还没离开,就与抓取交互器产生碰撞，并被弹飞没进入插槽中则重置刚体属性
            Invoke(nameof(RecoverCollision), 1f);
        }
        
        #endregion

        #region 拖拽

        private void OnGrabEnter_Drag()
        {
            var rayHit = grabData.raycastHit;
            if (rayHit.HasValue && grabbedObject)
            {
                var hit = rayHit.Value;
                hitLocalPointOnGrab = grabbedObject.transform.InverseTransformPoint(hit.point);
                distanceFromOriginOnGrab = hit.distance;
            }
        }

        private void OnHolding_Drag()
        {
            var ray = holdData.ray;
            if (ray.HasValue)
            {
                // 碰撞点世界坐标
                Vector3 hitWorldPointOnGrab = grabbedObject.transform.TransformPoint(hitLocalPointOnGrab);

                // 射线终点
                Vector3 dragPoint = ray.Value.origin + ray.Value.direction * distanceFromOriginOnGrab;

                // 对世界碰撞点施力
                hitOffset = dragPoint - hitWorldPointOnGrab;
                hitOffsetScale = _dragHoldForceCoefficient * Time.fixedDeltaTime;
                hitPosition = hitWorldPointOnGrab;

                // 绘制射线终点与碰撞点世界坐标的线段
                Debug.DrawLine(dragPoint, hitWorldPointOnGrab, Color.yellow);
            }
        }

        private Vector3? hitOffset = null;
        private float? hitOffsetScale = null;
        private Vector3? hitPosition = null;

        #endregion

        #region 拿

        private void OnGrabEnter_Carry()
        {
            grabDirectionOnEnter = holdPosition - grabbedObject.grabPoint.position;
        }

        private bool IsGrabbing_Carry()
        {
            var grabbedTransfrom = grabbedObject.transform;
            var dis = grabbedObjectFollowPosition - grabbedTransfrom.position;

            if (touchColliders.Count > 0)
            {
                // 抓取器已经和被抓对象碰撞
                foreach (var c in touchColliders)
                {
                    if (c.transform.IsChildOf(grabbedObject.transform))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // 使用初始抓取方向来判断当前位置是否超过了抓握点判断抓握完成, 或通过位置是否相近判断抓握完成
                if (Vector3.Angle(grabDirectionOnEnter, grabbedTransfrom.position - holdPosition) < 90
                    || dis.sqrMagnitude < 0.01f)
                {
                    return false;
                }
            }

            var rig = grabbedObject.targetRigidbody;
            rig.velocity = dis.normalized * (_grabMoveSpeed * Time.fixedDeltaTime + rig.velocity.magnitude);
            grabbedObject.transform.rotation = Quaternion.Slerp(grabbedTransfrom.rotation, grabbedObjectFollowRotation, _grabRotationSpeed * Time.fixedDeltaTime);

            return true;
        }

        private void OnGrabExit_Carry()
        {
            //1、 忽略抓取器与抓对象碰撞
            IngoreCollision();

            //2、 设置抓对象线速度和角速度为0
            grabbedObject.targetRigidbody.SetVelocityZero();

            //3、 抓对象为自由移动物体，将其移到保持点
            grabbedObject.targetRigidbody.SetRigidbodyPose(grabbedObjectFollowPosition, grabbedObjectFollowRotation);
        }

        private void OnHolding_Carry()
        {
            if (_releaseMode == EReleaseMode.VelocityTrack)
            {
                _velocityTracker.Record(grabbedObject.position, grabbedObject.rotation);
            }
        }
       
        private void OnReleaseEnter_Carry()
        {
            switch (_releaseMode)
            {
                case EReleaseMode.VelocityTrack:
                    {
                        var rig = grabbedObject.targetRigidbody;
                        if (rig && _velocityTracker.TryGetVelocity(out var velocity, out var rotation))
                        {
                            rig.velocity = velocity;
                            rig.angularVelocity = rotation;
                        }
                        break;
                    }
                case EReleaseMode.Throw:
                    {
                        if (grabbedObject.targetRigidbody)
                        {
                            grabbedObject.targetRigidbody.AddForce(_directionInfo.data * _throwForce, ForceMode.Impulse);
                        }
                        break;
                    }
                case EReleaseMode.PutOnRay:
                    {
                        // 有效射线数据时
                        if (releaseData.raycastHit.HasValue)
                        {
                            var raycastHit = releaseData.raycastHit.Value;
                            // 优先拾取射线点中的插槽交互器对象
                            var ss = raycastHit.transform.GetComponentInParent<SingleSocket>();
                            if (ss)
                            {
                                putPosition = ss._grabPoseReference.position;
                            }
                            else// 放置位置 = 射线碰撞点 + 物体包围盒高度的一半 * 碰撞法线
                            {
                                var position = raycastHit.point;
                                if (CommonFun.GetBounds(out var bounds, grabbedObject.targetTransform))
                                {
                                    position += raycastHit.normal * bounds.size.y / 2;
                                }
                                putPosition = position;
                            }
                        }
                        else // 未拾取到射线则使用当前抓点到地面的射线进行放置
                        {
                            if (CommonFun.GetBounds(out var bounds, grabbedObject.targetTransform))
                            {
                                // 射线成功时，放置位置 = 碰撞点 + 上向量 * 物体包围盒高度的一半
                                if (Physics.Raycast(holdPosition, Vector3.down, out var hit))
                                {
                                    putPosition = hit.point + Vector3.up * bounds.size.y / 2;
                                }
                                else // 射线失败时，放置位置 = 抓点位置 + 下向量 * 物体尺寸最长斜边
                                {
                                    putPosition = holdPosition + Vector3.down * bounds.size.magnitude;
                                }
                            }
                            else
                            {
                                putPosition = holdPosition + Vector3.down;
                            }
                        }
                        releaseDirectionOnEnter = putPosition - grabbedObject.position;
                        break;
                    }
            }
        }

        private Vector3 releaseDirectionOnEnter = Vector3.zero;

        private bool IsReleaseing_Carry()
        {
            switch (_releaseMode)
            {
                case EReleaseMode.PutOnRay:
                    {
                        var dis = putPosition - grabbedObject.position;
                        if (grabbedObject.isCollision || dis.sqrMagnitude < 0.01f || Vector3.Angle(releaseDirectionOnEnter, dis) > 90)
                        {
                            return false;
                        }

                        grabbedObject.targetRigidbody.velocity = dis.normalized * (_grabMoveSpeed * Time.fixedDeltaTime + grabbedObject.targetRigidbody.velocity.magnitude);
                        return true;
                    }
            }
            return false;
        }

        private void OnReleaseExit_Carry()
        {
            switch (_releaseMode)
            {
                case EReleaseMode.VelocityTrack:
                    {
                        break;
                    }
                case EReleaseMode.Throw:
                    {
                        break;
                    }
                case EReleaseMode.PutOnRay:
                    {
                        grabbedObject.targetRigidbody.SetVelocityZero();
                        break;
                    }
            }
        }

        [Group("拿设置", textEN = "Carry Settings")]
        /// <summary>
        /// 抓或放过程最大时长(秒)
        /// </summary>
        [Name("拿过程最大时长(秒)")]
        public float _grabOrReleaseMaxTime = 10;
        private float _outTimeCounter = 0;

        /// <summary>
        /// 持有最大质量
        /// </summary>
        [Name("拿最大质量")]
        [Tip("被抓物体质量小于本值时，持有对象；被抓物体质量大于本值时，拖拽对象")]
        [Min(0)]
        public float _holdMaxMass = 10;

        /// <summary>
        /// 持有参考点
        /// </summary>
        [Name("拿参考点")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _holdPoint;

        /// <summary>
        /// 抓住对象移动速度
        /// </summary>
        [Name("拿对象移动速度")]
        [Tip("速度单位为：米/秒")]
        [Min(0)]
        public float _grabMoveSpeed = 1;

        /// <summary>
        /// 抓住对象旋转速度
        /// </summary>
        [Name("拿对象旋转速度")]
        [Min(0)]
        public float _grabRotationSpeed = 1;

        /// <summary>
        /// 持有位置
        /// </summary>
        public Vector3 holdPosition => _holdPoint ? _holdPoint.position : transform.position;

        /// <summary>
        /// 持有旋转量
        /// </summary>
        public Quaternion holdRotation => _holdPoint ? _holdPoint.rotation : transform.rotation;

        /// <summary>
        /// 被抓对象最终移动到的持有点
        /// </summary>
        private Vector3 grabbedObjectFollowPosition => holdPosition - grabbedObject.grabPointPositionOffset;

        /// <summary>
        /// 被抓对象最终移动到的持有点
        /// </summary>
        private Quaternion grabbedObjectFollowRotation => holdRotation * Quaternion.Inverse(grabbedObject.grabPointRotationOffset);

        private Vector3 grabDirectionOnEnter = Vector3.zero;

        /// <summary>
        /// 抓时局部坐标系下的碰撞点
        /// </summary>
        private Vector3 hitLocalPointOnGrab;

        /// <summary>
        /// 用于记录抓住对象时，射线原点与射线碰撞点的距离
        /// </summary>
        private float distanceFromOriginOnGrab;

        /// <summary>
        /// 摆放位置
        /// </summary>
        private Vector3 putPosition;

        /// <summary>
        /// 扔模式
        /// </summary>
        public enum EReleaseMode
        {
            [Name("速度追踪")]
            [Tip("速度追踪器会在一段时间内追踪抓取器的移动及旋转速度", "The speed tracker will track the movement and rotation speed of the gripper for a period of time")]
            VelocityTrack,

            [Name("扔")]
            [Tip("朝设定的方向对可抓对象施加力作用", "Apply force to the gripping object in the set direction")]
            Throw,

            [Name("放置于射线投射点")]
            PutOnRay,
        }

        /// <summary>
        /// 放模式
        /// </summary>
        [Name("放模式")]
        [EnumPopup]
        public EReleaseMode _releaseMode = EReleaseMode.PutOnRay;

        /// <summary>
        /// 速度追踪器
        /// </summary>
        [Name("速度追踪器")]
        [Tip("用于追踪一段时间内，抓取器的速度，并实现扔速度的计算")]
        [HideInSuperInspector(nameof(_releaseMode), EValidityCheckType.NotEqual | EValidityCheckType.Or, EReleaseMode.VelocityTrack)]
        public VelocityTracker _velocityTracker = new VelocityTracker();

        /// <summary>
        /// 扔力
        /// </summary>
        [Name("扔力")]
        [Min(0)]
        [HideInSuperInspector(nameof(_releaseMode), EValidityCheckType.NotEqual | EValidityCheckType.Or, EReleaseMode.Throw)]
        public float _throwForce = 100;

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        [HideInSuperInspector(nameof(_releaseMode), EValidityCheckType.NotEqual | EValidityCheckType.Or, EReleaseMode.Throw)]
        public Vector3Data _directionInfo = new Vector3Data();

        private Collider[] handColliders = null;

        [Readonly]
        [HideInSuperInspector]
        public List<Collider> touchColliders = new List<Collider>();

        private void OnCollisionEnter(Collision collision)
        {
            var c = collision.collider;
            if (!touchColliders.Contains(c))
            {
                touchColliders.Add(c);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            var c = collision.collider;
            touchColliders.Remove(c);
        }

        private void IngoreCollision()
        {
            grabbedObjectColliders.AddRange(grabbedObject.transform.GetComponentsInChildren<Collider>());
            PhysicsHelper.IgnoreCollision(handColliders, grabbedObjectColliders, true);
        }

        private void RecoverCollision()
        {
            // 恢复抓取器与抓对象碰撞
            PhysicsHelper.IgnoreCollision(handColliders, grabbedObjectColliders, false);
            grabbedObjectColliders.Clear();
        }

        private List<Collider> grabbedObjectColliders = new List<Collider>();

        #endregion
    }
}
