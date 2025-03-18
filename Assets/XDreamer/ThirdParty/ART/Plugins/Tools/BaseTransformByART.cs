using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginART.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 基础变换通过ART
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [RequireManager(typeof(ARTManager))]
    [Owner(typeof(ARTManager))]
    public abstract class BaseTransformByART : InteractProvider, ITransformByART, ITool
    {
        /// <summary>
        /// ART流客户端:用于与ART进行数据流通信的ART流客户端
        /// </summary>
        [Name("ART流客户端")]
        [Tip("用于与ART进行数据流通信的ART流客户端", "ART stream client for data stream communication with ART")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ARTStreamClient _streamClient;

        /// <summary>
        /// ART流客户端
        /// </summary>
        public ARTStreamClient streamClient => this.XGetComponentInParentOrGlobal(ref _streamClient, true);

        /// <summary>
        /// 目标变换
        /// </summary>
        public abstract Transform targetTransform { get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public abstract EDataType dataType { get; set; }

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
        public IARTObjectOwner owner => this.GetARTObjectOwner();

        IComponentOwner IHasOwner<IComponentOwner>.owner => owner;

        IOwner IHasOwner.owner => owner;

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
        public virtual void Update()
        {
            UpdatePose();
        }

        /// <summary>
        /// 更新姿态
        /// </summary>
        protected virtual void UpdatePose()
        {
            var rbState = _streamClient ? _streamClient.GetState(dataType, rigidBodyID) : default;
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
        }

        /// <summary>
        ///重置
        /// </summary>
        public virtual void Reset()
        {
            if (streamClient) { }
        }
    }
}
