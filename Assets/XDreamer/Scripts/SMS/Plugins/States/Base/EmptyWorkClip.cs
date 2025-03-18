using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 空工作剪辑
    /// </summary>
    [Name("空工作剪辑")]
    [Tip("空工作剪辑不具有任何控制表现逻辑;可用于做补间填充;")]
    public class EmptyWorkClip : WorkClip<EmptyWorkClip>
    {
        protected override void OnSetPercent(Percent percent, StateData stateData) { }
    }
}
