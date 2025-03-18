using System;
using System.Linq;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 基础主控制器：在当前游戏对象上各层级中唯一存在的控制器，即在子父层级中当前类型的组件有且最对允许出现一次；
    /// </summary>
    public abstract class BaseMainController : BaseController, IMainController, IOnEnable
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!this.IsUniqueInHierachy(out Component conflictComponent))
            {
                var type = GetType();
                Debug.LogErrorFormat("在父级游戏对象[{0}]上已经存在[{1}]({2})类型的组件，当前游戏对象[{3}]该类型组件禁止启用！",
                    CommonFun.GameObjectToString(conflictComponent.gameObject),
                    CommonFun.Name(type),
                    type.FullName,
                    CommonFun.GameObjectToString(gameObject));

                enabled = false;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (!this.IsUniqueInHierachy(out Component conflictComponent))
            {
                var type = GetType();
                Debug.LogErrorFormat("在父级游戏对象[{0}]上已经存在[{1}]({2})类型的组件，当前游戏对象[{3}]无法添加该类型组件！",
                    CommonFun.GameObjectToString(conflictComponent.gameObject),
                    CommonFun.Name(type),
                    type.FullName,
                    CommonFun.GameObjectToString(gameObject));

                DestroyImmediate(this);
            }
        }
    }

    /// <summary>
    /// 主控制器
    /// </summary>
    public interface IMainController : IController { }

    /// <summary>
    /// 组件扩展
    /// </summary>
    public static class ComponentExtension
    {
        /// <summary>
        /// 判断传入参数类型的组件在层级上是否唯一
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool IsUniqueInHierachy(this Component component) => IsUniqueInHierachy(component, out _);

        /// <summary>
        /// 判断传入参数类型的组件在层级上是否唯一
        /// </summary>
        /// <param name="component"></param>
        /// <param name="conflictComponent">冲突组件：父级上存在的与传入参数类型相同的组件对象；返回值为False时，本输出参数有效；</param>
        /// <returns></returns>
        public static bool IsUniqueInHierachy(this Component component, out Component conflictComponent)
        {
            if (!component) throw new ArgumentNullException(nameof(component));
            var components = component.GetComponentsInParent(component.GetType(), true);

            Component retComponent = null;
            if (components.Any(c => {

                if (c && c != component)
                {
                    retComponent = c;
                    return true;
                }
                return false;
            }))
            {
                //有组件冲突
                conflictComponent = retComponent;
                return false;
            }

            //无组件冲突
            conflictComponent = retComponent;
            return true;
        }
    }
}
