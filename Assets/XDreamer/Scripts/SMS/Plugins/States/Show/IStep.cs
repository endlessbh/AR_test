using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginSMS.States.Show
{
    [Name("步骤")]
    [XCSJ.Attributes.Icon(index = 33536)]
    public interface IStep : ITreeNodeGraphExtension
    {
        new IStep parent { get; }

        new IStep[] children { get; }

        EStepState stepState { get; }
    }

    [Name("步骤状态")]
    public enum EStepState
    {
        [Name("无")]
        None,

        [Name("未完成")]
        Unfinished,

        [Name("活跃")]
        Active,

        [Name("已完成")]
        Finished
    }
}
