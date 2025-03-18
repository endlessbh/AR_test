using System;
using System.Collections.Generic;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Base
{
    /// <summary>
    /// 基础相机核心子控制器检查器
    /// </summary>
    [Name("基础相机核心子控制器检查器")]
    [CustomEditor(typeof(BaseCameraCoreController), true)]
    public class BaseCameraCoreControllerInspector : BaseCameraCoreControllerInspector<BaseCameraCoreController>
    {
    }

    /// <summary>
    /// 基础相机核心子控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCameraCoreControllerInspector<T> : BaseCameraSubComtrollerInspector<T>
       where T : BaseCameraCoreController
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        private static Dictionary<Type, CategoryList> categoryLists = new Dictionary<Type, CategoryList>();

        private CategoryList categoryList;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var type = target.GetType();
            if (categoryLists.TryGetValue(type, out categoryList) || categoryList == null)
            {
                categoryLists[type] = categoryList = EditorToolsHelper.GetWithPurposes(type.Name);
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CategoryListExtension.DrawVertical(categoryList);
        }
    }
}
