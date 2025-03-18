using System;
using Holoville.HOTween;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// 默认补间处理程序对象
    /// </summary>
    public class  DefaultTweenHandler : InstanceClass<DefaultTweenHandler>, ITweenHandler
    {
        /// <summary>
        /// 将变换补间到目标位置与旋转
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleteCallback"></param>
        /// <param name="callbackParams"></param>
        /// <returns></returns>
        public object To(Transform transform, Vector3 position, Quaternion rotation, float duration, Action<object, object[]> onCompleteCallback, params object[] callbackParams)
        {
            if (!transform) return null;
            var tweenParms = new TweenParms()
                    .Prop(nameof(position), position)
                    .Prop(nameof(rotation), rotation)
                    .Ease(EaseType.Linear)
                    .OnComplete((TweenEvent e) =>
                    {
                        onCompleteCallback?.Invoke(e, e.parms);
                    }, callbackParams);

            //执行补间切换
            return HOTween.To(transform, duration, tweenParms);
        }

        /// <summary>
        /// 杀死，即结束补间
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        public int Kill(object tweener)
        {
            return HOTween.Kill(tweener);
        }
    }
}
