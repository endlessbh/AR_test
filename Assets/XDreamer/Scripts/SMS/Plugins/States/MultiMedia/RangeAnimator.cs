using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 区间动画:区间动画组件是播放Unity的Animator区间动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RangeAnimator))]
    [Tip("区间动画组件是播放Unity的Animator区间动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。", "The interval animation component is an object that plays the interval animation of unity's animator. You can set an interval in the middle of playing the whole animation. After playing, the component switches to the finished state.")]
    [XCSJ.Attributes.Icon(index = 33641)]
    public class RangeAnimator : UnityAnimator<RangeAnimator>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "区间动画";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(RangeAnimator))]
        [Tip("区间动画组件是播放Unity的Animator区间动画的对象。可设置播放整个动画的中间一段区间。播放完毕后，组件切换为完成态。", "The interval animation component is an object that plays the interval animation of unity's animator. You can set an interval in the middle of playing the whole animation. After playing, the component switches to the finished state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("Take区间")]
        [Tip("状态关联的动画(动作)在完整动画(FBX)上的帧区间;", "The frame interval of the animation (action) associated with the state on the complete animation (FBX);")]
        public Vector2Int takeRange = new Vector2Int();

        [Name("播放区间")]
        public Vector2Int playRange = new Vector2Int();

        public int takeFrameCount => takeRange.y - takeRange.x;

        public int playFrameCount => playRange.y - playRange.x;

        public int takeToPlayFrameCount => playRange.x - takeRange.x;

        /// <summary>
        /// 将播放区间百分比转化Take区间百分比
        /// </summary>
        /// <param name="percent">播放区间百分比</param>
        /// <returns>转化后的Take区间百分比</returns>
        private double PlayToTake(double percent)
        {
            var takeFrameCount = this.takeFrameCount;
            return takeFrameCount == 0 ? 0 : (percent * playFrameCount + takeToPlayFrameCount) / takeFrameCount;
        }

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            PlayAnimator(PlayToTake(percent.percent01OfWorkCurve));
        }

        public override string ToFriendlyString() => playRange.ToString();
    }
}
