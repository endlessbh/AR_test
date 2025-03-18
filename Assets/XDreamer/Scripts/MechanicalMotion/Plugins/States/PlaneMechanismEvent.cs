using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using static XCSJ.PluginMechanicalMotion.Tools.PlaneMechanism;

namespace XCSJ.PluginMechanicalMotion.States
{
    /// <summary>
    /// 平面机构运动控制
    /// </summary>
    [ComponentMenu(MechanicalMotionManager.Title + "/" + Title, typeof(MechanicalMotionManager))]
    [Name(Title, nameof(PlaneMechanismEvent))]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Owner(typeof(MechanicalMotionManager))]
    public class PlaneMechanismEvent : Trigger<PlaneMechanismEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "平面机构事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MechanicalMotionManager.Title, typeof(MechanicalMotionManager))]
        [StateComponentMenu(MechanicalMotionManager.Title + "/" + Title, typeof(MechanicalMotionManager))]
        [Name(Title, nameof(PlaneMechanismEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 平面机构
        /// </summary>
        [Name("平面机构")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlaneMechanism _planeMechanism;

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.VelocityChanged;

        /// <summary>
        /// 速度变量
        /// </summary>
        [Name("速度变量")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _velocityVar = "";

        /// <summary>
        /// 当前值变量
        /// </summary>
        [Name("当前值变量")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _currentValueVar = "";

        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            PlaneMechanism.onEventCallback += OnEvent;
        }

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            PlaneMechanism.onEventCallback -= OnEvent;
        }

        /// <summary>
        /// 平面机构事件回调
        /// </summary>
        /// <param name="planeMechanism"></param>
        /// <param name="eventType"></param>
        protected void OnEvent(PlaneMechanism planeMechanism, EventData eventData)
        {
            if (_planeMechanism == planeMechanism && !finished)
            {
                finished = _eventType == eventData.eventType;
                if (finished)
                {
                    var instance = ScriptManager.instance;
                    if (instance)
                    {
                        instance.TrySetHierarchyVarValue(_velocityVar, planeMechanism.velocity.ToString());
                        instance.TrySetHierarchyVarValue(_currentValueVar, planeMechanism.currentValue.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _planeMechanism ? _planeMechanism.mechanismName : "";
        }
    }
}

