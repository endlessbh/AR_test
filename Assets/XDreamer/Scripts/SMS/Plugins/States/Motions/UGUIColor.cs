using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// UGUI颜色:UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(UGUIColor))]
    [Tip("UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "Ugui color component is the color gradient animation of ugui. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [RequireComponent(typeof(GameObjectSet))]
    public class UGUIColor : Motion<UGUIColor, UGUIColor.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI颜色";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(UGUIColor))]
        [Tip("UGUI颜色组件是UGUI的颜色渐变动画。游戏对象在设定的时间区间内执行材质颜色的变化动作，播放完毕后，组件切换为完成态。如果游戏对象没有材质，则执行失败。", "Ugui color component is the color gradient animation of ugui. The game object executes the change action of material color within the set time interval. After playing, the component switches to the completed state. If the game object has no material, the execution fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("颜色")]
        public UnityEngine.Color color = UnityEngine.Color.green;

        [Name("包含成员")]
        public bool includeChildren = true;

        public class Recorder : GraphicRecorder, IPercentRecorder<UGUIColor>
        {
            private UGUIColor color;

            public void Record(UGUIColor color)
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
                            Record(go.GetComponentsInChildren<Graphic>());
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
                    i.SetPercent(percent, color.color);
                }
            }
        }
    }
}
