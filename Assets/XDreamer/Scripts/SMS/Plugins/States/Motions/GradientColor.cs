using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渐变色:渐变色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [Name(Title, nameof(GradientColor))]
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Tip("渐变色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The gradient component is the color gradient animation of the GameObject. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GradientColor : Motion<GradientColor, GradientColor.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渐变色";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(GradientColor))]
        [Tip("渐变色组件是游戏对象的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The gradient component is the color gradient animation of the GameObject. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("渐变色")]
        public Gradient gradient = new Gradient();

        [Name("包含成员")]
        public bool includeChildren = true;

        public class Recorder : RendererRecorder, IPercentRecorder<GradientColor>
        {
            private GradientColor color;

            public void Record(GradientColor color)
            {
                this.color = color;
                if (!color.gameObjectSet) return;
                _records.Clear();
                foreach (var go in color.gameObjectSet.objects)
                {
                    if (go)
                    {
                        if (color.includeChildren)
                        {
                            Record(go.GetComponentsInChildren<Renderer>());
                        }
                        else
                        {
                            Record(go);
                        }
                    }
                }
            }

            public void SetPercent(Percent percent)
            {
                var color = this.color.gradient.Evaluate((float)percent.percent01OfWorkCurve);
                foreach (var i in _records)
                {
                    i.SetColor(color);
                }
            }
        }
    }
}
