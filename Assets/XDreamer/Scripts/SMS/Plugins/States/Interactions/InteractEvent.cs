using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Interactions
{
    /// <summary>
    /// 交互事件
    /// </summary>
    [ComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
    [Name(Title, nameof(InteractEvent))]
    [Tip("用于监听交互事件的触发器", "Trigger for listening to interact event")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class InteractEvent : Trigger<InteractEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(InteractionCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
        [Name(Title, nameof(InteractEvent))]
        [XCSJ.Attributes.Icon(EIcon.Event)]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 交互比较器
        /// </summary>
        [Name("交互比较器")]
        public InteractComparer _interactComparer = new InteractComparer();

        /// <summary>
        /// 命令名称存储变量
        /// </summary>
        [Name("命令名称存储变量")]
        [Tip("成功匹配条件，组件处于完成态才进行赋值操作", "The condition is successfully matched, and the assignment can be performed only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdNameVariable = "";

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractEntry += CheckInteractState;
            InteractObject.onInteractProcessing += CheckInteractState;
            InteractObject.onInteractFinished += CheckInteractState;
            InteractObject.onInteractAborted += CheckInteractState;
            InteractObject.onInteractExit += CheckInteractState;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            InteractObject.onInteractEntry -= CheckInteractState;
            InteractObject.onInteractProcessing -= CheckInteractState;
            InteractObject.onInteractFinished -= CheckInteractState;
            InteractObject.onInteractAborted -= CheckInteractState;
            InteractObject.onInteractExit -= CheckInteractState;
        }

        private void CheckInteractState(InteractObject interactor, InteractData interactData)
        {
            if (!finished && _interactComparer.IsMatch(interactData))
            {
                finished = true;
                var instance = ScriptManager.instance;
                if (instance)
                {
                    instance.TrySetHierarchyVarValue(_cmdNameVariable, interactData.cmdName);
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _interactComparer.DataValidity();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_interactComparer._matchRule).Replace("/", "");
    }
}
