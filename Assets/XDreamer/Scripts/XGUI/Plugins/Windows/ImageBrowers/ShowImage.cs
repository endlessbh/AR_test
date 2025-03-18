using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 根据图片的原始比例，初始化图片全屏效果
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [Name("显示图片")]
    public class ShowImage : View
    {
        /// <summary>
        /// 画布
        /// </summary>
        [Name("画布")]
        public Canvas _canvas;

        private RectTransform _canvasRectTransform = null;

        Image image;

        void Awake()
        {
            image = GetComponent<Image>();
            if (!_canvas)
            {
                _canvas = GetComponentInParent<Canvas>();
            }
            if (_canvas)
            {
                _canvasRectTransform = _canvas.GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 重置图片
        /// </summary>
        /// <param name="sprite"></param>
        public void ResetImage(Sprite sprite)
        {
            image.sprite = sprite;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            ResetSize();
        }

        /// <summary>
        /// 重置尺寸
        /// </summary>
        public void ResetSize()
        {
            float screenaspect = (float)Screen.width / Screen.height;

            float imageaspect = GetImageAspect();
            Vector2 newSize = Vector2.zero;
            // 根据屏幕宽高比与图片宽高比，适配图片新的大小
            if (screenaspect <= imageaspect)
            {
                newSize = new Vector2(_canvasRectTransform.sizeDelta.x, _canvasRectTransform.sizeDelta.x / imageaspect);
            }
            else
            {
                newSize = new Vector2(_canvasRectTransform.sizeDelta.y * imageaspect, _canvasRectTransform.sizeDelta.y);
            }

            rectTransform.sizeDelta = newSize;
        }

        /// <summary>
        /// 获取图片方向，即图片的宽高比
        /// </summary>
        /// <returns></returns>
        public float GetImageAspect()
        {
            float aspect = 1;
            if (image.overrideSprite != null && image.overrideSprite.rect.height != 0)
            {
                aspect = image.overrideSprite.rect.width / image.overrideSprite.rect.height;
            }
            return aspect;
        }
    }
}
