using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 单一动画:单一动画组件是播放Unity的Animator动画的对象。播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SingleAnimator))]
    [Tip("单一动画组件是播放Unity的Animator动画的对象。播放完毕后，组件切换为完成态。", "A single animation component is an object that plays unity's animator animation. After playing, the component switches to the finished state.")]
    [XCSJ.Attributes.Icon(index = 33642)]
    public class SingleAnimator : UnityAnimator<SingleAnimator>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "单一动画";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SingleAnimator))]
        [Tip("单一动画组件是播放Unity的Animator动画的对象。播放完毕后，组件切换为完成态。", "A single animation component is an object that plays unity's animator animation. After playing, the component switches to the finished state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
    }
}
