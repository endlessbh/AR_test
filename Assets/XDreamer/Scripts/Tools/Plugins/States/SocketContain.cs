using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.States
{
    /// <summary>
    /// 插槽包含比较
    /// </summary>
    [ComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
    [Name(Title, nameof(SocketContain))]
    [Tip("用于监听插件交互事件的触发器", "Trigger for listening to socket interact event")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class SocketContain : Trigger<SocketContain>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "插槽包含比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(InteractionCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
        [Name(Title, nameof(SocketContain))]
        [XCSJ.Attributes.Icon(EIcon.Event)]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 插槽交互器
        /// </summary>
        [Name("插槽交互器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseSocket _baseSocket;

        /// <summary>
        /// 插槽包含规则
        /// </summary>
        [Name("插槽包含规则")]
        [EnumPopup]
        public ESocketContainRule _socketContainRule = ESocketContainRule.Full;

        /// <summary>
        /// 可抓对象列表
        /// </summary>
        [Name("可抓对象列表")]
        [HideInSuperInspector(nameof(_socketContainRule), EValidityCheckType.NotEqual, ESocketContainRule.ContainGrabbable)]
        public Grabbable _grabbable;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractFinished += OnInteractFinished;

            Check();
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            InteractObject.onInteractFinished += OnInteractFinished;
        }

        public void OnInteractFinished(InteractObject interactObject, InteractData interactData)
        {
            if (interactObject == _baseSocket)
            {
                Check();
            }
        }

        private void Check()
        {
            if (finished) return;

            switch (_socketContainRule)
            {
                case ESocketContainRule.Full:
                    {
                        finished = _baseSocket.full;
                        break;
                    }
                case ESocketContainRule.ContainGrabbable:
                    {
                        finished = _baseSocket.Contains(_grabbable);
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _baseSocket;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_socketContainRule);
    }

    /// <summary>
    /// 插槽包含规则
    /// </summary>
    [Name("插槽包含规则")]
    public enum ESocketContainRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 插槽装满:插槽包含可抓对象，并且数量达到最大值
        /// </summary>
        [Name("插槽装满")]
        Full,

        /// <summary>
        /// 插槽包含可抓对象
        /// </summary>
        [Name("插槽包含可抓对象")]
        ContainGrabbable,
    }
}

