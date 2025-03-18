using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 滚动页面
    /// </summary>
    [Name("滚动页面")]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollPage : ImageBrowerGetter, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 图片数量
        /// </summary>
        [Name("图片数量")]
        [Tip("当前滚动页面容纳图片的数量", "The number of pictures the current scrolling page holds")]
        [Range(1, 20)]
        [Readonly(EEditorMode.Runtime)]
        public int _radioWithImageItem = 1;

        /// <summary>
        /// 水平布局
        /// </summary>
        [Name("水平布局")]
        public bool isHorizontal = true;

        /// <summary>
        /// 图片项尺寸
        /// </summary>
        public Vector2 imageItemSize
        {
            get
            {
                if (rectTransform)
                {
                    if (isHorizontal)
                    {
                        return new Vector2(rectTransform.sizeDelta.x / _radioWithImageItem, rectTransform.sizeDelta.y);
                    }
                    else
                    {
                        return new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y / _radioWithImageItem);
                    }
                }
                return Vector2.one; 
            }
        }

        /// <summary>
        /// 初始索引
        /// </summary>
        [Name("初始页")]
        [Min(0)]
        public int _initPageIndex = 0;

        /// <summary>
        /// 初始化页面
        /// </summary>
        [Name("启用时重置页")]
        public bool _initPageIndexOnEnable = true;

        /// <summary>
        /// 翻页规则
        /// </summary>
        [Name("翻页规则")]
        [EnumPopup]
        public ETurnPageRule _turnPageRule = ETurnPageRule.OnePage;

        /// <summary>
        /// 翻页规则
        /// </summary>
        [Name("翻页规则")]
        public enum ETurnPageRule
        {
            [Name("单页")]
            OnePage,

            [Name("惯性")]
            Inertia,
        }

        /// <summary>
        /// 滑动速度
        /// </summary>
        [Group("滑动配置")]
        [Name("滑动速度")]
        [HideInSuperInspector(nameof(_turnPageRule), EValidityCheckType.NotEqual, ETurnPageRule.OnePage)]
        public float smooting = 4;

        /// <summary>
        /// 最小偏移量
        /// </summary>
        [Name("最小偏移量")]
        [Range(0, 0.5f)]
        [HideInSuperInspector(nameof(_turnPageRule), EValidityCheckType.NotEqual, ETurnPageRule.OnePage)]
        public float offset = 0.01f;

        /// <summary>
        /// 最小偏移量
        /// </summary>
        [Name("惯性速度最小值")]
        [Tip("当滚动速度值低于当前值时，停止滚动并设置在固定页上", "When the scroll speed value is lower than the current value, stop scrolling and set it on the fixed page")]
        [Range(0, 1000)]
        [HideInSuperInspector(nameof(_turnPageRule), EValidityCheckType.NotEqual, ETurnPageRule.Inertia)]
        public float _minInertiaVelocityX = 100;

        /// <summary>
        /// 鼠标滚轮翻页触发最小值
        /// </summary>
        [Name("鼠标滚轮翻页触发最小值")]
        [Range(0, 1)]
        public float _mouseScrollWheel = 0.05f;

        /// <summary>
        /// 循环时瞬时定位
        /// </summary>
        [Name("循环瞬时定位")]
        [Tip("循环时瞬时定位到开始或者结束", "Instantaneous positioning to the beginning or end of the cycle")]
        public bool _loopGotoBeginOrEndImmediately = true;

        /// <summary>
        /// 滚动矩形
        /// </summary>
        public ScrollRect scrollRect
        {
            get
            {
                if (!_scrollRect)
                {
                    _scrollRect = GetComponent<ScrollRect>();
                }
                return _scrollRect;
            }
        }

        private ScrollRect _scrollRect = null;

        /// <summary>
        /// 滑动的起始坐标
        /// </summary>
        private float targetHorizontal = 0;

        /// <summary>
        /// 页面变更回调事件：当页面变更时回调，参数依次为：滚动页面、页面总数、当前页面索引；
        /// </summary>
        public static event Action<ScrollPage, int, int> onPageChanged;

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_initPageIndexOnEnable)
            {
                currentPageIndex = _initPageIndex;
            }
            ImageBrower.onImageListChanged += OnImageListChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ImageBrower.onImageListChanged -= OnImageListChanged;
        }

        private void OnImageListChanged(ImageBrower imageBrower, List<ImageItem> imageList)
        {
            if (this.imageBrower != imageBrower) return;

            currentPageIndex = _initPageIndex;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            //当在拖拽的时候要也会执行插值，所以会出现闪烁的效果
            if (!isDrag && !inertiaScroll && imageBrower.imageCount > 1)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetHorizontal, Time.deltaTime * smooting);
            }

            switch (_turnPageRule)
            {
                case ETurnPageRule.OnePage:
                    {

                        break;
                    }
                case ETurnPageRule.Inertia:
                    {
                        currentPageIndex = ScrollValueToIndex(scrollRect.horizontalNormalizedPosition);
                        if (inertiaScroll && Mathf.Abs(scrollRect.velocity.x) < _minInertiaVelocityX)
                        {
                            targetHorizontal = IndexToScrollValue(_currentPageIndex);
                            inertiaScroll = false;
                        }
                        break;
                    }
            }

            // 滚轮控制翻页
