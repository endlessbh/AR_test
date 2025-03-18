using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 基础子控制器；基于主控制器做功能扩展的基础控制器；
    /// </summary>
    public abstract class BaseSubController : BaseController, ISubController
    {
        /// <summary>
        /// 基础主控制器
        /// </summary>
        public abstract BaseMainController baseMainController { get; }

        IMainController ISubController.mainController => baseMainController;
    }

    /// <summary>
    /// 子控制器
    /// </summary>
    public interface ISubController : IController
    {
        /// <summary>
        /// 主控制器
        /// </summary>
        IMainController mainController { get; }
    }

    /// <summary>
    /// 子控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISubController<T> : ISubController
        where T : class, IMainController
    {
        /// <summary>
        /// 主控制器
        /// </summary>
        new T mainController { get; }
    }

    /// <summary>
    /// 基础子控制器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSubController<T> : BaseSubController, ISubController<T>
        where T : BaseMainController
    {
        /// <summary>
        /// 基础主控制器
        /// </summary>
        public override BaseMainController baseMainController => mainController;

        /// <summary>
        /// 主控制器
        /// </summary>
        [Name("主控制器")]
        [Tip("如当前参数无效，在组件启用时会从当前游戏对象以及父级游戏对象上查找本参数类型的组件", "If the current parameter is invalid, the component of this parameter type will be found from the current game object and the parent game object when the component is enabled")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public T _mainController;

        /// <summary>
        /// 主控制器
        /// </summary>
        public T mainController => this.XGetComponentInParent(ref _mainController);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!mainController)
            {
                var mainType = typeof(T);
                var type = GetType();

                Debug.LogWarningFormat("游戏对象[{0}]及其父级游戏对象上未找到[{1}]({2})类型的组件，导致组件该游戏对象上的组件[{3}]({4})禁止启用！",
                    CommonFun.GameObjectToString(gameObject),
                    CommonFun.Name(mainType),
                    mainType.FullName,
                    CommonFun.Name(type),
                    type.FullName);

                enabled = false;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (!mainController)
            {
                var mainType = typeof(T);

                Debug.LogWarningFormat("游戏对象[{0}]及其父级游戏对象上未找到[{1}]({2})类型的组件",
                    CommonFun.GameObjectToString(gameObject),
                    CommonFun.Name(mainType),
                    mainType.FullName);
            }
        }
    }
}