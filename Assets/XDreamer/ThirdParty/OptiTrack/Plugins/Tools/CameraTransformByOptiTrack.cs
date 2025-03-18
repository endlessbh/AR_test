using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginOptiTrack.Base;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginOptiTrack.Tools
{
    /// <summary>
    /// 相机变换通过OptiTrack：通过与Motive软件进行数据流通信，控制相机变换的姿态（位置与旋转）
    /// </summary>
    [Name("相机变换通过OptiTrack")]
    [Tip("通过与Motive软件进行数据流通信，控制相机变换的姿态（位置与旋转）", "Control the attitude (position and rotation) of the camera through data flow communication with Motive software")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraTransformer))]
    [Tool(OptiTrackHelper.Title)]
    [RequireManager(typeof(OptiTrackManager), typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [DisallowMultipleComponent]
    [Owner(typeof(OptiTrackManager))]
    public class CameraTransformByOptiTrack : BaseCameraTransformController, ITransformByOptiTrack
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
        /// 目标变换
        /// </summary>
        public Transform targetTransform => cameraTransformer.mainTransform;

        /// <summary>
        /// 刚体ID:用于与Motive软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与Motive软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with Motive software")]
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

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 空间类型
        /// </summary>
        public ESpaceType spaceType => _spaceType;

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
        protected virtual void Update()
        {
            UpdatePose();
        }

        void UpdatePose()
        {
#if XDREAMER_OPTITRACK
            var rbState = _streamingClient ? _streamingClient.GetLatestRigidBodyState(rigidBodyID) : default;
            if (rbState != null)
            {
                cameraTransformer.SetTransform(_spaceType, rbState.Pose.Position, rbState.Pose.Orientation);
            }
#endif
        }

        /// <summary>
        ///重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

#if XDREAMER_OPTITRACK
            if (streamingClient) { }
#endif

            _enableRuleByRuntimePlatform.Reset(EBool.Yes);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.Android, EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.IPhonePlayer, EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.WebGLPlayer, EBool.No);
        }
    }
}
