using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器闪烁:渲染器闪烁组件是渲染器闪烁的动画，渲染器在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererFlash))]
    [Tip("渲染器闪烁组件是渲染器闪烁的动画，渲染器在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "The renderer blinking component is the animation of the renderer blinking. The renderer blinks at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33627)]
    [RequireComponent(typeof(GameObjectSet))]
    public class RendererFlash : Flash<RendererFlash, RendererFlash.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器闪烁";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(RendererFlash))]
        [Tip("渲染器闪烁组件是渲染器闪烁的动画，渲染器在设定的时间区间内按设定的频率闪烁，在播放完毕后，组件切换为完成态。", "The renderer blinking component is the animation of the renderer blinking. The renderer blinks at the set frequency within the set time interval. After playing, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("包含成员")]
        public bool includeChildren = true;

        [Name("闪烁类型")]
        public enum EFlashType
        {
            [Name("可用")]
            Enabled = 0,

            [Name("颜色")]
            Color,

            [Name("材质")]
            Material,
        }

        [Name("闪烁类型")]
        [EnumPopup]
        public EFlashType flashType = EFlashType.Enabled;

        [Name("颜色")]
        [HideInSuperInspector(nameof(flashType), EValidityCheckType.NotEqual, EFlashType.Color)]
        public UnityEngine.Color color = UnityEngine.Color.green;

        [Name("材质")]
        [HideInSuperInspector(nameof(flashType), EValidityCheckType.NotEqual, EFlashType.Material)]
        public List<Material> materials = new List<Material>();

        public class Recorder : RendererRecorder, IPercentRecorder<RendererFlash>
        {
            public RendererFlash flash;

            public void Record(RendererFlash flash)
            {
                this.flash = flash;
                if (!flash.gameObjectSet) return;
                _records.Clear();

                foreach (var go in flash.gameObjectSet.objects)
                {
                    if (go)
                    {
                        Record(go);
                        if (flash.includeChildren) Record(go.GetComponentsInChildren<Renderer>());
                    }
                }
            }

            public void SetPercent(Percent percent)
            {
                switch (flash.flashType)
                {
                    case EFlashType.Enabled:
                        {
                            foreach (var info in _records)
                            {
                                info.renderer.enabled = !flash.inChangeArea;
                            }
                            break;
                        }
                    case EFlashType.Color:
                        {
                            foreach (var info in _records)
                            {
                                for (int i = 0; i < info.renderer.materials.Length; ++i)
                                {
                                    info.renderer.materials[i].color = flash.inChangeArea ? flash.color : info.colors[i];
                                }
                            }
                            break;
                        }
                    case EFlashType.Material:
                        {
                            foreach (var info in _records)
                            {
                                for (int i = 0; i < info.renderer.materials.Length; ++i)
                                {
                                    info.renderer.materials[i].SetColor("_Color", flash.inChangeArea ? flash.color : info.colors[i]);
                                }
                            }
                            break;
                        }
                }

            }
        }
    }
}
