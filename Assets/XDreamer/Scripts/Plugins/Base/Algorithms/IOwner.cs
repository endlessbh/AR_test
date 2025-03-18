using System.Linq;
using UnityEngine;
using XCSJ.Collections;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 拥有者接口
    /// </summary>
    public interface IOwner { }

    /// <summary>
    /// 组件拥有者：用于修饰组件<see cref="Component"/>子类型的拥有者接口
    /// </summary>
    public interface IComponentOwner : IOwner
    {
        /// <summary>
        /// 组件拥有者的游戏对象
        /// </summary>
        GameObject ownerGameObject { get; }
    }

    /// <summary>
    /// 拥有者扩展类
    /// </summary>
    public static class OwnerExtension
    {
        /// <summary>
        /// 获取根拥有者:即最顶层的拥有者；
        /// </summary>
        /// <typeparam name="TOwner">拥有者接口限定</typeparam>
        /// <param name="component">组件：根拥有者可能为本对象</param>
        /// <returns>根拥有者可能与父级拥有者是同一对象；也可能与直属拥有者是同一对象；</returns>
        public static TOwner GetRootOwner<TOwner>(this Component component) where TOwner : IComponentOwner
        {
            if (!component) return default;
            var owners = component.GetComponentsInParent<TOwner>(true);
            return owners.Length > 0 ? owners[owners.Length - 1] : default;
        }

        /// <summary>
        /// 获取父级拥有者:从组件所在游戏对象以及父级游戏对象上查找第一个不为组件对象且符合拥有者接口限定的有效组件对象；父级拥有者不可能为组件对象；
        /// </summary>
        /// <typeparam name="TOwner">拥有者接口限定</typeparam>
        /// <param name="component">组件：父级拥有者不可能为本组件对象</param>
        /// <returns>父级拥有者可能为null</returns>
        public static TOwner GetParentOwner<TOwner>(this Component component) where TOwner : IComponentOwner
        {
            if (!component) return default;
            var owners = component.GetComponentsInParent<TOwner>(true);
            return owners.FirstOrDefault(owner => (owner as Component) != component);
        }

        /// <summary>
        /// 获取直属拥有者:判断传入组件对象是否继承了拥有者接口限定,即直属拥有者只能为组件对象；
        /// </summary>
        /// <typeparam name="TOwner">拥有者接口限定</typeparam>
        /// <param name="component">组件：直属拥有者只能为本对象</param>
        /// <returns>如果组件不继承拥有者接口则返回null；否则返回组件对象；</returns>
        public static TOwner GetDirectOwner<TOwner>(this Component component) where TOwner : IComponentOwner
        {
            return component is TOwner owner ? owner : default;
        }

        /// <summary>
        /// 获取直属或父级拥有者：如果存在直属拥有者，则返回直属拥有者；否则返回父级拥有者；
        /// </summary>
        /// <typeparam name="TOwner">拥有者接口限定</typeparam>
        /// <param name="component">组件：如果本组件继承了拥有者接口限定则直接返回本组件对象；否者返回从组件所在游戏对象以及父级游戏对象上查找第一个符合限定接口类型的组件对象；</param>
        /// <returns></returns>
        public static TOwner GetDirectOrParentOwner<TOwner>(this Component component) where TOwner : IComponentOwner
        {
            if (!component) return default;
            if (component is TOwner owner) return owner;
            var owners = component.GetComponentsInParent<TOwner>(true);
            return owners.FirstOrDefault();
        }

        /// <summary>
        /// 获取父级或直属拥有者：如果存在父级拥有者，则返回父级拥有者；否则返回直属拥有者；
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <typeparam name="TOwner">拥有者接口限定</typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static TOwner GetParentOrDirectOwner<TOwner>(this Component component) where TOwner : IComponentOwner
        {
            if (!component) return default;
            var owners = component.GetComponentsInParent<TOwner>(true);
            return owners.FirstOrNew(co => (co as Component) != component, () => component is TOwner owner ? owner : default);
        }
    }

    /// <summary>
    /// 有拥有者接口
    /// </summary>
    public interface IHasOwner
    {
        /// <summary>
        /// 拥有者
        /// </summary>
        IOwner owner { get; }
    }

    /// <summary>
    /// 有拥有者泛型接口
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    public interface IHasOwner<TOwner> : IHasOwner
        where TOwner : IOwner
    {
        /// <summary>
        /// 拥有者
        /// </summary>
        new TOwner owner { get; }
    }

    /// <summary>
    /// 组件有拥有者接口
    /// </summary>
    public interface IComponentHasOwner : IHasOwner<IComponentOwner> { }

    /// <summary>
    /// 组件有拥有者泛型接口
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    public interface IComponentHasOwner<TOwner> : IComponentHasOwner
        where TOwner : IComponentOwner
    {
        /// <summary>
        /// 拥有者
        /// </summary>
        new TOwner owner { get; }
    }
}
