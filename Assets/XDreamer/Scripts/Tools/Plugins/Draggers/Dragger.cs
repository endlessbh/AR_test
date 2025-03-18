using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 抓枚举
    /// </summary>
    public enum EGrabCmd
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
    }

    /// <summary>
    /// 抓命令
    /// </summary>
    [Serializable]
    public class GrabCmd : Cmd<EGrabCmd> { }

    /// <summary>
    /// 抓命令列表
    /// </summary>
    [Serializable]
    public class GrabCmds : Cmds<EGrabCmd, GrabCmd> { }

    /// <summary>
    /// 拖拽交互器
    /// </summary>
    public abstract class Dragger : SelectionListener
    {
        #region 交互

        /// <summary>
        /// 抓命令
        /// </summary>
        [Name("抓命令")]
        [OnlyMemberElements]
        public GrabCmds _grabCmds = new GrabCmds();

        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _grabCmds.cmdNames;

        private static List<string> emptyCmdNames = new List<string>();

        /// <summary>
        /// 当前工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            if (busy) return emptyCmdNames;

            // 可开启释放
            if (grabbedObject)
            {
                return _grabCmds.GetCmdNames(EGrabCmd.Release, EGrabCmd.Hold);
            }
            else // 开启抓
            {
                return _grabCmds.GetCmdNames(EGrabCmd.Grab);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() => ResetCmd();

        /// <summary>
        /// 重置交互命令
        /// </summary>
        [ContextMenu("重置交互命令")]
        protected void ResetCmd()
        {
            _grabCmds.Reset();
        }

        /// <summary>
        /// 抓状态
        /// </summary>
        public enum EGrabState
        {
            /// <summary>
            /// 无：空手状态
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 抓进入
            /// </summary>
            [Name("抓开始")]
            GrabEnter,

            /// <summary>
            /// 抓过程中
            /// </summary>
            [Name("抓过程中")]
            Grabing,

            /// <summary>
            /// 抓退出
            /// </summary>
            [Name("抓结束")]
            GrabExit,

            /// <summary>
            /// 持有物品
            /// </summary>
            [Name("保持对象")]
            Holding,

            /// <summary>
            /// 放进入
            /// </summary>
            [Name("放开始")]
            ReleaseEnter,

            /// <summary>
            /// 放置过程中
            /// </summary>
            [Name("放过程中")]
            Releaseing,

            /// <summary>
            /// 放退出
            /// </summary>
            [Name("放结束")]
            ReleaseExit,
        }

        /// <summary>
        /// 抓状态
        /// </summary>
        public virtual EGrabState grabState { get => _grabState; protected set => _grabState = value; }

        /// <summary>
        /// 拖拽设置
        /// </summary>
        [Group("基础设置", textEN = "Base Settings")]
        [SerializeField]
        [Readonly]
        private EGrabState _grabState = EGrabState.None;

        /// <summary>
        /// 当前抓取器正在执行过程处理工作，无法接收外部指令
        /// </summary>
        public bool busy => grabState != EGrabState.None && grabState != EGrabState.Holding;

        /// <summary>
        /// 拖拽时标识UI
        /// </summary>
        protected virtual bool countUI => true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            holdCmdName = _grabCmds.GetCmdName(EGrabCmd.Hold);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ResetData();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (busy) return false;

            if (_grabCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabCmd.Grab:
                        {
                            _grabData = interactData;
                            return CanGrab(interactData, GetGrabbable(_grabData.interactable));
                        }
                    case EGrabCmd.Hold:
                        {
                            _holdData = interactData;
                            return CanHold(interactData, GetGrabbable(_holdData.interactable));
                        }
                    case EGrabCmd.Release:
                        {
                            _releaseData = interactData;
                            return CanRelease(interactData, GetGrabbable(_releaseData.interactable));
                        }
                }
            }
            return base.CanInteract(interactData);
        }

        /// <summary>
        /// 响应交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_grabCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabCmd.Grab:
                        {
                            grabbedObject = GetGrabbable();
                            if (grabbedObject)
                            {
                                grabState = EGrabState.GrabEnter;
                            }
                            return EInteractResult.Finished;
                        }
                    case EGrabCmd.Hold:
                        {
                            return grabState == EGrabState.Holding ? EInteractResult.Finished : EInteractResult.Aborted;
                        }
                    case EGrabCmd.Release:
                        {
                            grabState = EGrabState.ReleaseEnter;
                            return EInteractResult.Finished;
                        }
                }
            }
            return EInteractResult.Aborted;
        }

        /// <summary>
        /// 固定更新:主要操作物理对象，因此逻辑在固定更新中执行
        /// </summary>
        protected virtual void FixedUpdate()
        {
            try
            {
                //if (grabbedObject && (!grabbedObject.enabled || !grabbedObject.gameObject.activeInHierarchy))
                //{
                //    HandleException();
                //    return;
                //}
                switch (grabState)
                {
                    case EGrabState.GrabEnter:
                        {
                            _holdData = null;

                            OnGrabEnter();

                            BeginOnUI();

                            grabbedObject.TryInteractAsInteractable(_grabData, out _);

                            grabState = EGrabState.Grabing;

                            break;
                        }
                    case EGrabState.Grabing:
                        {
                            if (IsGrabbing())
                            {
                                OnGrabbing();
                            }
                            else
                            {
                                grabState = EGrabState.GrabExit;
                            }
                            break;
                        }
                    case EGrabState.GrabExit:
                        {
                            OnGrabExit();

                            grabState = EGrabState.Holding;

                            _grabData = null;

                            break;
                        }
                    case EGrabState.Holding:
                        {
                            if (_holdData != null)
                            {
                                OnHolding();

                                var data = CreateHoldData();
                                if (data != null)
                                {
                                    grabbedObject.TryInteractAsInteractable(data, out _);
                                }
                            }

                            break;
                        }
                    case EGrabState.ReleaseEnter:
                        {
                            _holdData = null;

                            OnReleaseEnter();

                            grabState = EGrabState.Releaseing;

                            break;
                        }
                    case EGrabState.Releaseing:
                        {
                            if (IsReleaseing())
                            {
                                OnReleaseing();
                            }
                            else
                            {
                                grabState = EGrabState.ReleaseExit;
                            }
                            break;
                        }
                    case EGrabState.ReleaseExit:
                        {
                            OnReleaseExit();

                            grabbedObject.TryInteractAsInteractable(_releaseData, out _);

                            ResetData();

                            break;
                        }
                }
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                Debug.LogException(e);
                ResetData();
            }
#else
            catch
            {
                ResetData();
            }
#endif
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public virtual void ResetData()
        {
            grabState = EGrabState.None;

            if (grabbedObject)
            {
                grabbedObject.RemoveGrabUsage(this);
            }
            grabbedObject = null;

            EndOnUI();

            _grabData = null;
            _holdData = null;
            _releaseData = null;
        }

        private bool isBeginOnUI = false;

        private void BeginOnUI()
        {
            if (countUI)
            {
                if (!isBeginOnUI)
                {
                    isBeginOnUI = true;
                    CommonFun.BeginOnUI();
                }
            }
        }

        private void EndOnUI()
        {
            if (countUI)
            {
                if (isBeginOnUI)
                {
                    isBeginOnUI = false;
                    CommonFun.EndOnUI();
                }
            }
        } 

        #endregion

        #region 抓

        /// <summary>
        /// 抓对象
        /// </summary>
        public Grabbable grabbedObject 
        { 
            get => _grabbedObject;
            protected set
            {
                _grabbedObject = value;
            }
        }

        /// <summary>
        /// 抓对象
        /// </summary>
        [Readonly]
        //[HideInSuperInspector]
        public Grabbable _grabbedObject;

        private InteractData _grabData = null;

        /// <summary>
        /// 抓数据
        /// </summary>
        public RayInteractData grabData => _grabData as RayInteractData;

        /// <summary>
        /// 能否抓
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanGrab(InteractData interactData, Grabbable grabbable) => !grabbedObject && grabState == EGrabState.None && (grabbable && grabbable.enabled && grabbable.CanInteractAsInteractable(interactData));

        /// <summary>
        /// 正在抓
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsGrabbing() => false;

        /// <summary>
        /// 抓进入
        /// </summary>
        protected virtual void OnGrabEnter() { }

        /// <summary>
        /// 抓过程中
        /// </summary>
        protected virtual void OnGrabbing() { }

        /// <summary>
        /// 抓退出
        /// </summary>
        protected virtual void OnGrabExit() { }

        /// <summary>
        /// 获取抓对象
        /// </summary>
        /// <returns></returns>
        protected virtual Grabbable GetGrabbable() => GetGrabbable(_grabData.interactable);

        /// <summary>
        /// 获取可抓对象
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected virtual Grabbable GetGrabbable(IInteractable interactable) => GetGrabbableInternal(interactable);

        /// <summary>
        /// 获取插槽交互器内的可抓对象
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected Grabbable GetGrabbableInternal(IInteractable interactable) => interactable.GetInteractable<Grabbable>();

        #endregion 抓

        #region 保持

        private InteractData _holdData = null;

        /// <summary>
        /// 保持数据
        /// </summary>
        public RayInteractData holdData => _holdData as RayInteractData;

        /// <summary>
        /// 能否保持
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanHold(InteractData interactData, Grabbable grabbable) => grabbedObject && grabState == EGrabState.Holding && grabbedObject.CanInteractAsInteractable(interactData);

        /// <summary>
        /// 保持中
        /// </summary>
        protected virtual void OnHolding() { }

        /// <summary>
        /// 尝试获取拖拽位置数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragPosition(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        /// <summary>
        /// 尝试获取拖拽旋转数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        /// <summary>
        /// 尝试获取拖拽缩放数据
        /// </summary>
        /// <returns></returns>
        protected virtual bool TryGetDragScale(out Vector3 scale)
        {
            scale = Vector3.one;
            return false;
        }

        /// <summary>
        /// 创建拖拽交互数据
        /// </summary>
        /// <returns></returns>
        protected virtual GrabInteractData CreateHoldData()
        {
            Vector3? position = null;
            Quaternion? rotation = null;
            Vector3? scale = null;

            if (TryGetDragPosition(out var pos))
            {
                position = pos;
            }

            if (TryGetDragRotation(out var r))
            {
                rotation = r;
            }

            if (TryGetDragScale(out var s))
            {
                scale = s;
            }

            // 没有真实数据，也会产生空数据，保证可抓对象能接受所有当前与之交互的拖拽器发送的保持数据
            return new GrabInteractData(position, rotation, scale, holdCmdName, this, grabbedObject);
        }

        private string holdCmdName;

        #endregion 保持

        #region 放

        private InteractData _releaseData = null;

        /// <summary>
        /// 放数据
        /// </summary>
        public RayInteractData releaseData => _releaseData as RayInteractData;

        /// <summary>
        /// 能否放
        /// </summary>
        /// <param name="grabbable"></param>
        /// <returns></returns>
        protected virtual bool CanRelease(InteractData interactData, Grabbable grabbable) => grabbedObject && grabState == EGrabState.Holding && grabbedObject.CanInteractAsInteractable(interactData);

        /// <summary>
        /// 是否正在放
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsReleaseing() => false;

        /// <summary>
        /// 放进入
        /// </summary>
        protected virtual void OnReleaseEnter() { }

        /// <summary>
        /// 放过程中
        /// </summary>
        protected virtual void OnReleaseing() { }

        /// <summary>
        /// 放退出
        /// </summary>
        protected virtual void OnReleaseExit() { }

        #endregion 放

        public const string DefautKey = nameof(Dragger);

        public override string GetUsageKey(string key = null) => DefautKey;
    }
}