using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginZVR.Base;
using UnityEngine.XR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginZVR.Tools
{
    /// <summary>
    /// 姿态IO通过ZVR
    /// </summary>
    [Name("姿态IO通过ZVR")]
    [Tip("通过ZVR模拟控制器姿态的输入输出", "The input and output of controller attitude are simulated by ZVR")]
    [RequireManager(typeof(ZVRManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [Tool(ZVRHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    [Owner(typeof(ZVRManager))]
    public class PoseIOByZVR : BaseAnalogControllerIO, IPoseIO, IZVRObject
    {
#if XDREAMER_ZVR

        /// <summary>
        /// ZvrGoku流客户端:用于与ZvrGoku软件进行数据流通信的ZVR流客户端
        /// </summary>
        [Name("ZvrGoku流客户端")]
        [Tip("用于与ZvrGoku软件进行数据流通信的ZVR流客户端", "ZVR stream client for data stream communication with ZvrGoku software")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ZvrGokuStreamClient _streamingClient;

        /// <summary>
        /// ZvrGoku流客户端
        /// </summary>
        public ZvrGokuStreamClient streamClient => this.XGetComponentInParentOrGlobal(ref _streamingClient, true);

#endif

        /// <summary>
        /// 拥有者
        /// </summary>
        public IZVRObjectOwner owner => this.GetZVRObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 刚体ID:用于与ZvrGoku软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与ZvrGoku软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with ZvrGoku software")]
        public int _rigidBodyID;

        /// <summary>
        /// 刚体ID
        /// </summary>
        public int rigidBodyID
        {
            get => _rigidBodyID;
            set => this.XModifyProperty(ref _rigidBodyID, value);
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
#if XDREAMER_ZVR
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
#if XDREAMER_ZVR
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
#if XDREAMER_ZVR
            if (streamClient) { }
#endif
        }
    }
}
