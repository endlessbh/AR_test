
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Tools.Grounds;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
	/// <summary>
	/// 车轮驱动器：控制车轮驱动、转向、刹车等
	/// </summary>
	[Name("车轮驱动器")]
	[Tip("控制车轮的驱动、转向、刹车等", "Control wheel drive, steering, braking, etc")]
    [XCSJ.Attributes.Icon(EIcon.Wheel)]
    [Tool(VehicleDriveHelper.CategoryComponentName)]
    [RequireComponent(typeof(WheelCollider))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    public class WheelDriver : VehicleDriverGetter
	{
		/// <summary>
		/// 车轮碰撞体
		/// </summary>
	    public WheelCollider wheelCollider
	    {
	        get
	        {
	            if (!_wheelCollider) _wheelCollider = GetComponent<WheelCollider>();
	            return _wheelCollider;
	        }
        }
        private WheelCollider _wheelCollider;

		/// <summary>
		/// 车轮碰撞点
		/// </summary>
        public WheelHit wheelHit { get; private set; }              
	
		/// <summary>
		/// 车辆是否接触地面
		/// </summary>
	    public bool isGrounded { get; private set; } = false;    
	
	    /// <summary>
	    /// 车轮模型
	    /// </summary>
	    [Name("车轮模型")]
		[ValidityCheck(EValidityCheckType.NotNull)]
		public Transform _wheelModel;        

		/// <summary>
		/// 驱动
		/// </summary>
		[Name("驱动")] 
		[Readonly(EEditorMode.Runtime)]
		public bool _canPower = false;

		[Name("驱动系数")]
		[Range(-1f, 1f)] 
		[HideInSuperInspector(nameof(_canPower), EValidityCheckType.False)]
		public float _powerMultiplier = 1;

		/// <summary>
		/// 转向
		/// </summary>
		[Name("转向")]
        [Readonly(EEditorMode.Runtime)]
        public bool _canSteer = false;

		[Name("转向系数")] 
		[Range(-1f, 1f)]
        [HideInSuperInspector(nameof(_canSteer), EValidityCheckType.False)]
        public float _steerMultiplier = 1;

		/// <summary>
		/// 刹车
		/// </summary>
		[Name("刹车")]
        [Readonly(EEditorMode.Runtime)]
        public bool canBrake = false;

		[Name("刹车系数")] 
		[Range(0f, 1f)]
        [HideInSuperInspector(nameof(canBrake), EValidityCheckType.False)]
        public float _brakeMultiplier = 1;

		/// <summary>
		/// 手刹
		/// </summary>
		[Name("手刹")]
        [Readonly(EEditorMode.Runtime)]
        public bool _canHandbrake = false;

		/// <summary>
		/// 手刹系数
		/// </summary>
		[Name("手刹系数")] 
		[Range(0f, 1f)]
        [HideInSuperInspector(nameof(_canHandbrake), EValidityCheckType.False)]
        public float _handbrakeMultiplier = 1;

		/// <summary>
		/// 牵引漂移
		/// </summary>
		[Name("牵引漂移")] 
		public float _tractionHelpedSidewaysStiffness = 1f;

		/// <summary>
		/// 总滑动摩擦力
		/// </summary>
		public float totalSlip { get; private set; } = 0f; 
	
	    //	车轮滑动曲线
	    private WheelFrictionCurve forwardFrictionCurve;        
	    private WheelFrictionCurve sidewaysFrictionCurve;  
	
		/// <summary>
		/// 当前地面摩擦力
		/// </summary>
	    public GroundMaterialFrictions currentFrictions { get; private set; }
	
		/// <summary>
		/// 当前地面索引
		/// </summary>
	    public int groundIndex { get; private set; }

		/// <summary>
		/// 地面材质配置
		/// </summary>
		public GroundMaterialConfig groundMaterialConfig => _groundMaterialConfig;
		private GroundMaterialConfig _groundMaterialConfig;

		/// <summary>
		/// 启动
		/// </summary>
		protected void Start()
	    {
			// 关联车轮模型
			if (!_wheelModel)
	        {
	            Debug.LogError(transform.name + "丢失车轮模型！");
	            return;
	        }

			_groundMaterialConfig = FindObjectOfType<GroundMaterialConfig>();

            forwardFrictionCurve = wheelCollider.forwardFriction;
            sidewaysFrictionCurve = wheelCollider.sidewaysFriction;
        }

		/// <summary>
		/// 更新
		/// </summary>
	    protected void Update()
	    {
	        if (!_wheelModel) return;

			// 更新车轮模型位置与角度
			wheelCollider.GetWorldPose(out Vector3 wheelPosition, out Quaternion wheelRotation);

			_wheelModel.position = wheelPosition;
			_wheelModel.rotation = wheelRotation;
	    }
	
		/// <summary>
		/// 更新
		/// </summary>
	    protected override void OnFixedUpdate()
	    {
	        if (!vehicleDriver || !vehicleDriver.enabled || vehicleDriver.brake==null) return;
	
	        isGrounded = wheelCollider.GetGroundHit(out WheelHit _wheelHit);
			wheelHit = _wheelHit;
            if (wheelHit.point == Vector3.zero)
            {
				groundIndex = 0;
			}
            else if (groundMaterialConfig)
            {
                groundIndex = groundMaterialConfig.GetGroundMaterialIndex(wheelHit, transform.position);
                currentFrictions = groundMaterialConfig.GetCurrentGroundMaterialFrictions(groundIndex);
            }

			// 应用地面摩擦力
            if (currentFrictions!=null)
            {
                // 设置车辆碰撞体的刚度为地面物理材质的刚度
                forwardFrictionCurve.stiffness = currentFrictions.forwardStiffness;

                // 手刹输入
                var hbInput = (_canPower && vehicleDriver.brake.handbrakeOn) ? 0.75f : 1f;
                sidewaysFrictionCurve.stiffness = currentFrictions.sidewaysStiffness * hbInput * _tractionHelpedSidewaysStiffness;

                wheelCollider.forwardFriction = forwardFrictionCurve;
                wheelCollider.sidewaysFriction = sidewaysFrictionCurve;

                // 应用地面阻尼
                wheelCollider.wheelDampingRate = currentFrictions.damp;
            }
	
	        // 计算总滑动
	        var minSlip = isGrounded ? (Mathf.Abs(wheelHit.forwardSlip) + Mathf.Abs(wheelHit.sidewaysSlip)) / 2 : 0f;
	        totalSlip = Mathf.Lerp(totalSlip, minSlip, Time.fixedDeltaTime * 5f);
	    }
	
	    /// <summary>
	    /// 绘制辅助线
	    /// </summary>
	    void OnDrawGizmos()
	    {
	#if UNITY_EDITOR
	        if (Application.isPlaying)
	        {
	            wheelCollider.GetGroundHit(out WheelHit hit);
	
	            // Drawing gizmos for wheel forces and slips.
	            //float extension = (-wheelCollider.transform.InverseTransformPoint(hit.point).y - (wheelCollider.radius * transform.lossyScale.y)) / wheelCollider.suspensionDistance;
	            //Debug.DrawLine(hit.point, hit.point + transform.up * (hit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
	            Debug.DrawLine(hit.point, hit.point - transform.forward * hit.forwardSlip * 2f, Color.green);
	            Debug.DrawLine(hit.point, hit.point - transform.right * hit.sidewaysSlip * 2f, Color.red);
	        }
	#endif
	    }
	
	    /// <summary>
	    /// 驱动力
	    /// </summary>
	    /// <param name="torque">Torque.</param>
	    public void SetMotorTorque(float torque)
	    {
			if (!_canPower) return;

	        wheelCollider.motorTorque = torque * _powerMultiplier;
        }
	
	    /// <summary>
	    /// 转向力
	    /// </summary>
	    /// <param name="steerInput">Steer input.</param>
	    /// <param name="angle">Angle.</param>
	    public void SetSteer(float steerInput)
	    {
	        if (!_canSteer) return;
	
	        steerInput *= _steerMultiplier;

            var tmpAngle = 2.55f * steerInput;
            var a1 = tmpAngle * Mathf.Atan(2.55f / (6 + 0.75f));
	        var a2 = tmpAngle * Mathf.Atan(2.55f / (6 - 0.75f));
	
	        var xSmallThenZero = transform.localPosition.x < 0;
	        wheelCollider.steerAngle = steerInput > 0f ? (xSmallThenZero ? a1 : a2) : (xSmallThenZero ? a2 : a1);
	    }
	
	    /// <summary>
	    /// 刹车
	    /// </summary>
	    /// <param name="torque">Torque.</param>
	    public void SetBrakeTorque(float torque)
	    {
            if (canBrake || _canHandbrake)
            {
                wheelCollider.brakeTorque = torque;
            }
	    }
	
		/// <summary>
		/// 获取横向值
		/// </summary>
		/// <returns></returns>
	    public float GetTravelValue()
	    {
	        return isGrounded ? -(transform.InverseTransformPoint(wheelHit.point).y + wheelCollider.radius) / wheelCollider.suspensionDistance : 1f;
	    }
	
	    /// <summary>
	    /// 前后轮
	    /// </summary>
	    /// <param name="angle"></param>
	    /// <param name="tractionAngle"></param>
	    public void SetSidewaysStiffness(float angle, float tractionAngle)
	    {
	        if (_canSteer)
	        {
	            _tractionHelpedSidewaysStiffness = angle * wheelCollider.steerAngle < 0 ? tractionAngle : 1f;
	        }
	    }

		/// <summary>
		/// 对齐模型
		/// </summary>
		public void AlignWithModel()
        {
            if (_wheelModel)
            {
                transform.XSetPosition(_wheelModel.position);
                transform.XSetRotation(_wheelModel.rotation);
            }
        }
	}
}