using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 定时器：用于添加具有延时执行功能的状态
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(Timer))]
    [Tip("用于添加具有延时执行功能的状态", "Used to add a state with delayed execution function")]
    [XCSJ.Attributes.Icon(EIcon.Timer)]
    public class Timer : WorkClip<Timer>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "定时器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(Timer))]
        [Tip("用于添加具有延时执行功能的状态", "Used to add a state with delayed execution function")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTimer(IGetStateCollection obj) => CreateNormalState(obj);

        public override string ToFriendlyString() => timeLength.ToString() + "秒";

        protected override void OnSetPercent(Percent percent, StateData stateData) { }
    }
}
