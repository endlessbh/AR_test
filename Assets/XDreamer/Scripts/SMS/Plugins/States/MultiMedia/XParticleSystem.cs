using UnityEngine;
using UnityEngine.Playables;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 粒子系统
    /// </summary>]
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(PlayableDirector))]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [Tip("播放粒子系统", "Play particle system")]
    public class XParticleSystem : WorkClip<XParticleSystem>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "粒子系统";

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(PlayableDirector))]
        [Tip("播放粒子系统", "Play particle system")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 粒子系统
        /// </summary>
        [Group("粒子系统属性")]
        [Name(Title)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(ParticleSystem))]
        public ParticleSystem particleSystem;

        [Name("包含子级")]
        [Tip("更新时同步设置子粒子系统", "Set up the sub particle system synchronously on update")]
        public bool withChildren = true;

        [Name("重新启动")]
        [Tip("重新启动并从头开始;为True时，使用时间模拟；为False时，使用增量时间模拟；", "Restart and start from scratch; When true, time simulation is used; When false, use incremental time simulation;")]
        public bool restart = false;

        [Name("固定时间间隔")]
        [Tip("仅基于“时间”选项中“固定时间”中的值以固定的时间间隔更新系统", "Updates the system at regular intervals based only on the value in fixed time in the time option")]
        public bool fixedTimeStep = false;

        /// <summary>
        /// 当进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            lastTime = 0;
            if (particleSystem)
            {
                particleSystem.Play();
            }
        }

        /// <summary>
        /// 当退出时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            lastTime = 0;
            if (particleSystem)
            {
                particleSystem.Stop();
            }
        }

        private double lastTime = 0;

        /// <summary>
        /// 当设置百分比时
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (!particleSystem) return;
            if (!particleSystem.isPlaying) particleSystem.Play();

            var nowTime = particleSystem.main.duration * percent.percent01OfWorkCurve;
            if (restart)
            {
                particleSystem.Simulate((float)nowTime, withChildren, restart, fixedTimeStep);
            }
            else
            {                
                var deltaTime = nowTime - lastTime;
                particleSystem.Simulate((float)deltaTime, withChildren, restart, fixedTimeStep);
                lastTime = nowTime;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && particleSystem;
        }

        public static float ParticleSystemLength(Transform transform)
        {
            ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0;
            //foreach (ParticleSystem ps in particleSystems)
            //{
            //    if (ps.emission.enabled)
            //    {
            //        if (ps.main.loop)
            //        {
            //            return -1f;
            //        }
            //        float dunration = 0f;
            //        if (ps.emission.rateOverTime <= 0)
            //        {
            //            dunration = ps.startDelay + ps.startLifetime;
            //        }
            //        else
            //        {
            //            dunration = ps.startDelay + Mathf.Max(ps.duration, ps.startLifetime);
            //        }
            //        if (dunration > maxDuration)
            //        {
            //            maxDuration = dunration;
            //        }
            //    }
            //}
            return maxDuration;
        }
    }
}
