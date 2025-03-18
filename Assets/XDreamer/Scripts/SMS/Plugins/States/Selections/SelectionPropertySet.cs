using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Selections
{
    /// <summary>
    /// 选择集属性设置:设置选择集最大个数、清除选择集等
    /// </summary>
    [ComponentMenu("选择集" + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SelectionPropertySet))]
    [Tip("设置选择集最大个数、清除选择集等", "Set the maximum number of selection sets, clear selection sets, etc")]
    public class SelectionPropertySet : BasePropertySet<SelectionPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "选择集属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("选择集", typeof(SMSManager))]
        [Name(Title, nameof(SelectionPropertySet))]
        [Tip("设置选择集最大个数、清除选择集等", "Set the maximum number of selection sets, clear selection sets, etc")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 添加
            /// </summary>
            [Name("添加")]
            Add,

            /// <summary>
            /// 移除
            /// </summary>
            [Name("移除")]
            Remove,

            /// <summary>
            /// 清空
            /// </summary>
            [Name("清空")]
            Clear,

            /// <summary>
            /// 设置
            /// </summary>
            [Name("设置")]
            Set,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name(Title, nameof(SelectionPropertySet))] 
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.Clear;

        /// <summary>
        /// 添加游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Add)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _addGO = new GameObjectPropertyValue();

        /// <summary>
        /// 删除游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Remove)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _removeGO = new GameObjectPropertyValue();

        /// <summary>
        /// 设置游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Set)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _setGO = new GameObjectPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Add:
                    {
                        Selection.AddIfNotContains(_addGO.GetValue());
                        break;
                    }
                case EPropertyName.Remove:
                    {
                        Selection.Remove(_removeGO.GetValue());
                        break;
                    }
                case EPropertyName.Clear:
                    {
                        Selection.Clear();
                        break;
                    }
                case EPropertyName.Set:
                    {
                        Selection.selection = _setGO.GetValue();
                        break;
                    }
            }
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_propertyName);
        }
    }
}
