using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础速度阻尼控制器
    /// </summary>
    [Name("基础速度阻尼控制器")]
    public abstract class BaseSpeedDampingController : BaseSpeedController, ICameraDamping
    {
        #region 阻尼

        /// <summary>
        /// 使用阻尼
        /// </summary>
        [Group("阻尼信息", defaultIsExpanded = false)]
        [Name("使用阻尼")]
        public bool _useDamping = false;

        /// <summary>
        /// 使用阻尼
        /// </summary>
        public bool useDamping
        {
            get => _useDamping;
            set => this.XModifyProperty(ref _useDamping, value);
        }

        /// <summary>
        /// 在阻尼中:是否正在阻尼处理过程中的标记量
        /// </summary>
        [Name("在阻尼中")]
        [Readonly]
        public bool _inDamping = false;

        /// <summary>
        /// 阻尼信息
        /// </summary>
        [Name("阻尼信息")]
        [HideInSuperInspector(nameof(_useDamping), EValidityCheckType.False)]
        [EndGroup(true)]
        public UpdateFloatRuntimePlatformInfo _dampingInfo = new UpdateFloatRuntimePlatformInfo();

        /// <summary>
        /// 阻尼系数
        /// </summary>
        public virtual float dampingCoefficient => 1;

        #endregion

        /// <summary>
        /// 处理阻尼
        /// </summary>
        protected abstract void HandleDamping();

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _dampingInfo.Init();
        }

        /// <summary>
        /// 延后更新：主要用于处理阻尼；
        /// </summary>
        public virtual void LateUpdate() => HandleDamping();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _dampingInfo.Reset(3);
            _dampingInfo.Reset(Application.platform);
        }
    }
}
