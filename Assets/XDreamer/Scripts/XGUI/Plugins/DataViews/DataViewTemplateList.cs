using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews
{
    /// <summary>
    /// 数据视图模版加载器：存储数据视图模版，创建数据视图时使用
    /// </summary>
    [Name("数据视图模版加载器")]
    [ExecuteInEditMode]
    [RequireManager(typeof(XGUIManager))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class DataViewTemplateList : InteractProvider
    {
        /// <summary>
        /// 数据视图模版列表
        /// </summary>
        [Name("数据视图模版列表")]
        public List<DataViewTemplate> _dataViewTemplates = new List<DataViewTemplate>();

        /// <summary>
        /// 数据视图模版
        /// </summary>
        [Serializable]
        public class DataViewTemplate
        {
            [Name("启用")]
            public bool _enable = true;

            [Name("数据视图")]
            public BaseModelView _dataView;
        }

        private bool addTemplate = false;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                RemoveResources();
            }

            if (!addTemplate)
            {
                AddResources();
            }
        }

        /// <summary>
        /// 启用：注册模版
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!addTemplate) AddResources();
        }

        /// <summary>
        /// 禁用：注销模版
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            RemoveResources();
        }

        private void AddResources()
        {
            foreach (var item in _dataViewTemplates)
            {
                if (item._enable) DataViewHelper.AddDataViewTemplate(item._dataView);
            }
            addTemplate = true;
        }

        private void RemoveResources()
        {
            foreach (var item in _dataViewTemplates)
            {
                DataViewHelper.RemoveDataViewTemplate(item._dataView);
            }
            addTemplate = false;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void OnValidate()
        {
            RemoveResources();
            AddResources();
        }
    }
}
