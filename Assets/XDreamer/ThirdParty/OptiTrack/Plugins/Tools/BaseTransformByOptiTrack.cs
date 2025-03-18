using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginOptiTrack.Base;
using XCSJ.Tools;

namespace XCSJ.PluginOptiTrack.Tools
{
    /// <summary>
    /// 基础变换通过OptiTrack
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [RequireManager(typeof(OptiTrackManager))]
    [Owner(typeof(OptiTrackManager))]
    public abstract class BaseTransformByOptiTrack : InteractProvider, ITransformByOptiTrack, ITool
    {
#if XDREAMER_OPTITRACK

        /// <summary>
        /// OptiTrack流客户端:用于与Motive软件进行数据流通信的OptiTrack流客户端
        /// </summary>
        [Name("OptiTrack流客户端")]
        [Tip("用于与Motive软件进行数据流通信的OptiTrack流客户端", "Optitrack stream client for data stream communication with Motive software")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public OptitrackStreamingClient _streamingClient;

        /// <summary>
        /// OptiTrack流客户端
        /// </summary>
        public OptitrackStreamingClient streamingClient => this.XGetComponentInParentOrGlobal(ref _streamingClient, true);
#endif

        /// <summary>
        /// 目标变换
        /// </summary>
        public abstract Transform targetTransform { get; }

        /// <summary>
        /// 刚体ID
        /// </summary>
        public abstract int rigidBodyID { get; set; }

        /// <summary>
        /// 空间类型
        /// </summary>
        public abstract ESpaceType spaceType { get; }

        /// <summary>
        /// 拥有者
        /// </summary>
        public IOptiTrackObjectOwner owner => this.GetOptiTrackObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_OPTITRACK
            if (enabled && !streamingClient)
            {
                var mainType = typeof(OptitrackStreamingClient);
                var type = GetType();
                Debug.LogWarningFormat("游戏对象[{0}]及其父级、全局游戏对象上未找到[{1}]({2})类型的组件，导致组件该游戏对象上的组件[{3}]({4})禁止启用！",
                   CommonFun.GameObjectToString(gameObject),
                   CommonFun.Name(mainType),
                   mainType.FullName,
                   CommonFun.Name(type),
                   type.FullName);

                enabled = false;
            }
#endif
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            UpdatePose();
        }

        /// <summary>
        /// 更新姿态
        /// </summary>
        protected virtual void UpdatePose()
        {
#if XDREAMER_OPTITRACK
            var rbState = _streamingClient ? _streamingClient.GetLatestRigidBodyState(rigidBodyID) : default;
            if (rbState != null)
            {
                switch (spaceType)
                {
                    case ESpaceType.Local:
                        {
                            var targetTransform = this.targetTransform;
                            targetTransform.localPosition = rbState.Pose.Position;
                            targetTransform.localRotation = rbState.Pose.Orientation;
                            break;
                        }
                    case ESpaceType.World:
                        {
                            var targetTransform = this.targetTransform;
                            targetTransform.position = rbState.Pose.Position;
                            targetTransform.rotation = rbState.Pose.Orientation;
                            break;
                        }
                }
            }
#endif
        }

        /// <summary>
        ///重置
        /// </summary>
        public virtual void Reset()
        {
#if XDREAMER_OPTITRACK
            if (streamingClient) { }
#endif
        }
    }
}
