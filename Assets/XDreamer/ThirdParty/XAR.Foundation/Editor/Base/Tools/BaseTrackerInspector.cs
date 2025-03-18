using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginXAR.Foundation;
using XCSJ.PluginXAR.Foundation.Base.Tools;

namespace XCSJ.EditorXAR.Foundation.Base.Tools
{
    /// <summary>
    /// 基础跟踪器检查器
    /// </summary>
    [Name("基础跟踪器检查器")]
    [CustomEditor(typeof(BaseTracker), true)]
    public class BaseTrackerInspector : MBInspector<BaseTracker>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        private static Dictionary<Type, CategoryList> categoryLists = new Dictionary<Type, CategoryList>();

        /// <summary>
        /// 目录列表
        /// </summary>
        private CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            var type = target.GetType();
            if (categoryLists.TryGetValue(type, out categoryList) || categoryList == null)
            {
                categoryLists[type] = categoryList = EditorToolsHelper.GetWithPurposes(type.Name, nameof(BaseTracker));
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("选中[" + XARFoundationHelper.Title + "]管理器") && XARFoundationManager.instance)
            {
                Selection.activeObject = XARFoundationManager.instance;
            }
            CategoryListExtension.DrawVertical(categoryList);
        }
    }

    /// <summary>
    /// 基础跟踪器检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTrackerInspector<T> : BaseTrackerInspector
        where T : BaseTracker
    {
        /// <summary>
        /// 跟踪器
        /// </summary>
        public T tracker => target as T;
    }
}
