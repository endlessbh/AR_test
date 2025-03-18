using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Draggers
{
    #region 拖拽记录命令

    /// <summary>
    /// 拖拽记录命令枚举
    /// </summary>
    public enum EDragRecordCmd
    {
        [Name("记录可抓对象变换")]
        RecordGrabbableTransform,

        [Name("还原所有记录")]
        RecoverAllRecord,

        [Name("还原上一次记录")]
        RecoverLastRecord,

        [Name("清除记录")]
        Clear,
    }

    /// <summary>
    /// 拖拽记录命令
    /// </summary>
    [Serializable]
    public class DragRecordCmd : Cmd<EDragRecordCmd> { }

    /// <summary>
    /// 拖拽记录命令列表
    /// </summary>
    [Serializable]
    public class DragRecordCmds : Cmds<EDragRecordCmd, DragRecordCmd> { }

    #endregion

    /// <summary>
    /// 拖拽记录器
    /// </summary>
    [Name("拖拽记录器")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool("选择集", nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    public class DragRecorder : Interactor
    {
        #region 交互输入

        /// <summary>
        /// 拖拽记录命令
        /// </summary>
        [Name("拖拽记录命令")]
        public DragRecordCmds _dragRecordCmds = new DragRecordCmds();

        /// <summary>
        /// 所有命令
        /// </summary>
        public override List<string> cmds => _dragRecordCmds.cmdNames;

        /// <summary>
        /// 当输入交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactData"></param>
        protected override void OnInputInteract(InteractObject interactor, InteractData interactData)
        {
            base.OnInputInteract(interactor, interactData);

            if (interactData.interactState != EInteractState.Finished) return;

            if (interactor is Dragger dragger && dragger && _dragger.Contains(dragger))
            {
                if (dragger._grabCmds.TryGetECmd(interactData.cmdName, out var grabCmd))
                {
                    switch (grabCmd)
                    {
                        case EGrabCmd.Grab:
                            {
                                RecordGrabbableTransfrom(interactData.interactable);
                                break;
                            }
                    }
                }
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 记录处理规则
        /// </summary>
        public enum ERecordHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 清除
            /// </summary>
            [Name("禁用时清除记录")]
            ClearOnDisable,

            /// <summary>
            /// 禁用时全部恢复并清除
            /// </summary>
            [Name("禁用时恢复全部并清除记录")]
            RecoverAllAndClearOnDisable,
        }

        /// <summary>
        /// 记录处理规则
        /// </summary>
        [Name("记录处理规则")]
        [EnumPopup]
        public ERecordHandleRule _recordHandleRule = ERecordHandleRule.ClearOnDisable;

        /// <summary>
        /// 记录拖拽器列表
        /// </summary>
        [Name("记录拖拽器列表")]
        public List<Dragger> _dragger =  new List<Dragger>();

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => _dragRecordCmds.Reset();

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            switch (_recordHandleRule)
            {
                case ERecordHandleRule.ClearOnDisable:
                    {
                        ClearRecord();
                        break;
                    }
                case ERecordHandleRule.RecoverAllAndClearOnDisable:
                    {
                        RecoverAllAndClearRecord();
                        break;
                    }
            }
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 变换记录字典
        /// </summary>
        private TransformRecorder transformRecorder = new TransformRecorder();

        /// <summary>
        /// 变换记录数量
        /// </summary>
        public int recordTransformCount => transformRecorder.recordCount;

        /// <summary>
        /// 交互回调
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_dragRecordCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EDragRecordCmd.RecordGrabbableTransform:
                        {
                            return RecordGrabbableTransfrom(interactData.interactable) ? EInteractResult.Finished : EInteractResult.Aborted;
                        }
                    case EDragRecordCmd.RecoverAllRecord:
                        {
                            RecoverAllAndClearRecord();
                            return EInteractResult.Finished;
                        }
                    case EDragRecordCmd.RecoverLastRecord:
                        {
                            RecoverLastRecord();
                            return EInteractResult.Finished;
                        }
                    case EDragRecordCmd.Clear:
                        {
                            ClearRecord();
                            return EInteractResult.Finished;
                        }
                }
            }

            return EInteractResult.Aborted;
        }

        private bool RecordGrabbableTransfrom(InteractObject interactObject)
        {
            if (interactObject)
            {
                var grabbable = interactObject.GetComponent<Grabbable>();
                if (grabbable)
                {
                    transformRecorder.Record(grabbable.transform);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 还原上一次记录
        /// </summary>
        public void RecoverLastRecord() => transformRecorder.RecoverAndRemoveLast();

        /// <summary>
        /// 还原所有记录
        /// </summary>
        public void RecoverAllAndClearRecord() => transformRecorder.ReverseRecoverAndClear();

        /// <summary>
        /// 清除记录
        /// </summary>
        public void ClearRecord() => transformRecorder.Clear(); 

        #endregion
    }
}
