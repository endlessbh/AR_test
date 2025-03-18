using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART.Base;
using UnityEngine.XR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 姿态IO通过ART
    /// </summary>
    [Name("姿态IO通过ART")]
    [Tip("通过ART模拟控制器姿态的输入输出", "Simulate the input and output of the controller attitude through ART")]
    [RequireManager(typeof(ARTManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [Tool(ARTHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    [Owner(typeof(ARTManager))]
    public class PoseIOByART : BaseAnalogControllerIO, IPoseIO, IARTObject
    {
        /// <summary>
        /// ART流客户端:用于与ART软件进行数据流通信的ART流客户端
        /// </summary>
        [Name("ART流客户端")]
        [Tip("用于与ART软件进行数据流通信的ART流客户端", "ART stream client for data stream communication with ART software")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ARTStreamClient _streamClient;

        /// <summary>
        /// ARTGoku流客户端
        /// </summary>
        public ARTStreamClient streamClient => this.XGetComponentInParentOrGlobal(ref _streamClient, true);

        /// <summary>
        /// 拥有者
        /// </summary>
        public IARTObjectOwner owner => this.GetARTObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 数据类型
        /// </summary>
        [Name("数据类型")]
        [EnumPopup]
        public EDataType _dataType = EDataType.Body;

        /// <summary>
        /// 数据类型
        /// </summary>
        public EDataType dataType
        {
            get => _dataType;
            set => this.XModifyProperty(ref _dataType, value);
        }

        /// <summary>
        /// 刚体ID:用于与ART软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与ART软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with ART software")]
        [Range(0, 10)]
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
            var rbState = _streamClient ? _streamClient.GetState(dataType, rigidBodyID) : default;
            if (rbState != null)
            {
                position = rbState.Pose.Position;
                rotation = rbState.Pose.Orientation;
                return InputTrackingState.Position | InputTrackingState.Rotation;
            }
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
            var rbState = _streamClient ? _streamClient.GetState(dataType, rigidBodyID) : default;
            if (rbState != null)
            {
                position = rbState.Pose.Position;
                rotation = rbState.Pose.Orientation;
                return PoseDataFlags.Position | PoseDataFlags.Rotation;
            }
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
            if (streamClient) { }
        }
    }
}
