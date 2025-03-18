using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    public abstract class UnityAnimator<T> : WorkClip<T> where T : UnityAnimator<T>
    {
        [Group("Animator属性")]
        [Name("Animator")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Animator))]
        public Animator animator = null;

        [Name("层索引")]
        public int layerIndex = 0;

        [Name("状态名称")]
        public string stateName = "Take 001";

        public void PlayAnimator(double percent)
        {
            try
            {
                if (!animator) return;
                animator.speed = 0;
                animator.Play(stateName, layerIndex, (float)percent);
            }
            catch (Exception ex)
            {
                LogException(this, nameof(PlayAnimator), ex);
            }
        }

        protected override void OnSetPercent(Percent percent, StateData stateData) => PlayAnimator(percent.percent01OfWorkCurve);
       
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            switch(data.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        SetPercent(0);
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return animator;
        }
    }
}
