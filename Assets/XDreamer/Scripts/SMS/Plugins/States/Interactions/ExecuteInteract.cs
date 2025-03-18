using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginSMS.States.Interactions
{
    /// <summary>
    /// 执行交互
    /// </summary>
    [ComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
    [Name(Title, nameof(ExecuteInteract))]
    [XCSJ.Attributes.Icon(EIcon.Run)]
    public class ExecuteInteract : LifecycleExecutor<ExecuteInteract>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "执行交互";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(InteractionCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
        [Name(Title, nameof(ExecuteInteract))]
        [XCSJ.Attributes.Icon(EIcon.Run)]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 交互信息
        /// </summary>
        [Name("交互信息")]
        public InteractInfo _interactInfo = new InteractInfo();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _interactInfo._interactor;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _interactInfo._cmdName.GetValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            _interactInfo.TryInteract(out _);
        }
    }
}
