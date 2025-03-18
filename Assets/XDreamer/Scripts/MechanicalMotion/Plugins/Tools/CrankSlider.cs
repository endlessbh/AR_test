using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 曲柄滑块:曲柄滑块机构是指用曲柄和滑块来实现转动和移动相互转换的平面连杆机构。曲柄滑块机构中与机架构成移动副的构件为滑块，通过转动副联接曲柄和滑块的构件为连杆。
    /// </summary>
    [Name("曲柄滑块")]
    [Tip("曲柄滑块机构是指用曲柄和滑块来实现转动和移动相互转换的平面连杆机构。曲柄滑块机构中与机架构成移动副的构件为滑块，通过转动副联接曲柄和滑块的构件为连杆。", "Crank slider mechanism is a planar linkage mechanism that uses crank and slider to realize the conversion between rotation and movement. In the crank slider mechanism, the component constituting the moving pair with the frame is the slider, and the component connecting the crank and the slider through the rotating pair is the connecting rod.")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    public class CrankSlider : Mechanism
    {
        /// <summary>
        /// 运动平面法线
        /// </summary>
        [Name("运动平面法线")]
        [Tip("曲柄滑块运动所在平面的法线", "Normal of the plane where the crank slider moves")]
        public Vector3Data _motionNormals = new Vector3Data();

        /// <summary>
        /// 曲柄:曲柄为主动件且等速转动
        /// </summary>
        [Group("关节配置")]        
        [Name("曲柄")]
        [Tip("曲柄为主动件且等速转动", "The crank is the driving part and rotates at the same speed.")]
        public RotationMechanism _crank;

        /// <summary>
        /// 连杆:通过转动副联接曲柄和滑块的构件为连杆
        /// </summary>
        [Name("连杆")]
        [Tip("通过转动副联接曲柄和滑块的构件为连杆", "The component connecting the crank and the slider through the rotating pair is the connecting rod.")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _connectingRod;

        /// <summary>
        /// 连杆朝向上向量
        /// </summary>
        [Name("连杆朝向上向量")]
        public Vector3Data _connectingReferenceAxis = new Vector3Data();

        /// <summary>
        /// 滑块:曲柄滑块机构中与机架构成移动副的构件为滑块
        /// </summary>
        [Name("滑块")]
        [Tip("曲柄滑块机构中与机架构成移动副的构件为滑块", "In the crank slider mechanism, the component constituting the moving pair with the frame is the slider.")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _slider;

        /// <summary>
        /// 有效
        /// </summary>
        public bool valid => _connectingRod && _slider;

        private bool intersection = false;

        private float rockerRadius = 0;

        /// <summary>
        /// 重置：设定运动轴引用为自身
        /// </summary>
        public void Reset()
        {
            _motionNormals._transform._transfrom = transform;
            _connectingReferenceAxis._dataType = EVector3DataType.Vector3;
            _connectingReferenceAxis._vector3._value = Vector3.up;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public void Awake()
        {
            if (valid)
            {
                rockerRadius = (_connectingRod.position - _slider.position).magnitude;
                intersection = MathU.Intersection(_connectingRod.position, rockerRadius, _slider.position, _slider.forward, _motionNormals.data, out Vector3 p1, out Vector3 p2);
            }
        }

        /// <summary>
        /// 能否运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion() => intersection;

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion()
        {
            if (!valid) return;

            intersection = MathU.Intersection(_connectingRod.position, rockerRadius, _slider.position, _slider.forward, _motionNormals.data, out Vector3 p1, out Vector3 p2);
            if (intersection)
            {
                // 采用更靠近上次坐标的点
                _slider.transform.position = (p1 - _slider.transform.position).sqrMagnitude < (p2 - _slider.transform.position).sqrMagnitude ? p1 : p2;
            }

            _connectingRod.LookAt(_slider, _connectingReferenceAxis.data);
        }
    }
}

