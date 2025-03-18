using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Components;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 组件启用区间:可用区间组件是组件启用与禁用的动画。组件在设定的时间区间内执行启用或者禁用的命令，播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentEnabledRange))]
    [Tip("可用区间组件是组件启用与禁用的动画。组件在设定的时间区间内执行启用或者禁用的命令，播放完毕后，组件切换为完成态。", "The available interval component is the animation that the component enables and disables. The component executes the enable or disable command within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33618)]
    [RequireComponent(typeof(ComponentSet))]
    public class ComponentEnabledRange : RangeHandle<ComponentEnabledRange, ComponentEnabledRange.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件启用区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(ComponentEnabledRange))]
        [Tip("可用区间组件是组件启用与禁用的动画。组件在设定的时间区间内执行启用或者禁用的命令，播放完毕后，组件切换为完成态。", "The available interval component is the animation that the component enables and disables. The component executes the enable or disable command within the set time interval. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public ComponentSet componentSet => GetComponent<ComponentSet>(true);       

        public class Recorder : ComponentRecorder, IRangeHandleRecorder<ComponentEnabledRange>
        {
            public ComponentEnabledRange rangeHandle;

            public void Record(ComponentEnabledRange rangeHandle)
            {
                this.rangeHandle = rangeHandle;
                if (!rangeHandle.componentSet) return;
                _records.Clear();
                Record(rangeHandle.componentSet.objects);
            }

            public void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
            {
                foreach (var info in _records)
                {
                    info.component.XSetEnable(boolValue);
                }
            }

            public void SetPercent(Percent percent)
            {
                if(percent.leftRange)
                {
                    SetRange(rangeHandle.leftRange);
                }
                else if(percent.rightRange)
                {
                    SetRange(rangeHandle.rightRange);
                }
                else
                {
                    SetRange(rangeHandle.inRange);
                }
            }
        }
    }
}
