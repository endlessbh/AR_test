using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 编辑器处理器扩展
    /// </summary>
    public static class EditorHandlerExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            //调用插件处理器扩展初始化
            PluginsHandlerExtension.Init();

            UnityObjectHandler.unityObjectHandler = DefaultUnityObjectHandlerForEditor.instance;

            DefaultEditorHandler.instance.Init();
            EditorHandler.editorHandler = DefaultEditorHandler.instance;
            EditorHandler.undoHandler = DefaultUndoHandler.instance;
            EditorHandler.viewHandler = DefaultViewHandler.instance;

            DefaultUndoHandler.Init();

            DefaultDataHandler.onGetIconInLibByMemberInfo += GetIcon;
            DefaultDataHandler.onGetIconInLibByPath += GetIcon;

            SceneViewInfo.Init();

            CheckUnityVersion();
        }

        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private static Texture2D GetIcon(MemberInfo memberInfo) => EditorIconHelper.GetIconInLib(memberInfo);

        /// <summary>
        /// 获取图标
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Texture2D GetIcon(string path) => EditorIconHelper.GetIconInLib(path);

        /// <summary>
        /// 检查Unity版本
        /// </summary>
        private static void CheckUnityVersion()
        {
            bool needRefresh = false;

#if UNITY_2018_4_23
            {
                //问题：在Unity2018.4.23f1的LTS版本中Library/PackageCache/com.unity.messenger@1.0.7-preview/Editor/Third-Party/websocket-sharp.dll与XDreamer提供的Assets/Plugins/WebSocket/websocket-sharp.dll冲突
                //解决方法：移除XDreamer提供的Assets/Plugins/WebSocket/websocket-sharp.dll文件
                try
                {
                    var filePath = Application.dataPath + "/../Library/PackageCache/com.unity.messenger@1.0.7-preview/Editor/Third-Party/websocket-sharp.dll";
                    if (FileHelper.Exists(filePath))
                    {
                        File.Delete("Assets/Plugins/WebSocket/websocket-sharp.dll");
                        File.Delete("Assets/Plugins/WebSocket/websocket-sharp.dll.meta");
                    }
                    needRefresh = true;
                }
                catch { }
            }
#endif
            if (needRefresh) AssetDatabase.Refresh();
        }
    }
}
