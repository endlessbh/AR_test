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
    /// 日志项视图
    /// </summary>
    [Name("日志项视图")]
    public class LogItemView : UIItem<LogItemView>
    {
        /// <summary>
        /// 日志文本
        /// </summary>
        [Name("日志文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _logText;

        /// <summary>
        /// 消息
        /// </summary>
        public string log
        {
            get => _logText.text;
            set
            {
                _logText.text = value;
            }
        }

        /// <summary>
        /// 设置消息文本渐隐的效果
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="duration"></param>
        /// <param name="ignoreTimeScale"></param>
        public void CrossFadeText(float alpha, float duration, bool ignoreTimeScale)
        {
            _logText.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
        }
    }
}
