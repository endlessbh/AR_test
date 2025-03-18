using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.Base
{
    /// <summary>
    /// 跳过状态；可用于跳过当前跳转的入状态并将当前跳转的出状态激活
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Goto)]
    [Name("跳过状态", nameof(SkipState))]
    [Tip("可用于跳过当前跳转的入状态并将当前跳转的出状态激活", "It can be used to skip the in state of the current jump and activate the out state of the current jump")]
    [RequireManager(typeof(SMSManager))]
    public abstract class SkipState : TransitionComponent
    {
        /// <summary>
        /// 可自动完成
        /// </summary>
        [Name("可自动完成")]
        [Tip("为True时，本跳转组件一直处于完成态，即入状态完成后本跳转组件标识可通过（即完成态）；为False时，则跳转组件的完成态等同于默认完成态标记量的状态；", "When it is true, the jump component is always in the completed state, that is, after the entry state is completed, the jump component identification can pass (that is, the completed state); When it is false, the completion status of the jump component is equal to the status of the default completion status flag quantity;")]
        public bool canAutoFinished = true;

        /// <summary>
        /// 将要跳过入状态时的回调
        /// </summary>
        public event Action<SkipState, StateData> onWillSkipState;

        /// <summary>
        /// 跳过入状态后的回调
        /// </summary>
        public event Action<SkipState, StateData> onSkipState;

        /// <summary>
        /// 将要跳过入状态时的回调
        /// </summary>
        /// <param name="stateData"></param>
        protected virtual void OnWillSkipState(StateData stateData) { }

        /// <summary>
        /// 判断能否跳过；如可跳过返回True，否则返回False；
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        protected abstract bool CanSkip(StateData stateData);

        /// <summary>
        /// 尝试跳过；如成功跳过返回True，否则返回False；
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        protected virtual bool TrySkip(StateData stateData)
        {
            if (CanSkip(stateData))
            {
                Skip(stateData);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 跳过入状态，激活出状态
        /// </summary>
        /// <param name="stateData"></param>
        protected void Skip(StateData stateData)
        {
            var transition = this.parent;

            //入状态处于繁忙状态时才可以跳过
            if (transition.inState.busy)
            {
                onWillSkipState?.Invoke(this, stateData);
                OnWillSkipState(stateData);

                SkipHelper.Skip(stateData, transition);

                onSkipState?.Invoke(this, stateData);
            }
        }

        /// <summary>
        /// 更新时回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            TrySkip(stateData);
        }

        /// <summary>
        /// 当前对象是否处于完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => canAutoFinished || base.Finished();
    }
}
