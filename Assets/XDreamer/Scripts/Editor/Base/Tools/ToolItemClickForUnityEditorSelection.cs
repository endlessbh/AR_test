using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Tools
{
    /// <summary>
    /// Unity编辑器选择集专用的工具项点击类
    /// </summary>
    public class ToolItemClickForUnityEditorSelection : IItemClick
    {
        /// <summary>
        /// 在播放状态下能否添加
        /// </summary>
        public virtual bool canClickInPlaying => false;

        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public virtual bool CanClick(IItem toolItem) => Selection.activeGameObject && (canClickInPlaying || !Application.isPlaying);

        /// <summary>
        /// 点击时
        /// </summary>
        /// <param name="toolItem"></param>
        public virtual void OnClick(IItem toolItem)
        {
            if (toolItem.memberInfo is Type type)
            {
                var go = Selection.activeGameObject;
                if (!go)
                {
                    Log.WarningFormat("无效游戏对象无法添加组件[{0}]({1})!",
                        CommonFun.Name(type),
                        type.FullName);
                    return;
                }
                if (Application.isPlaying && !canClickInPlaying)
                {
                    Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因运行时禁止添加任何此类型组件!",
                        CommonFun.GameObjectToString(Selection.activeGameObject),
                        CommonFun.Name(type),
                        type.FullName);
                    return;
                }
                if (go.GetComponent<XDreamer>())
                {
                    Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因该游戏对象为{3}根游戏对象，禁止添加任何此类型组件!",
                        CommonFun.GameObjectToString(Selection.activeGameObject),
                        CommonFun.Name(type),
                        type.FullName,
                        Product.Name);
                    return;
                }

                var manager = go.GetComponent<Manager>();
                if (manager)
                {
                    Log.WarningFormat("为游戏对象[{0}]添加组件[{1}]({2})失败，因该游戏对象包含管理器插件[{3}]({4})，禁止添加任何此类型组件!",
                        CommonFun.GameObjectToString(Selection.activeGameObject),
                        CommonFun.Name(type),
                        type.FullName,
                        CommonFun.Name(manager.GetType()),
                        manager.GetType().FullName);
                    return;
                }

                go.XAddComponent(type);
            }
        }
    }

    /// <summary>
    /// 有1个组件类型限定的Unity编辑器选择集专用的工具项点击类：同时要求选择集对象上必须存在指定类型的组件（即<see cref="Component"/>的子类）或接口；
    /// </summary>
    /// <typeparam name="TComponent1">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    public class ToolItemClickForUnityEditorSelection<TComponent1> : ToolItemClickForUnityEditorSelection
    {
        /// <summary>
        /// 判断选择集对象上是否存在指定类型的组件（即<see cref="Component"/>的子类）或接口；
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public virtual bool HasComponent<TComponent>() => Selection.activeGameObject.GetComponent(typeof(TComponent));

        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public override bool CanClick(IItem toolItem) => base.CanClick(toolItem) && HasComponent<TComponent1>();
    }

    /// <summary>
    /// 有2个组件类型限定的Unity编辑器选择集专用的工具项点击类：同时要求选择集对象上必须存在指定类型的组件（即<see cref="Component"/>的子类）或接口；
    /// </summary>
    /// <typeparam name="TComponent1">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    /// <typeparam name="TComponent2">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    public class ToolItemClickForUnityEditorSelection<TComponent1, TComponent2> : ToolItemClickForUnityEditorSelection<TComponent1>
    {
        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public override bool CanClick(IItem toolItem) => base.CanClick(toolItem) && HasComponent<TComponent2>();
    }

    /// <summary>
    /// 有3个组件类型限定的Unity编辑器选择集专用的工具项点击类：同时要求选择集对象上必须存在指定类型的组件（即<see cref="Component"/>的子类）或接口；
    /// </summary>
    /// <typeparam name="TComponent1">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    /// <typeparam name="TComponent2">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    /// <typeparam name="TComponent3">选择集对象上必须存在的组件类型，可以是组件类（即<see cref="Component"/>的子类）或接口；</typeparam>
    public class ToolItemClickForUnityEditorSelection<TComponent1, TComponent2, TComponent3> : ToolItemClickForUnityEditorSelection<TComponent1, TComponent2>
    {
        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public override bool CanClick(IItem toolItem) => base.CanClick(toolItem) && HasComponent<TComponent3>();
    }
}
