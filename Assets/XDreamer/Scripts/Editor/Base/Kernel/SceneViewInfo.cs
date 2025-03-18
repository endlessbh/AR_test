using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 场景视图信息类
    /// </summary>
    public class SceneViewInfo : ISceneView
    {
        /// <summary>
        /// 静态场景视图对象
        /// </summary>
        public static SceneViewInfo sceneViewInfo { get; } = new SceneViewInfo();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            EditorHandler.OnSceneGUI(sceneViewInfo.Update(sceneView));
        }

        /// <summary>
        /// 场景视图对象
        /// </summary>
        public SceneView sceneView { get; private set; } = null;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="sceneView"></param>
        /// <returns></returns>
        private SceneViewInfo Update(SceneView sceneView)
        {
            this.sceneView = sceneView;
            return this;
        }
    }
}
