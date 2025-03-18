using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMechanicalMotion.States
{
    /// <summary>
    /// 平面机构运动控制
    /// </summary>
    [ComponentMenu(MechanicalMotionManager.Title + "/" + Title, typeof(MechanicalMotionManager))]
    [Name(Title, nameof(PlaneMechanismMotion))]
    [XCSJ.Attributes.Icon(nameof(XCSJ.PluginMechanicalMotion.Tools.RotationMechanism))]
    [Owner(typeof(MechanicalMotionManager))]
    public class PlaneMechanismMotion : WorkClip<PlaneMechanismMotion>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "平面机构运动控制";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(MechanicalMotionManager.Title, typeof(MechanicalMotionManager))]
        [StateComponentMenu(MechanicalMotionManager.Title + "/" + Title, typeof(MechanicalMotionManager))]
        [Name(Title, nameof(PlaneMechanismMotion))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 平面机构
        /// </summary>
        [Name("平面机构")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlaneMechanism _planeMechanism;

        /// <summary>
        /// 运动范围
        /// </summary>
        [Name("运动范围")]
        [Tip("当前范围要比平面机构所限定的范围要小", "The current range is smaller than that limited by the planar mechanism")]
        [LimitRange(-10000, 10000)]
        public Vector2 _range = new Vector2(-1,1);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (_planeMechanism) _planeMechanism.SetValue(percent.percent01OfWorkCurve * (_range.y - _range.x) + _range.x);
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _planeMechanism? _planeMechanism.mechanismName:"";
        }
    }
}

