using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 画布组动画
    /// </summary>
    [Name("画布组动画", nameof(CanvasGroupAnimation))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class CanvasGroupAnimation : PlayableContent
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public enum EPropertyType
        {
            [Name("无")]
            None,

            [Name("透明度")]
            Alpha,
        }

        /// <summary>
        /// 画布组
        /// </summary>
        [Name("画布组")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public CanvasGroup _canvasGroup;

        /// <summary>
        /// 参数类型
        /// </summary>
        [Name("参数类型")]
        [EnumPopup]
        public EPropertyType _propertyType = EPropertyType.Alpha;

        /// <summary>
        /// 透明度变化范围
        /// </summary>
        [Name("透明度变化范围")]
        [LimitRange(0,1)]
        [HideInSuperInspector(nameof(_propertyType), EValidityCheckType.NotEqual, EPropertyType.Alpha)]
        public Vector2 _alphaRange = new Vector2(0,1);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 设置百分比回调
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            _canvasGroup.alpha = Mathf.Lerp(_alphaRange.x, _alphaRange.y, (float)percent.percent01OfWorkCurve);
        }
    }
}