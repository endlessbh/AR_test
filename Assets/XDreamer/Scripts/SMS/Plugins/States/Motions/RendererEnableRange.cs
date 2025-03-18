using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器启用区间:渲染器启用区间组件是渲染器启用禁用的动画。渲染器在设定的时间区间内执行激活或非激活的命令，播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererEnableRange))]
    [Tip("渲染器启用区间组件是渲染器启用禁用的动画。渲染器在设定的时间区间内执行激活或非激活的命令，播放完毕后，组件切换为完成态。", "The renderer enable interval component is the animation that the renderer enables or disables. The renderer executes the active or inactive command within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33626)]
    public class RendererEnableRange : RendererRangeHandle<RendererEnableRange>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器启用区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(RendererEnableRange))]
        [Tip("渲染器启用区间组件是渲染器启用禁用的动画。渲染器在设定的时间区间内执行激活或非激活的命令，播放完毕后，组件切换为完成态。", "The renderer enable interval component is the animation that the renderer enables or disables. The renderer executes the active or inactive command within the set time interval. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        protected override void OnSetPercent(Recorder recorder, Percent percent)
        {
            if(percent.leftRange)
            {
                OnSetPercent(recorder, leftRange);
            }
            else if(percent.rightRange)
            {
                OnSetPercent(recorder, rightRange);
            }
            else
            {
                OnSetPercent(recorder, inRange);
            }
        }

        protected override void OnSetPercent(Recorder recorder, EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate)
        {
            foreach (var info in recorder._records)
            {
                info.SetEnabled(boolValue);
            }
        }
    }
}
