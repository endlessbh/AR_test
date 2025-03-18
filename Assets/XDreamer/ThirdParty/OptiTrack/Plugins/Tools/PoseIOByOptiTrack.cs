using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginOptiTrack.Base;
using UnityEngine.XR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginOptiTrack.Tools
{
    /// <summary>
    /// 姿态IO通过OptiTrack
    /// </summary>
    [Name("姿态IO通过OptiTrack")]
    [Tip("通过OptiTrack模拟控制器姿态的输入输出", "Simulate the input and output of the controller attitude through OptiTrack")]
    [RequireManager(typeof(OptiTrackManager),typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [Tool(OptiTrackHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    [Owner(typeof(OptiTrackManager))]
    public class PoseIOByOptiTrack : BaseAnalogControllerIO, IPoseIO, IOptiTrackObject
    {
#if XDREAMER_OPTITRACK

        /// <summary>
        /// OptiTrack流客户端:用于与Motive软件进行数据流通信的OptiTrack流客户端
        /// </summary>
        [Name("OptiTrack流客户端")]
        [Tip("用于与Motive软件进行数据流通信的OptiTrack流客户端", "OptiTrack stream client for data stream communication with Motive software")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public OptitrackStreamingClient _streamingClient;

        /// <summary>
        /// OptiTrack流客户端
        /// </summary>
        public OptitrackStreamingClient streamingClient => this.XGetComponentInParentOrGlobal(ref _streamingClient, true);

#endif

        /// <summary>
        /// 拥有者
        /// </summary>
        public IOptiTrackObjectOwner owner => this.GetOptiTrackObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 刚体ID:用于与Motive软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与Motive软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with Motive software")]
        public int _rigidBodyID;

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID
        {
            get => _rigidBodyID;
            set => this.XModifyProperty(ref _rigidBodyID, value);
        }

        protected virtual void Start()
        {
#if XDREAMER_OPTITRACK
            if (!streamingClient)
            {
                var mainType = typeof(OptitrackStreamingClient);
                var type = GetType();
                Debug.LogWarningFormat("游戏对象[{0}]及其父级、全局游戏对象上未找到[{1}]({2})类型的组件，导致组件该游戏对象上的组件[{3}]({4})功能失效！",
                   CommonFun.GameObjectToString(gameObject),
                   CommonFun.Name(mainType),
                   mainType.FullName,
                   CommonFun.Name(type),
                   type.FullName);
            }
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
#if XDREAMER_OPTITRACK
            var rbState = _streamingClient ? _streamingClient.GetLatestRigidBodyState(rigidBodyID) : default;
            if (rbState != null)
            {

                position = rbState.Pose.Position;
                rotation = rbState.Pose.Orientation;
                return InputTrackingState.Position | InputTrackingState.Rotation;
            }
#endif
            position = default;
            rotation = default;
            return InputTrackingState.None;
        }

#else

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public PoseDataFlags TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
#if XDREAMER_OPTITRACK
            var rbState = _streamingClient ? _streamingClient.GetLatestRigidBodyState(rigidBodyID) : default;
            if (rbState != null)
            {

                position = rbState.Pose.Position;
                rotation = rbState.Pose.Orientation;
                return PoseDataFlags.Position | PoseDataFlags.Rotation;
            }
#endif
            position = default;
            rotation = default;
            return PoseDataFlags.NoData;
        }

#endif

#endif

        /// <summary>
        ///重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
#if XDREAMER_OPTITRACK
            if (streamingClient) { }
#endif
        }
    }
}
