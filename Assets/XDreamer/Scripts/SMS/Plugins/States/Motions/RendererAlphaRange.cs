using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器透明度区间:渲染器透明度区间组件是渲染器的透明渐变动画。渲染器在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererAlphaRange))]
    [Tip("渲染器透明度区间组件是渲染器的透明渐变动画。渲染器在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "The renderer transparency interval component is the transparent gradient animation of the renderer. The renderer executes the change action of material transparency within the set time interval. After playing, the component switches to the finished state. If the renderer does not have a material, the execution fails.")]
    [XCSJ.Attributes.Icon(index = 33624)]
    public class RendererAlphaRange : RendererRangeHandle<RendererAlphaRange, float>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器透明度区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(RendererAlphaRange))]
        [Tip("渲染器透明度区间组件是渲染器的透明渐变动画。渲染器在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "The renderer transparency interval component is the transparent gradient animation of the renderer. The renderer executes the change action of material transparency within the set time interval. After playing, the component switches to the finished state. If the renderer does not have a material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        protected override void OnSetValue(Recorder recorder, EBool boolValue, ref float value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetAlpha(value);
                        }
                        break;
                    }
                case EBool.No:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.RecoverColor();
                        }
                        break;
                    }
                case EBool.Switch:
                    {
                        if (switchFlag = !switchFlag)
                        {
                            foreach (var info in recorder._records)
                            {
                                info.SetAlpha(value);
                            }
                        }
                        else
                        {
                            foreach (var info in recorder._records)
                            {
                                info.RecoverColor();
                            }
                        }
                        break;
                    }
            }
        }

        public override void Reset()
        {
            base.Reset();
            onEntryValue = 0;
            leftValue = 1;
            inValue = 1;
            rightValue = 1;
            onExitValue = 1;
        }
    }
}
