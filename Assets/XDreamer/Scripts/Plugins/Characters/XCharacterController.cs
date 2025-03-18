using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Characters
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    [Name("角色控制器")]
    [RequireComponent(typeof(CharacterMovement))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    public class XCharacterController : BaseMainController
    {
        #region 运动

        /// <summary>
        /// 角色运动组件：基于当前游戏对象在<see cref="Awake"/>时调用<see cref="Component.GetComponent{T}"/>获取对应类型组件
        /// </summary>
        public CharacterMovement movement { get; private set; }

        /// <summary>
        /// 角色是否下落中
        /// </summary>
        public bool isFalling => !movement.isGrounded && movement.velocity.y < 0.0001f;

        /// <summary>
        /// 角色是否站在地面上
        /// </summary>
        public bool isGrounded => movement.isGrounded;

        /// <summary>
        /// 运动方向
        /// </summary>
        [Group("运动", defaultIsExpanded = false)]
        [Name("运动方向")]
        [Readonly]
        public Vector3 _moveDirection;

        /// <summary>
        /// 运动输入命令。所需的运动方向。
        /// </summary>
        public Vector3 moveDirection
        {
            get { return _moveDirection; }
            set { _moveDirection = Vector3.ClampMagnitude(value, 1.0f); }
        }

        /// <summary>
        /// 速度:实时运动速度(单位：米/秒).根据不同的角色控制输入，本值会被动态修改；
        /// </summary>
        [Name("速度")]
        [Tip("实时运动速度(单位：米/秒).根据不同的角色控制输入，本值会被动态修改；", "Real time movement speed in meters per second This value will be dynamically modified according to different role control inputs;")]
        public float _speed = 5.0f;

        /// <summary>
        /// 速度
        /// </summary>
        public float speed
        {
            get { return _speed; }
            set { _speed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 最大速度:实时运动速度可达到的最大值(单位：米/秒).
        /// </summary>
        [Name("最大速度")]
        [Tip("实时运动速度可达到的最大值(单位：米/秒).", "The maximum value of real-time movement speed (unit: M / s)")]
        public float _maxSpeed = 5;

        /// <summary>
        /// 最大速度:实时运动速度可达到的最大值(单位：米/秒).
        /// </summary>
        public float maxSpeed
        {
            get { return _maxSpeed; }
            set { _maxSpeed = Mathf.Max(speed, value); }
        }

        /// <summary>
        /// 角速度:最大转弯速度(单位：角度/秒).
        /// </summary>
        [Name("角速度")]
        [Tip("最大转弯速度(单位：角度/秒).", "Maximum turning speed in angle / sec")]
        public float _angularSpeed = 540.0f;

        /// <summary>
        /// 角速度
        /// </summary>
        public float angularSpeed
        {
            get { return _angularSpeed; }
            set { _angularSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 加速度:速度的变化率
        /// </summary>
        [Name("加速度")]
        [Tip("速度的变化率", "Rate of change of speed")]
        public float _acceleration = 50.0f;

        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration
        {
            get { return movement.isGrounded ? _acceleration : _acceleration * airControl; }
            set { _acceleration = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 减速度:角色减速时速度的变化率
        /// </summary>
        [Name("减速度")]
        [Tip("角色减速时速度的变化率", "The rate of change in speed when a character decelerates")]
        public float _deceleration = 20.0f;

        /// <summary>
        /// 减速度
        /// </summary>
        public float deceleration
        {
            get { return movement.isGrounded ? _deceleration : _deceleration * airControl; }
            set { _deceleration = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 允许纵向运动:允许沿y轴移动并禁用重力
        /// </summary>
        [Name("允许纵向运动")]
        [Tip("允许沿y轴移动并禁用重力", "Allows movement along the Y axis and disables gravity")]
        public bool _allowVerticalMovement;

        /// <summary>
        /// 允许沿y轴移动并禁用重力。
        /// 例如: 飞行、爬梯子等.
        /// </summary>
        public bool allowVerticalMovement
        {
            get { return _allowVerticalMovement; }
            set
            {
                _allowVerticalMovement = value;

                if (movement)
                {
                    movement.useGravity = !_allowVerticalMovement;
                }
            }
        }

        #endregion

        #region 摩擦

        /// <summary>
        /// 地面摩擦:影响移动控制的设置；数值越高，方向变化越快；如果不使用制动摩擦，这也会影响制动时更快停下的能力。
        /// </summary>
        [Name("地面摩擦")]
        [Tip("影响移动控制的设置；数值越高，方向变化越快；如果不使用制动摩擦，这也会影响制动时更快停下的能力。", "Settings affecting movement control; The higher the value, the faster the direction change; If brake friction is not used, it will also affect the ability to stop faster when braking.")]
        public float _groundFriction = 8f;

        /// <summary>
        /// 地面摩擦:影响移动控制的设置；数值越高，方向变化越快；如果不使用制动摩擦，这也会影响制动时更快停下的能力。
        /// </summary>
        public float groundFriction
        {
            get { return _groundFriction; }
            set { _groundFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 使用制动摩擦:标识是否使用制动摩擦来减缓角色的速度；如不勾选，将使用地面摩擦。
        /// </summary>
        [Name("使用制动摩擦")]
        [Tip("标识是否使用制动摩擦来减缓角色的速度；如不勾选，将使用地面摩擦。", "Identify whether braking friction is used to slow down the character; If unchecked, ground friction will be used.")]
        public bool _useBrakingFriction;

        /// <summary>
        /// 使用制动摩擦:标识是否使用制动摩擦来减缓角色的速度；如不勾选，将使用地面摩擦。
        /// </summary>
        public bool useBrakingFriction
        {
            get { return _useBrakingFriction; }
            set { _useBrakingFriction = value; }
        }

        /// <summary>
        /// 制动摩擦:制动时的摩擦系数(没有输入加速度时)；仅在使用制动摩擦为True时使用，否则将使用地面摩擦
        /// </summary>
        [Name("制动摩擦")]
        [Tip("制动时的摩擦系数(没有输入加速度时)；仅在使用制动摩擦为True时使用，否则将使用地面摩擦", "Friction coefficient during braking (without input acceleration); Use only if brake friction is true, otherwise ground friction will be used")]
        public float _brakingFriction = 8f;

        /// <summary>
        /// 制动摩擦:制动时的摩擦系数(没有输入加速度时)；仅在使用制动摩擦为True时使用，否则将使用地面摩擦
        /// </summary>
        public float brakingFriction
        {
            get { return _brakingFriction; }
            set { _brakingFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 空气摩擦:角色不在地面时使用的摩擦系数
        /// </summary>
        [Name("空气摩擦")]
        [Tip("角色不在地面时使用的摩擦系数", "Coefficient of friction used when the character is not on the ground")]
        public float _airFriction = 0;

        /// <summary>
        /// 空气摩擦:角色不在地面时使用的摩擦系数
        /// </summary>
        public float airFriction
        {
            get { return _airFriction; }
            set { _airFriction = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 空气控制:当角色不在地面时，可以使用的横向移动控制量；0无控制,1完全控制。
        /// </summary>
        [Name("空气控制")]
        [Tip("当角色不在地面时，可以使用的横向移动控制量；0无控制,1完全控制。", "The amount of lateral movement control that can be used when the character is not on the ground; 0 no control, 1 full control.")]
        [Range(0.0f, 1.0f)]
        public float _airControl = 0.2f;

        /// <summary>
        /// 空气控制:当角色不在地面时，可以使用的横向移动控制量；0无控制,1完全控制。
        /// </summary>
        public float airControl
        {
            get { return _airControl; }
            set { _airControl = Mathf.Clamp01(value); }
        }

        #endregion

        #region 跳跃

        /// <summary>
        /// 基础跳跃高度:初始跳跃高度(单位：米)
        /// </summary>
        [Group("跳跃", defaultIsExpanded = false)]
        [Name("基础跳跃高度")]
        [Tip("初始跳跃高度(单位：米)", "Initial jump height (unit: m)")]
        public float _baseJumpHeight = 1.5f;

        /// <summary>
        /// 基础跳跃高度
        /// </summary>
        public float baseJumpHeight
        {
            get { return _baseJumpHeight; }
            set { _baseJumpHeight = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 计算的跳跃冲量
        /// </summary>
        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * baseJumpHeight * movement.gravity); }
        }

        /// <summary>
        /// 额外跳跃时间:额外的跳跃时间(单位：秒)；例如，一直按住跳跃按钮；
        /// </summary>
        [Name("额外跳跃时间")]
        [Tip("额外的跳跃时间(单位：秒)；例如，一直按住跳跃按钮；", "Additional jump time in seconds; For example, keep holding down the jump button;")]
        public float _extraJumpTime = 0.5f;

        /// <summary>
        /// 额外跳跃时间:额外的跳跃时间(单位：秒)；例如，一直按住跳跃按钮；
        /// </summary>
        public float extraJumpTime
        {
            get { return _extraJumpTime; }
            set { _extraJumpTime = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 额外跳跃力度:按住跳跃按钮时的加速度(单位：米/秒^2);根据经验，将其配置为角色的重力。
        /// </summary>
        [Name("额外跳跃力度")]
        [Tip("按住跳跃按钮时的加速度(单位：米/秒^2);根据经验，将其配置为角色的重力。", "Acceleration when the jump button is pressed and held (unit: M / S ^ 2); Based on experience, configure it as the character's gravity.")]
        public float _extraJumpPower = 25.0f;

        /// <summary>
        /// 额外跳跃力度:按住跳跃按钮时的加速度(单位：米/秒^2);根据经验，将其配置为角色的重力。
        /// </summary>
        public float extraJumpPower
        {
            get { return _extraJumpPower; }
            set { _extraJumpPower = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 跳跃容忍时间:在接触到地面之前多久，可以再次按下跳跃，并仍然执行跳跃；建议值为0.05f到0.5f之间；本值也可理解为跳跃的冷却时间。
        /// </summary>
        [Name("跳跃容忍时间")]
        [Tip("在接触到地面之前多久，可以再次按下跳跃，并仍然执行跳跃；建议值为0.05f到0.5f之间；本值也可理解为跳跃的冷却时间。", "Acceleration when the jump button is pressed and held (unit: M / S ^ 2); Based on experience, configure it as the character's gravity.")]
        public float _jumpToleranceTime = 0.15f;

        /// <summary>
        /// 跳跃容忍时间:在接触到地面之前多久，可以再次按下跳跃，并仍然执行跳跃；建议值为0.05f到0.5f之间；本值也可理解为跳跃的冷却时间。
        /// </summary>
        public float jumpToleranceTime
        {
            get { return _jumpToleranceTime; }
            set { _jumpToleranceTime = Mathf.Clamp(value, 0.0f, 1.0f); }
        }

        /// <summary>
        /// 最大空中跳跃:最大空中跳跃的值；0禁用半空跳跃；1允许二段跳，2允许三段跳以此类推。
        /// </summary>
        [Name("最大空中跳跃")]
        [Tip("最大空中跳跃的值；0禁用半空跳跃；1允许二段跳，2允许三段跳以此类推。", "The value of the maximum air jump; 0 disable half air jumping; 1 allow two-stage jump, 2 allow three-stage jump, and so on.")]
        public float _maxMidAirJumps = 1;

        /// <summary>
        /// 最大空中跳跃:最大空中跳跃的值；0禁用半空跳跃；1允许二段跳，2允许三段跳以此类推。
        /// </summary>
        public float maxMidAirJumps
        {
            get { return _maxMidAirJumps; }
            set { _maxMidAirJumps = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 能否跳跃
        /// </summary>
        protected bool _canJump = true;

        /// <summary>
        /// 跳跃
        /// </summary>
        [Name("跳跃")]
        [Readonly]
        public bool _jump;

        /// <summary>
        /// 跳跃的输入命令
        /// </summary>
        public bool jump
        {
            get { return _jump; }
            set
            {
                // 如果解除跳跃，允许再次跳跃
                if (_jump && value == false)
                {
                    _canJump = true;
                    _jumpButtonHeldDownTimer = 0.0f;
                }

                // 更新跳跃值；如果跳跃，更新保持跳跃的计数器
                _jump = value;
                if (_jump)
                {
                    _jumpButtonHeldDownTimer += Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// 跳跃中
        /// </summary>
        [Name("跳跃中")]
        [Readonly]
        public bool _isJumping;

        /// <summary>
        /// 角色是否正在跳跃中：跳跃输入之后运动上移中；
        /// </summary>
        public bool isJumping
        {
            get
            {
                // 我们在跳跃模式，但只是下降
                if (_isJumping && movement.velocity.y < 0.0001f)
                {
                    _isJumping = false;
                }

                return _isJumping;
            }
        }

        /// <summary>
        /// 更新跳跃计数器
        /// </summary>
        protected bool _updateJumpTimer;

        /// <summary>
        /// 跳跃计数器
        /// </summary>
        protected float _jumpTimer;

        /// <summary>
        /// 跳跃按键保持按下计数器
        /// </summary>
        protected float _jumpButtonHeldDownTimer;

        /// <summary>
        /// 二段跳计数
        /// </summary>
        protected int _midAirJumpCount;

        #endregion

        #region 动画

        /// <summary>
        /// 动画器：基于当前游戏对象在<see cref="Awake"/>时调用<see cref="Component.GetComponentInChildren{T}"/>获取对应类型组件
        /// </summary>
        public Animator animator { get; set; }

        /// <summary>
        /// 根运动控制器：基于当前游戏对象在<see cref="Awake"/>时调用<see cref="Component.GetComponentInChildren{T}"/>获取对应类型组件
        /// </summary>
        public RootMotionController rootMotionController { get; set; }

        /// <summary>
        /// 使用根运动:标识是否使用根运动；如果为true，则动画速度将覆盖移动速度，同时需要将根运动控制器（RootMotionController）附加到Animator所在的游戏对象上。
        /// </summary>
        [Group("动画", defaultIsExpanded = false)]
        [Name("使用根运动")]
        [Tip("标识是否使用根运动；如果为true，则动画速度将覆盖移动速度，同时需要将根运动控制器（RootMotionController）附加到Animator所在的游戏对象上。", "Identify whether to use root motion; If true, the animation speed will override the movement speed, and the root motion controller needs to be attached to the game object where the animator is located.")]
        public bool _useRootMotion;

        /// <summary>
        /// 使用根运动:标识是否使用根运动；如果为true，则动画速度将覆盖移动速度，同时需要将根运动控制器（RootMotionController）附加到Animator所在的游戏对象上。
        /// </summary>
        public bool useRootMotion
        {
            get { return _useRootMotion; }
            set { _useRootMotion = value; }
        }

        /// <summary>
        /// 是否可应用根动画
        /// </summary>
        public bool applyRootMotion
        {
            get { return animator != null && animator.applyRootMotion; }
            set
            {
                if (animator != null)
                {
                    animator.applyRootMotion = value;
                }
            }
        }

        #endregion

        #region 变换

        private Transform _transform;

        /// <summary>
        /// 角色变换
        /// </summary>
        public Transform characterTransform
        {
            get
            {
                if (!_transform) _transform = transform;
                return _transform;
            }
        }

        #endregion

        #region 角色相机控制器

        /// <summary>
        /// 角色相机控制器
        /// </summary>
        [Group("相机", defaultIsExpanded = false)]
        [Name("角色相机控制器")]
        public BaseCharacterCameraController _characterCameraController;

        /// <summary>
        /// 角色相机控制器
        /// </summary>
        public BaseCharacterCameraController characterCameraController => this.XGetComponentInChildren(ref _characterCameraController);

        #endregion

        #region 角色变换器

        /// <summary>
        /// 角色变换器
        /// </summary>
        [Group("变换（移动旋转）", defaultIsExpanded = false, textEN = "Transformer (Move Rotate)")]
        [Name("角色变换器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCharacterTransformer _characterTransformer;

        /// <summary>
        /// 角色变换器
        /// </summary>
        public BaseCharacterTransformer characterTransformer
        {
            get
            {
                if (_characterTransformer) return _characterTransformer;

                this.XGetComponentInChildren(ref _characterTransformer);
                if (_characterTransformer) return _characterTransformer;

                var transform = this.transform;
                var dstName = CommonFun.Name(typeof(CharacterTransformer));
                var dst = transform.Find(dstName);
                if (dst)
                {
                    _characterTransformer = dst.XGetOrAddComponent<CharacterTransformer>();
                    return _characterTransformer;
                }

                _characterTransformer = transform.XCreateChild<CharacterTransformer>();
                return _characterTransformer;
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public void Move(Vector3 value, int moveMode)
        {
            try
            {
                characterTransformer.Move(value, moveMode);
            }
            catch { }
        }

        /// <summary>
        /// 计算所需的移动速度。
        /// 例如：将输入（移动方向）转换为运动速度矢量， 如使用导航网格代理所需的速度等。    
        /// </summary>
        protected virtual Vector3 CalcDesiredVelocity()
        {
            try
            {
                return characterTransformer.CalcDesiredVelocity();
            }
            catch
            {
                return DefaultDesiredVelocity();
            }
        }

        /// <summary>
        /// 缺省的所需移动速度
        /// </summary>
        /// <returns></returns>
        public Vector3 DefaultDesiredVelocity()
        {
            // 如果正在应用根运动和根运动（例如：接地）
            // 使用动画速度作为动画完全控制
            if (useRootMotion && applyRootMotion)
            {
                return rootMotionController.animVelocity;
            }

            // 将输入（移动方向）转换为速度矢量
            return moveDirection * speed;
        }

        /// <summary>
        /// 执行角色运动逻辑：必须在<see cref="FixedUpdate"/>中调用；
        /// </summary>
        protected void Move()
        {
            // 如果正在应用根运动和根运动（例如：接地），
            // 非加速/非减速的移动，让动画完全控制
            var desiredVelocity = characterTransformer.CalcDesiredVelocity();

            if (useRootMotion && applyRootMotion)
            {
                movement.Move(desiredVelocity, speed, !allowVerticalMovement);
            }
            else
            {
                // 以加速度和摩擦力移动
                var currentFriction = isGrounded ? groundFriction : airFriction;
                var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

                movement.Move(desiredVelocity, speed, acceleration, deceleration, currentFriction,
                    currentBrakingFriction, !allowVerticalMovement);
            }

            // 跳跃逻辑
            Jump();
            MidAirJump();
            UpdateJumpTimer();

            // 更新根运动状态，
            // 是否应启用运动器根运动？（例如：接地）
            applyRootMotion = useRootMotion && movement.isGrounded;
        }

        #endregion

        #region 旋转方法

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public void Rotate(Vector3 value, int rotateMode)
        {
            try
            {
                characterTransformer.Rotate(value, rotateMode);
            }
            catch { }
        }

        /// <summary>
        /// 将角色直接旋转到给定的方向向量：此时不再考虑角速度限定；
        /// </summary>
        /// <param name="direction">目标方向</param>
        /// <param name="onlyLateral">是否只限与XZ面</param>
        public void RotateTowardsDirect(Vector3 direction, bool onlyLateral = true)
        {
            movement.RotateTo(direction, onlyLateral);
        }

        /// <summary>
        /// 将角色旋转到给定的方向向量
        /// </summary>
        /// <param name="direction">目标方向</param>
        /// <param name="onlyLateral">是否只限与XZ面</param>
        public void RotateTowards(Vector3 direction, bool onlyLateral = true)
        {
            movement.Rotate(direction, angularSpeed, onlyLateral);
        }

        /// <summary>
        /// 向移动方向向量（输入）旋转角色.即与<see cref="moveDirection"/>方向一致；
        /// </summary>
        /// <param name="onlyLateral">是否只限与XZ面</param>
        public void RotateTowardsMoveDirection(bool onlyLateral = true)
        {
            RotateTowards(moveDirection, onlyLateral);
        }

        /// <summary>
        /// 将角色朝其速度向量旋转.即与<see cref="CharacterMovement.velocity"/>方向一致；
        /// </summary>
        /// <param name="onlyLateral">是否只限与XZ面</param>
        public void RotateTowardsVelocity(bool onlyLateral = true)
        {
            RotateTowards(movement.velocity, onlyLateral);
        }

        #endregion

        #region 跳跃方法

        /// <summary>
        /// 执行跳转逻辑
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
        /// 空中跳跃逻辑
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
        /// 执行可变跳跃高度逻辑
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

        #endregion

        #region MB方法

        /// <summary>
        /// 验证编辑器公开的字段
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
        /// 唤醒：初始化
        /// </summary>
        public virtual void Awake()
        {
            movement = GetComponent<CharacterMovement>();
            animator = GetComponentInChildren<Animator>();
            rootMotionController = GetComponentInChildren<RootMotionController>();
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        public virtual void FixedUpdate()
        {
            // 执行角色运动逻辑
            Move();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!characterTransformer) { }
            if (!characterCameraController) { }
        }

        #endregion
    }
}
