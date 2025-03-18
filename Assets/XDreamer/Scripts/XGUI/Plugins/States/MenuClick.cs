using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.States
{
    /// <summary>
    /// 菜单点击
    /// </summary>
    [ComponentMenu("XGUI/" + Title, typeof(XGUIManager))]
    [Name(Title, nameof(MenuClick))]
    [Tip("用于监听弹出菜单点击事件的触发器", "Trigger for listening to pop-up menu click event")]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    [Owner(typeof(XGUIManager))]
    public class MenuClick : Trigger<MenuClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "菜单点击";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("XGUI", typeof(XGUIManager))]
        [StateComponentMenu("XGUI/" + Title, typeof(XGUIManager))]
        [Name(Title, nameof(MenuClick))]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        public static State CreateButtonClick(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 使用全局菜单
        /// </summary>
        [Name("使用全局菜单")]
        public bool _useGlobalMenu = true;

        /// <summary>
        /// 弹出菜单控制器
        /// </summary>
        [Name("弹出菜单控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_useGlobalMenu), EValidityCheckType.True)]
        public PopupMenu _menuController;

        /// <summary>
        /// 关联菜单控制器对象
        /// </summary>
        public PopupMenu menu => _useGlobalMenu ? XGUIHelper.popupMenu : _menuController;

        /// <summary>
        /// 交互比较器
        /// </summary>
        [Name("交互比较器")]
        public InteractComparer _interactComparer = new InteractComparer();

        /// <summary>
        /// 点击菜单名称存储变量
        /// </summary>
        [Name("点击菜单名称存储变量")]
        [Tip("成功匹配条件，组件处于完成态才进行赋值操作", "The condition is successfully matched, and the assignment can be performed only when the component is in the completed state")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _clickMenuNameVariable = "";

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            MenuItemView.onMenuClick += OnMenuClick;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            MenuItemView.onMenuClick -= OnMenuClick;
        }

        /// <summary>
        /// 菜单按钮点击回调函数
        /// </summary>
        /// <param name="contentMenuItemView"></param>
        /// <param name="popupMenuViewController"></param>
        /// <param name="menuName"></param>
        /// <param name="interactData"></param>
        private void OnMenuClick(MenuItemView contentMenuItemView, PopupMenu popupMenuViewController, string menuName, InteractData interactData)
        {
            if (menu == popupMenuViewController)
            {
                if (_interactComparer.IsMatch(interactData))
                {
                    var instance = ScriptManager.instance;
                    if (instance)
                    {
                        instance.TrySetHierarchyVarValue(_clickMenuNameVariable, menuName);
                    }
                    finished = true;
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _interactComparer.DataValidity() && menu;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_interactComparer._matchRule).Replace("/","");
    }
}
