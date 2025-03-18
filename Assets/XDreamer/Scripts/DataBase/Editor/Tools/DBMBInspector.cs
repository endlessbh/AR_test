using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginDataBase;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 数据库组件检查器
    /// </summary>
    [CustomEditor(typeof(DBMB), true)]
    [Name("数据库组件检查器")]
    public class DBMBInspector : DBMBInspector<DBMB> { }

    /// <summary>
    /// 数据库组件检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DBMBInspector<T> : MBInspector<T> where T : DBMB
    {
        private CategoryList categoryList;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            categoryList = EditorToolsHelper.GetWithPurposes(typeof(T).Name, nameof(DBMB));
        }

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);
            EditorGUILayout.TextField("数据库名称(只读)", targetObject.dbName);
            EditorGUILayout.TextField("数据库显示名称(只读)", targetObject.dbDisplayName);
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
