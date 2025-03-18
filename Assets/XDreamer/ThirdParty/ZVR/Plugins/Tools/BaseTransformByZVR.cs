using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginZVR.Base;
using XCSJ.Tools;

namespace XCSJ.PluginZVR.Tools
{
    /// <summary>
    /// 基础变换通过ZVR
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [RequireManager(typeof(ZVRManager))]
    [Owner(typeof(ZVRManager))]
    public abstract class BaseTransformByZVR : InteractProvider, ITransformByZVR, ITool
    {
#if XDREAMER_ZVR

        /// <summary>
        /// ZvrGoku流客户端:用于与ZvrGoku软件进行数据流通信的ZVR流客户端
        /// </summary>
        [Name("ZvrGoku流客户端")]
        [Tip("用于与ZvrGoku软件进行数据流通信的ZVR流客户端", "ZVR stream client for data stream communication with ZvrGoku software")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ZvrGokuStreamClient _streamClient;

        /// <summary>
        /// ZvrGoku流客户端
        /// </summary>
        public ZvrGokuStreamClient streamClient => this.XGetComponentInParentOrGlobal(ref _streamClient, true);

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
        public IZVRObjectOwner owner => this.GetZVRObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_ZVR
            if (enabled && !streamClient)
            {
                var mainType = typeof(ZvrGokuStreamClient);
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
#if XDREAMER_ZVR
            var rbState = _streamClient ? _streamClient.GetLatestRigidBodyState(rigidBodyID) : default;
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
#if XDREAMER_ZVR
            if (streamClient) { }
#endif
        }
    }
}
