using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 平移关节：
    /// 1、沿着移动轴在限定长度范围内移动
    /// 2、可使用【可配置关节】进行受力计算模拟
    /// </summary>
    [Name("平移关节")]
    [Tip("沿着移动轴在限定长度范围内移动；可使用【可配置关节】进行受力计算模拟", "Move along the moving axis within the limited length range; The force calculation simulation can be performed using [Configurable joint]")]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [Tool(PhysicsManager.Title, nameof(InteractableVirtual), rootType = typeof(PhysicsManager))]
    [DisallowMultipleComponent]
    public class MoveJoint : PhysicsJoint
    {
        #region 平移属性

        /// <summary>
        /// 可配置关节移动范围限制
        /// </summary>
        [Name("可配置关节移动范围限制")]
        [Tip("可配置关节的移动范围是一个正负对称的设定值", "The movement range of the configurable joint is a positive and negative symmetrical set value")]
        [Min(0)]
        [HideInSuperInspector(nameof(_useJoint), EValidityCheckType.False)]
        public float _configurableJointLimit = 0.1f;

        /// <summary>
        /// 移动范围限制
        /// </summary>
        [Name("移动范围限制")]
        [HideInSuperInspector(nameof(_useJoint), EValidityCheckType.True)]
        public Vector2 _range = new Vector2(0, 1);

        /// <summary>
        /// 初始移动偏移量
        /// </summary>
        [Name("初始移动偏移量")]
        [Readonly(EEditorMode.Runtime)]
        public float _initOffset = 0;

        /// <summary>
        /// 本地移动轴
        /// </summary>
        [Name("移动轴")]
        public Vector3Data _moveAxis = new Vector3Data();

        /// <summary>
        /// 世界变换轴
        /// </summary>
        public Vector3 worldMoveAxis => _moveAxis.data;

        /// <summary>
        /// 最小点
        /// </summary>
        public Vector3 minPoint => orgin.position + worldMoveAxis * minValue;

        /// <summary>
        /// 最大点
        /// </summary>
        public Vector3 maxPoint => orgin.position + worldMoveAxis * maxValue;

        /// <summary>
        /// 当前刻度值
        /// </summary>
        public override int currentStep
        {
            get => base.currentStep;
            set
            {
                base.currentStep = value;

                // 刷新位置
                grabbable.position = minPoint + worldMoveAxis * currentStep * stepSize;
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public override float minValue => _useJoint ? -_configurableJointLimit : _range.x;

        /// <summary>
        /// 最大值
        /// </summary>
        public override float maxValue => _useJoint ? _configurableJointLimit : _range.y;

        /// <summary>
        /// 当前值
        /// </summary>
        public override float currentValue => Mathf.Clamp(Vector3.Dot(grabbable.position - orgin.position, worldMoveAxis), minValue, maxValue);

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _moveAxis.SetTransform(grabbable.targetTransform);
            _moveAxis._dataType = EVector3DataType.TransformForward;
        }

        protected void OnValidate()
        {
            _initOffset = Mathf.Clamp(_initOffset, minValue, maxValue);

            if (_range.y < _range.x) _range.y = _range.x;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (orgin)
            {
                orgin.position += _initOffset * worldMoveAxis;

                if (_useJoint)
                {
                    CreateConfigurableJoint();
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (configurableJoint)
            {
                Destroy(configurableJoint);
            }
        }

        #endregion

        #region 平移

        /// <summary>
        /// 执行移动
        /// </summary>
        /// <param name="grabInteractData"></param>
        protected override void DoMotion(GrabInteractData grabInteractData)
        {
            // 位移量在移动轴上的投影
            float projected = Vector3.Dot(grabInteractData.position.Value - grabbable.position, worldMoveAxis);

            // 通过投影值求出目标点，并设置可抓对象位置
            grabbable.position = Vector3.MoveTowards(grabbable.position, (projected > 0 ? maxPoint : minPoint), Mathf.Abs(projected));
        }

        /// <summary>
        /// 通过当前位置求解刻度
        /// </summary>
        protected override void Snap() => currentStep = Mathf.RoundToInt((grabbable.position - minPoint).magnitude / stepSize);

        #endregion

        #region 可配置关节

        private ConfigurableJoint configurableJoint;

        private void CreateConfigurableJoint()
        {
            var connectedBody = orgin.XAddComponent<Rigidbody>();
            connectedBody.isKinematic = true;

            // 在移动对象上创建可配置关节
            configurableJoint = grabbable.targetTransform.XGetOrAddComponent<ConfigurableJoint>();
            if (configurableJoint)
            {
                // 设置连接刚体对象
                configurableJoint.autoConfigureConnectedAnchor = false;
                configurableJoint.anchor = Vector3.zero;
                configurableJoint.connectedAnchor = Vector3.zero;
                configurableJoint.connectedBody = connectedBody;

                // 设置移动限定范围
                var linearLimit = configurableJoint.linearLimit;
                linearLimit.limit = rangeSize / 2;
                configurableJoint.linearLimit = linearLimit;

                // 禁止旋转
                configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
                configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

                // 默认使用Z轴作为活动范围
                configurableJoint.xMotion = ConfigurableJointMotion.Locked;
                configurableJoint.yMotion = ConfigurableJointMotion.Locked;
                configurableJoint.zMotion = ConfigurableJointMotion.Locked;

                switch (_moveAxis._dataType)
                {
                    case EVector3DataType.TransformUp:
                    case EVector3DataType.TransformDown:
                        {
                            configurableJoint.yMotion = ConfigurableJointMotion.Limited;
                            break;
                        }
                    case EVector3DataType.TransformForward:
                    case EVector3DataType.TransformBack:
                        {
                            configurableJoint.zMotion = ConfigurableJointMotion.Limited;
                            break;
                        }
                    case EVector3DataType.TransformLeft:
                    case EVector3DataType.TransformRight:
                        {
                            configurableJoint.xMotion = ConfigurableJointMotion.Limited;
                            break;
                        }
                }
            }
        }

        private void SetConfigurableJointXYZDrive(Vector3 xyzDriveSpring)
        {
            var tmpDrive = configurableJoint.xDrive;
            tmpDrive.positionSpring = xyzDriveSpring.x;
            configurableJoint.xDrive = tmpDrive;

            tmpDrive = configurableJoint.yDrive;
            tmpDrive.positionSpring = xyzDriveSpring.y;
            configurableJoint.yDrive = tmpDrive;

            tmpDrive = configurableJoint.zDrive;
            tmpDrive.positionSpring = xyzDriveSpring.z;
            configurableJoint.zDrive = tmpDrive;
        } 

        #endregion
    }
}
