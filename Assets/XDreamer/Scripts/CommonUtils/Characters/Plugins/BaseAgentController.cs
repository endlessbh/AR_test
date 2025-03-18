using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// Base Agent (NavMesh) Controller.
    /// 
    /// Base class for a 'NavMeshAgent' controlled characters.
    /// It inherits from 'BaseCharacterController' and extends it to control a 'NavMeshAgent'
    /// and intelligently move in response to mouse click (click to move).
    /// 
    /// As the base character controller, this default behaviour can easily be modified completely replaced in a derived class.
    /// </summary>
    [Name("基础代理控制器")]
    [RequireComponent(typeof(NavMeshAgent))]
    public class BaseAgentController : BaseCharacterController
    {
        #region EDITOR EXPOSED FIELDS

        [Group("导航")]
        [Name("自动制动")]
        [Tip("标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。", "Identify whether the agent should brake automatically to avoid exceeding the destination; If true, the agent automatically brakes as it approaches its destination.")]
        [SerializeField]
        private bool _autoBraking = true;

        [Name("制动距离")]
        [Tip("距离目标位置多远时，开始制动（即减速区域的长度值）", "Start braking when how far away from the target position (i.e. the length value of deceleration area)")]
        [SerializeField]
        private float _brakingDistance = 2.0f;

        [Name("停止距离")]
        [Tip("在距离目标位置多远时，即可以停止", "It can stop when it is far from the target position")]
        [SerializeField]
        private float _stoppingDistance = 1.0f;

        [Name("地面层")]
        [Tip("标识哪些层被认为是地面(即可行走区域)；在由地面点击检测使用。", "Identify which layers are considered to be the ground (i.e. walking area); Used by ground click detection.")]
        [SerializeField]
        public LayerMask groundMask = 1;            // Default layer

        [Name("目标位置输入")]
        [Tip("点击触发移动到目标位置", "Click trigger to move to the target position")]
        [Input]
        public string destinationInput = "Fire2";

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached NavMeshAgent component.
        /// </summary>

        public NavMeshAgent agent { get; private set; }

        /// <summary>
        /// Should the agent brake automatically to avoid overshooting the destination point?
        /// If this property is set to true, the agent will brake automatically as it nears the destination.
        /// </summary>

        public bool autoBraking
        {
            get { return _autoBraking; }
            set
            {
                _autoBraking = value;

                if (agent != null)
                    agent.autoBraking = _autoBraking;
            }
        }

        /// <summary>
        /// Distance from target position to start braking.
        /// </summary>

        public float brakingDistance
        {
            get { return _brakingDistance; }
            set { _brakingDistance = Mathf.Max(0.0001f, value); }
        }

        /// <summary>
        /// The ratio (0 - 1 range) of the agent's remaining distance and the braking distance.
        /// 1 If no auto braking or if agent's remaining distance is greater than brakingDistance.
        /// less than 1, if agent's remaining distance is less than brakingDistance.
        /// </summary>

        public float brakingRatio
        {
            get
            {
                if (!autoBraking || agent == null)
                    return 1f;

                return agent.hasPath ? Mathf.Clamp(agent.remainingDistance / brakingDistance, 0.1f, 1f) : 1f;
            }
        }

        /// <summary>
        /// Stop within this distance from the target position.
        /// </summary>

        public float stoppingDistance
        {
            get { return _stoppingDistance; }
            set
            {
                _stoppingDistance = Mathf.Max(0.0f, value);

                if (agent != null)
                    agent.stoppingDistance = _stoppingDistance;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Synchronize the NavMesh Agent simulation position with the character movement position,
        /// we control the agent.
        /// 
        /// NOTE: Must be called in LateUpdate method.
        /// </summary>

        protected void SyncAgent()
        {
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;

            agent.acceleration = acceleration;
            agent.velocity = movement.velocity;

            agent.nextPosition = transform.position;
        }

        /// <summary>
        /// Assign the character's desired move direction (input) based on agent's info.
        /// </summary>

        protected virtual void SetMoveDirection()
        {
            // If agent is not moving, return

            moveDirection = Vector3.zero;

            if (!agent.hasPath)
                return;

            // If destination not reached,
            // feed agent's desired velocity (lateral only) as the character move direction

            if (agent.remainingDistance > stoppingDistance)
                moveDirection = agent.desiredVelocity.OnlyXZ();
            else
            {
                // If destination is reached,
                // reset stop agent and clear its path
                
                agent.ResetPath();
            }
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' CalcDesiredVelocity method,
        /// adding auto braking support.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            SetMoveDirection();

            var desiredVelocity = base.CalcDesiredVelocity();
            return autoBraking ? desiredVelocity * brakingRatio : desiredVelocity;
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput method,
        /// to perform custom input code, in this case, click-to-move.
        /// </summary>

        protected override void HandleInput()
        {
            if (!enableInput) return;

            // Handle mouse input

            if (!XInput.GetButton(destinationInput))
                return;

            // If mouse right click,
            // found click position in the world

            var ray = Camera.main.ScreenPointToRay(XInput.mousePosition);

            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask.value))
                return;

            // Set agent destination to ground hit point

            agent.SetDestination(hitInfo.point);
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Validate this editor exposed fields.
        /// </summary>

        public override void OnValidate()
        {
            // Calls the parent class' version of method.

            base.OnValidate();

            // This class validation

            autoBraking = _autoBraking;

            brakingDistance = _brakingDistance;
            stoppingDistance = _stoppingDistance;
        }

        /// <summary>
        /// Initialize this.
        /// </summary>

        public override void Awake()
        {
            // Calls the parent class' version of method.

            base.Awake();

            // Cache and initialize components

            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.autoBraking = autoBraking;
                agent.stoppingDistance = stoppingDistance;

                // Turn-off NavMeshAgent control,
                // we control it, not the other way

                agent.updatePosition = false;
                agent.updateRotation = false;

                agent.updateUpAxis = false;
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        "NavMeshAgentCharacterController: There is no 'NavMeshAgent' attached to the '{0}' game object.\n" +
                        "Please add a 'NavMeshAgent' to the '{0}' game object.",
                        name));
            }
        }

        public virtual void LateUpdate()
        {
            // Synchronize agent with character movement

            SyncAgent();
        }

        #endregion
    }
}
