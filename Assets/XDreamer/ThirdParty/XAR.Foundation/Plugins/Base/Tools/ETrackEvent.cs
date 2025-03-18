using XCSJ.Attributes;

namespace XCSJ.PluginXAR.Foundation.Base.Tools
{
    /// <summary>
    /// 跟踪事件
    /// </summary>
    [Name("跟踪事件")]
    public enum ETrackEvent
    {
        [Name("无")]
        [Tip("不做任何操作")]
        None,

        [Name("当已添加时")]
        OnAdded,

        [Name("当已移除时")]
        OnRemoved,

        [Name("当已更新时")]
        OnUpdated,

        [Name("当无时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为无时回调；")]
        OnNone,

        [Name("当到无时")]
        [Tip("上次跟踪状态不为无，本次跟踪状态为无时回调；")]
        OnToNone,

        [Name("当总是无时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为无时回调；")]
        OnNoneAlways,

        [Name("当无到限定时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为限定时回调；")]
        OnNoneToLimited,

        [Name("当无到跟踪中时")]
        [Tip("上次跟踪状态为无，本次跟踪状态为跟踪中时回调；")]
        OnNoneToTracking,

        [Name("当限定时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为限定时回调；")]
        OnLimited,

        [Name("当到限定时")]
        [Tip("上次跟踪状态不为限定，本次跟踪状态为限定时回调；")]
        OnToLimited,

        [Name("当总是限定时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为限定时回调；")]
        OnLimitedAlways,

        [Name("当限定到无时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为无时回调；")]
        OnLimitedToNone,

        [Name("当限定到跟踪中时")]
        [Tip("上次跟踪状态为限定，本次跟踪状态为跟踪中时回调；")]
        OnLimitedToTracking,

        [Name("当跟踪中时")]
        [Tip("不考虑上次跟踪状态，本次跟踪状态为跟踪中时回调；")]
        OnTracking,

        [Name("当到跟踪中时")]
        [Tip("上次跟踪状态不为跟踪中，本次跟踪状态为跟踪中时回调；")]
        OnToTracking,

        [Name("当总是跟踪时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为跟踪中时回调；")]
        OnTrackingAlways,

        [Name("当跟踪中到无时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为无时回调；")]
        OnTrackingToNone,

        [Name("当跟踪中到限定时")]
        [Tip("上次跟踪状态为跟踪中，本次跟踪状态为限定时回调；")]
        OnTrackingToLimited,
    }
}
