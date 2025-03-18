using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Draggers
{
    #region 插槽结果命令

    /// <summary>
    /// 插槽结果命令
    /// </summary>
    public enum ESocketResultCmd
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
        /// 进入触发区域
        /// </summary>
        [Name("进入触发区域")]
        EntryTriggerArea = 1000,

        /// <summary>
        /// 停留触发器区域
        /// </summary>
        [Name("停留触发区域")]
        StayTriggerArea,

        /// <summary>
        /// 退出触发区域
        /// </summary>
        [Name("退出触发区域")]
        ExitTriggerArea,
    }


    /// <summary>
    /// 插槽结果命令
    /// </summary>
    [Serializable]
    public class SocketResultCmd : Cmd<ESocketResultCmd> { }

    /// <summary>
    /// 插槽结果命令集
    /// </summary>
    [Serializable]
    public class SocketResultCmds : Cmds<ESocketResultCmd, SocketResultCmd> { }

    #endregion

    /// <summary>
    /// 单一插槽 ： 
    /// 1、仅容纳一个可抓对象的插槽
    /// 2、支持刚体或非刚体对象的触发吸附
    /// 3、当可抓对象没有拖拽器作用时，插槽吸附力才会产生作用。
    /// 4、插槽的命令由内部产生，反馈到外部
    /// </summary>
    [Name("单一插槽")]
    [Tip("仅容纳一个对象的插槽")]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager), index = InteractionCategory.InteractorIndex)]
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class SingleSocket : BaseSocket
    {
        #region 用途关键字

        /// <summary>
        /// 触发区域用途关键字
        /// </summary>
        public const string TriggerAreaUsageKey = nameof(SingleSocket) + "." + nameof(ESocketResultCmd.EntryTriggerArea);

        /// <summary>
        /// 抓用途关键字
        /// </summary>
        public const string GrabUsageKey = nameof(SingleSocket) + "." + nameof(ESocketResultCmd.Grab);

        #endregion

        #region 交互输入

        /// <summary>
        /// 插槽命令列表
        /// </summary>
        [Name("插槽命令列表")]
        public GrabResultCmds _socketCmds = new GrabResultCmds();

        /// <summary>
        /// 插槽命令
        /// </summary>
        public override List<string> cmds => _socketCmds.cmdNames;

        /// <summary>
        /// 输入交互状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (isCollisionTriggerMode || interactData.interactState != EInteractState.Finished) return;

            if (interactData.interactor is Grabbable grabbable && grabbable && _socketCmds.TryGetECmd(interactData.cmdName, out var inCmd))
            {
                var inArea = InTriggerArea(grabbable.transform);
                HandleTriggerArea(inArea, grabbable);

                switch (inCmd)
                {
                    case EGrabResultCmd.Grab:// 可抓对象被抓 => 插槽的放
                        {
                            TryInteractInternal(EGrabResultCmd.Release, grabbable, interactData);
                            break;
                        }
                    case EGrabResultCmd.Release:// 可抓对象被放 => 插槽的抓
                        {
                            if (grabbable.grabberCount == 0 && inArea)
                            {
                                if (CanGrabByRule(grabbable))
                                {
                                    TryInteractInternal(EGrabResultCmd.Grab, grabbable);
                                }
                                else
                                {
                                    OnGrabFalseRule(grabbable, interactData);
                                }
                            }
                            break;
                        }
                }
            }
        }

        #endregion

        #region 交互输出

        /// <summary>
        /// 插槽结果命令列表
        /// </summary>
        [Name("插槽结果命令列表")]
        public SocketResultCmds _socketResultCmds = new SocketResultCmds();

        /// <summary>
        /// 插槽结果命令列表
        /// </summary>
        public override List<string> resultCmds => _socketResultCmds.cmdNames;

        #endregion

        #region 工作模式

        /// <summary>
        /// 工作模式
        /// </summary>
        public enum EWorkMode
        {
            /// <summary>
            /// 抓
            /// </summary>
            [Name("触发和抓")]
            [Tip("插槽吸附对象")]
            TriggerAndGrab,

            /// <summary>
            /// 触发
            /// </summary>
            [Name("触发")]
            [Tip("插槽感应对象进出触发区")]
            Trigger,

            /// <summary>
            /// 抓
            /// </summary>
            [Name("抓")]
            [Tip("插槽吸附对象")]
            Grab,
        }

        /// <summary>
        /// 工作模式
        /// </summary>
        [Group("插槽设置", textEN = "Socket Settings")]
        [Name("工作模式")]
        [EnumPopup]
        public EWorkMode _workMode = EWorkMode.TriggerAndGrab;

        /// <summary>
        /// 是否触发模式
        /// </summary>
        public bool isTriggerMode => _workMode == EWorkMode.Trigger || _workMode == EWorkMode.TriggerAndGrab;

        /// <summary>
        /// 是否抓模式
        /// </summary>
        public bool isGrabMode => _workMode == EWorkMode.Grab || _workMode == EWorkMode.TriggerAndGrab;

        #endregion

        #region 插槽标签

        /// <summary>
        /// 插槽标签
        /// </summary>
        [PropertyKey(index = 1)]
        public const string SocketTag = "插槽标签";

        /// <summary>
        /// 插槽标签匹配
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        private bool IsMatchSocketTag(Grabbable grabbable) => grabbable ? _tagProperty.IsMatch(grabbable.socketTags) : false;

        #endregion

        #region 触发模式

        /// <summary>
        /// 触发模式
        /// </summary>
        public enum ETriggerMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 距离
            /// </summary>
            [Name("距离")]
            [Tip("检测可抓对象的中心与插槽中心的距离是否小于设定距离", "Check if the distance between the center of the grabable object and the center of the slot is less than the set distance")]
            Distance,

            /// <summary>
            /// 包围盒
            /// </summary>
            [Name("包围盒")]
            [Tip("检测可抓对象的包围盒与插槽包围盒是否交差", "Check if the bounding box of the grabable object intersects with the slot bounding box")]
            Bounds,

            /// <summary>
            /// 碰撞
            /// </summary>
            [Name("碰撞")]
            Collision = 1000,
        }

        /// <summary>
        /// 触发模式
        /// </summary>
        [Name("触发模式")]
        [EnumPopup]
        public ETriggerMode _triggerMode = ETriggerMode.Distance;


        /// <summary>
        /// 自定义包围盒
        /// </summary>
        [Name("自定义包围盒")]
        [Tip("为True时，使用属性设定的包围盒;为False时，使用实时计算的包围盒", "When true, use the bounding box set by the attribute; When false, use real-time computed bounding box")]
        [HideInSuperInspector(nameof(_triggerMode), EValidityCheckType.NotEqual, ETriggerMode.Bounds)]
        public bool _customsBounds = false;

        /// <summary>
        /// 包围盒
        /// </summary>
        [Name("包围盒")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_triggerMode), EValidityCheckType.NotEqual | EValidityCheckType.Or, ETriggerMode.Bounds, nameof(_customsBounds), EValidityCheckType.False)]
        [ComponentPopup]
        public BoundsProvider _boundsProvider;

        /// <summary>
        /// 触发距离
        /// </summary>
        [Name("触发距离")]
        [HideInSuperInspector(nameof(_triggerMode), EValidityCheckType.NotEqual, ETriggerMode.Distance)]
        [Min(0)]
        public float _triggerDistance = 1;

        /// <summary>
        /// 触发变换：作为触发模式的变换源数据计算触发
        /// </summary>
        [Name("触发变换")]
        public Transform triggerTransform => transform;

        /// <summary>
        /// 是否碰撞触发模式
        /// </summary>
        public bool isCollisionTriggerMode => _triggerMode == ETriggerMode.Collision;

        #endregion

        #region 抓模式

        /// <summary>
        /// 抓模式
        /// </summary>
        public enum EGrabMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 同步姿态
            /// </summary>
            [Name("同步姿态")]
            SyncPose,
        }

        /// <summary>
        /// 抓模式
        /// </summary>
        [Name("抓模式")]
        [EnumPopup]
        public EGrabMode _grabMode = EGrabMode.SyncPose;

        /// <summary>
        /// 姿态
        /// </summary>
        public enum EPose
        {
            /// <summary>
            /// 位置与旋转
            /// </summary>
            [Name("位置与旋转")]
            PositionAndRotation,

            /// <summary>
            /// 位置
            /// </summary>
            [Name("位置")]
            Position,

            /// <summary>
            /// 旋转
            /// </summary>
            [Name("旋转")]
            Rotation,
        }

        /// <summary>
        /// 姿态
        /// </summary>
        [Name("姿态")]
        [EnumPopup]
        public EPose _pose = EPose.PositionAndRotation;

        /// <summary>
        /// 抓住姿态参考对象
        /// </summary>
        [Name("抓姿态参考对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _grabPoseReference;

        /// <summary>
        /// 抓住姿态参考对象
        /// </summary>
        public Transform grabPoseReference => _grabPoseReference ? _grabPoseReference : transform;

        private bool CanGrabByRule(Grabbable grabbable)
        {
            var usageData = grabbable.usage.Get(TriggerAreaUsageKey);
            if (usageData != null)
            {
                var users = usageData.users;
                switch (users.Count)
                {
                    case 0: return false;
                    case 1: return users[0] == this;
                    default:
                        {
                            SingleSocket user = null;
                            float minDistance = float.MaxValue;
                            var target = grabbable.transform;
                            foreach (var item in users)
                            {
                                if (item is SingleSocket socket && socket)
                                {
                                    var dis = socket.GetDistance(target);
                                    if (dis < minDistance)
                                    {
                                        user = socket;
                                        minDistance = dis;
                                    }
                                }
                            }
                            return user == this;
                        }
                }
            }
            return false;
        }

        /// <summary>
        /// 抓失败处理规则
        /// </summary>
        public enum EGrabFalseRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 退出触发区域
            /// </summary>
            [Name("退出触发区域")]
            ExitTriggerArea,
        }

        /// <summary>
        /// 抓失败处理规则
        /// </summary>
        [Name("抓失败处理规则")]
        [EnumPopup]
        public EGrabFalseRule _onGrabFalseRule = EGrabFalseRule.ExitTriggerArea;

        private void OnGrabFalseRule(Grabbable grabbable, InteractData interactData)
        {
            switch (_onGrabFalseRule)
            {
                case EGrabFalseRule.ExitTriggerArea:
                    {
                        ExitTriggerArea(grabbable, interactData);
                        break;
                    }
            }
        }

        #endregion

        #region Unity 消息方法

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _socketCmds.Reset();
            _socketResultCmds.Reset();

            _tagProperty._tagPropertyDatas.Add(new TagPropertyData(SocketTag));

            var collider = GetComponent<Collider>();
            if (collider)
            {
                collider.isTrigger = true;
            }

            if (!_grabPoseReference) _grabPoseReference = transform;

            if (CommonFun.GetBounds(out var _bounds, gameObject))
            {
                _triggerDistance = _bounds.size.magnitude/2;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (isCollisionTriggerMode)
            {
                // 检测碰撞体触发属性值
                var ownCollider = GetComponent<Collider>();
                if (ownCollider && !ownCollider.isTrigger)
                {
                    Debug.LogWarningFormat("{0}所在碰撞体触发器属性需要设定为启用", CommonFun.GameObjectComponentToString(this));
                }
            }
            else
            {
                if (_triggerMode == ETriggerMode.Bounds && _customsBounds && !_boundsProvider)
                {
                    Debug.LogErrorFormat("{0}单一插槽未指定包围盒提供者", CommonFun.GameObjectComponentToString(this));
                    enabled = false;
                    return;
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            switch (_grabMode)
            {
                case EGrabMode.SyncPose:
                    {
                        if (grabbedObject)
                        {
                            var poseObject = grabPoseReference;
                            switch (_pose)
                            {
                                case EPose.PositionAndRotation:
                                    {
                                        grabbedObject.SetPoseAndCallFinished(poseObject.position, poseObject.rotation);
                                        break;
                                    }
                                case EPose.Position:
                                    {
                                        grabbedObject.SetPositionAndCallFinished(poseObject.position);
                                        break;
                                    }
                                case EPose.Rotation:
                                    {
                                        grabbedObject.SetRotationAndCallFinished(poseObject.rotation);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 触发器进入
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (!isCollisionTriggerMode) return;

            HandleTriggerArea(true, collider.GetComponentInParent<Grabbable>());
        }

        /// <summary>
        /// 触发器停留
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerStay(Collider collider)
        {
            if (!isCollisionTriggerMode) return;

            // 只处于处于插槽内的被抓对象
            HandleTriggerArea(true, grabbedObject);
        }

        /// <summary>
        /// 触发器退出
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerExit(Collider collider)
        {
            if (!isCollisionTriggerMode) return;

            HandleTriggerArea(false, collider.GetComponentInParent<Grabbable>());
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (isTriggerMode && !isCollisionTriggerMode && _grabPoseReference)
            {
                switch (_triggerMode)
                {
                    case ETriggerMode.Distance:
                        {
                            var orgGizmos = Gizmos.color;
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireSphere(_grabPoseReference.position, _triggerDistance);
                            Gizmos.color = orgGizmos;
                            break;
                        }
                    case ETriggerMode.Bounds:
                        {
                            var orgGizmos = Gizmos.color;
                            Gizmos.color = Color.green;
                            if (_customsBounds)
                            {
                                if (_boundsProvider && _boundsProvider.TryGetBounds(out var b))
                                {
                                    Gizmos.DrawWireCube(b.center, b.size);
                                }
                            }
                            else
                            {
                                if (CommonFun.GetBounds(out var b, triggerTransform))
                                {
                                    Gizmos.DrawWireCube(b.center, b.size);
                                }
                            }
                            Gizmos.color = orgGizmos;
                            break;
                        }
                }

            }
#endif
        }

        #endregion

        #region 触发处理

        /// <summary>
        /// 可抓对象
        /// </summary>
        [Readonly]
        [Name("可抓对象")]
        public Grabbable grabbedObject;

        /// <summary>
        /// 在触发区中可抓对象列表
        /// </summary>
        [Readonly]
        [Name("在触发区中可抓对象列表")]
        public List<Grabbable> grabbablesInTriggerArea = new List<Grabbable>();

        private void HandleTriggerArea(bool inArea, Grabbable grabbable, InteractData interactData = null)
        {
            if (!grabbable && !IsMatchSocketTag(grabbable)) return;

            if (inArea)
            {
                EntryTriggerArea(grabbable, interactData);
            }
            else
            {
                ExitTriggerArea(grabbable, interactData);
            }
        }

        private void EntryTriggerArea(Grabbable grabbable, InteractData interactData = null)
        {
            if (grabbable.usage.Contains(TriggerAreaUsageKey, this))
            {
                if (grabbablesInTriggerArea.Contains(grabbable))
                {
                    CallFinished(new InteractData(ESocketResultCmd.StayTriggerArea, _socketResultCmds.GetCmdName(ESocketResultCmd.StayTriggerArea), this, grabbable));
                }
            }
            else
            {
                if (grabbable.usage.Add(TriggerAreaUsageKey, usageData => this))
                {
                    if (grabbablesInTriggerArea.AddWithDistinct(grabbable))
                    {
                        CallFinished(new InteractData(ESocketResultCmd.EntryTriggerArea, _socketResultCmds.GetCmdName(ESocketResultCmd.EntryTriggerArea), this, grabbable));
                    }
                }
            }
        }

        private void ExitTriggerArea(Grabbable grabbable, InteractData interactData = null)
        {
            var inUsage = grabbable.usage.Remove(TriggerAreaUsageKey, usageData => usageData.Contains(this) ? this : default);
            if (inUsage)
            {
                if (grabbablesInTriggerArea.Remove(grabbable))
                {
                    CallFinished(new InteractData(ESocketResultCmd.ExitTriggerArea, _socketResultCmds.GetCmdName(ESocketResultCmd.ExitTriggerArea), this, grabbable));
                }
            }
        }

        private bool InTriggerArea(Transform transform)
        {
            switch (_triggerMode)
            {
                case ETriggerMode.Distance: return GetDistance(transform) < _triggerDistance;
                case ETriggerMode.Bounds:
                    {
                        if (CommonFun.GetBounds(out var b1, transform))
                        {
                            if (_customsBounds)
                            {
                                if (_boundsProvider && _boundsProvider.TryGetBounds(out var b2))
                                {
                                    return b1.Intersects(b2);
                                }
                            }
                            else if (CommonFun.GetBounds(out var b2, triggerTransform))
                            {
                                return b1.Intersects(b2);
                            }
                        }
                        break;
                    }
            }
            return false;
        }

        /// <summary>
        /// 获取传入对象与插槽参考对象距离
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        private float GetDistance(Transform transform) => Vector3.Distance(triggerTransform.position, transform.position);

        #endregion

        #region 交互处理

        /// <summary>
        /// 插槽满
        /// </summary>
        public override bool full => grabbedObject;

        /// <summary>
        /// 是否包含可抓对象
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        public override bool Contains(Grabbable grabbable) => grabbedObject && grabbedObject == grabbable;

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (_socketCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabResultCmd.Grab: return isGrabMode && !grabbedObject && IsMatchSocketTag(GetGrabbable(interactData));
                    case EGrabResultCmd.Hold:
                    case EGrabResultCmd.Release: return isGrabMode && grabbedObject;
                }
            }

            return base.CanInteract(interactData);
        }

        /// <summary>
        /// 当交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_socketCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabResultCmd.Grab:
                        {
                            var grabbable = GetGrabbable(interactData);
                            var inUsage = grabbable.AddGrabUsage(this);
                            if (inUsage)
                            {
                                grabbedObject = grabbable;
                                var rig = grabbedObject.GetComponent<Rigidbody>();
                                if (rig)
                                {
                                    rig.isKinematic = true;
                                }
                                return EInteractResult.Finished;
                            }
                            break;
                        }
                    case EGrabResultCmd.Hold:
                        {
                            break;
                        }
                    case EGrabResultCmd.Release:
                        {
                            var grabbable = GetGrabbable(interactData);
                            if (grabbable == grabbedObject)
                            {
                                var inUsage = grabbable.RemoveGrabUsage(this);
                                if (inUsage)
                                {
                                    grabbedObject = null;
                                    return EInteractResult.Finished;
                                }
                            }
                            break;
                        }
                }
            }
            return EInteractResult.Aborted;
        }

        private Grabbable GetGrabbable(InteractData interactData) => (interactData.cloneSource != null ? interactData.cloneSource.interactor : interactData.interactable) as Grabbable;

        private bool TryInteractInternal(EGrabResultCmd socketCmd, Grabbable grabbable, InteractData interactData = null)
        {
            if (interactData == null)
            {
                interactData = new InteractData(socketCmd, _socketCmds.GetCmdName(socketCmd), this, grabbable);
            }
            else
            {
                interactData = this.CloneInteractData(interactData);
                interactData.SetCmd(_socketCmds.GetCmdName(socketCmd), this, socketCmd);
            }
            return TryInteract(interactData, out _);
        }

        #endregion
    }

    #region 基础插槽

    /// <summary>
    /// 基础插槽
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    public abstract class BaseSocket : InteractUsageObject
    {
        /// <summary>
        /// 满
        /// </summary>
        public abstract bool full { get; }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        public abstract bool Contains(Grabbable grabbable);
    } 

    #endregion
}
