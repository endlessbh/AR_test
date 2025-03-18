using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 用于滚动页面指示的小圆点
    /// </summary>
    [Name("滚动页面标记")]
    public class ScrollPageToggle : ScrollPageGetter
    {
        /// <summary>
        /// 切换组
        /// </summary>
        [Name("切换组")]
        public ToggleGroup toggleGroup;

        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换模版")]
        public Toggle togglePrefab;

        /// <summary>
        /// 切换列表
        /// </summary>
        [Name("切换列表")]
        public List<Toggle> toggleList = new List<Toggle>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

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

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            // 初始化
            if (scrollPage)
            {
                OnScrollPageChanged();
            }
        }

        private void OnImageListChanged(ImageBrower imageBrower, List<ImageItem> imageList)
        {
            if (this.scrollPage.imageBrower != imageBrower) return;

            var pageCount = imageList.Count;
            if (pageCount != toggleList.Count)
            {
                if (pageCount > toggleList.Count)
                {
                    int cc = pageCount - toggleList.Count;
                    for (int i = 0; i < cc; i++)
                    {
                        toggleList.Add(CreateToggle());
                    }
                }
                else if (pageCount < toggleList.Count)
                {
                    while (toggleList.Count > pageCount)
                    {
                        Toggle t = toggleList[toggleList.Count - 1];
                        toggleList.Remove(t);
                        DestroyImmediate(t.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// 创建切换
        /// </summary>
        /// <returns></returns>
        private Toggle CreateToggle()
        {
            var t = GameObject.Instantiate<Toggle>(togglePrefab);
            t.gameObject.SetActive(true);
            t.transform.SetParent(toggleGroup.transform);
            t.transform.localScale = Vector3.one;
            t.transform.localPosition = Vector3.zero;
            return t;
        }

        /// <summary>
        /// 当滚动页面变更时回调
        /// </summary>
        protected override void OnScrollPageChanged()
        {
            SetToggleOn(scrollPage.currentPageIndex);
        }

        /// <summary>
        /// 滚动条滚动
        /// </summary>
        /// <param name="value"></param>
        protected override void OnScrollRectValueChanged(Vector2 value)
        {
            SetToggleOn(scrollPage.ScrollValueToIndex(value.x));
        }

        /// <summary>
        /// 被滚动条触发切换
        /// </summary>
        private bool isOnByScroll = false;

        /// <summary>
        /// 这只切换选中
        /// </summary>
        /// <param name="currentPageIndex"></param>
        private void SetToggleOn(int currentPageIndex)
        {
            if (currentPageIndex >= 0 && currentPageIndex < toggleList.Count)
            {
                isOnByScroll = true;
                toggleList[currentPageIndex].isOn = true;
                isOnByScroll = false;
            }
        }

        /// <summary>
        /// 切换页面
        /// </summary>
        /// <param name="toggle"></param>
        public void SwitchPage(Toggle toggle)
        {
            // 非主动点击触发，返回
            if (isOnByScroll) return;

            var index = toggleList.FindIndex(t => t == toggle);
            if (index >= 0 && toggle.isOn)
            {
                scrollPage.GotoPage(index);
            }
        }
    }
}
