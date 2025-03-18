using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginART.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 相机变换通过ART：通过与ART软件进行数据流通信，控制相机变换的姿态（位置与旋转）
    /// </summary>
    [Name("相机变换通过ART")]
    [Tip("通过与ART软件进行数据流通信，控制相机变换的姿态（位置与旋转）", "Through data flow communication with ART software, the attitude (position and rotation) of the camera is controlled")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraTransformer))]
    [Tool(ARTHelper.Title)]
    [RequireManager(typeof(ARTManager), typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [DisallowMultipleComponent]
    [Owner(typeof(ARTManager))]
    public class CameraTransformByART : BaseCameraTransformController, ITransformByART
    {
        /// <summary>
        /// ART流客户端:用于与ART进行数据流通信的ART流客户端
        /// </summary>
        [Name("ART流客户端")]
        [Tip("用于与ART进行数据流通信的ART流客户端", "ART stream client for data stream communication with art")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ARTStreamClient _streamClient;

        /// <summary>
        /// ART流客户端
        /// </summary>
        public ARTStreamClient streamClient => this.XGetComponentInParentOrGlobal(ref _streamClient, true);

        /// <summary>
        /// 拥有者
        /// </summary>
        public IARTObjectOwner owner => this.GetARTObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform => cameraTransformer.mainTransform;

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

            if (enabled && !streamClient)
            {
                var mainType = typeof(ARTStreamClient);
                var type = GetType();
                Debug.LogWarningFormat("游戏对象[{0}]及其父级、全局游戏对象上未找到[{1}]({2})类型的组件，导致组件该游戏对象上的组件[{3}]({4})禁止启用！",
                   CommonFun.GameObjectToString(gameObject),
                   CommonFun.Name(mainType),
                   mainType.FullName,
                   CommonFun.Name(type),
                   type.FullName);

                enabled = false;
            }
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
            var rbState = _streamClient ? _streamClient.GetState(dataType, rigidBodyID) : default;
            if (rbState != null)
            {
                cameraTransformer.SetTransform(_spaceType, rbState.Pose.Position, rbState.Pose.Orientation);
            }
        }

        /// <summary>
        ///重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (streamClient) { }

            _enableRuleByRuntimePlatform.Reset(EBool.Yes);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.Android, EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.IPhonePlayer, EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.WebGLPlayer, EBool.No);
        }
    }
}
