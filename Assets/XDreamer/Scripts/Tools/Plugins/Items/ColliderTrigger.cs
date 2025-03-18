using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Items
{
    #region 碰撞体触发器结果命令

    /// <summary>
    /// 碰撞体触发器结果命令枚举
    /// </summary>
    public enum EColliderTriggerResultCmd
    {
        /// <summary>
        /// 碰撞进入
        /// </summary>
        [Name("碰撞进入")]
        OnCollisionEnter,

        /// <summary>
        /// 碰撞退出
        /// </summary>
        [Name("碰撞退出")]
        OnCollisionExit,

        /// <summary>
        /// 碰撞停留
        /// </summary>
        [Name("碰撞停留")]
        OnCollisionStay,

        /// <summary>
        /// 触发器进入
        /// </summary>
        [Name("触发器进入")]
        OnTriggerEnter,

        /// <summary>
        /// 触发器退出
        /// </summary>
        [Name("触发器退出")]
        OnTriggerExit,

        /// <summary>
        /// 触发器停留
        /// </summary>
        [Name("触发器停留")]
        OnTriggerStay,

        /// <summary>
        /// 鼠标按下
        /// </summary>
        [Name("鼠标按下")]
        OnMouseDown,

        /// <summary>
        /// 鼠标拖拽
        /// </summary>
        [Name("鼠标拖拽")]
        OnMouseDrag,

        /// <summary>
        /// 鼠标进入
        /// </summary>
        [Name("鼠标进入")]
        OnMouseEnter,

        /// <summary>
        /// 鼠标退出
        /// </summary>
        [Name("鼠标退出")]
        OnMouseExit,

        /// <summary>
        /// 鼠标悬停
        /// </summary>
        [Name("鼠标悬停")]
        OnMouseOver,

        /// <summary>
        /// 鼠标弹起
        /// </summary>
        [Name("鼠标弹起")]
        OnMouseUp,

        /// <summary>
        /// 鼠标按下弹起作为按钮
        /// </summary>
        [Name("鼠标按下弹起作为按钮")]
        OnMouseUpAsButton,
    }

    /// <summary>
    /// 碰撞体触发器结果命令
    /// </summary>
    [Serializable]
    public class ColliderTriggerResultCmd : Cmd<EColliderTriggerResultCmd> { }

    /// <summary>
    /// 碰撞体触发器结果命令列表
    /// </summary>
    [Serializable]
    public class ColliderTriggerResultCmds : Cmds<EColliderTriggerResultCmd, ColliderTriggerResultCmd> { }

    #endregion

    /// <summary>
    /// 碰撞触发器:
    /// 使用物理系统的碰撞触发事件(OnTrigger)时, 碰撞体需要勾选触发器选项
    /// 触发条件：
    /// 1、双方都有碰撞体
    /// 2、运动的一方必须是刚体（当前对象可不为刚体，为被碰撞对象）
    /// 3、至少一方勾选Trigger触发器（当前对象勾选触发器属性）
    /// </summary>
    [Name("碰撞体触发器")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    [RequireManager(typeof(ToolsManager))]
    [Tool(InteractionCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    public class ColliderTrigger : Interactor
    {
        #region 交互输入

        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _colliderTriggerResultCmds.cmdNames;

        #endregion

        #region 交互输出

        /// <summary>
        /// 碰撞体触发器结果命令列表
        /// </summary>
        [Name("碰撞体触发器结果命令列表")]
        public ColliderTriggerResultCmds _colliderTriggerResultCmds = new ColliderTriggerResultCmds();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() => _colliderTriggerResultCmds.Reset();

        /// <summary>
        /// 自身刚体
        /// </summary>
        protected Rigidbody ownRigidbody
        {
            get
            {
                if (!_ownRigidbody)
                {
                    _ownRigidbody = GetComponentInParent<Rigidbody>();
                }
                return _ownRigidbody;
            }
        }
        private Rigidbody _ownRigidbody;

        #endregion

        #region 碰撞触发

        /// <summary>
        /// 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 OnCollisionEnter
        /// </summary>
        protected void OnCollisionEnter(Collision collision) => CallCollisionFinish(EColliderTriggerResultCmd.OnCollisionEnter, collision);

        /// <summary>
        /// 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 OnCollisionExit
        /// </summary>
        protected void OnCollisionExit(Collision collision) => CallCollisionFinish(EColliderTriggerResultCmd.OnCollisionExit, collision);

        /// <summary>
        /// 每当此碰撞器/刚体接触到刚体/碰撞器时，OnCollisionStay 将在每一帧被调用一次
        /// </summary>
        protected void OnCollisionStay(Collision collision) => CallCollisionFinish(EColliderTriggerResultCmd.OnCollisionStay, collision);

        /// <summary>
        /// 调用碰撞完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collision"></param>
        protected virtual void CallCollisionFinish(EColliderTriggerResultCmd cmd, Collision collision) =>
            CallFinished(CreateCollisionData(cmd, collision));

        /// <summary>
        /// 创建碰撞数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collision"></param>
        /// <returns></returns>
        protected virtual ColliderInteractData CreateCollisionData(EColliderTriggerResultCmd cmd, Collision collision) => new ColliderInteractData(collision, cmd, _colliderTriggerResultCmds.GetCmdName(cmd), this, collision.gameObject.GetComponent<InteractableEntity>());

        #endregion

        #region 触发器触发

        /// <summary>
        /// 如果另一个碰撞器进入了触发器，则调用 OnTriggerEnter
        /// </summary>
        private void OnTriggerEnter(Collider other) => CallTriggerFinish(EColliderTriggerResultCmd.OnTriggerEnter, other);

        /// <summary>
        /// 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
        /// </summary>
        private void OnTriggerExit(Collider other) => CallTriggerFinish(EColliderTriggerResultCmd.OnTriggerExit, other);

        /// <summary>
        /// 对于触动触发器的所有“另一个碰撞器”，OnTriggerStay 将在每一帧被调用一次
        /// </summary>
        private void OnTriggerStay(Collider other) => CallTriggerFinish(EColliderTriggerResultCmd.OnTriggerStay, other);

        /// <summary>
        /// 调用触发完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collider"></param>
        protected virtual void CallTriggerFinish(EColliderTriggerResultCmd cmd, Collider collider) =>
            CallFinished(CreateTriggerData(cmd, collider));

        /// <summary>
        /// 创建触发数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual ColliderInteractData CreateTriggerData(EColliderTriggerResultCmd cmd, Collider collider) => new ColliderInteractData(collider, cmd, _colliderTriggerResultCmds.GetCmdName(cmd), this, collider.GetComponents<InteractObject>());

        #endregion

        #region 鼠标触发

        /// <summary>
        /// 当用户在 GUIElement 或碰撞器上按鼠标按钮时调用 OnMouseDown
        /// </summary>
        private void OnMouseDown() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseDown);

        /// <summary>
        /// 当用户在 GUIElement 或碰撞器上单击鼠标并保持按住鼠标时调用 OnMouseDrag
        /// </summary>
        private void OnMouseDrag() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseDrag);

        /// <summary>
        /// 当鼠标进入 GUIElement 或碰撞器时调用 OnMouseEnter
        /// </summary>
        private void OnMouseEnter() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseEnter);

        /// <summary>
        /// 当鼠标不再停留在 GUIElement 或碰撞器上时调用 OnMouseExit
        /// </summary>
        private void OnMouseExit() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseExit);

        /// <summary>
        /// 当鼠标停留在 GUIElement 或碰撞器上时每帧都调用 OnMouseOver
        /// </summary>
        private void OnMouseOver() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseOver);

        /// <summary>
        /// 当用户松开鼠标按钮时调用 OnMouseUp
        /// </summary>
        private void OnMouseUp() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseUp);

        /// <summary>
        /// 仅当在同一 GUIElement 或碰撞器上按下鼠标，在松开时调用 OnMouseUpAsButton
        /// </summary>
        private void OnMouseUpAsButton() => CallMouseFinish(EColliderTriggerResultCmd.OnMouseUpAsButton);

        private void CallMouseFinish(EColliderTriggerResultCmd cmd) => CallFinished(new ColliderInteractData(cmd, _colliderTriggerResultCmds.GetCmdName(cmd), this));

        #endregion
    }

    /// <summary>
    /// 碰撞交互数据
    /// </summary>
    public class ColliderInteractData : InteractData
    {
        /// <summary>
        /// 碰撞
        /// </summary>
        public Collision collision { get; private set; }

        /// <summary>
        /// 碰撞体
        /// </summary>
        public Collider collider { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eCmd"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="interactObjects"></param>
        public ColliderInteractData(Enum eCmd, string cmdName, InteractObject interactObject, params InteractObject[] interactObjects) : base(eCmd, cmdName, interactObject, interactObjects) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collision"></param>
        /// <param name="eCmd"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="interactObjects"></param>
        public ColliderInteractData(Collision collision, Enum eCmd, string cmdName, InteractObject interactObject, params InteractObject[] interactObjects) : base(eCmd, cmdName, interactObject, interactObjects)
        {
            this.collision = collision;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="eCmd"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactObject"></param>
        /// <param name="interactObjects"></param>
        public ColliderInteractData(Collider collider, Enum eCmd, string cmdName, InteractObject interactObject, params InteractObject[] interactObjects) : base(eCmd, cmdName, interactObject, interactObjects)
        {
            this.collider = collider;
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is ColliderInteractData colliderInteractData)
            {
                colliderInteractData.collision = collision;
                colliderInteractData.collider = collider;
            }
        }
    }
}
