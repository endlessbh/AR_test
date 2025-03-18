using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// Base First Person Controller.
    /// 
    /// Base class for a first person controller.
    /// It inherits from 'BaseCharacterController' and extends it to perform classic FPS movement.
    /// 
    /// As the base character controllers, this default behaviour can easily be modified or completely replaced in a derived class. 
    /// </summary>
    [Name("基础第一人称控制器")]
    public class BaseFirstPersonController : BaseCharacterController
    {
        #region 编辑器公开字段

        [Group("第一人称")]
        [Name("前进速度")]
        [Tip("向前移动时的速度", "Speed when moving forward")]
        [SerializeField]
        private float _forwardSpeed = 5.0f;

        [Name("后退速度")]
        [Tip("向后移动时的速度", "Speed when moving backward")]
        [SerializeField]
        private float _backwardSpeed = 3.0f;

        [Name("横向速度")]
        [Tip("横向移动时的速度", "Speed when moving laterally")]
        [SerializeField]
        private float _strafeSpeed = 4.0f;

        [Name("跑步速度倍数")]
        [Tip("跑步时的速度是行走时速度的倍数", "Running speed is a multiple of walking speed")]
        [SerializeField]
        private float _runSpeedMultiplier = 2.0f;

        [Name("跑步输入")]
        [Tip("对应输入按钮按下保持时,将使用跑步速度倍数", "When you press the run button, enter the corresponding speed multiple")]
        [Input]
        public string runInput = "Fire3";

        #endregion

        #region 属性

        /// <summary>
        /// Cached camera transform.
        /// </summary>
        public Transform cameraTransform { get; private set; }

        /// <summary>
        /// Cached MouseLook component.
        /// </summary>
        public ObsoleteMouseLook mouseLook { get; private set; }

        /// <summary>
        /// Speed when moving forward.
        /// </summary>
        public float forwardSpeed
        {
            get { return _forwardSpeed; }
            set { _forwardSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed when moving backwards.
        /// </summary>
        public float backwardSpeed
        {
            get { return _backwardSpeed; }
            set { _backwardSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed when moving sideways.
        /// </summary>
        public float strafeSpeed
        {
            get { return _strafeSpeed; }
            set { _strafeSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Speed multiplier while running.
        /// </summary>
        public float runSpeedMultiplier
        {
            get { return _runSpeedMultiplier; }
            set { _runSpeedMultiplier = Mathf.Max(value, 1.0f); }
        }

        /// <summary>
        /// Run input command.
        /// </summary>
        public bool run { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// Perform 'Look' rotation.
        /// This rotate the character along its y-axis (yaw) and a child camera along its local x-axis (pitch).
        /// </summary>
        protected virtual void RotateView()
        {
            mouseLook.LookRotation(movement, cameraTransform);
        }

        /// <summary>
        /// Override the default ECM UpdateRotation to perform typical fps rotation.
        /// </summary>
        protected override void UpdateRotation()
        {
            RotateView();
        }

        /// <summary>
        /// Get target speed, relative to input moveDirection,
        /// eg: forward, backward or strafe.
        /// </summary>
        protected virtual float GetTargetSpeed()
        {
            // Defaults to forward speed

            var targetSpeed = forwardSpeed;

            // Strafe

            if (moveDirection.x > 0.0f || moveDirection.x < 0.0f)
                targetSpeed = strafeSpeed;

            // Backwards

            if (moveDirection.z < 0.0f)
                targetSpeed = backwardSpeed;

            // Forward handled last as if strafing and moving forward at the same time,
            // forward speed should take precedence

            if (moveDirection.z > 0.0f)
                targetSpeed = forwardSpeed;

            // Handle run speed modifier

            return run ? targetSpeed * runSpeedMultiplier : targetSpeed;
        }

        /// <summary>
        /// Overrides CalcDesiredVelocity to generate a velocity vector relative to view direction
        /// eg: forward, backward or strafe.
        /// </summary>
        protected override Vector3 CalcDesiredVelocity()
        {
            // Set character's target speed (eg: moving forward, backward or strafe)

            speed = GetTargetSpeed();

            // Return desired velocity relative to view direction and target speed

            return transform.TransformDirection(base.CalcDesiredVelocity());
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput method,
        /// to perform custom input code. 
        /// </summary>
        protected override void HandleInput()
        {
            if (!enableInput) return;

            moveDirection = new Vector3
            {
                x = XInput.GetAxisRaw(leftRightInput),
                y = 0.0f,
                z = XInput.GetAxisRaw(forwardBackInput)
            };

            run = XInput.GetButton(runInput);

            jump = XInput.GetButton(jumpInput);
        }

        #endregion

        #region MB方法

        /// <summary>
        /// Validate this editor exposed fields.
        /// </summary>
        public override void OnValidate()
        {
            // Call the parent class' version of method

            base.OnValidate();

            // Validate this editor exposed fields

            forwardSpeed = _forwardSpeed;
            backwardSpeed = _backwardSpeed;
            strafeSpeed = _strafeSpeed;

            runSpeedMultiplier = _runSpeedMultiplier;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>
        public override void Awake()
        {
            // Call the parent class' version of method

            base.Awake();

            // Cache and initialize this components

            mouseLook = GetComponent<ObsoleteMouseLook>();
            if (mouseLook == null)
            {
                Debug.LogError(
                    string.Format(
                        "BaseFPSController: No 'MouseLook' found. Please add a 'MouseLook' component to '{0}' game object",
                        name));
            }

            var cam = GetComponentInChildren<Camera>();
            if (cam == null)
            {
                Debug.LogError(
                    string.Format(
                        "BaseFPSController: No 'Camera' found. Please parent a camera to '{0}' game object.", name));
            }
            else
            {
                cameraTransform = cam.transform;
                mouseLook.Init(transform, cameraTransform);
            }
        }

        #endregion
    }
}
