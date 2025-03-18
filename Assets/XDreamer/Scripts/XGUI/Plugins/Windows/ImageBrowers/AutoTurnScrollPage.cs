using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.AI.Autos;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Views.Texts;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 自动翻页
    /// </summary>
    [Name("自动翻页")]
    //[RequireComponent(typeof(ScrollPage))]
    //[Tool(XGUICategory.Component, nameof(XGUIManager))]
    public class AutoTurnScrollPage : View
    {
        /// <summary>
        /// 翻页滚动规则
        /// </summary>
        [Group("翻页")]
        [Name("滚动规则")]
        [EnumPopup]
        public EScrollRule _autoScrollRule = EScrollRule.AutoScroll;

        /// <summary>
        /// 递增
        /// </summary>
        [Name("递增")]
        [HideInSuperInspector(nameof(_autoScrollRule), EValidityCheckType.Equal, EScrollRule.None)]
        public bool _increate = true;

        /// <summary>
        /// 滚动间隔时间
        /// </summary>
        [Name("滚动间隔时间")]
        [HideInSuperInspector(nameof(_autoScrollRule), EValidityCheckType.Equal, EScrollRule.None)]
        public float _scrollPageDelayTime = 3;

        private ScrollPage _scrollPage = null;

        private float timeCounter = 0;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            timeCounter = 0;
            _scrollPage = GetComponent<ScrollPage>();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (_autoScrollRule == EScrollRule.None) return;

            timeCounter += Time.deltaTime;
            if (timeCounter > _scrollPageDelayTime)
            {
                timeCounter = 0;

                if (_increate)
                {
                    _scrollPage.NextPage(_autoScrollRule == EScrollRule.AutoScrollLoop);
                }
                else
                {
                    _scrollPage.PreviousPage(_autoScrollRule == EScrollRule.AutoScrollLoop);
                }
            }
        }

        /// <summary>
        /// 滚动规则
        /// </summary>
        [Name("滚动规则")]
        public enum EScrollRule
        {
            [Name("无")]
            None,

            [Name("自动滚动")]
            AutoScroll,

            [Name("自动循环滚动")]
            AutoScrollLoop,
        }
    }
}
