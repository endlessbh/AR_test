using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 旋转关节：
    /// 1、绕着旋转轴在限定角度范围内转动
    /// 2、可使用【铰链关节】进行受力计算模拟
    /// </summary>
    [Name("旋转关节")]
    [Tip("绕着旋转轴在限定角度范围内转动；可使用【铰链关节】进行受力计算模拟", "Rotate around the rotation axis within the limited angle range; The force calculation simulation can be performed using [hinge joint]")]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [Tool(PhysicsManager.Title, nameof(InteractableVirtual), rootType = typeof(PhysicsManager))]
    [DisallowMultipleComponent]
    public class RotationJoint : PhysicsJoint
    {
        #region 旋转属性

        /// <summary>
        /// 限定范围
        /// </summary>
        [Name("限定范围")]
        [Tip("基于游戏对象初始量的相对值")]
        [LimitRange(-360, 360)]
        public Vector2 _range = new Vector2(0, 90);

        /// <summary>
        /// 初始朝向
        /// </summary>
        [Name("初始朝向")]
        public Vector3 _localInitDirection = Vector3.forward;

        /// <summary>
        /// 旋转轴
        /// </summary>
        [Name("旋转轴")]
        public Vector3Data _rotationAxis = new Vector3Data();

        /// <summary>
        /// 世界旋转轴
        /// </summary>
        public Vector3 worldRotationAxis => _rotationAxis.data;

        /// <summary>
        /// 最小值
        /// </summary>
        public override float minValue => _range.x;

        /// <summary>
        /// 最大值
        /// </summary>
        public override float maxValue => _range.y;

        /// <summary>
        /// 当前值
        /// </summary>
        public override float currentValue => _useJoint ? _hingeJoint.angle : _currentValue;
        private float _currentValue = 0;

        /// <summary>
        /// 当前刻度值
        /// </summary>
        public override int currentStep
        {
            get => base.currentStep;
            set
            {
                base.currentStep = value;

                grabbable.rotation = Quaternion.AngleAxis(currentValue, _rotationAxis.data) * orgin.rotation;
            }
        }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _rotationAxis.SetTransform(grabbable.targetTransform);
            _rotationAxis._dataType = EVector3DataType.TransformUp;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (orgin)
            {
                orginDirection = orgin.TransformDirection(_localInitDirection);

                if (_useJoint)
                {
                    CreateHingeJoint();
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_hingeJoint)
            {
                Destroy(_hingeJoint);
            }
        }

        #endregion

        #region 旋转

        private Vector3 orginDirection;
        private Quaternion lastDraggerRotation;

        /// <summary>
        /// 抓回调
        /// </summary>
        /// <param name="interactDatas"></param>
        protected override void OnGrab(Dictionary<Dragger, GrabInteractData> interactDatas)
        {
            base.OnGrab(interactDatas);

            var data = interactDatas.Values.FirstOrDefault();// 只使用第一个拖拽器作为驱动
            if (data != null && data.rotation.HasValue)
            {
                lastDraggerRotation = data.rotation.Value;
            }
        }

        /// <summary>
        /// 执行旋转
        /// </summary>
        /// <param name="grabInteractData"></param>
        protected override void DoMotion(GrabInteractData grabInteractData)
        {
            // 求拖拽器的旋转偏移量
            var draggerRotation = grabInteractData.rotation.Value;

            CalculateRotation(draggerRotation * Quaternion.Inverse(lastDraggerRotation));

            lastDraggerRotation = draggerRotation;
        }

        private void CalculateRotation(Quaternion offset)
        {
            // 求出当前机关被拖拽后的新朝向
            var newDirection = offset * grabbable.targetTransform.TransformDirection(_localInitDirection);

            var axis = _rotationAxis.data;
            newDirection = Vector3.ProjectOnPlane(newDirection, axis);

            // 原始朝向与机关新朝向的夹角
            _currentValue = Vector3.SignedAngle(orginDirection, newDirection, axis);

            //使用刻度角更新旋转
            grabbable.rotation = Quaternion.AngleAxis(_currentValue, axis) * orgin.rotation;
        }

        /// <summary>
        /// 吸附
        /// </summary>
        protected override void Snap()
        {
            currentStep = Mathf.RoundToInt(Mathf.Abs(_currentValue - _range.x) / stepSize);
            _currentValue = currentStep * stepSize + _range.x;
        }

        #endregion

        #region 铰链

        private HingeJoint _hingeJoint;

        private void CreateHingeJoint()
        {
            var connectedBody = orgin.XGetOrAddComponent<Rigidbody>();
            connectedBody.isKinematic = true;

            // 在移动对象上创建可配置关节
            _hingeJoint = grabbable.targetTransform.XGetOrAddComponent<HingeJoint>();
            if (_hingeJoint)
            {
                // 设置连接刚体
                _hingeJoint.autoConfigureConnectedAnchor = false;
                _hingeJoint.anchor = Vector3.zero;
                _hingeJoint.connectedAnchor = Vector3.zero;
                _hingeJoint.connectedBody = connectedBody;

                // 设置角度范围
                _hingeJoint.useLimits = true;
                var limit = _hingeJoint.limits;
                limit.min = minValue;
                limit.max = maxValue;
                _hingeJoint.limits = limit;

                // 设置旋转轴，铰链旋转轴是本地坐标系下的转轴
                _hingeJoint.axis = _hingeJoint.transform.InverseTransformDirection(_rotationAxis.data);
            }
        }

        #endregion
    }
}
