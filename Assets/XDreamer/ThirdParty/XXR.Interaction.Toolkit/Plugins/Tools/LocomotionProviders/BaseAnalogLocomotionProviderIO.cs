using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
    /// <summary>
    /// 基础模拟运动提供者IO
    /// </summary>
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    public abstract class BaseAnalogLocomotionProviderIO : InteractProvider, IAnalogLocomotionProviderIO
    {
        /// <summary>
        /// 提供者
        /// </summary>
        [Name("提供者")]
        public AnalogLocomotionProvider _locomotionProvider;

        /// <summary>
        /// 提供者
        /// </summary>
        public AnalogLocomotionProvider locomotionProvider => this.XGetComponentInParent(ref _locomotionProvider);

        private AnalogLocomotionProvider registedProvider ;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Regist();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Unregist();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (locomotionProvider) { }
        }

        private void Regist()
        {
            var locomotionProvider = this.locomotionProvider;
            if (registedProvider == locomotionProvider) return;
            Unregist();
            if (locomotionProvider && locomotionProvider.RegistIO(this))
            {
                registedProvider = locomotionProvider;
            }
        }

        private void Unregist()
        {
            if (registedProvider)
            {
                registedProvider.UnregistIO(this);
                registedProvider = null;
            }
        }

        /// <summary>
        /// 更新输入输出
        /// </summary>
        /// <param name="analogLocomotionProvider"></param>
        public abstract void UpdateIO(AnalogLocomotionProvider analogLocomotionProvider);
    }

    /// <summary>
    /// 模拟运动提供者IO接口
    /// </summary>
    public interface IAnalogLocomotionProviderIO
    {
        /// <summary>
        /// 更新输入输出
        /// </summary>
        /// <param name="analogLocomotionProvider"></param>
        void UpdateIO(AnalogLocomotionProvider analogLocomotionProvider);
    }
}
