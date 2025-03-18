using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器颜色区间:渲染器颜色区间组件是渲染器的颜色渐变动画。渲染器在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererColorRange))]
    [Tip("渲染器颜色区间组件是渲染器的颜色渐变动画。渲染器在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "The renderer color interval component is the color ramp animation of the renderer. The renderer executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the renderer does not have a material, the execution fails.")]
    [XCSJ.Attributes.Icon(index = 33625)]
    public class RendererColorRange : RendererRangeHandle<RendererColorRange, UnityEngine.Color>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器颜色区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(RendererColorRange))]
        [Tip("渲染器颜色区间组件是渲染器的颜色渐变动画。渲染器在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果渲染器没有材质，则执行失败。", "The renderer color interval component is the color ramp animation of the renderer. The renderer executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the renderer does not have a material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        protected override void OnSetValue(Recorder recorder, EBool boolValue, ref UnityEngine.Color value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetColor(value);
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
                                info.SetColor(value);
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
            onEntryValue = UnityEngine.Color.green;
            leftValue = UnityEngine.Color.green;
            inValue = UnityEngine.Color.green;
            rightValue = UnityEngine.Color.green;
            onExitValue = UnityEngine.Color.green;
        }
    }
}
