using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginZVR.Base
{
    /// <summary>
    /// ZVR对象扩展类
    /// </summary>
    public static class ZVRObjectExtension
    {
        /// <summary>
        /// 获取ZVR对象拥有者
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IZVRObjectOwner GetZVRObjectOwner<TComponent>(this TComponent component) where TComponent : Component, IZVRObject => component.GetParentOrDirectOwner<IZVRObjectOwner>();

        /// <summary>
        /// 获取ZVR对象拥有者
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IZVRObjectOwner GetZVRObjectOwner(this Component component) => component.GetParentOrDirectOwner<IZVRObjectOwner>();

        /// <summary>
        /// 获取ZVR对象拥有者
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IZVRObjectOwner GetZVRObjectOwner(this IZVRObject obj) => GetZVRObjectOwner(obj as Component);

        /// <summary>
        /// 获取ZVR对象拥有者游戏对象
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static GameObject GetZVRObjectOwnerGameObject(this Component component)
        {
            var owner = GetZVRObjectOwner(component)?.ownerGameObject;
            return owner ? owner : (component ? component.gameObject : default);
        }

        /// <summary>
        /// 获取ZVR对象拥有者游戏对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GameObject GetZVRObjectOwnerGameObject(this IZVRObject obj) => GetZVRObjectOwnerGameObject(obj as Component);
    }

    /// <summary>
    /// ZVR对象拥有者接口
    /// </summary>
    public interface IZVRObjectOwner : IComponentOwner { }

    /// <summary>
    /// ZVR对象：与ZvrGoku通过刚体ID进行关联绑定的对象
    /// </summary>
    public interface IZVRObject : IComponentHasOwner<IZVRObjectOwner>
    {
#if XDREAMER_ZVR

        /// <summary>
        /// ZvrGoku流客户端
        /// </summary>
        ZvrGokuStreamClient streamClient { get; }

#endif

        /// <summary>
        /// 刚体ID:用于与ZvrGoku软件进行数据流通信的刚体ID；
        /// </summary>
        int rigidBodyID { get; set; }
    }

    /// <summary>
    /// 变换通过ZVR
    /// </summary>
    public interface ITransformByZVR : IZVRObject
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
}
