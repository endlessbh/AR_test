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
    /// 透明度:透明度组件是游戏对象的透明渐变动画。游戏对象在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(Alpha))]
    [Tip("透明度组件是游戏对象的透明渐变动画。游戏对象在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The transparency component is the transparent gradient animation of the game object. The game object executes the change action of material transparency within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
    [XCSJ.Attributes.Icon(index = 33613)]
    [RequireComponent(typeof(GameObjectSet))]
    public class Alpha : Motion<Alpha, Alpha.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "透明度";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(Alpha))]
        [Tip("透明度组件是游戏对象的透明渐变动画。游戏对象在设定的时间区间内执行材质透明度的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "The transparency component is the transparent gradient animation of the game object. The game object executes the change action of material transparency within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Range(0, 1)]
        [Name("透明度")]
        public float alpha;

        [Name("包含成员")]
        public bool includeChildren = true;

        public class Recorder : RendererRecorder, IPercentRecorder<Alpha>
        {
            private Alpha alpha;

            public void Record(Alpha alpha)
            {
                this.alpha = alpha;
                if (!alpha.gameObjectSet) return;
                _records.Clear();
                foreach (var go in alpha.gameObjectSet.objects)
                {
                    if (go)
                    {
                        if (alpha.includeChildren)
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
                foreach (var i in _records)
                {
                    i.SetPercent(percent, alpha.alpha);
                }
            }
        }       
    }
}
