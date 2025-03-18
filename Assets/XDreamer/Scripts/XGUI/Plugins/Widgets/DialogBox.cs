using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 对话框
    /// </summary>
    [Name("对话框")]
    public class DialogBox : View
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; private set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Sprite icon { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; private set; }

        [Name("执行结果后对话框激活")]
        public EBool _activeOnResult = EBool.No;

        public static event Action<DialogBox, string> onResult;

        public void OnResult(string result)
        {
            gameObject.SetActive(CommonFun.BoolChange(gameObject.activeSelf, _activeOnResult));
            onResult?.Invoke(this, result);
        }

        public void Yes() => OnResult(nameof(Yes));

        public void No() => OnResult(nameof(No));

        public void OK() => OnResult(nameof(OK));

        public void Cancel() => OnResult(nameof(Cancel));

        public void Close() => OnResult(nameof(Close));

        private static string[] _results = new string[] { nameof(Yes), nameof(No), nameof(OK), nameof(Cancel), nameof(Close) };

        public static string[] results => _results;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="callback"></param>
        /// <param name="icon"></param>
        public void Show(string title, string description, Sprite icon = null)
        {
            this.title = title;
            this.description = description;
            this.icon = icon;

            gameObject.XSetActive(true);
            transform.SetAsLastSibling();
        }
    }
}
