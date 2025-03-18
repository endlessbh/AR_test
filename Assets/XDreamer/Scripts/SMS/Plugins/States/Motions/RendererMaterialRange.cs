using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器材质区间:渲染器材质区间组件是渲染器的材质切换动画。渲染器在设定的时间区间内执行材质切换动作，播放完毕后，组件切换为完成态。如果没有渲染器，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, "RendererMaterialRange")]
    [Tip("渲染器材质区间组件是渲染器的材质切换动画。渲染器在设定的时间区间内执行材质切换动作，播放完毕后，组件切换为完成态。如果没有渲染器，则执行失败。", "The renderer material interval component is the renderer's material switching animation. The renderer performs the material switching action within the set time interval. After playing, the component switches to the completed state. If there is no renderer, the execution fails.")]
    [XCSJ.Attributes.Icon(index = 33628)]
    public class RendererMaterialRange : RendererRangeHandle<RendererMaterialRange, Material[]>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器材质区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, "RendererMaterialRange")]
        [Tip("渲染器材质区间组件是渲染器的材质切换动画。渲染器在设定的时间区间内执行材质切换动作，播放完毕后，组件切换为完成态。如果没有渲染器，则执行失败。", "The renderer material interval component is the renderer's material switching animation. The renderer performs the material switching action within the set time interval. After playing, the component switches to the completed state. If there is no renderer, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("设置材质规则")]
        [EnumPopup]
        public ESetMaterialRule setMaterialRule = ESetMaterialRule.Direct;

        protected override void OnSetValue(Recorder recorder, EBool boolValue, ref Material[] value, ref bool switchFlag)
        {
            switch (boolValue)
            {
                case EBool.Yes:
                    {
                        SetMaterial(recorder, value, setMaterialRule);
                        break;
                    }
                case EBool.No:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.RecoverMaterial();
                        }
                        break;
                    }
                case EBool.Switch:
                    {
                        if (switchFlag = !switchFlag)
                        {
                            SetMaterial(recorder, value, setMaterialRule);
                        }
                        else
                        {
                            foreach (var info in recorder._records)
                            {
                                info.RecoverMaterial();
                            }
                        }
                        break;
                    }
            }
        }

        private void SetMaterial(Recorder recorder, Material[] value, ESetMaterialRule setMaterialRule)
        {
            switch (setMaterialRule)
            {
                case ESetMaterialRule.Fill:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.FillMaterialSize(value);
                        }
                        break;
                    }
                case ESetMaterialRule.Direct:
                default:
                    {
                        foreach (var info in recorder._records)
                        {
                            info.SetMaterial(value);
                        }
                        break;
                    }
            }
        }
    }

    [Name("设置材质规则")]
    public enum ESetMaterialRule
    {
        [Name("直接替换")]
        Direct,

        [Name("填充")]
        [Tip("保持原材质个数不变，进行填充", "Keep the number of original materials unchanged for filling")]
        Fill
    }
}
