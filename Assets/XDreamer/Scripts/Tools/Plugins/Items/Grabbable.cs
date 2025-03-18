using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.DataBase.UserSystem;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Base.Recorders;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.Scripts;
using static UnityEngine.UI.GridLayoutGroup;
using static XCSJ.PluginTools.Items.Grabbable;

namespace XCSJ.PluginTools.Items
{
    #region 抓结果命令

    /// <summary>
    /// 抓结果命令枚举
    /// </summary>
    public enum EGrabResultCmd
    {
        /// <summary>
        /// 抓
        /// </summary>
        [Name("抓")]
        Grab,

        /// <summary>
        /// 放
        /// </summary>
        [Name("放")]
        Release,

        /// <summary>
        /// 保持
        /// </summary>
        [Name("保持")]
        Hold,

        /// <summary>
        /// 变换已改变
        /// </summary>
        [Name("变换已改变")]
        TransformChanged,
    }

    /// <summary>
    /// 抓结果命令
    /// </summary>
    [Serializable]
    public class GrabResultCmd : Cmd<EGrabResultCmd> { }

    /// <summary>
    /// 抓结果命令集
    /// </summary>
    [Serializable]
    public class GrabResultCmds : Cmds<EGrabResultCmd, GrabResultCmd> { }

    #endregion

