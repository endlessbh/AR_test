using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginHoloLens
{
    [Owner(typeof(HoloLensManager))]
    public abstract class HoloLensStateComponent<T> : StateComponent<T>
    where T : HoloLensStateComponent<T>
    {
        protected InputListener inputListener = null;

        protected virtual GameObject gameObj {get;} = null;

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (gameObj && !inputListener)
            {
                inputListener = CommonFun.GetOrAddComponent<InputListener>(gameObj);
            }
        }
    }
}
