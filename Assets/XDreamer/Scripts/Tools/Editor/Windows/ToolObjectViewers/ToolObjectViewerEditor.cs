using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 工具对象查看器编辑器
    /// </summary>
    [ToolObjectViewerEditor(typeof(Component), true)]
    public class ToolObjectViewerEditor
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        public Type componentType { get; internal set; }

        /// <summary>
        /// 组件列表
        /// </summary>
        public List<Component> components { get; internal set; } = new List<Component>();

        /// <summary>
        /// 启用
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// 禁用
        /// </summary>
        public virtual void OnDisable() { }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public virtual void OnGUI() => DefaultDrawGUI();

        /// <summary>
        /// 默认绘制GUI
        /// </summary>
        public void DefaultDrawGUI()
        {
            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("组件对象", "组件对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("拥有者", "组件拥有者所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活并启用", "组件与所在游戏对象（包含父级游戏对象）是否激活并启用；如父层级不激活，当前相机所在游戏对象也不激活；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("激活", "组件所在游戏对象是否激活；编辑态可修改，运行态只读；"), UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("启用", "组件是否启用；编辑态可修改，运行态只读；"), UICommonOption.Width32);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //游戏对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //拥有者
                if (component is IComponentHasOwner hasComponentOwner)
                {
                    var owner = hasComponentOwner.owner?.ownerGameObject;
                    EditorGUILayout.ObjectField(owner, typeof(GameObject), true);
                }
                else
                {
                    EditorGUILayout.ObjectField(component.gameObject, typeof(GameObject), true);
                }

                EditorGUI.BeginDisabledGroup(isPlaying);
                {
                    if (component is Behaviour behaviour)
                    {
                        //激活并启用
                        EditorGUILayout.Toggle(behaviour.isActiveAndEnabled, UICommonOption.Width60);
                    }
                    else
                    {
                        //激活并启用
                        EditorGUILayout.Toggle(component.gameObject.activeInHierarchy && component.GetComponentEnabled(), UICommonOption.Width60);
                    }

                    //激活
                    var activeSelf = component.gameObject.activeSelf;
                    if (EditorGUILayout.Toggle(activeSelf, UICommonOption.Width32) != activeSelf) component.gameObject.XSetActive(!activeSelf);

                    //启用
                    var enable = component.GetComponentEnabled();
                    if (EditorGUILayout.Toggle(enable, UICommonOption.Width32) != enable) component.XSetEnable(!enable);
                }

                UICommonFun.EndHorizontal();
            }
        }

        internal static ToolObjectViewerEditor GetEditor(Type componentType) => Cache.GetCacheValue(componentType);

        /// <summary>
        /// 缓存
        /// </summary>
        class Cache : TICache<Cache, Type, ToolObjectViewerEditor>
        {
            /// <summary>
            /// 构建值
            /// </summary>
            /// <param name="componentType"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, ToolObjectViewerEditor> CreateValue(Type componentType)
            {
                if (componentType == null)
                {
                    Debug.LogError("获取工具列表编辑器时,参数中组件类型为null!");
                    return new KeyValuePair<bool, ToolObjectViewerEditor>(true, null);
                }
                if (!typeof(Component).IsAssignableFrom(componentType))
                {
                    Debug.LogErrorFormat("获取工具列表编辑器时,参数中组件类型[{0}]非有效组件类型[{1}]的子类型!", componentType.FullName, typeof(Component).FullName);
                    return new KeyValuePair<bool, ToolObjectViewerEditor>(true, null);
                }
                var editorType = ToolObjectViewerEditorAttribute.GetEditorType(componentType);
                if (editorType == null)
                {
                    Debug.LogErrorFormat("获取工具列表编辑器时,组件类型[{0}]对应的工具列表编辑器类型无效!", componentType.FullName);
                    return new KeyValuePair<bool, ToolObjectViewerEditor>(true, null);
                }
                if (!typeof(ToolObjectViewerEditor).IsAssignableFrom(editorType))
                {
                    Debug.LogErrorFormat("获取工具列表编辑器时,组件类型[{0}]对应的工具列表编辑器类型[{1}]非有效工具列表编辑器类型[{2}]的子类型!", componentType.FullName, editorType.FullName, typeof(ToolObjectViewerEditor).FullName);
                    return new KeyValuePair<bool, ToolObjectViewerEditor>(true, null);
                }
                var editor = TypeHelper.CreateInstance(editorType) as ToolObjectViewerEditor;
                if (editor == null)
                {
                    Debug.LogErrorFormat("获取工具列表编辑器时,组件类型[{0}]对应的工具列表编辑器类型[{1}]未找到有效的默认构造函数（即无参数的构造函数）!", componentType.FullName, editorType.FullName);
                    return new KeyValuePair<bool, ToolObjectViewerEditor>(true, null);
                }
                editor.componentType = componentType;
                return new KeyValuePair<bool, ToolObjectViewerEditor>(true, editor);
            }
        }
    }

    /// <summary>
    /// 工具对象查看器编辑器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ToolObjectViewerEditor<T> : ToolObjectViewerEditor where T : Component, ITool
    {
        /// <summary>
        /// 工具组件列表
        /// </summary>
        public IEnumerable<T> toolComponents => components.Cast(c => c as T);

        /// <summary>
        /// 所有工具组件
        /// </summary>
        public static IEnumerable<T> allToolComponents => CommonFun.GetComponentsInChildren<T>(true);
    }
}
