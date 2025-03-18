using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginSMS.Transitions.Base
{
    /// <summary>
    /// 跳过助手
    /// </summary>
    public class SkipHelper
    {
        /// <summary>
        /// 跳过：将跳转的入状态退出，出状态激活
        /// </summary>
        /// <param name="data"></param>
        /// <param name="transition"></param>
        public static void Skip(StateData data, Transition transition)
        {            
            transition.inState.OnExit(data);

            transition.inState.active = false;

            if (transition.inState is ExitState)
            {
                transition.inState.parent.active = false;
            }

            transition.outState.active = true;
        }
    }
}
