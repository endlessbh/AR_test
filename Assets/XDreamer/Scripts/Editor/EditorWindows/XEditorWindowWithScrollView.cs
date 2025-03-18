using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.EditorWindows
{
    /// <summary>
    /// 用于明文的带滚动视图的编辑器窗口基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class XEditorWindowWithScrollView<T> : EditorWindowWithScrollView<T>
        where T : XEditorWindowWithScrollView<T>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        /// <summary>
        /// 当绘制场景GUI时
        /// </summary>
        /// <param name="sceneView"></param>
        public virtual void OnSceneGUI(SceneView sceneView) { }

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            AddEditScriptMenu(menu);
#endif
        }
    }

    /// <summary>
    /// 用于明文的编辑器窗口基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class XEditorWindow<T> : EditorWindow<T>
        where T : XEditorWindow<T>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        /// <summary>
        /// 当绘制场景GUI时
        /// </summary>
        /// <param name="sceneView"></param>
        public virtual void OnSceneGUI(SceneView sceneView) { }

        /// <summary>
        /// 添加项到菜单：窗口增加点击的菜单项
        /// </summary>
        /// <param name="menu"></param>
        public override void AddItemsToMenu(GenericMenu menu)
        {
            base.AddItemsToMenu(menu);
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            AddEditScriptMenu(menu);
#endif
        }
    }
}
