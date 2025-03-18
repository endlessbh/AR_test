using UnityEngine;
using UnityEngine.AI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 角色移动在代理中通过输入
    /// </summary>
    [Name("角色移动在代理中通过输入")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Move, /*nameof(XCharacterController),*/ nameof(CharacterTransformer))]
    public class CharacterMoveInAgentByInput : BaseCharacterToolController
    {
        /// <summary>
        /// 导航网格代理
        /// </summary>
        [Group("导航网格代理")]
        [Name("导航网格代理")]
        public NavMeshAgent _agent;

        /// <summary>
        /// 导航网格代理
        /// </summary>
        public NavMeshAgent agent
        {
            get
            {
                if (!_agent)
                {
                    if (mainController)
                    {
                        _agent = mainController.GetComponentInParent<NavMeshAgent>();
                    }
                    else
                    {

                        _agent = GetComponentInParent<NavMeshAgent>();
                    }
                }
                return _agent;
            }
        }

        /// <summary>
        /// 自动制动:标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。
        /// </summary>
        [Name("自动制动")]
        [Tip("标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。", "Identify whether the agent should brake automatically to avoid exceeding the destination; If true, the agent automatically brakes as it approaches its destination.")]
        public bool _autoBraking = true;

        /// <summary>
        /// 自动制动:标识代理是否应该自动刹车以避免超出目的地点；如果为True，那么此代理会在靠近目的地时自动刹车。
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
        /// 制动距离:距离目标位置多远时，开始制动（即减速区域的长度值）
        /// </summary>
        [Name("制动距离")]
        [Tip("距离目标位置多远时，开始制动（即减速区域的长度值）", "Start braking when how far away from the target position (i.e. the length value of deceleration area)")]
        public float _brakingDistance = 2.0f;

        /// <summary>
        /// 制动距离:距离目标位置多远时，开始制动（即减速区域的长度值）
        /// </summary>
        public float brakingDistance
        {
            get { return _brakingDistance; }
            set { _brakingDistance = Mathf.Max(0.0001f, value); }
        }

        /// <summary>
        /// 代理的剩余距离与制动距离的比率（0-1范围）。
        /// 如果没有自动制动或代理的剩余距离大于制动距离，则等于1。
        /// 如果代理的剩余距离小于制动距离，则小于1。
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
        /// 停止距离:在距离目标位置多远时，即可以停止
        /// </summary>
        [Name("停止距离")]
        [Tip("在距离目标位置多远时，即可以停止", "It can stop when it is far from the target position")]
        [SerializeField]
        private float _stoppingDistance = 1.0f;

        /// <summary>
        /// 停止距离:在距离目标位置多远时，即可以停止
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

        /// <summary>
        /// 地面层:标识哪些层被认为是地面(即可行走区域)；在由地面点击检测使用。
        /// </summary>
        [Name("地面层")]
        [Tip("标识哪些层被认为是地面(即可行走区域)；在由地面点击检测使用。", "Identify which layers are considered to be the ground (i.e. walking area); Used by ground click detection.")]
        public LayerMask _groundMask = 1;

        /// <summary>
        /// 移动模式
        /// </summary>
        [Group("移动", defaultIsExpanded = false)]
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.World;

        /// <summary>
        /// 目标位置输入:点击触发移动到目标位置
        /// </summary>
        [Name("目标位置输入")]
        [Tip("点击触发移动到目标位置", "Click trigger to move to the target position")]
        [Input]
        public InputAxis _destinationInput = new InputAxis();

        #region 输入处理

        /// <summary>
        /// 输入处理
        /// </summary>
        [Name("输入处理")]
        public InputHandler _inputHandler = new InputHandler();

        #endregion

        /// <summary>
        /// 主相机
        /// </summary>
        public Camera mainCamera => mainController.characterCameraController.characterCamera;

        /// <summary>
        /// 同步代理
        /// </summary>
        protected void SyncAgent()
        {
            var character = mainController;
            agent.speed = character.speed;
            agent.angularSpeed = character.angularSpeed;

            agent.acceleration = character.acceleration;
            agent.velocity = character.movement.velocity;

            agent.nextPosition = character.characterTransform.position;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (_inputHandler.GetInput() is IInput input)
            {
                if (_destinationInput.CanInput(input))
                {
                    if (input.GetButton(_destinationInput._inputName))
                    {
                        var ray = mainCamera.ScreenPointToRay(XInput.mousePosition);
                        if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _groundMask.value))
                        {
                            //将代理目标设置为地面命中点
                            agent.SetDestination(hitInfo.point);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public virtual void LateUpdate()
        {
            // 同步代理与角色移动
            SyncAgent();
        }

        private bool TryGetMoveDirection(out Vector3 moveDirection)
        {
            if (agent.hasPath)//代理未移动
            {
                // 如未到目的地,角色移动方向时进给代理的所需速度（仅横向）
                if (agent.remainingDistance > stoppingDistance)
                {
                    moveDirection = agent.desiredVelocity.OnlyXZ();
                    return true;
                }
                else
                {
                    // 到达目的地,重置停止代理并清除其路径
                    agent.ResetPath();
                }
            }

            moveDirection = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        public void FixedUpdate()
        {
            if(TryGetMoveDirection(out Vector3 moveDirection))
            {
                mainController.Move(moveDirection, (int)_moveMode);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _destinationInput._inputName = "Fire2";//鼠标右键
            _destinationInput._mouseButtons.Add(EMouseButton.Always);
        }
    }
}