    /// <summary>
    /// 可抓对象:
    /// 1、可抓对象在三维空间中被抓取器改变位置、角度或形状称为“受力”
    /// 2、抓取器有多个时，采用“合力”方式作用于可抓对象。（例如：两个手搬起石头）
    /// 3、可抓对象可将作用“力”传递给其主体受力对象。（例如：拉门把手，把手将力传递给门，门转动。）
    /// </summary>
    [Name("可抓对象")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool("常用", rootType = typeof(ToolsManager))]
    [Tool(InteractionCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    public class Grabbable : InteractableVirtual, IUsageHost
    {
        #region 用途关键字

        /// <summary>
        /// 抓用途关键字
        /// </summary>
        public const string GrabbedUsageKey = nameof(EGrabResultCmd.Grab);

        #endregion

        #region 交互输入

        /// <summary>
        /// 抓取命令列表
        /// </summary>
        [Name("抓取命令列表")]
        public GrabCmds _grabcmds = new GrabCmds();

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> cmds => _grabcmds.cmdNames;

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData) => cmds;

        #endregion

        #region 交互输出

        /// <summary>
        /// 抓取结果命令列表
        /// </summary>
        [Name("抓取结果命令列表")]
        public GrabResultCmds _grabResultCmds = new GrabResultCmds();

        #endregion

        #region 抓设置

        /// <summary>
        /// 受控变换替换对象
        /// </summary>
        [Group("抓设置", textEN = "Grab Settings", defaultIsExpanded = false)]
        [Name("受控变换替换对象")]
        public Transform _transformOverride = null;

        /// <summary>
        /// 刚体替换对象
        /// </summary>
        [Name("受控刚体替换对象")]
        public Rigidbody _rigidbodyOverride = null;

        /// <summary>
        /// 被抓参考点
        /// </summary>
        [Name("被抓参考点")]
        public Transform _grabPoint;

        /// <summary>
        /// 拖拽器最小与最大数量
        /// </summary>
        [Name("拖拽器最小与最大数量")]
        [Tip("当拖拽器数量未达到最小值时，可抓对象无法受控移动；当拖拽器数量已经达到最大值，则无法接受新的拖拽器对其进行控制；", "When the number of drags does not reach the minimum value, the grabable object cannot be controlled to move; When the number of drags has reached the maximum, it is not acceptable to accept new drags for control;")]
        [LimitRange(1, 100)]
        public Vector2Int _draggerMinMaxCount = new Vector2Int(1, 2);

        /// <summary>
        /// 最小拖拽器数量：当作用在本对象上的拖拽器大于等于本值时，才可拖拽对象
        /// </summary>
        public int minCountOfDragger => _draggerMinMaxCount.x;

        /// <summary>
        /// 最大拖拽器数量：当作用在本对象上的拖拽器等于本值时，不可再接受新拖拽器控制
        /// </summary>
        public int maxCountOfDragger => _draggerMinMaxCount.y;

        /// <summary>
        /// 抓取点
        /// </summary>
        public Transform grabPoint => _grabPoint ? _grabPoint : transform;

        /// <summary>
        /// 抓点与可抓对象位置偏移量
        /// </summary>
        public Vector3 grabPointPositionOffset => _grabPoint ? _grabPoint.position - targetTransform.position : Vector3.zero;

        /// <summary>
        /// 抓点与可抓对象角度偏移量
        /// </summary>
        public Quaternion grabPointRotationOffset => _grabPoint ? _grabPoint.localRotation : Quaternion.identity;

        /// <summary>
        /// 刚体记录器
        /// </summary>
        private RigidbodyRecorder rigidbodyRecorder = new RigidbodyRecorder();

        /// <summary>
        /// 刚体
        /// </summary>
        public Rigidbody ownRigidbody
        {
            get
            {
                if (!_rigidbody)
                {
                    _rigidbody = GetComponentInParent<Rigidbody>();
                }
                return _rigidbody;
            }
        }
        private Rigidbody _rigidbody;

        #endregion

        #region 插槽标签

        /// <summary>
        /// 插槽标签
        /// </summary>
        public string[] socketTags => _tagProperty.GetValues(SingleSocket.SocketTag);

        #endregion

        #region 连接体及变换信息

        /// <summary>
        /// 与其他对象关联
        /// </summary>
        public virtual bool isConnectToOther { get => _isConnectToOther; set => _isConnectToOther = value; }
        private bool _isConnectToOther;

        /// <summary>
        /// 受控目标对象
        /// </summary>
        public virtual Transform targetTransform => _transformOverride ? _transformOverride : transform;

        /// <summary>
        /// 受控目标刚体
        /// </summary>
        public virtual Rigidbody targetRigidbody => _rigidbodyOverride ? _rigidbodyOverride : ownRigidbody;

        /// <summary>
        /// 位置
        /// </summary>
        public virtual Vector3 position
        {
            get => targetTransform.position;
            set
            {
                value += grabPointPositionOffset;
                if (targetRigidbody)
                {
                    // 使用当前方法有插值效果（直接设置targetRigidbody.position无插值效果）
                    targetRigidbody.MovePosition(value);
                }
                else
                {
                    targetTransform.position = value;
                }
            }
        }

        /// <summary>
        /// 本地坐标
        /// </summary>
        public Vector3 localPosition
        {
            get => targetTransform.localPosition;
            set => targetTransform.localPosition = value;
        }

        /// <summary>
        /// 旋转量
        /// </summary>
        public virtual Quaternion rotation
        {
            get => targetTransform.rotation;
            set
            {
                value = value * grabPointRotationOffset;
                if (targetRigidbody)// 有刚体时，设置刚体旋转量
                {
                    // 使用当前方法有插值效果（直接设置targetRigidbody.rotation无插值效果）
                    targetRigidbody.MoveRotation(value);
                }
                else
                {
                    targetTransform.rotation = value;
                }
            }
        }

        /// <summary>
        /// 本地坐标
        /// </summary>
        public Quaternion localRotation
        {
            get => targetTransform.localRotation;
            set => targetTransform.localRotation = value;
        }

        /// <summary>
        /// 设置姿态并调用完成
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="rotation"></param>
        public void SetPoseAndCallFinished(Vector3 postion, Quaternion rotation)
        {
            this.position = postion;
            this.rotation = rotation;

            CallFinished(new InteractData(_grabResultCmds.GetCmdName(EGrabResultCmd.TransformChanged), this));
        }

        /// <summary>
        /// 设置位置并调用完成
        /// </summary>
        /// <param name="postion"></param>
        public void SetPositionAndCallFinished(Vector3 postion)
        {
            this.position = postion;

            CallFinished(new InteractData(_grabResultCmds.GetCmdName(EGrabResultCmd.TransformChanged), this));
        }

        /// <summary>
        /// 设置旋转并调用完成
        /// </summary>
        /// <param name="postion"></param>
        public void SetRotationAndCallFinished(Quaternion rotation)
        {
            this.rotation = rotation;

            CallFinished(new InteractData(_grabResultCmds.GetCmdName(EGrabResultCmd.TransformChanged), this));
        }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _grabcmds.Reset();
            _grabResultCmds.Reset();

            _rigidbodyOverride = ownRigidbody;
            if (_rigidbodyOverride)// 默认初始化刚体属性为连续检测
            {
                _rigidbodyOverride.interpolation = RigidbodyInterpolation.Interpolate;
            }

            _transformOverride = _rigidbodyOverride ? _rigidbodyOverride.transform : transform;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // 对象受铰链控制
            if (!_isConnectToOther && GetComponentInParent<Joint>())
            {
                _isConnectToOther = true;
            }

            AddMeshCollider();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            draggerInteractDatas.Clear();
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // 可更新抓对象变换数据
            if (doMotion)
            {
                doMotion = false;

                OnHold(draggerInteractDatas);

                draggerInteractDatas.Clear();
            }
        }

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collision"></param>
        protected void OnCollisionEnter(Collision collision)
        {
            //Debug.Log(name + "=> :" + collision.collider.name);

            AddCollider(collision.collider);
        }

        /// <summary>
        /// 碰撞退出
        /// </summary>
        /// <param name="collision"></param>
        protected void OnCollisionExit(Collision collision)
        {
            //Debug.Log(name + "<= :" + collision.collider.name);
            RemoveCollider(collision.collider);
        }

        /// <summary>
        /// 触发进入
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.name + "触发进入 :" + name);
            AddCollider(other);
        }

        /// <summary>
        /// 触发退出
        /// </summary>
        /// <param name="other"></param>
        protected void OnTriggerExit(Collider other)
        {
            //Debug.Log(other.name + "触发离开 :" + name);
            RemoveCollider(other);
        }

        #endregion

        #region 拖拽

        private void AddMeshCollider()
        {
            // 自动添加MeshCollider
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                if (!renderer.GetComponent<Collider>())
                {
                    renderer.XAddComponent<MeshCollider>();
                }
            }
        }

        private Dictionary<Dragger, GrabInteractData> draggerInteractDatas = new Dictionary<Dragger, GrabInteractData>();

        private bool doMotion = false;

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData)
        {
            if (base.CanInteractAsInteractable(interactData) && interactData.interactor is Dragger dragger)
            {
                if (_grabcmds.TryGetECmd(interactData.cmdName, out var grabCmd))
                {
                    switch (grabCmd)
                    {
                        case EGrabCmd.Grab: return CanGrabbed(dragger);
                        case EGrabCmd.Hold: return CanHold(dragger);
                        case EGrabCmd.Release: return CanRelease(dragger);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 能否被抓
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanGrabbed(Dragger dragger) => !IsUsageContain(dragger) && usage.GetCount(GrabbedUsageKey) < maxCountOfDragger;

        /// <summary>
        /// 能否保持
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanHold(Dragger dragger) => IsUsageContain(dragger) && usage.GetCount(GrabbedUsageKey) >= minCountOfDragger;

        /// <summary>
        /// 能否释放
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanRelease(Dragger dragger) => IsUsageContain(dragger);

        private bool IsUsageContain(Dragger dragger) => usage.Contains(GrabbedUsageKey, dragger);

        /// <summary>
        /// 执行交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteractAsInteractable(InteractData interactData)
        {
            if (_grabcmds.TryGetECmd(interactData.cmdName, out var grabCmd))
            {
                switch (grabCmd)
                {
                    case EGrabCmd.Grab:
                        {
                            if (AddGrabUsage(interactData.interactor as Dragger))
                            {
                                CallFinished(new HoldedInteractData(_grabcmds.GetCmdName(EGrabCmd.Grab), this, draggerInteractDatas));

                                CallFinished(CloneInteractData(interactData));


                                return EInteractResult.Finished;
                            }
                            return EInteractResult.Aborted;
                        }
                    case EGrabCmd.Hold:
                        {
                            var dragger = interactData.interactor as Dragger;
                            if (interactData is GrabInteractData data)
                            {
                                draggerInteractDatas[dragger] = data;

                                // 获取了所有拖拽器保持数据后，才更新被抓对象
                                if (draggerInteractDatas.Count >= grabberCount)
                                {
                                    doMotion = true;
                                }
                            }
                            return EInteractResult.Finished;
                        }
                    case EGrabCmd.Release:
                        {
                            if (RemoveGrabUsage(interactData.interactor as Dragger))
                            {
                                CallFinished(new HoldedInteractData(_grabcmds.GetCmdName(EGrabCmd.Release), this, draggerInteractDatas));

                                CallFinished(CloneInteractData(interactData));

                                return EInteractResult.Finished;
                            }
                            return EInteractResult.Aborted;
                        }
                }
            }
            return base.OnInteractAsInteractable(interactData);
        }

        /// <summary>
        /// 保持回调
        /// </summary>
        /// <param name="interactDatas"></param>
        protected virtual void OnHold(Dictionary<Dragger, GrabInteractData> interactDatas)
        {
            var totalPosition = Vector3.zero;
            var totalRotation = Quaternion.identity;
            var totalLocalScale = Vector3.zero;

            int positionDataCount = 0;
            int rotationDataCount = 0;
            int scaleDataCount = 0;
            var hitDatas = new List<(Vector3, Vector3)>();
            int hitDataCount = 0;

            var applyRigidbody = targetRigidbody;
            foreach (var data in draggerInteractDatas.Values)
            {
                if (data.position.HasValue)
                {
                    ++positionDataCount;
                    totalPosition += data.position.Value;
                }

                if (data.rotation.HasValue)
                {
                    ++rotationDataCount;
                    totalRotation *= data.rotation.Value;
                }

                if (data.scale.HasValue)
                {
                    ++scaleDataCount;
                    totalLocalScale += data.scale.Value;
                }

                if (applyRigidbody && data.hitOffset.HasValue && data.hitPosition.HasValue)
                {
                    ++hitDataCount;
                    hitDatas.Add((data.hitOffset.Value * data.hitOffsetScale.Value, data.hitPosition.Value));
                }
            }

            // 优先设置撞击数据
            if (hitDataCount > 0)
            {
                if (isConnectToOther)// 施力
                {
                    foreach (var data in hitDatas)
                    {
                        applyRigidbody.AddForceAtPosition(data.Item1, data.Item2, ForceMode.Force);
                    }
                }
                else // 设置速度
                {
                    var totalVelocity = Vector3.zero;
                    hitDatas.ForEach(data => totalVelocity += data.Item1);
                    applyRigidbody.velocity = totalVelocity;
                }
            }
            else
            {
                // 设置位置
                if (positionDataCount > 0)
                {
                    position = totalPosition / positionDataCount;
                }

                // 设置旋转量
                if (rotationDataCount > 0)
                {
                    rotation = totalRotation;
                }

                // 设置缩放量
                if (scaleDataCount > 0)
                {
                    targetTransform.localScale = totalLocalScale / scaleDataCount;
                }

            }

            if (hitDataCount > 0 || positionDataCount > 0 || rotationDataCount > 0 || scaleDataCount > 0)
            {
                CallFinished(new HoldedInteractData(_grabcmds.GetCmdName(EGrabCmd.Hold), this, draggerInteractDatas));
                CallFinished(new InteractData(_grabResultCmds.GetCmdName(EGrabResultCmd.TransformChanged), this));
            }
        }

        /// <summary>
        /// 抓取器数量
        /// </summary>
        public int grabberCount => usage.GetCount(GrabbedUsageKey);

        /// <summary>
        /// 是否被拖拽器拖拽
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public bool IsGrabbedBy(InteractUsageObject grabber) => usage.Contains(GrabbedUsageKey, grabber);

        /// <summary>
        /// 是否仅仅被拖拽器拖拽
        /// </summary>
        /// <param name="grabber"></param>
        /// <returns></returns>
        public bool IsOnlyGrabbedBy(InteractUsageObject grabber) => usage.Contains(GrabbedUsageKey, u => u.users.Count == 1 && u.users[0] == grabber);

        /// <summary>
        /// 添加抓用途: 使用固定抓关键字
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        public bool AddGrabUsage(InteractUsageObject grabber)
        {
            if (grabber && usage.Add(GrabbedUsageKey, grabber))
            {
                if (grabberCount == 1 && targetRigidbody)
                {
                    rigidbodyRecorder.Record(targetRigidbody);
                    targetRigidbody.useGravity = false; // 被抓对象的刚体停止使用重力
                    //targetRigidbody.isKinematic = true;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  移除抓用途: 使用固定抓关键字
        /// </summary>
        /// <param name="dragger"></param>
        public bool RemoveGrabUsage(InteractUsageObject grabber)
        {
            if (grabber && usage.Remove(GrabbedUsageKey, grabber))
            {
                if (grabberCount == 0)
                {
                    rigidbodyRecorder.RecoverExcludeVelocity();
                    rigidbodyRecorder.Clear();
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 碰撞判断

        /// <summary>
        /// 是否产生碰撞
        /// </summary>
        public bool isCollision => entercolliders.Count > 0;

        /// <summary>
        /// 与可抓对象产生碰撞进入或触发器进入的碰撞体
        /// </summary>
        public List<Collider> entercolliders { get; private set; } = new List<Collider>();

        private void AddCollider(Collider collider)
        {
            if (!entercolliders.Contains(collider))
            {
                entercolliders.Add(collider);
            }
        }

        private void RemoveCollider(Collider collider)
        {
            entercolliders.Remove(collider);
        }

        #endregion
    }

    #region GrabbableUser

    /// <summary>
    /// 可抓对象使用者
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public abstract class GrabbableUser : Interactor
    {
        /// <summary>
        /// 可抓对象
        /// </summary>
        public Grabbable grabbable => this.XGetComponentInParent(ref _grabbable);

        private Grabbable _grabbable;
    }

    #endregion

    #region GrabInteractData

    /// <summary>
    /// 持有交互数据
    /// </summary>
    public class HoldedInteractData : InteractData<HoldedInteractData>
    {
        /// <summary>
        /// 拖拽器与抓数据
        /// </summary>
        public Dictionary<Dragger, GrabInteractData> draggerInteractDatas { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public HoldedInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="draggerInteractDatas"></param>
        public HoldedInteractData(string cmdName, InteractObject interactObject, Dictionary<Dragger, GrabInteractData> draggerInteractDatas) : base(cmdName, interactObject)
        {
            this.draggerInteractDatas = draggerInteractDatas;
        }

        /// <summary>
        /// 复制函数
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is HoldedInteractData holdedInteractData)
            {
                holdedInteractData.draggerInteractDatas = this.draggerInteractDatas;
            }
        }
    }

    /// <summary>
    /// 抓交互数据
    /// </summary>
    public class GrabInteractData : InteractData<GrabInteractData>
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3? position { get; private set; } = null;

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion? rotation { get; private set; } = null;

        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3? scale { get; private set; } = null;

        /// <summary>
        /// 撞击偏差量
        /// </summary>
        public Vector3? hitOffset { get; private set; } = null;

        /// <summary>
        /// 撞击偏差缩放量
        /// </summary>
        public float? hitOffsetScale { get; private set; } = null;

        /// <summary>
        /// 撞击用点
        /// </summary>
        public Vector3? hitPosition { get; private set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GrabInteractData() { }

        /// <summary>
        /// 构造函数:使用位置、旋转和缩放等数据构建交互数据
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? position, Quaternion? rotation, Vector3? scale, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        /// <summary>
        /// 构造函数:使用撞击偏差量和撞击点构建交互数据
        /// </summary>
        /// <param name="hitOffset"></param>
        /// <param name="hitOffsetScale"></param>
        /// <param name="hitPosition"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? hitOffset, float? hitOffsetScale, Vector3? hitPosition, string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables)
        {
            this.hitOffset = hitOffset;
            this.hitOffsetScale = hitOffsetScale;
            this.hitPosition = hitPosition;
        }

        /// <summary>
        /// 构造函数:使用目标位置、撞击偏差量和撞击点构建交互数据
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="hitOffset"></param>
        /// <param name="hitOffsetScale"></param>
        /// <param name="hitPosition"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public GrabInteractData(Vector3? position, Quaternion? rotation, Vector3? hitOffset, float? hitOffsetScale, Vector3? hitPosition, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(hitOffset, hitOffsetScale, hitPosition, cmdName, interactor, interactables)
        {
            this.position = position;
            this.rotation = rotation;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            var grabData = interactData as GrabInteractData;
            if (grabData != null)
            {
                grabData.position = position;
                grabData.rotation = rotation;
                grabData.scale = scale;
                grabData.hitOffset = hitOffset;
                grabData.hitOffsetScale = hitOffsetScale;
                grabData.hitPosition = hitPosition;
            }
        }
    }

    #endregion
}
