using UnityEngine.Playables;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 可播放导引器
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(PlayableDirector))]
    [Tip("播放Unity的时间轴中的可播放导引器", "Playable guides in the timeline of play unity")]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    public class XPlayableDirector : WorkClip<XPlayableDirector>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "可播放导引器";

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(PlayableDirector))]
        [Tip("播放Unity的时间轴中的可播放导引器", "Playable guides in the timeline of play unity")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 可播放导引器对象
        /// </summary>
        [Group("可播放导引器属性")]
        [Name(Title)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(PlayableDirector))]
        public PlayableDirector playableDirector;

        /// <summary>
        /// 当进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (playableDirector)
            {
                playableDirector.Play();
            }
        }

        /// <summary>
        /// 当退出时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (playableDirector)
            {
                playableDirector.Stop();
            }
        }

        /// <summary>
        /// 当设置百分比时
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (!playableDirector) return;
            if (playableDirector.state != PlayState.Playing) playableDirector.Play();
            playableDirector.time = playableDirector.duration * percent.percent01OfWorkCurve;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && playableDirector;
        }
    }
}
