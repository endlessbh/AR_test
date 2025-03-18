using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 缩放:缩放组件是游戏对象的缩放动画。在给定的时间内，游戏对象在XYZ三轴上做缩放运动，缩放完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Scale))]
    [Tip("缩放组件是游戏对象的缩放动画。在给定的时间内，游戏对象在XYZ三轴上做缩放运动，缩放完成后，组件切换为完成态。", "The zoom component is the zoom animation of the game object. Within a given time, the game object does scaling motion on the XYZ three-axis. After scaling, the component switches to the completed state.")]
    [DisallowMultipleComponent]
    [Attributes.Icon]
    public class Scale : TransformMotion<Scale>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "缩放";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(Scale))]
        [Tip("缩放组件是游戏对象的缩放动画。在给定的时间内，游戏对象在XYZ三轴上做缩放运动，缩放完成后，组件切换为完成态。", "The zoom component is the zoom animation of the game object. Within a given time, the game object does scaling motion on the XYZ three-axis. After scaling, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Scale)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("值")]
        public Vector3 value = Vector3.one;

        [Name("相对")]
        public bool relative = false;

        private Dictionary<Transform, Vector3> targetScaleValueDic = new Dictionary<Transform, Vector3>();

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            (recorder as Recorder)?._records.ForEach(i =>
            {
                targetScaleValueDic[i.transform] = relative ? Vector3.Scale(i.transform.localScale,value):value;
            });
        }

        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            info.transform.localScale = Vector3.Lerp(info.localScale, targetScaleValueDic[info.transform], (float)percent.percent01OfWorkCurve);
        }
    }
}
