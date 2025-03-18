using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.LineNotes;

namespace XCSJ.PluginTimelines.UI
{
    /// <summary>
    /// 打字动画:用于一个播放打字效果
    /// </summary>
    [Name("打字动画", nameof(TextTypewriter))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class TextTypewriter : PlayableContent, INoteText
    {
        /// <summary>
        /// 播放文本
        /// </summary>
        [Name("播放文本")]
        public string _playText = "";

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text;

        /// <summary>
        /// 标注文本
        /// </summary>
        public string noteText  {  get => _playText;  set => _playText = value; }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if(!_text) _text = GetComponent<Text>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_text && string.IsNullOrEmpty(_playText))
            {
                _playText = _text.text;
                _text.text = "";
            }
        }

        /// <summary>
        /// 设置百分比回调
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            if (!string.IsNullOrEmpty(noteText))
            {
                _text.text = noteText.Substring(0, (int)Mathf.Lerp(0f, noteText.Length, (float)percent.percent01OfWorkCurve));
            }
        }
    }
}
