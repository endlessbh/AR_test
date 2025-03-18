using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.PluginXGUI.Views.ScrollViews;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 日志视图控制器
    /// </summary>
    [Name("日志视图控制器")]
    public class LogViewController : UIContainer<LogItemView>
    {
        /// <summary>
        /// 日志最大数量
        /// </summary>
        [Name("日志最大数量")]
        [Min(1)]
        public int _logMaxCount = 100;

        /// <summary>
        /// 文本滚动方向
        /// </summary>
        public enum ETextScrollDirection
        {
            [Name("从上到下")]
            TopToBottom,

            [Name("从下到上")]
            BottomToTop,
        }
        [Name("文本在窗口中滚动方向")]
        [EnumPopup]
        public ETextScrollDirection _textScrollDirection = ETextScrollDirection.TopToBottom;

        public enum ELogFormatRule
        {
            [Name("无")]
            None,

            [Name("时间前缀")]
            TimePrefix,
        }

        /// <summary>
        /// 日志格式规则
        /// </summary>
        [Name("日志格式规则")]
        [EnumPopup]
        public ELogFormatRule _logFormatRule = ELogFormatRule.TimePrefix;

        /// <summary>
        /// 时间格式
        /// </summary>
        [Name("时间格式")]
        public string _timeFormat = "hh:mm:ss";

        /// <summary>
        /// 缺省文本颜色
        /// </summary>
        [Name("缺省文本颜色")]
        public Color _defaultColor = Color.white;

        ///// <summary>
        ///// 启用渐隐
        ///// </summary>
        //[Group("渐隐设置", textEN = "Fade Settings")]
        //[Name("启用渐隐")]
        //public bool _enableFade = false;

        ///// <summary>
        ///// 延迟渐隐时间
        ///// </summary>
        //[Name("延迟渐隐时间")]
        //public float _delayTime = 5;

        ///// <summary>
        ///// 渐隐时间
        ///// </summary>
        //[Name("渐隐时间")]
        //public float _fadeTime = 3;

        ///// <summary>
        ///// 忽略时间缩放
        ///// </summary>
        //[Name("忽略时间缩放")]
        //public bool _ignoreTimeScale = true;

        private List<LogItemView> logItemViews = new List<LogItemView>();

        private LogItemView fadeItemView;

        /// <summary>
        /// 添加消息：默认采用白色文本
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message) => AddMessage(message, _defaultColor);

        /// <summary>
        /// 添加带颜色的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void AddMessage(string message, Color color)
        {
            if (itemCount >= _logMaxCount)
            {
                pool.Free(pool.workObjects[0]);
            }
            var item = pool.Alloc();
            logItemViews.Add(item);
            string msg = message;
            switch (_logFormatRule)
            {
                case ELogFormatRule.TimePrefix: msg = string.Format("{0}:{1}", DateTime.Now.ToString(_timeFormat), message); break;
            }
            item.log = XGUIHelper.ToColorString(msg, color);
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            SetTemplateParentPivotAndAnchor();
        }

        private void SetTemplateParentPivotAndAnchor()
        {
            // 调整模版父级对象的坐标系
            if (!_template) return;
                
            var rectTransform = _template.transform.parent as RectTransform;
            if (rectTransform)
            {
                switch (_textScrollDirection)
                {
                    case ETextScrollDirection.TopToBottom:
                        {
                            rectTransform.pivot = rectTransform.anchorMin = new Vector2(0, 1);
                            rectTransform.anchorMax = Vector2.one;
                            break;
                        }
                    case ETextScrollDirection.BottomToTop:
                        {
                            rectTransform.pivot = rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = new Vector2(1, 0);
                            break;
                        }
                }
            }
        }

#endif

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            UpdateFadeMessage();
        }

        #region 消息排序

        protected override void OnNewItem(LogItemView item)
        {
            OnSort(item);
        }

        protected override void OnAllocItem(LogItemView item)
        {
            OnSort(item);
        }

        private void OnSort(LogItemView item)
        {
            switch (_textScrollDirection)
            {
                case ETextScrollDirection.TopToBottom: item.transform.SetAsLastSibling(); break;
                case ETextScrollDirection.BottomToTop: item.transform.SetAsLastSibling(); break;
            }
        }

        #endregion

        #region 消息项渐隐

        private void UpdateFadeMessage()
        {
            //if (_enableFade && !fadeItemView)
            //{
            //    if (logItemViews.Count > 0)
            //    {
            //        fadeItemView = logItemViews[0];
            //        logItemViews.RemoveAt(0);
            //        FadeMessage(_delayTime, _fadeTime, _ignoreTimeScale);
            //    }
            //}
        }

        private void FadeMessage(float delay, float duration, bool ignoreTimeScale)
        {
            StartCoroutine(DelayCrossFade(delay, duration, ignoreTimeScale));
            Invoke(nameof(FreeCurrentFadeItem), delay + duration);
        }

        private IEnumerator DelayCrossFade(float delay, float duration, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);

            fadeItemView.CrossFadeText(0f, duration, ignoreTimeScale);
        }

        private void FreeCurrentFadeItem()
        {
            fadeItemView.log = "";

            fadeItemView.CrossFadeText(1, 0, true);

            pool.Free(fadeItemView);
            fadeItemView = null;
        } 

        #endregion
    }
}
