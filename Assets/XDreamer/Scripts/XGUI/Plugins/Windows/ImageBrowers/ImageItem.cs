using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 图片项
    /// </summary>
    [Name("图片项")]
    [RequireComponent(typeof(Image))]
    public class ImageItem : ScrollPageGetter
    {
        /// <summary>
        /// 图片对象
        /// </summary>
        [Name("图片对象")]
        public Image _image = null;

        /// <summary>
        /// 父级
        /// </summary>
        [Name("父级")]
        [HideInSuperInspector]
        public RectTransform parent;

        /// <summary>
        /// 关联图像
        /// </summary>
        public Image image { get => _image; set => _image = value; }

        private ImageBrower imageBrower = null;

        private Button button = null;

        /// <summary>
        /// 项索引编号
        /// </summary>
        private int index = 0;

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="sprite"></param>
        public void Init(ImageBrower imageBrower, Sprite sprite, int index)
        {
            this.imageBrower = imageBrower;
            image.sprite = sprite;
            this.index = index;
        }

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (!_image)
            {
                _image = GetComponent<Image>();
            }
            button = GetComponentInChildren<Button>();
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            var layout = transform.GetComponent<LayoutElement>();
            if (layout)
            {
                layout.preferredWidth = imageBrower.scrollPage.imageItemSize.x;
                layout.preferredHeight = imageBrower.scrollPage.imageItemSize.y;
            }
            else
            {
                // 旧代码将当前组件关联在图片项的子对象上，因此这里是子对象向父对象上找
                layout = transform.parent.GetComponent<LayoutElement>();
                if (parent && transform.parent.GetComponent<RectTransform>() && layout)
                {
                    transform.parent.GetComponent<RectTransform>().sizeDelta = parent.sizeDelta;
                    layout.preferredWidth = parent.sizeDelta.x;
                    layout.preferredHeight = parent.sizeDelta.y;
                    GetComponent<RectTransform>().sizeDelta = parent.sizeDelta;
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (button)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (button)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (scrollPage.currentPageIndex != index) return;

            imageBrower.FullscreenImage();
        }

        /// <summary>
        /// 滑动条滚动
        /// </summary>
        protected override void OnScrollPageChanged() { }

        /// <summary>
        /// 响应滚动条值变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnScrollRectValueChanged(Vector2 value)
        {
            SetScale(scrollPage.ScrollValueToFloatIndex(value.x));
        }

        private void SetScale(float index)
        {
            if (imageBrower && imageBrower._scaleItemEnable)
            {
                var offsetIndex = Mathf.Abs(this.index - index);
                var scale = offsetIndex < 0.02f ? 1: imageBrower._scaleByCenterImage / offsetIndex;
                if (image)
                {
                    image.transform.localScale = Vector3.one * Mathf.Clamp01(scale);
                }
            }
        }
    }
}
