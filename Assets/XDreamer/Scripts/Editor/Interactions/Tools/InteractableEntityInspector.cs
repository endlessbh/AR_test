using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Windows;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 可交互实体检查器
    /// </summary>
    [Name("可交互实体检查器")]
    [CustomEditor(typeof(InteractableEntity), true)]
    [CanEditMultipleObjects]
    public class InteractableEntityInspector : InteractUsageObjectInspector<InteractableEntity>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(InteractableVirtual)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);
        }
    }

    /// <summary>
    /// 可交互实体查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(InteractableEntity), true)]
    public class InteractableEntityViewer : ToolObjectViewerEditor<InteractableEntity>
    {
        /// <summary>
        /// 当绘制GUI
        /// </summary>
        public override void OnGUI() => Draw(toolComponents);

        /// <summary>
        /// 绘制
        /// </summary>
        public static void Draw() => Draw(allToolComponents);

        /// <summary>
        /// 绘制
        /// </summary>
        public static void Draw(IEnumerable<InteractableEntity> components)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("NO.", UICommonOption.Width32);
                GUILayout.Label("可交互实体");
                GUILayout.Label("悬停", UICommonOption.Width48);
                GUILayout.Label("选择", UICommonOption.Width48);
                GUILayout.Label("激活", UICommonOption.Width48);
            }
            EditorGUILayout.EndHorizontal();

            if (components == null) return;

            int i = 0;
            foreach (var component in components)
            {
                if (!component) continue;

                UICommonFun.BeginHorizontal(i);
                {
                    //编号
                    EditorGUILayout.LabelField((++i).ToString(), UICommonOption.Width32);

                    //组件
                    EditorGUILayout.ObjectField(component, component.GetType(), true);

                    //悬停
                    EditorGUILayout.Toggle(component.isHovered, UICommonOption.Width48);

                    //选择
                    EditorGUILayout.Toggle(component.isSelected, UICommonOption.Width48);

                    //激活
                    EditorGUILayout.Toggle(component.isActived, UICommonOption.Width48);
                }
                UICommonFun.EndHorizontal();
            }
        }
    }
}
