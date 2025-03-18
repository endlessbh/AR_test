using UnityEngine;
using UnityEngine.UI;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTimelines.UI
{
    public class ImagePlayableInteractor : PlayableContent
    {
        public Image _image;

        public Color _sourceColor = Color.white;

        public Color _targetColor = Color.white;

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_image)
            {
                this.XGetComponent<Image>(ref _image);
            }
        }

        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            _image.color = Color.Lerp(_sourceColor, _targetColor, (float)percent.percent01OfWorkCurve);
        }
    }
}

