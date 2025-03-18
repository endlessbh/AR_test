using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 曲柄摇杆：曲柄摇杆机构是指具有一个曲柄和一个摇杆的铰链四杆机构。通常，曲柄为主动件且等速转动，而摇杆为从动件作变速往返摆动，连杆作平面复合运动
    /// </summary>
    [Name("曲柄摇杆")]
    [Tip("曲柄摇杆机构是指具有一个曲柄和一个摇杆的铰链四杆机构。通常，曲柄为主动件且等速转动，而摇杆为从动件作变速往返摆动，连杆作平面复合运动", "Crank rocker mechanism refers to a hinged four-bar mechanism with a crank and a rocker. Usually, the crank is the driving part and rotates at the same speed, while the rocker is the driven part to change the speed and swing back and forth, and the connecting rod makes a plane compound motion")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    public class CrankRocker : Mechanism
    {
        [Name("运动平面法线")]
        [Tip("曲柄摇杆运动所在平面的法线", "Normal of the plane where the crank rocker moves")]
        public Vector3Data _motionNormals = new Vector3Data();

        /// <summary>
        /// 曲柄:曲柄为主动件且等速转动
        /// </summary>
        [Group("关节配置")]
        [Name("曲柄")]
        [Tip("曲柄为主动件且等速转动", "The crank is the driving part and rotates at the same speed")]
        public RotationMechanism _crank;

        /// <summary>
        /// 连杆:连杆连接曲柄和摇杆，作平面复合运动
        /// </summary>
        [Name("连杆")]
        [Tip("连杆连接曲柄和摇杆，作平面复合运动", "The connecting rod connects the crank and rocker for plane compound motion")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _connectingRod;

        /// <summary>
        /// 连杆与摇杆关节
        /// </summary>
        [Name("连杆与摇杆关节")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _linkAndRockerJoint;

        /// <summary>
        /// 摇杆:摇杆为从动件作变速往返摆动
        /// </summary>
        [Name("摇杆")]
        [Tip("摇杆为从动件作变速往返摆动", "Rocker is the driven part to change the speed and swing back and forth")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _rocker;

        /// <summary>
        /// 摇杆半径
        /// </summary>
        protected float rockerRadius = 0;

        /// <summary>
        /// 连杆半径
        /// </summary>
        protected float linkRadius = 0;

        public bool valid => _connectingRod && _linkAndRockerJoint && _rocker;

        private Rocker linkComponent = null;
        private Rocker rockerComponent = null;

        /// <summary>
        /// 重置：设定运动轴引用为自身
        /// </summary>
        public void Reset()
        {
            _motionNormals._transform._transfrom = transform;
        }

        /// <summary>
        /// 唤醒：初始化数据
        /// </summary>
        public void Awake()
        {
            InitData();
        }

        private void InitData()
        {
            if (!valid) return;

            linkRadius = (_connectingRod.position - _linkAndRockerJoint.position).magnitude;
            rockerRadius = (_rocker.position - _linkAndRockerJoint.position).magnitude;

            linkComponent = _connectingRod.XGetOrAddComponent<Rocker>();
            linkComponent._lookatJoint = _linkAndRockerJoint;

            rockerComponent = _rocker.XGetOrAddComponent<Rocker>();
            rockerComponent._lookatJoint = _linkAndRockerJoint;
        }

        /// <summary>
        /// 能否运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion() => valid;

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion()
        {
            // 获取交点
            if (MathU.Intersection(_connectingRod.position, linkRadius, _rocker.position, rockerRadius, _motionNormals.data, out Vector3 p1, out Vector3 p2))
            {
                _linkAndRockerJoint.position = (p1 - _linkAndRockerJoint.position).sqrMagnitude < (p2 - _linkAndRockerJoint.position).sqrMagnitude ? p1 : p2;
            }

            // 驱动link
            linkComponent.DoMotion();

            // 驱动Rocker
            rockerComponent.DoMotion();
        }
    }
}

