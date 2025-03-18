using UnityEngine;
using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Collections.Generic;
using XCSJ.PluginXGUI.Styles.Base;
using System.Linq;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Styles.Tools
{
    /// <summary>
    /// 样式配置器
    /// </summary>
    [Name("样式配置器")]
    [Tip("当场景中的UI需要在运行态下进行变换时，需使用样式配置器关联对应样式", "When the UI in the scene needs to be changed in the running state, you need to use the style configurator to associate the corresponding style")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [Tool(XGUICategory.Component, rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public class StyleConfiger : SingleInstanceMB<StyleConfiger>
    {
        /// <summary>
        /// 场景默认样式名称
        /// </summary>
        [Name("场景默认样式名称")]
        public string _styleName = "";

        /// <summary>
        /// 样式列表
        /// </summary>
        [Name("样式列表")]
        public List<XStyle> _styles = new List<XStyle>();

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            SetStyleNameByCache();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            if (string.IsNullOrEmpty(_styleName))
            {
                SetStyleNameByCache();
            }
            else
            {
                XStyleCache.SetDefaultStyle(_styleName);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XStyleCache.onDefaultStyleChanged += OnDefaultStyleChanged;

            if (Application.isPlaying)
            {
                XStyleCache.Register(_styles);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XStyleCache.onDefaultStyleChanged -= OnDefaultStyleChanged;

            if (Application.isPlaying)
            {
                XStyleCache.Unregister(_styles);
            }
        }

        /// <summary>
        /// 样式改变
        /// </summary>
        /// <param name="style"></param>
        private void OnDefaultStyleChanged(XStyle style)
        {
            SetStyleNameByCache();
        }

        private void SetStyleNameByCache()
        {
            if (XStyleCache.defaultStyle)
            {
                _styleName = XStyleCache.defaultStyle.name;
            }
        }
    }
}
