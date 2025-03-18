using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 图片浏览器
    /// </summary>
    [Name("图片浏览器")]
    public class ImageBrower : ViewInteractor
    {
        /// <summary>
        /// 滚动页面
        /// </summary>
        [Name("滚动页面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ScrollPage _scrollPage;

        /// <summary>
        /// 滚动页面
        /// </summary>
        public ScrollPage scrollPage => this.XGetComponentInChildren(ref _scrollPage);

        /// <summary>
        /// 图片放大浏览对象
        /// </summary>
        [Name("图片放大浏览对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject viewPanel;

        private ShowImage showImage = null;

        /// <summary>
        /// 图片项生成模版
        /// </summary>
        [Name("图片项模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject itemPrefab;

        /// <summary>
        /// 启用图片缩放
        /// </summary>
        [Name("启用图片项缩放")]
        public bool _scaleItemEnable = false;

        /// <summary>
        /// 与中心图片缩放比
        /// </summary>
        [Name("与中心图片缩放比")]
        [Tip("处于浏览器中心的图像比例为1，左右两侧图像为当前值，再往外侧延伸的值为当前值的1/2，以此类推", "The proportion of the image in the center of the browser is 1, the left and right images are the current value, and the value extending to the outside is 1 / 2 of the current value, and so on")]
        [Range(0, 1)]
        [HideInSuperInspector(nameof(_scaleItemEnable), EValidityCheckType.False)]
        public float _scaleByCenterImage = 0.75f;

        /// <summary>
        /// 图片列表
        /// </summary>
        [Name("图片列表")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public Sprite[] images;

        /// <summary>
        /// 图片列表
        /// </summary>
        internal List<ImageItem> _limitImageList { get; private set; } = new List<ImageItem>();

        /// <summary>
        /// 图标数量
        /// </summary>
        public int imageCount => _limitImageList.Count;

        /// <summary>
        /// 图片列表变化回调
        /// </summary>
        public static event Action<ImageBrower, List<ImageItem>> onImageListChanged = null;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            if (viewPanel)
            {
                showImage = viewPanel.GetComponentInChildren<ShowImage>(true);
            }
            CreateImages();
        }

        private void CreateImages()
        {
            if (!itemPrefab) return;
            itemPrefab.SetActive(false);

            _limitImageList.Clear();

            int index = 0;
            foreach (var img in images)
            {
                if (img)
                {
                    _limitImageList.Add(CreateItem(img, index++));
                }
            }
            onImageListChanged?.Invoke(this, _limitImageList);
        }

        private ImageItem CreateItem(Sprite sprite, int index)
        {
            var go = GameObject.Instantiate<GameObject>(itemPrefab);
            go.SetActive(true);
            go.transform.SetParent(itemPrefab.transform.parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
			var imgItem = go.GetComponentInChildren<ImageItem>();
            if(!imgItem)
            {
                imgItem = go.AddComponent<ImageItem>();
            }
            imgItem.Init(this, sprite, index);
            return imgItem;
        }

        /// <summary>
        /// 当进入全屏图像时回调，参数依次为：图片浏览器、待全屏的图像、全屏的显示图像
        /// </summary>
        public static event Action<ImageBrower, Image, ShowImage> onEntryFullscreenImage;

        /// <summary>
        /// 当退出全屏图像时回调，参数依次为：图片浏览器
        /// </summary>
        public static event Action<ImageBrower> onExitFullscreenImage;

        public void ShowImageView(Image image)
        {
            if (viewPanel)
            {
                viewPanel.SetActive(true);
                if (showImage)
                {
                    showImage.ResetImage(image.sprite);
                }
                onEntryFullscreenImage?.Invoke(this, image, showImage);
            }
        }

        /// <summary>
        /// 隐藏图片视图
        /// </summary>
        public void HideImageView()
        {
            if (viewPanel)
            {
                viewPanel.SetActive(false);
                onExitFullscreenImage?.Invoke(this);
            }
        }

        /// <summary>
        /// 全屏图片
        /// </summary>
        public void FullscreenImage()
        {
            ShowImageView(_limitImageList[scrollPage.currentPageIndex].image);
        }
    }

    /// <summary>
    /// 图片浏览器获取器
    /// </summary>
    public abstract class ImageBrowerGetter : View
    {
        /// <summary>
        /// 图片浏览器
        /// </summary>
        [Name("图片浏览器")]
        public ImageBrower _imageBrower;

        /// <summary>
        /// 图片浏览器
        /// </summary>
        public ImageBrower imageBrower => this.XGetComponentInParentOrGlobal(ref _imageBrower);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (imageBrower) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (imageBrower) { }
        }
    }
}