using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 页索引信息
    /// </summary>
    [Name("页索引信息")]
    public class ScrollPageIndexText : ScrollPageGetter
    {
        /// <summary>
        /// 当前页文本
        /// </summary>
        [Name("当前页文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _currentPageIndex = null;

        /// <summary>
        /// 总页数文本
        /// </summary>
        [Name("总页数文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _totalPageIndex = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected void Start()
        {
            SetText();
        }

        /// <summary>
        /// 当滚动页面变更时回调
        /// </summary>
        protected override void OnScrollPageChanged()
        {
            SetText();
        }

        private void SetText()
        {
            if (_currentPageIndex)
            {
                _currentPageIndex.text = (scrollPage.currentPageIndex + 1).ToString();
            }

            if (_totalPageIndex)
            {
                _totalPageIndex.text = scrollPage.pageCount.ToString();
            }
        }
    }
}