#if UNITY_STANDALONE
            var scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");

            if ((scrollWheelValue > _mouseScrollWheel || scrollWheelValue < -_mouseScrollWheel)
                 && (scrollRect.velocity.x < 0.5f || MathX.Approximately(scrollRect.horizontalNormalizedPosition, targetHorizontal)))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
                {
                    if (scrollWheelValue < 0)
                    {
                        NextPage();
                    }
                    else
                    {
                        PreviousPage();
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 是否拖拽结束
        /// </summary>
        private bool isDrag = false;
        private float beginDragPos = 0;
        private bool inertiaScroll = false;

        /// <summary>
        /// 当开始拖拽时
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            beginDragPos = scrollRect.horizontalNormalizedPosition;
        }

        /// <summary>
        /// 当结束拖拽时
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;

            var endDragPos = scrollRect.horizontalNormalizedPosition;
            var dragOffset = endDragPos - beginDragPos;

            if (Mathf.Abs(dragOffset) < offset)
            {
                return;
            }

            switch (_turnPageRule)
            {
                case ETurnPageRule.OnePage:
                    {
                        if (endDragPos > beginDragPos)
                        {
                            NextPage();
                        }
                        else if (endDragPos < beginDragPos)
                        {
                            PreviousPage();
                        }
                        break;
                    }
                case ETurnPageRule.Inertia:
                    {
                        inertiaScroll = true;
                        break;
                    }
            }
        }

        #region 翻页

        /// <summary>
        /// 当前页面索引
        /// </summary>
        public int currentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                if (!imageBrower || imageBrower.imageCount <= 1 || value < 0 || value >= imageBrower.imageCount)
                {
                    return;
                }

                if (_currentPageIndex != value)
                {
                    _currentPageIndex = value;

                    onPageChanged?.Invoke(this, pageCount, _currentPageIndex);
                }
            }
        }
        private int _currentPageIndex = -1;

        private float pageLength => imageItemSize.x;

        private float pageTotalLength => pageCount * pageLength;

        private float screenLength => rectTransform.sizeDelta.x;

        /// <summary>
        /// 最大页数
        /// </summary>
        public int pageCount => imageBrower.imageCount;

        /// <summary>
        /// 旧算法：
        /// 页面：0，1，2，3  索引从0开始
        /// 每页占的比列：0/3=0  1/3=0.333  2/3=0.6666 3/3=1
        /// 
        /// 滚动页面索引(从1开始)转滑竿值的通用算法:
        /// 页面总长度 = pageTotalLength
        /// 屏幕长度 = screenLength
        /// 单页长度 = pageLength
        /// 当前页面中心 = currentPageCenter =（当前页数 - 0.5）* 单页长度
        /// 滚动条归一化长度 = normalLength = 滑动页面总长度 - 屏幕长度
        /// 项中心归一化定位 = currentPageCenterInScreen = (当前页面中心 - 屏幕中心)/滚动条归一化长度
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float IndexToScrollValue(int index)
        {
            float currentPageCenter = (index + 1 - 0.5f) * pageLength;
            float normalLength = pageTotalLength - screenLength;
            float currentPageCenterInScreen = (currentPageCenter - screenLength / 2) / normalLength;
            return Mathf.Clamp01(_imageBrower.imageCount > 1 ? currentPageCenterInScreen : 0);
        }

        /// <summary>
        /// 滑竿值转索引值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int ScrollValueToIndex(float value)
        {
            //并四舍五入
            return Mathf.FloorToInt(ScrollValueToFloatIndex(value) + 0.5f);
        }

        /// <summary>
        /// 滑竿值转浮点数索引值：使用页面转滑竿值算法的逆向求解
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float ScrollValueToFloatIndex(float value)
        {
            return (Mathf.Clamp01(value) * (pageTotalLength - screenLength) + screenLength / 2) / pageLength + 0.5f - 1;
        }

        /// <summary>
        /// 前一页
        /// </summary>
        public void PreviousPage() => PreviousPage(false);

        /// <summary>
        /// 前一页
        /// </summary>
        /// <param name="isLoop"></param>
        public void PreviousPage(bool isLoop)
        {
            var previous = currentPageIndex - 1;

            if (previous < 0)
            {
                if (isLoop)
                {
                    GotoPage(pageCount - 1, _loopGotoBeginOrEndImmediately);
                }
            }
            else
            {
                GotoPage(previous);
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPage() => NextPage(false);

        /// <summary>
        /// 下一页
        /// </summary>
        public void NextPage(bool isLoop)
        {
            var next = currentPageIndex + 1;

            if (next >= pageCount)
            {
                if (isLoop)
                {
                    GotoPage(0, _loopGotoBeginOrEndImmediately);
                }
            }
            else
            {
                GotoPage(next);
            }
        } 

        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="immediately"></param>
        public void GotoPage(int pageIndex, bool immediately = false)
        {
            var tmp = IndexToScrollValue(pageIndex);
            // 滚动条无法移动
            if (MathX.Approximately(tmp, targetHorizontal))
            {
                return;
            }

            currentPageIndex = pageIndex;
            targetHorizontal = tmp;
            if (immediately)
            {
                scrollRect.horizontalNormalizedPosition = targetHorizontal;
            }
        }

        #endregion
    }

    /// <summary>
    /// 滚动页面获取器
    /// </summary>
    public abstract class ScrollPageGetter : View
    {
        /// <summary>
        /// 滚动页面
        /// </summary>
        [Name("滚动页面")]
        public ScrollPage _scrollPage;

        /// <summary>
        /// 图片浏览器
        /// </summary>
        public ScrollPage scrollPage => this.XGetComponentInParentOrGlobal(ref _scrollPage);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (scrollPage) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (scrollPage) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ScrollPage.onPageChanged += OnScrollPageChanged;

            if (scrollPage)
            {
                scrollPage.scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ScrollPage.onPageChanged -= OnScrollPageChanged;

            if (scrollPage)
            {
                scrollPage.scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
            }
        }

        /// <summary>
        /// 当滚动页面变更时回调
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="currentPageIndex"></param>
        private void OnScrollPageChanged(ScrollPage scrollPage, int pageCount, int currentPageIndex)
        {
            if (this.scrollPage != scrollPage) return;

            OnScrollPageChanged();
        }

        /// <summary>
        /// 滚动页面变化
        /// </summary>
        protected abstract void OnScrollPageChanged();

        /// <summary>
        /// 滚动条滚动
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnScrollRectValueChanged(Vector2 value) { }
    }
}
