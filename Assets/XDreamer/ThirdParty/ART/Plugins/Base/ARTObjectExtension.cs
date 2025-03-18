using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART.Base
{
    /// <summary>
    /// ART对象扩展类
    /// </summary>
    public static class ARTObjectExtension
    {
        /// <summary>
        /// 获取ART对象拥有者
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IARTObjectOwner GetARTObjectOwner<TComponent>(this TComponent component) where TComponent : Component, IARTObject => component.GetParentOrDirectOwner<IARTObjectOwner>();

        /// <summary>
        /// 获取ART对象拥有者
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IARTObjectOwner GetARTObjectOwner(this Component component) => component.GetParentOrDirectOwner<IARTObjectOwner>();

        /// <summary>
        /// 获取ART对象拥有者
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IARTObjectOwner GetARTObjectOwner(this IARTObject obj) => GetARTObjectOwner(obj as Component);

        /// <summary>
        /// 获取ART对象拥有者游戏对象
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static GameObject GetARTObjectOwnerGameObject(this Component component)
        {
            var owner = GetARTObjectOwner(component)?.ownerGameObject;
            return owner ? owner : (component ? component.gameObject : default);
        }

        /// <summary>
        /// 获取ART对象拥有者游戏对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GameObject GetARTObjectOwnerGameObject(this IARTObject obj) => GetARTObjectOwnerGameObject(obj as Component);
    }

    /// <summary>
    /// ART对象拥有者接口
    /// </summary>
    public interface IARTObjectOwner : IComponentOwner { }

    /// <summary>
    /// ART对象：与ART通过数据类型与ID进行关联绑定的对象
    /// </summary>
    public interface IARTObject : IComponentHasOwner<IARTObjectOwner>
    {
        /// <summary>
        /// ART流客户端
        /// </summary>
        ARTStreamClient streamClient { get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        EDataType dataType { get; set; }

        /// <summary>
        /// 刚体ID:用于与ART软件进行数据流通信的刚体ID；
        /// </summary>
        int rigidBodyID { get; set; }
    }

    /// <summary>
    /// 变换通过ART
    /// </summary>
    public interface ITransformByART : IARTObject
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        Transform targetTransform { get; }

        /// <summary>
        /// 空间类型
        /// </summary>
        ESpaceType spaceType { get; }
    }

    /// <summary>
    /// ART Flystick对象
    /// </summary>
    public interface IARTFlystickObject : IARTObject
    {
        /// <summary>
        /// Flystick手柄
        /// </summary>
        EFlystick flystick { get; set; }
    }
}
