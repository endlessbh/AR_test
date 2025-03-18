using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// Base Character Controller.
    /// 
    /// A general purpose character controller and base of other controllers.
    /// It handles keyboard input, and allows for a variable height jump, however this default behaviour
    /// can easily be modified or completely replaced overriding this related methods in a derived class.
    /// </summary>
    [Name("基础角色控制器")]
    [RequireComponent(typeof(CharacterMovement))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    public class BaseCharacterController : BaseCharacterMB, IInputEventSender
    {
        #region 编辑器公开字段

        [Group("运动")]
        [Name("速度")]
        [Tip("最大移动速度(单位：米/秒).", "Maximum movement speed in meters per second")]
        [SerializeField]
        private float _speed = 5.0f;

        [Name("角速度")]
        [Tip("最大转弯速度(单位：角度/秒).", "Maximum turning speed in angle / sec")]
        [SerializeField]
        private float _angularSpeed = 540.0f;

        [Name("加速度")]
        [Tip("速度的变化率", "Rate of change of speed")]
        [SerializeField]
        private float _acceleration = 50.0f;

        [Name("减速度")]
        [Tip("角色减速时速度的变化率", "The rate of change in speed when a character decelerates")]
        [SerializeField]
        private float _deceleration = 20.0f;

        [Name("地面摩擦")]
        [Tip("影响移动控制的设置；数值越高，方向变化越快；如果不使用制动摩擦，这也会影响制动时更快停下的能力。", "Settings affecting movement control; The higher the value, the faster the direction change; If brake friction is not used, it will also affect the ability to stop faster when braking.")]
        [SerializeField]
        private float _groundFriction = 8f;

        [Name("使用制动摩擦")]
        [Tip("标识是否使用制动摩擦来减缓角色的速度；如不勾选，将使用地面摩擦。", "Identify whether braking friction is used to slow down the character; If unchecked, ground friction will be used.")]
        [SerializeField]
        private bool _useBrakingFriction;

        [Name("制动摩擦")]
        [Tip("制动时的摩擦系数(没有输入加速度时)；仅在使用制动摩擦为True时使用，否则将使用地面摩擦", "Friction coefficient during braking (without input acceleration); Use only if brake friction is true, otherwise ground friction will be used")]
        [SerializeField]
        private float _brakingFriction = 8f;

        [Name("空气摩擦")]
        [Tip("角色不在地面时使用的摩擦系数", "Coefficient of friction used when the character is not on the ground")]
        [SerializeField]
        private float _airFriction = 0;

        [Name("空气控制")]
        [Tip("当角色不在地面时，可以使用的横向移动控制量；0无控制,1完全控制。", "The amount of lateral movement control that can be used when the character is not on the ground; 0 no control, 1 full control.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float _airControl = 0.2f;

        [Group("跳跃")]
        [Name("基础跳跃高度")]
        [Tip("初始跳跃高度(单位：米)", "Initial jump height (unit: m)")]
        [SerializeField]
        private float _baseJumpHeight = 1.5f;

        [Name("额外跳跃时间")]
        [Tip("额外的跳跃时间(单位：秒)；例如，一直按住跳跃按钮；", "Additional jump time in seconds; For example, keep holding down the jump button;")]
        [SerializeField]
        private float _extraJumpTime = 0.5f;

        [Name("额外跳跃力度")]
        [Tip("按住跳跃按钮时的加速度(单位：米/秒^2);根据经验，将其配置为角色的重力。", "Acceleration when the jump button is pressed and held (unit: M / S ^ 2); Based on experience, configure it as the character's gravity.")]
        [SerializeField]
        private float _extraJumpPower = 25.0f;

        [Name("跳跃容忍时间")]
        [Tip("在接触到地面之前多久，可以再次按下跳跃，并仍然执行跳跃；建议值为0.05f到0.5f之间；本值也可理解为跳跃的冷却时间。", "How long before touching the ground, you can press the jump again and still perform the jump; The recommended value is between 0.05f and 0.5f; This value can also be understood as the cooling time of the jump.")]
        [SerializeField]
        private float _jumpToleranceTime = 0.15f;

        [Name("最大空中跳跃")]
        [Tip("最大空中跳跃的值；0禁用半空跳跃；1允许二段跳，2允许三段跳以此类推。", "The value of the maximum air jump; 0 disable half air jumping; 1 allow two-stage jump, 2 allow three-stage jump, and so on.")]
        [SerializeField]
        private float _maxMidAirJumps = 1;

        [Group("动画")]
        [Name("使用根运动")]
        [Tip("标识是否使用根运动；如果为true，则动画速度将覆盖移动速度，同时需要将根运动控制器（RootMotionController）附加到Animator所在的游戏对象上。", "Identify whether to use root motion; If true, the animation speed will override the movement speed, and the root motion controller needs to be attached to the game object where the animator is located.")]
        [SerializeField]
        private bool _useRootMotion;

        [Group("输入控制")]
        [Name("启用输入")]
        [Tip("标识对角色输入控制是否启用", "Identifies whether the role input control is enabled")]
        public bool _enableInput = true;

        [Name("前后移动输入")]
        [Tip("对应角色的Z轴变化控制", "Z-axis change control of corresponding character")]
        [Input]
        public string forwardBackInput = "Vertical";

        [Name("左右移动输入")]
        [Tip("对应角色的X轴变化控制", "X-axis change control of corresponding character")]
        [Input]
        public string leftRightInput = "Horizontal";

        [Name("跳跃输入")]
        [Tip("对应角色的Y轴变化控制", "Y-axis change control of corresponding character")]
        [Input]
        public string jumpInput = "Jump";

        #endregion

        #region 私有字段

        private Vector3 _moveDirection;

        protected bool _canJump = true;
        protected bool _jump;
        protected bool _isJumping;

        protected bool _updateJumpTimer;
        protected float _jumpTimer;
        protected float _jumpButtonHeldDownTimer;

        protected int _midAirJumpCount;

        private bool _allowVerticalMovement;

        #endregion

        #region 属性

        /// <summary>
        /// Cached CharacterMovement component.
        /// </summary>
        public CharacterMovement movement { get; private set; }

        /// <summary>
        /// Cached animator component (if any).
        /// </summary>
        public Animator animator { get; set; }

        /// <summary>
        /// Cached root motion controller component (if any).
        /// </summary>
        public RootMotionController rootMotionController { get; set; }

        /// <summary>
        /// Allow movement along y-axis and disable gravity force.
        /// Eg: Flying, Ladder climb, etc.
        /// </summary>
        public bool allowVerticalMovement
        {
            get { return _allowVerticalMovement; }
            set
            {
                _allowVerticalMovement = value;

                if (movement)
                    movement.useGravity = !_allowVerticalMovement;
            }
        }

        /// <summary>
        /// Maximum movement speed (in m/s).
        /// </summary>
        public float speed
        {
            get { return _speed; }
            set { _speed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Maximum turning speed (in deg/s).
        /// </summary>
        public float angularSpeed
        {
            get { return _angularSpeed; }
            set { _angularSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The rate of change of velocity.
        /// </summary>
        public float acceleration
        {
            get { return movement.isGrounded ? _acceleration : _acceleration * airControl; }
            set { _acceleration = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The rate at which the character's slows down.
        /// </summary>
        public float deceleration
        {
            get { return movement.isGrounded ? _deceleration : _deceleration * airControl; }
            set { _deceleration = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Setting that affects movement control. Higher values allow faster changes in direction.
        /// If useBrakingFriction is false, this also affects the ability to stop more quickly when braking.
        /// </summary>
        public float groundFriction
        {
            get { return _groundFriction; }
            set { _groundFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Should brakingFriction be used to slow the character?
        /// If false, groundFriction will be used.
        /// </summary>
        public bool useBrakingFriction
        {
            get { return _useBrakingFriction; }
            set { _useBrakingFriction = value; }
        }

        /// <summary>
        /// Friction applied when braking (eg: when there is no input acceleration).  
        /// Only used if useBrakingFriction is true, otherwise groundFriction is used.
        /// 
        /// Braking is composed of friction (velocity dependent drag) and constant deceleration.
        /// </summary>
        public float brakingFriction
        {
            get { return _brakingFriction; }
            set { _brakingFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Friction applied when not grounded (eg: falling, flying, etc).
        /// If useBrakingFriction is false, this also affects the ability to stop more quickly when braking while in air. 
        /// </summary>
        public float airFriction
        {
            get { return _airFriction; }
            set { _airFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// When not grounded, the amount of lateral movement control available to the character.
        /// 0 is no control, 1 is full control.
        /// </summary>
        public float airControl
        {
            get { return _airControl; }
            set { _airControl = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// The initial jump height (in meters).
        /// </summary>
        public float baseJumpHeight
        {
            get { return _baseJumpHeight; }
            set { _baseJumpHeight = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Computed jump impulse.
        /// </summary>
        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * baseJumpHeight * movement.gravity); }
        }

        /// <summary>
        /// The extra jump time (e.g. holding jump button) in seconds.
        /// </summary>
        public float extraJumpTime
        {
            get { return _extraJumpTime; }
            set { _extraJumpTime = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Acceleration while jump button is held down, given in meters / sec^2.
        /// </summary>
        public float extraJumpPower
        {
            get { return _extraJumpPower; }
            set { _extraJumpPower = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// How early before hitting the ground you can press jump, and still perform the jump.
        /// Typical values goes from 0.05f to 0.5f, the higher the value, the easier to "chain" jumps and vice-versa.
        /// </summary>
        public float jumpToleranceTime
        {
            get { return _jumpToleranceTime; }
            set { _jumpToleranceTime = Mathf.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// Maximum mid-air jumps.
        /// </summary>
        public float maxMidAirJumps
        {
            get { return _maxMidAirJumps; }
            set { _maxMidAirJumps = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Should move with root motion?
        /// </summary>
        public bool useRootMotion
        {
            get { return _useRootMotion; }
            set { _useRootMotion = value; }
        }

        /// <summary>
        /// Should root motion be applied?
        /// </summary>
        public bool applyRootMotion
        {
            get { return animator != null && animator.applyRootMotion; }
            set
            {
                if (animator != null)
                    animator.applyRootMotion = value;
            }
        }

        /// <summary>
        /// Jump input command.
        /// </summary>
        public bool jump
        {
            get { return _jump; }
            set
            {
                // If jump is released, allow to jump again

                if (_jump && value == false)
                {
                    _canJump = true;
                    _jumpButtonHeldDownTimer = 0.0f;
                }

                // Update jump value; if pressed, update held down timer

                _jump = value;
                if (_jump)
                    _jumpButtonHeldDownTimer += Time.deltaTime;
            }
        }

        /// <summary>
        /// Is the character jumping? (moving up result of jump button press).
        /// </summary>
        public bool isJumping
        {
            get
            {
                // We are in jump mode but just falling

                if (_isJumping && movement.velocity.y < 0.0001f)
                    _isJumping = false;

                return _isJumping;
            }
        }

        /// <summary>
        /// True if character is falling, false if not.
        /// </summary>
        public bool isFalling
        {
            get { return !movement.isGrounded && movement.velocity.y < 0.0001f; }
        }

        /// <summary>
        /// Is the character standing on 'ground'?
        /// </summary>
        public bool isGrounded
        {
            get { return movement.isGrounded; }
        }

        /// <summary>
        /// Movement input command. The desired move direction.
        /// </summary>
        public Vector3 moveDirection
        {
            get { return _moveDirection; }
            set { _moveDirection = Vector3.ClampMagnitude(value, 1.0f); }
        }

        #endregion

        #region 方法

        /// <summary>
        /// Rotate the character towards a given direction vector.
        /// </summary>
        /// <param name="direction">The target direction</param>
        /// <param name="onlyLateral">Should it be restricted to XZ only?</param>
        public void RotateTowards(Vector3 direction, bool onlyLateral = true)
        {
            movement.Rotate(direction, angularSpeed, onlyLateral);
        }

        /// <summary>
        /// Rotate the character towards move direction vector (input).
        /// </summary>
        /// <param name="onlyLateral">Should it be restricted to XZ only?</param>
        public void RotateTowardsMoveDirection(bool onlyLateral = true)
        {
            RotateTowards(moveDirection, onlyLateral);
        }

        /// <summary>
        /// Rotate the character towards its velocity vector.
        /// </summary>
        /// <param name="onlyLateral">Should it be restricted to XZ only?</param>
        public void RotateTowardsVelocity(bool onlyLateral = true)
        {
            RotateTowards(movement.velocity, onlyLateral);
        }

        /// <summary>
        /// Perform jump logic.
        /// </summary>
        protected void Jump()
        {
            // If jump button not pressed, or still not released, return

            if (!_jump || !_canJump)
                return;

            // If not grounded, return

            if (!movement.isGrounded)
                return;

            // Is jump button pressed within jump tolerance?

            if (_jumpButtonHeldDownTimer > _jumpToleranceTime)
                return;

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag
            _updateJumpTimer = true;    // Allow mid-air jump to be variable height

            // Apply jump impulse

            movement.ApplyVerticalImpulse(jumpImpulse);

            // 'Pause' grounding, allowing character to safely leave the 'ground'

            movement.DisableGrounding();
        }

        /// <summary>
        /// Mid-air jump logic.
        /// </summary>
        protected void MidAirJump()
        {
            // Reset mid-air jumps counter

            if (_midAirJumpCount > 0 && movement.isGrounded)
                _midAirJumpCount = 0;

            // If jump button not pressed, or still not released, return

            if (!_jump || !_canJump)
                return;

            // If grounded, return

            if (movement.isGrounded)
                return;

            // Have mid-air jumps?

            if (_midAirJumpCount >= _maxMidAirJumps)
                return;

            _midAirJumpCount++;         // Increase mid-air jumps counter

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag
            _updateJumpTimer = true;    // Allow mid-air jump to be variable height

            // Apply jump impulse

            movement.ApplyVerticalImpulse(jumpImpulse);
        }

        /// <summary>
        /// Perform variable jump height logic.
        /// </summary>
        protected void UpdateJumpTimer()
        {
            if (!_updateJumpTimer)
                return;

            // If jump button is held down and extra jump time is not exceeded...

            if (_jump && _jumpTimer < _extraJumpTime)
            {
                // Calculate how far through the extra jump time we are (jumpProgress),

                var jumpProgress = _jumpTimer / _extraJumpTime;

                // Apply proportional extra jump power (acceleration) to simulate variable height jump,
                // this method offers better control and less 'floaty' feel.

                var proportionalJumpPower = Mathf.Lerp(_extraJumpPower, 0f, jumpProgress);
                movement.ApplyForce(Vector3.up * proportionalJumpPower, ForceMode.Acceleration);

                // Update jump timer

                _jumpTimer = Mathf.Min(_jumpTimer + Time.deltaTime, _extraJumpTime);
            }
            else
            {
                // Button released or extra jump time ends, reset info

                _jumpTimer = 0.0f;
                _updateJumpTimer = false;
            }
        }

        /// <summary>
        /// 计算所需的移动速度。Calculate the desired movement velocity.
        /// 例如：将输入（移动方向）转换为运动速度矢量，Eg: Convert the input (moveDirection) to movement velocity vector,
        /// 使用NavMesh代理所需的速度等。    use navmesh agent desired velocity, etc.
        /// </summary>
        protected virtual Vector3 CalcDesiredVelocity()
        {
            // If using root motion and root motion is being applied (eg: grounded),
            // use animation velocity as animation takes full control

            if (useRootMotion && applyRootMotion)
                return rootMotionController.animVelocity;

            // else, convert input (moveDirection) to velocity vector

            return moveDirection * speed;
        }

        /// <summary>
        /// Perform character movement logic.
        /// 
        /// NOTE: Must be called in FixedUpdate.
        /// </summary>
        protected void Move()
        {
            // Apply movement

            // If using root motion and root motion is being applied (eg: grounded),
            // move without accel / decel, let the animation takes full control

            var desiredVelocity = CalcDesiredVelocity();

            if (useRootMotion && applyRootMotion)
                movement.Move(desiredVelocity, speed, !allowVerticalMovement);
            else
            {
                // Move with acceleration and friction

                var currentFriction = isGrounded ? groundFriction : airFriction;
                var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

                movement.Move(desiredVelocity, speed, acceleration, deceleration, currentFriction,
                    currentBrakingFriction, !allowVerticalMovement);
            }

            // Jump logic

            Jump();
            MidAirJump();
            UpdateJumpTimer();

            // Update root motion state,
            // should animator root motion be enabled? (eg: is grounded)

            applyRootMotion = useRootMotion && movement.isGrounded;
        }

        /// <summary>
        /// Perform character animation.
        /// </summary>
        protected virtual void Animate() { }

        /// <summary>
        /// Update character's rotation.
        /// By default ECM will rotate the character towards its movement direction,
        /// however this default behavior can easily be modified overriding this method in a derived class.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            // Rotate towards movement direction (input)

            RotateTowardsMoveDirection();
        }

        /// <summary>
        /// Handles input.
        /// </summary>
        protected virtual void HandleInput()
        {
            if (!enableInput) return;

            moveDirection = new Vector3
            {
                x = XInput.GetAxisRaw(leftRightInput),
                y = 0.0f,
                z = XInput.GetAxisRaw(forwardBackInput)
            };

            jump = XInput.GetButton(jumpInput);
        }

        #endregion

        #region MB方法

        /// <summary>
        /// Validate editor exposed fields.
        /// 
        /// NOTE: If you override this, it is important to call the parent class' version of method
        /// eg: base.OnValidate, in the derived class method implementation, in order to fully validate the class.  
        /// </summary>
        public virtual void OnValidate()
        {
            speed = _speed;
            angularSpeed = _angularSpeed;

            acceleration = _acceleration;
            deceleration = _deceleration;

            groundFriction = _groundFriction;
            brakingFriction = _brakingFriction;

            airFriction = _airFriction;
            airControl = _airControl;

            baseJumpHeight = _baseJumpHeight;
            extraJumpTime = _extraJumpTime;
            extraJumpPower = _extraJumpPower;
            jumpToleranceTime = _jumpToleranceTime;
            maxMidAirJumps = _maxMidAirJumps;
        }

        /// <summary>
        /// Initialize this component.
        /// 
        /// NOTE: If you override this, it is important to call the parent class' version of method,
        /// (eg: base.Awake) in the derived class method implementation, in order to fully initialize the class. 
        /// </summary>
        public virtual void Awake()
        {
            // Cache components
            movement = GetComponent<CharacterMovement>();

            animator = GetComponentInChildren<Animator>();

            rootMotionController = GetComponentInChildren<RootMotionController>();

            this.Find(handlers);
        }

        public virtual void FixedUpdate()
        {
            // Perform character movement
            Move();
        }

        public virtual void Update()
        {
            // Handle input
            HandleInput();

            // Update character rotation
            UpdateRotation();

            // Perform character animation
            Animate();
        }

        #endregion

        #region IInputEventSender

        public bool enableSend { get; set; } = true;

        public bool enableInput { get => _enableInput; set => _enableInput = value; }

        public List<IEventHandler> handlers { get; } = new List<IEventHandler>();

        #endregion
    }
}