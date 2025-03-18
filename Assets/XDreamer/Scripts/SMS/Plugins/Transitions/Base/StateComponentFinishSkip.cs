using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.Base
{
    [ComponentMenu("跳过/状态组件完成跳过", typeof(SMSManager))]
    [Name("状态组件完成跳过")]
    [Tip("当入状态中有一个状态组件为完成态则跳过", "Skip when one of the status components in the input status is in the completed status")]
    public class StateComponentFinishSkip : TransitionComponent
    {
        [Name("入状态所有组件")]
        public bool allComponentOfInState = true;

        [Name("入状态组件")]
        [ArrayElement(EArrayElementHandleRule.CanDelete)]
        [HideInSuperInspector(nameof(allComponentOfInState), EValidityCheckType.Equal, true)]
        public List<StateComponent> componentsInState = new List<StateComponent>();

        private List<StateComponent> checkComponents = null;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            checkComponents = allComponentOfInState ? parent.inState.components.ToList() : componentsInState;
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            
            if(checkComponents.Exists(c=>c.Finished()))
            {
                SkipHelper.Skip(data, parent);
            }
        }

        public override bool Finished() => true;
    }
}
