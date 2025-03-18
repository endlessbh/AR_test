using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 防侧倾杆
    /// 一般左右轮为一组，但如果车辆（大轮赛车）重心过高，也可以采用前后轮为一组
    /// </summary>
    [Name("防侧倾杆")]
    [Tip("一般左右轮为一组，但如果车辆（大轮赛车）重心过高，也可以采用前后轮为一组", "Generally, the left and right wheels are a group, but if the center of gravity of the vehicle (big wheel racing) is too high, the front and rear wheels can also be used as a group")]
    [XCSJ.Attributes.Icon(EIcon.VehicleAxis)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class AntiRollBar : VehicleDriverGetter
	{
        /// <summary>
        /// 左轮
        /// </summary>
        [Name("左轮")]
		[ValidityCheck(EValidityCheckType.NotNull)]
		[Readonly(EEditorMode.Runtime)]
        public WheelDriver _leftWheel;

        /// <summary>
        /// 右轮
        /// </summary>
        [Name("右轮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _rightWheel;

        /// <summary>
        /// 防侧倾水平力:防止翻车,加强稳定性
        /// </summary>
        [Name("防侧倾水平力")]
        [Tip("防止翻车,加强稳定性", "Prevent rollover and enhance stability")]
        public float _force = 500f;

		private bool valid = false;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            valid = _leftWheel && _rightWheel;
        }

        /// <summary>
        /// 防侧倾杆
        /// </summary>
        protected override void OnFixedUpdate()
	    {
            if (!valid) return;

	        var antiRollForce = (_leftWheel.GetTravelValue() - _rightWheel.GetTravelValue()) * _force;
	
	        AddForceToRigid(_leftWheel, -antiRollForce);
	        AddForceToRigid(_rightWheel, antiRollForce);
	    }
	
	    private void AddForceToRigid(WheelDriver wheelCollider, float strength)
	    {
	        if (wheelCollider.isGrounded)
	        {
	            vehicleDriver.rigid.AddForceAtPosition(wheelCollider.transform.up * strength, wheelCollider.transform.position);
	        }
	    }
	}
}