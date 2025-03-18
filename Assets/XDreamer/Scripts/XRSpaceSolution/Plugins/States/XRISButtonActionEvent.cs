using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Scripts;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Tools.Controllers;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;

namespace XCSJ.PluginXRSpaceSolution.States
{
    /// <summary>
    /// XRIS按钮动作事件: XRIS按钮动作事件
    /// </summary>
    [ComponentMenu("XRIS/" + Title, typeof(XRSpaceSolutionManager))]
    [Name(Title, nameof(XRISButtonActionEvent))]
    [Tip("XRIS按钮动作事件")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(XRSpaceSolutionManager))]
    public class XRISButtonActionEvent : Trigger<XRISButtonActionEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XRIS按钮动作事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("XRIS", typeof(XRSpaceSolutionManager))]
        [StateComponentMenu("XRIS/" + Title, typeof(XRSpaceSolutionManager))]
        [Name(Title, nameof(XRISButtonActionEvent))]
        [Tip("XRIS按钮动作事件")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 按钮动作事件列表
        /// </summary>
        [Name("按钮动作事件列表")]
        [Tip("按钮动作事件列表中任意按钮被按压时则认为成立")]
        [ArrayElement]
        public List<ButtonActionEvent> _buttonActionEvents = new List<ButtonActionEvent>();

        /// <summary>
        /// 动作名变量字符串
        /// </summary>
        [Name("动作名变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _actionNameVarString = "";

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Handle(XRSpace.spaceConfigInstance);
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            XDreamerEvents.onXRAnswerReceived += OnXRAnswerReceived;
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (finished) return;
            var buttonActionEvent = _buttonActionEvents.FirstOrDefault(bae => bae.HasPressed());
            if (buttonActionEvent == null) return;
            finished = true;

            var script = ScriptManager.instance;
            if (script)
            {
                script.TrySetOrAddSetHierarchyVarValue(_actionNameVarString, buttonActionEvent.actionName);
            }
        }

        private void OnXRAnswerReceived(XRAnswer answer) => Handle(answer as XRSpaceConfigA);

        private void Handle(XRSpaceConfigA xRSpaceConfigA)
        {
            if (xRSpaceConfigA == null) return;
            _buttonActionEvents.ForEach(bae => bae.Handle(xRSpaceConfigA));
        }
    }

    /// <summary>
    /// 按钮动作事件
    /// </summary>
    [Serializable]
    public class ButtonActionEvent : InteractHandler
    {
        /// <summary>
        /// 按钮动作事件类型
        /// </summary>
        [Name("按钮动作事件类型")]
        [EnumPopup]
        public EButtonActionEventType _buttonActionEventType = EButtonActionEventType.Activate;

        /// <summary>
        /// 有按压
        /// </summary>
        /// <returns></returns>
        public bool HasPressed()
        {
            switch (_buttonActionEventType)
            {
                case EButtonActionEventType.Select: return Select();
                case EButtonActionEventType.Activate: return Activate();
                case EButtonActionEventType.UI: return UI();
                default:return false;
            }
        }
    }

    /// <summary>
    /// 按钮动作事件类型
    /// </summary>
    [Name("按钮动作事件类型")]
    public enum EButtonActionEventType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 选择
        /// </summary>
        [Name("选择")]
        Select,

        /// <summary>
        /// 激活
        /// </summary>
        [Name("激活")]
        Activate,

        /// <summary>
        /// UI
        /// </summary>
        [Name("UI")]
        UI,
    }
}
