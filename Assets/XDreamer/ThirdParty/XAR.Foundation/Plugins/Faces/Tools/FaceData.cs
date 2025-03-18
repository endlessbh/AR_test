using System;
using XCSJ.Attributes;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using UnityEngine;
using XCSJ.PluginCommonUtils;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Faces.Tools
{
    /// <summary>
    /// 面部数据
    /// </summary>
    [Name("面部数据")]
    [Serializable]
    public class FaceData
#if XDREAMER_AR_FOUNDATION
        : TrackData<XRFace, ARFace>
#else
        : TrackData
#endif

    {
        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        [EnumPopup]
        public ELinkRule _linkRule = ELinkRule.TrackingSequence;

        /// <summary>
        /// 关联规则
        /// </summary>
        [Name("关联规则")]
        public enum ELinkRule
        {
            /// <summary>
            /// 无:不做任何操作，即在尝试更新时不执行关联检测操作；
            /// </summary>
            [Name("无")]
            [Tip("不做任何操作，即在尝试更新时不执行关联检测操作；", "Do not do anything, that is, do not perform association detection when trying to update;")]
            None,

            /// <summary>
            /// 跟踪顺序：将跟踪识别到的第[跟踪顺序值]个AR面部对象与当前对象执行关联；仅在触发[当已添加时]的跟踪器事件时，执行基于本规则的关联检测操作；
            /// </summary>
            [Name("跟踪顺序")]
            [Tip("将跟踪识别到的第[跟踪顺序值]个AR面部对象与当前对象执行关联；仅在触发[当已添加时]的跟踪器事件时，执行基于本规则的关联检测操作；", "Associate the [tracking order value] ar face object recognized by tracking with the current object; The association detection operation based on this rule is performed only when the tracker event [when added] is triggered;")]
            TrackingSequence,
        }

        #region 跟踪顺序

        /// <summary>
        /// 跟踪顺序数据
        /// </summary>
        [Serializable]
        [Name("跟踪顺序数据")]
        public class TrackingSequenceData
        {
            /// <summary>
            /// 跟踪顺序：将跟踪识别到的第[跟踪顺序值]个AR面部对象与当前对象执行关联；
            /// </summary>
            [Name("跟踪顺序")]
            [Tip("将跟踪识别到的第[跟踪顺序值]个AR面部对象与当前对象执行关联；", "Associate the [tracking order value] ar face object recognized by tracking with the current object;")]
            [Range(1, 8)]
            public int _trackingSequence = 1;

            /// <summary>
            /// 跟踪索引：将跟踪顺序值<see cref="_trackingSequence"/>转化为以0开始的跟踪索引值；
            /// </summary>
            public int trackingIndex
            {
                get => _trackingSequence - 1;
                set => _trackingSequence = value + 1;
            }
        }

        /// <summary>
        /// 跟踪顺序数据
        /// </summary>
        [Name("跟踪顺序数据")]
        [HideInSuperInspector(nameof(_linkRule), EValidityCheckType.NotEqual, ELinkRule.TrackingSequence)]
        [OnlyMemberElements]
        public TrackingSequenceData _trackingSequenceData = new TrackingSequenceData();

        #endregion

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 尝试更新
        /// </summary>
        /// <param name="trackable"></param>
        /// <param name="trackEvent"></param>
        /// <param name="trackEntity"></param>
        /// <returns></returns>
        public override bool TryUpdate(ARFace trackable, ETrackEvent trackEvent, ITrackEntity<XRFace, ARFace> trackEntity)
        {
            switch (trackEvent)
            {
                case ETrackEvent.OnAdded:
                    {
                        switch (_linkRule)
                        {
                            case ELinkRule.TrackingSequence:
                                {
                                    if (this.trackable) break;
                                    if (trackEntity.trackables.TryGetByIndex(_trackingSequenceData.trackingIndex, out ARFace face)
                                        && face == trackable)
                                    {
                                        //可关联
                                        Update(trackable);
                                        return trackable;
                                    }
                                    return false;
                                }
                        }
                        break;
                    }
            }
            return base.TryUpdate(trackable, trackEvent, trackEntity);
        }

#endif
    }
}
