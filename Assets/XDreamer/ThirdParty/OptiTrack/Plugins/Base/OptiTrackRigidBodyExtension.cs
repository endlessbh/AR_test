using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginOptiTrack.Base
{
    /// <summary>
    /// OptiTrack对象扩展类
    /// </summary>
    public static class OptiTrackObjectExtension
    {
        /// <summary>
        /// 获取OptiTrack对象拥有者
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IOptiTrackObjectOwner GetOptiTrackObjectOwner<TComponent>(this TComponent component) where TComponent : Component, IOptiTrackObject => component.GetParentOrDirectOwner<IOptiTrackObjectOwner>();

        /// <summary>
        /// 获取OptiTrack对象拥有者
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IOptiTrackObjectOwner GetOptiTrackObjectOwner(this Component component) => component.GetParentOrDirectOwner<IOptiTrackObjectOwner>();

        /// <summary>
        /// 获取OptiTrack对象拥有者
        /// </summary>
        /// <param name="associateToOptiTrackRigidBody"></param>
        /// <returns></returns>
        public static IOptiTrackObjectOwner GetOptiTrackObjectOwner(this IOptiTrackObject obj) => GetOptiTrackObjectOwner(obj as Component);

        /// <summary>
        /// 获取OptiTrack对象拥有者游戏对象
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static GameObject GetOptiTrackObjectOwnerGameObject(this Component component)
        {
            var owner = GetOptiTrackObjectOwner(component)?.ownerGameObject;
            return owner ? owner : (component ? component.gameObject : default);
        }

        /// <summary>
        /// 获取OptiTrack对象拥有者游戏对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static GameObject GetOptiTrackObjectOwnerGameObject(this IOptiTrackObject obj) => GetOptiTrackObjectOwnerGameObject(obj as Component);
    }

    /// <summary>
    /// OptiTrack对象拥有者接口
    /// </summary>
    public interface IOptiTrackObjectOwner : IComponentOwner { }

    /// <summary>
    /// OptiTrack对象：与OptiTrack通过对象ID进行关联绑定的对象
    /// </summary>
    public interface IOptiTrackObject : IComponentHasOwner<IOptiTrackObjectOwner>
    {
#if XDREAMER_OPTITRACK

        /// <summary>
        /// OptiTrack流客户端
        /// </summary>
        OptitrackStreamingClient streamingClient { get; }

#endif

        /// <summary>
        /// 刚体ID:用于与Motive软件进行数据流通信的刚体ID；
        /// </summary>
        int rigidBodyID { get; set; }
    }

    /// <summary>
    /// 变换通过OptiTrack
    /// </summary>
    public interface ITransformByOptiTrack : IOptiTrackObject
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
